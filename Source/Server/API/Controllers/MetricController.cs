using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using AuthorizationAPI = YardLight.Interface.Authorization;
using Log = BrassLoon.Interface.Log;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricController : APIContollerBase
    {
        private readonly Log.IMetricService _metricService;

        public MetricController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService,
            Log.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService
            ) : base(settings, settingsFactory, metricService, exceptionService, userService)
        { 
            _metricService = metricService;
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
                    List<Log.Models.Metric> metrics = await _metricService.Search(settings, _settings.Value.LogDomainId.Value, maxTimestamp.Value, eventCode);
                    result = Ok(metrics);
                }
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-metrics-search", DateTime.UtcNow.Subtract(start).TotalSeconds, new Dictionary<string, string> { { nameof(maxTimestamp), maxTimestamp?.ToString("o") } });
            }
            return result;
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
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-metric-event-codes", DateTime.UtcNow.Subtract(start).TotalSeconds);
            }
            return result;
        }
    }
}
