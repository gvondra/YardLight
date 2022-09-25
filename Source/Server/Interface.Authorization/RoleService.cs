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
    public class RoleService : IRoleService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public RoleService(RestUtil restUtil,
            IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<Role> Create(ISettings settings, Role role)
        {
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "api", "Role");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Post, role);
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<Role> response = await _service.Send<Role>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<IEnumerable<Role>> GetAll(ISettings settings)
        {
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "api", "Role");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Get);
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<List<Role>> response = await _service.Send<List<Role>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<Role> Update(ISettings settings, Role role)
        {
            if (!role.RoleId.HasValue)
                throw new ArgumentNullException(nameof(role.RoleId));
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "api", "Role", "{id}");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Put, role);
            request.AddPathParameter("id", role.RoleId.Value.ToString());
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<Role> response = await _service.Send<Role>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }
    }
}
