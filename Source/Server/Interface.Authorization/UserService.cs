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
    public class UserService : IUserService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public UserService(RestUtil restUtil,
            IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<User> Get(ISettings settings)
        {
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "User");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Get);
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<User> response = await _service.Send<User>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<User> Get(ISettings settings, Guid id)
        {
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "User", id.ToString("N"));
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Get);
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<User> response = await _service.Send<User>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<User> GetByEmailAddress(ISettings settings, string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress));
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "User");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Get);
            request.AddQueryParameter("emailAddress", emailAddress);
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<User> response = await _service.Send<User>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<User> Update(ISettings settings, User user)
        {
            if (!user.UserId.HasValue || user.UserId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(user.UserId));
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "User", "{id}");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Put, user);
            request.AddPathParameter("id", user.UserId.Value.ToString("N"));
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<User> response = await _service.Send<User>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }
    }
}
