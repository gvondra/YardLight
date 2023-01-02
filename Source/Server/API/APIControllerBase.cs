using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            AuthorizationAPI.IUserService userService,
            ILogger logger
            ) : base(userService, logger)
        { 
            _settingsFactory = settingsFactory;
            _settings = settings;
        }

        protected async Task WriteMetrics(string eventCode, double? magnitude, IActionResult actionResult = null, Dictionary<string, string> data = null)
        {
            if (!string.IsNullOrEmpty(_settings.Value.BrassLoonLogApiBaseAddress))
                await base.WriteMetrics(                
                    _settingsFactory.CreateAuthorization(_settings.Value),
                    _settings.Value.AuthorizationDomainId.Value,
                    eventCode, 
                    magnitude: magnitude,
                    actionResult: actionResult,
                    data: data
                    );
        }

        protected Task WriteMetrics(string eventCode, DateTime? startTime, IActionResult actionResult = null, Dictionary<string, string> data = null)
        {
            double? magnitude = null;
            if (startTime.HasValue)
            {
                startTime = startTime.Value.ToUniversalTime();
                magnitude = DateTime.UtcNow.Subtract(startTime.Value).TotalSeconds;
            }
            return WriteMetrics(eventCode, magnitude, actionResult, data);
        }

        protected override void WriteException(Exception exception)
        {
            if (!string.IsNullOrEmpty(_settings.Value.BrassLoonLogApiBaseAddress))
                base.WriteException(exception);
            else
                Console.WriteLine(exception.ToString());
        }

        protected Task<Guid?> GetCurrentUserId(AuthorizationAPI.ISettings settings)
        {
            return GetCurrentUserId(settings, _settings.Value.AuthorizationDomainId.Value);
        }
    }
}
