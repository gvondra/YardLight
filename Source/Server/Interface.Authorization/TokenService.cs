using YardLight.Interface.Authorization.Models;
using BrassLoon.RestClient;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Authorization
{
    public class TokenService : ITokenService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public TokenService(RestUtil restUtil,
            IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<string> Create(ISettings settings)
        {
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "api", "Token");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Post);
            request.AddJwtAuthorizationToken(settings.GetToken);
            IResponse<string> response = await Policy
                .HandleResult<IResponse<string>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => _service.Send<string>(request));
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<string> Create(ISettings settings, ClientCredential clientCredential)
        {
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = _restUtil.AppendPath(builder.Path, "api", "Token", "ClientCredential");
            IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Post, clientCredential);
            IResponse<string> response = await Policy
                .HandleResult<IResponse<string>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => _service.Send<string>(request));
            _restUtil.CheckSuccess(response);
            return response.Value;
        }
    }
}
