using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using YardLight.CommonAPI;
using Log = BrassLoon.Interface.Log;
namespace API
{
    public abstract class APIContollerBase : CommonControllerBase
    {
        protected readonly ISettingsFactory _settingsFactory;
        protected readonly IOptions<Settings> _settings;

        public APIContollerBase(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService, 
            Log.IExceptionService exceptionService
            ) : base(metricService, exceptionService)
        { 
            _settingsFactory = settingsFactory;
            _settings = settings;
        }

        protected async Task WriteMetrics(string eventCode, double? magnitude, Dictionary<string, string> data = null)
        {
            if (!string.IsNullOrEmpty(_settings.Value.BrassLoonLogApiBaseAddress))
                await base.WriteMetrics(_settingsFactory.CreateLog(_settings.Value), _settings.Value.LogDomainId.Value, eventCode, magnitude, data);
        }

        protected async Task WriteException(Exception exception)
        {
            if (!string.IsNullOrEmpty(_settings.Value.BrassLoonLogApiBaseAddress))
                await base.WriteException(_settingsFactory.CreateLog(_settings.Value), _settings.Value.LogDomainId.Value, exception);
            else
                Console.WriteLine(exception.ToString());
        }
    }
}
