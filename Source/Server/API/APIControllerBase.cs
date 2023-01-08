using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardLight.CommonAPI;
using YardLight.Framework;
using AuthorizationAPI = BrassLoon.Interface.Authorization;

namespace API
{
    public abstract class APIControllerBase : CommonControllerBase
    {
        protected readonly ISettingsFactory _settingsFactory;
        protected readonly IOptions<Settings> _settings;
        private CoreSettings _coreSettings;
        private AuthorizationSettings _authorizationSettings;

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

        protected virtual Task<Guid?> GetCurrentUserId()
        {
            return GetCurrentUserId(GetAuthorizationSettings(), _settings.Value.AuthorizationDomainId.Value);
        }

        [Obsolete()]
        protected virtual Task<Guid?> GetCurrentUserId(AuthorizationAPI.ISettings settings)
        {
            return GetCurrentUserId(settings, _settings.Value.AuthorizationDomainId.Value);
        }

        protected virtual Task<string> GetCurrentUserEmailAddress(AuthorizationAPI.ISettings settings)
        {
            return GetCurrentUserEmailAddress(settings, _settings.Value.AuthorizationDomainId.Value);
        }

        protected virtual async Task<ValueTuple<IActionResult, IProject>> GetProjectForCurrentUser(IProjectFactory projectFactory, Guid projectId)
        {
            if (projectId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(projectId));
            IActionResult actionResult = null;
            IProject project = null;
            string emailAddress = await GetCurrentUserEmailAddress(GetAuthorizationSettings());
            if (string.IsNullOrEmpty(emailAddress))
                actionResult = StatusCode(StatusCodes.Status500InternalServerError, "UserNotFound");
            if (actionResult == null)
                project = await projectFactory.Get(GetCoreSettings(), emailAddress, projectId);
            if (actionResult == null && project == null)
                actionResult = NotFound();
            return (actionResult, project);
        }

        protected virtual CoreSettings GetCoreSettings()
        {
            if (_coreSettings == null)
                _coreSettings = _settingsFactory.CreateCore(_settings.Value);
            return _coreSettings;
        }

        protected virtual AuthorizationSettings GetAuthorizationSettings()
        {
            if (_authorizationSettings == null)
                _authorizationSettings = _settingsFactory.CreateAuthorization(_settings.Value);
            return _authorizationSettings;
        }
    }
}
