using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YardLight.Interface.Models;
using AuthorizationAPI = BrassLoon.Interface.Authorization;
using Log = BrassLoon.Interface.Log;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricController : APIControllerBase
    {
        private readonly Log.IMetricService _metricService;
        private readonly AuthorizationAPI.IUserService _userService;

        public MetricController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService,
            AuthorizationAPI.IUserService userService,
            ILogger<MetricController> logger
            ) : base(settings, settingsFactory, userService, logger)
        { 
            _metricService = metricService;
            _userService = userService;
        }

        [HttpGet()]
        [Authorize(YardLight.CommonAPI.Constants.POLICY_LOG_READ)]
        public async Task<IActionResult> Search([FromQuery] DateTime? maxTimestamp, [FromQuery] string eventCode)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && !maxTimestamp.HasValue)
                    result = BadRequest("Missing maxTimestamp parameter value");
                if (result == null && string.IsNullOrEmpty(eventCode))
                    result = BadRequest("Missing event code parameter value");
                if (result == null && !_settings.Value.LogDomainId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "Missing log domain id configuration value");
                if (result == null)
                {
                    Log.ISettings settings = _settingsFactory.CreateLog(_settings.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    List<Metric> metrics = (await _metricService.Search(settings, _settings.Value.LogDomainId.Value, maxTimestamp.Value, eventCode))
                        .Select(m => mapper.Map<Metric>(m))
                        .ToList();
                    await AddUserData(metrics);
                    result = Ok(metrics);
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-metrics-search", start, result, new Dictionary<string, string> { { nameof(maxTimestamp), maxTimestamp?.ToString("o") } });
            }
            return result;
        }

        [NonAction]
        private async Task AddUserData(List<Metric> metrics)
        {
            if (metrics != null)
            {
                Guid id;
                Dictionary<Guid, AuthorizationAPI.Models.User> userCache = new Dictionary<Guid, AuthorizationAPI.Models.User>();
                AuthorizationAPI.Models.User user;
                AuthorizationAPI.ISettings settings = GetAuthorizationSettings();
                foreach (Metric metric in metrics.Where(m => !string.IsNullOrEmpty(m.Requestor)))
                {
                    if (Guid.TryParse(metric.Requestor.Trim(), out id))
                    {
                        if (!userCache.ContainsKey(id))
                        {
                            user = await _userService.Get(settings, _settings.Value.AuthorizationDomainId.Value, id);
                            if (user != null)
                                userCache.Add(id, user);
                        }
                        if (userCache.ContainsKey(id))
                            metric.RequestorName = userCache[id].Name ?? string.Empty;
                    }
                }
            }
        }

        [HttpGet("/api/MetricEventCode")]
        [Authorize(YardLight.CommonAPI.Constants.POLICY_LOG_READ)]
        public async Task<IActionResult> GetEventCodes()
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && !_settings.Value.LogDomainId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "Missing log domain id configuration value");
                if (result == null)
                {
                    Log.ISettings settings = _settingsFactory.CreateLog(_settings.Value);
                    result = Ok(
                        await _metricService.GetEventCodes(settings, _settings.Value.LogDomainId.Value)
                        );
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-metric-event-codes", start, result);
            }
            return result;
        }
    }
}
