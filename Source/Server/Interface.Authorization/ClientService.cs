using YardLight.Interface.Authorization.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Authorization
{
    public class ClientService : IClientService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public ClientService(RestUtil restUtil,
            IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<Client> Create(ISettings settings, ClientSaveRequest saveRequest)
        {
            if (saveRequest.Client == null)
                throw new ArgumentNullException(nameof(saveRequest.Client));
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "Client");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Post, saveRequest);
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<Client> response = await _service.Send<Client>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<Client> Create(ISettings settings, Client client)
        {
            return Create(settings, new ClientSaveRequest { Client = client });
        }

        public Task<Client> Create(ISettings settings, Client client, string secret)
        {
            return Create(settings, new ClientSaveRequest { Client = client, Secret = secret });
        }

        public async Task<string> CreateSecret(ISettings settings)
        {
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "ClientCredentialSecret");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Get);
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<string> response = await _service.Send<string>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<Client> Get(ISettings settings, Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "Client", "{id}");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Get);
            request.AddPathParameter("id", id.ToString("N"));
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<Client> response = await _service.Send<Client>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<IEnumerable<Client>> GetAll(ISettings settings)
        {
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "Client");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Get);
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<List<Client>> response = await _service.Send<List<Client>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<Client> Update(ISettings settings, ClientSaveRequest saveRequest)
        {
            if (saveRequest.Client == null)
                throw new ArgumentNullException(nameof(saveRequest.Client));
            if (!saveRequest.Client.ClientId.HasValue || saveRequest.Client.ClientId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(saveRequest.Client.ClientId));
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "Client", "{id}");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Put, saveRequest);
            request.AddPathParameter("id", saveRequest.Client.ClientId.Value.ToString("N"));
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<Client> response = await _service.Send<Client>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<Client> Update(ISettings settings, Client client)
        {
            return Update(settings, new ClientSaveRequest { Client = client });
        }

        public Task<Client> Update(ISettings settings, Client client, string secret)
        {
            return Update(settings, new ClientSaveRequest { Client = client, Secret = secret });
        }
    }
}
