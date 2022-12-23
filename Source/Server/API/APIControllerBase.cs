using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using YardLight.CommonAPI;
using AuthorizationAPI = BrassLoon.Interface.Authorization;
using Log = BrassLoon.Interface.Log;
namespace API
{
    public abstract class APIControllerBase : CommonControllerBase
    {
        protected readonly ISettingsFactory _settingsFactory;
        protected readonly IOptions<Settings> _settings;

        public APIControllerBase(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService, 
            Log.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService
            ) : base(metricService, exceptionService, userService)
        { 
            _settingsFactory = settingsFactory;
            _settings = settings;
        }

        protected async Task WriteMetrics(string eventCode, double? magnitude, Dictionary<string, string> data = null)
        {
            if (!string.IsNullOrEmpty(_settings.Value.BrassLoonLogApiBaseAddress))
                await base.WriteMetrics(
                    _settingsFactory.CreateLog(_settings.Value),                     
                    _settingsFactory.CreateAuthorization(_settings.Value, GetUserToken()),
                    _settings.Value.AuthorizationDomainId.Value,
                    _settings.Value.LogDomainId.Value,
                    eventCode, 
                    magnitude,
                    data
                    );
        }

        protected async Task WriteException(Exception exception)
        {
            if (!string.IsNullOrEmpty(_settings.Value.BrassLoonLogApiBaseAddress))
                await base.WriteException(_settingsFactory.CreateLog(_settings.Value), _settings.Value.LogDomainId.Value, exception);
            else
                Console.WriteLine(exception.ToString());
        }

        protected Task<Guid?> GetCurrentUserId(AuthorizationAPI.ISettings settings)
        {
            return GetCurrentUserId(settings, _settings.Value.AuthorizationDomainId.Value);
        }
    }
}
