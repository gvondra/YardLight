using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardLight.CommonAPI;
using LogAPI = BrassLoon.Interface.Log;

namespace AuthorizationAPI
{
    public abstract class AuthorizationControllerBase : CommonControllerBase
    {
        protected readonly ISettingsFactory _settingsFactory;
        protected readonly IOptions<Settings> _settings;

        protected AuthorizationControllerBase(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            LogAPI.IMetricService metricService,
            LogAPI.IExceptionService exceptionService)
            : base(metricService, exceptionService)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
        }

        protected async Task WriteMetrics(string eventCode, double? magnitude, Dictionary<string, string> data = null)
        {
            if (!string.IsNullOrEmpty(_settings.Value.LogApiBaseAddress))
                await base.WriteMetrics(_settingsFactory.CreateLog(_settings.Value, GetUserToken()), _settings.Value.LogDomainId, eventCode, magnitude, data);
        }

        protected async Task WriteException(Exception exception)
        {
            if (!string.IsNullOrEmpty(_settings.Value.LogApiBaseAddress))
                await base.WriteException(_settingsFactory.CreateLog(_settings.Value, GetUserToken()), _settings.Value.LogDomainId, exception);
            else
                Console.WriteLine(exception.ToString());
        }
    }
}
