﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using AuthorizationAPI = YardLight.Interface.Authorization;
using Log = BrassLoon.Interface.Log;
using Models = YardLight.Interface.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExceptionController : APIControllerBase
    {
        private readonly Log.IExceptionService _exceptionService;

        public ExceptionController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService,
            Log.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService
            ) : base(settings, settingsFactory, metricService, exceptionService, userService)
        {
            _exceptionService = exceptionService;
        }

        [HttpGet()]
        [Authorize(YardLight.CommonAPI.Constants.POLICY_LOG_READ)]
        public async Task<IActionResult> Search([FromQuery] DateTime? maxTimestamp)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && !maxTimestamp.HasValue)
                    result = BadRequest("Missing maxTimestamp parameter value");
                if (result == null && !_settings.Value.LogDomainId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "Missing log domain id configuration value");
                if (result == null)
                {
                    Log.ISettings settings = _settingsFactory.CreateLog(_settings.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    IEnumerable<Models.Exception> exceptions = (await _exceptionService.Search(settings, _settings.Value.LogDomainId.Value, maxTimestamp.Value))
                        .Select<Log.Models.Exception, Models.Exception>(e => mapper.Map<Models.Exception>(e));
                    result = Ok(exceptions);
                }
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-exceptions-search", DateTime.UtcNow.Subtract(start).TotalSeconds, new Dictionary<string, string> { { nameof(maxTimestamp), maxTimestamp?.ToString("o") } });
            }
            return result;
        }
    }
}
