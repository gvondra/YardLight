using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AuthorizationAPI = BrassLoon.Interface.Authorization;
using LogAPI = BrassLoon.Interface.Log;

namespace YardLight.CommonAPI
{
    public abstract class CommonControllerBase : Controller
    {
        private readonly static Polly.Policy _currentUserCache = Polly.Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromMinutes(3)));
        private readonly LogAPI.IMetricService _metricService;
        private readonly LogAPI.IExceptionService _exceptionService;
        private readonly AuthorizationAPI.IUserService _userService;

        protected CommonControllerBase(LogAPI.IMetricService metricService,
            LogAPI.IExceptionService exceptionService) : this(metricService, exceptionService, null)
        {}

        protected CommonControllerBase(LogAPI.IMetricService metricService,
            LogAPI.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService)
        {
            _metricService = metricService;
            _exceptionService = exceptionService;
            _userService = userService; 
        }

        protected string GetCurrentUserReferenceId() 
            => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        protected string GetCurrentUserEmailAddress()
            => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        protected bool UserHasRole(string role)
        {
            return (User.Identity?.IsAuthenticated ?? false)
                && User.Claims.Any(
                c => string.Equals(ClaimTypes.Role, c.Type, StringComparison.OrdinalIgnoreCase) && string.Equals(role, c.Value, StringComparison.OrdinalIgnoreCase)
                );
        }

        protected async Task<AuthorizationAPI.Models.User> GetCurrentUser(AuthorizationAPI.ISettings settings, Guid domainId)
        {
            AuthorizationAPI.Models.User user = null;
            if (_userService != null)
                user = await _currentUserCache.Execute(
                    (context) => _userService.Get(settings, domainId),
                    new Context(GetCurrentUserReferenceId())
                    );
            return user;
        }

        protected async Task<Guid?> GetCurrentUserId(AuthorizationAPI.ISettings settings, Guid domainId)
        {
            Guid? id = null;
            AuthorizationAPI.Models.User user = await GetCurrentUser(settings, domainId);
            if (user != null)
                id = user.UserId;
            return id;
        }

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

        protected Task WriteMetrics(LogAPI.ISettings settings, Guid domainId, string eventCode, double? magnitude, Dictionary<string, string> data = null)
            => WriteMetrics(settings, domainId, eventCode, magnitude, userId: null, data: data);

        protected async Task WriteMetrics(LogAPI.ISettings logSettings, 
            AuthorizationAPI.ISettings authSettings, 
            Guid authorizationDomainId,
            Guid loggingDomainId,
            string eventCode, 
            double? magnitude, 
            Dictionary<string, string> data = null)
        {
            try
            {
                await WriteMetrics(logSettings, loggingDomainId, eventCode, magnitude, userId: await GetCurrentUserId(authSettings, authorizationDomainId), data: data);
            }
            catch (Exception ex)
            {
                await WriteException(logSettings, loggingDomainId, ex);
            }
        }

        protected async Task WriteMetrics(LogAPI.ISettings settings, Guid domainId, string eventCode, double? magnitude, Guid? userId, Dictionary<string, string> data)
        {
            string status = string.Empty;
            try
            {
                if (Response != null)
                    status = ((int)Response.StatusCode).ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting status for metric log: " + ex.Message);
            }
            try
            {
                await _metricService.Create(settings, domainId, eventCode, magnitude ?? 0.0, status, userId?.ToString("N"), data);
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
