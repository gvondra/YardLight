using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Models = YardLight.Interface.Models;

namespace YardLight.Interface
{
    public class ExceptionService : IExceptionService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public ExceptionService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<List<Models.Exception>> Search(ISettings settings, DateTime maxTimestamp)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Exception")
                .AddQueryParameter("maxTimestamp", maxTimestamp.ToString("o"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<List<Models.Exception>> response = await _service.Send<List<Models.Exception>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }
    }
}
