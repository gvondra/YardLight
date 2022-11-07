using LogAPI = BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YardLight.CommonAPI
{
    public abstract class CommonControllerBase : Controller
    {
        private readonly LogAPI.IMetricService _metricService;
        private readonly LogAPI.IExceptionService _exceptionService;

        protected CommonControllerBase(LogAPI.IMetricService metricService,
            LogAPI.IExceptionService exceptionService)
        {
            _metricService = metricService;
            _exceptionService = exceptionService;
        }

        protected string GetCurrentUserReferenceId() 
            => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        protected string GetCurrentUserEmailAddress()
            => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        protected string GetUserToken()
        {
            string token = null;
            KeyValuePair<string, StringValues>? header = Request.Headers.FirstOrDefault(h => string.Equals(h.Key, "Authorization", StringComparison.OrdinalIgnoreCase));
            if (header.HasValue && header.Value.Value.Count == 1)
            {
                token = header.Value.Value[0];
                Match match = Regex.Match(token, @"bearer\s+(\S+)", RegexOptions.IgnoreCase);
                if (match != null && match.Success && match.Groups != null && match.Groups.Count == 2)
                {
                    token = match.Groups[1].Value;
                }
            }
            return token;
        }

        protected async Task WriteMetrics(LogAPI.ISettings settings, Guid domainId, string eventCode, double? magnitude, Dictionary<string, string> data = null)
        {
            string status = string.Empty;
            if (Response != null)
                status = ((int)Response.StatusCode).ToString();
            try
            {
                await _metricService.Create(settings, domainId, eventCode, magnitude ?? 0.0, status: status, data: data);
            }
            catch (Exception ex)
            {
                await WriteException(settings, domainId, ex);
            }
        }

        protected async Task WriteException(LogAPI.ISettings settings, Guid domainId, Exception exception)
        {
            try
            {
                await _exceptionService.Create(settings, domainId, exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
