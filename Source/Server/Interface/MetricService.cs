using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface
{
    public class MetricService : IMetricService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public MetricService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<List<string>> GetEventCodes(ISettings settings)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("api/MetricEventCode")
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<List<string>> response = await _service.Send<List<string>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<dynamic[]> Search(ISettings settings, DateTime maxTimestamp, string eventCode)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("api/Metric")
                .AddQueryParameter("maxTimestamp", maxTimestamp.ToString("o"))
                .AddQueryParameter("eventCode", eventCode)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<List<dynamic>> response = await _service.Send<List<dynamic>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value.ToArray();
        }
    }
}
