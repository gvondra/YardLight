using BrassLoon.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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

namespace YardLight.CommonAPI
{
    public abstract class CommonControllerBase : Controller
    {
        private readonly static Polly.Policy _currentUserCache = Polly.Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromMinutes(3)));
        private readonly AuthorizationAPI.IUserService _userService;
        private readonly ILogger _logger;

        protected CommonControllerBase(
            AuthorizationAPI.IUserService userService,
            ILogger logger)
        {
            _userService = userService; 
            _logger = logger;
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
            {
                string referenceId = GetCurrentUserReferenceId();
                List<AuthorizationAPI.Models.User> users = await _currentUserCache.Execute(
                    (context) => _userService.Search(settings, domainId, referenceId: referenceId),
                    new Context(referenceId)
                    ) ?? new List<AuthorizationAPI.Models.User>();
                user = users.FirstOrDefault();
            }                
            return user;
        }

        protected virtual async Task<Guid?> GetCurrentUserId(AuthorizationAPI.ISettings settings, Guid domainId)
        {
            Guid? id = null;
            AuthorizationAPI.Models.User user = await GetCurrentUser(settings, domainId);
            if (user != null)
                id = user.UserId;
            return id;
        }

        protected virtual async Task<string> GetCurrentUserEmailAddress(AuthorizationAPI.ISettings settings, Guid domainId)
        {
            string emailAddress = null;
            AuthorizationAPI.Models.User user = await GetCurrentUser(settings, domainId);
            if (user != null)
                emailAddress = user.EmailAddress;
            return emailAddress;
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

        protected async Task WriteMetrics(
            AuthorizationAPI.ISettings authSettings, 
            Guid authorizationDomainId,
            string eventCode, 
            double? magnitude = null,
            IActionResult actionResult = null,
            Dictionary<string, string> data = null)
        {
            try
            {
                WriteMetrics(eventCode, 
                    magnitude: magnitude, 
                    userId: await GetCurrentUserId(authSettings, authorizationDomainId), 
                    actionResult: actionResult,
                    data: data
                    );
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        protected void WriteMetrics(
            string eventCode, 
            double? magnitude = null, 
            Guid? userId = null, 
            IActionResult actionResult = null,
            Dictionary<string, string> data = null)
        {
            string status = string.Empty;
            if (actionResult != null)
            {
                if (typeof(ObjectResult).IsAssignableFrom(actionResult.GetType()))
                {
                    status = ((ObjectResult)actionResult).StatusCode.ToString();
                }
                else if (typeof(StatusCodeResult).IsAssignableFrom(actionResult.GetType()))
                {
                    status = ((StatusCodeResult)actionResult).StatusCode.ToString();
                }
            }
            try
            {
                if (string.IsNullOrEmpty(status) && Response != null)
                    status = ((int)Response.StatusCode).ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting status for metric log: " + ex.Message);
            }
            try
            {
                Metric metric = new Metric
                {
                    Data = data,
                    EventCode = eventCode,
                    Magnitude = magnitude ?? 0.0,
                    Requestor = userId?.ToString("N"),
                    Status = status
                };
                _logger.LogMetric(metric);
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        protected virtual void WriteException(Exception exception)
        {
            try
            {
                _logger.LogError(exception, exception.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }        
    }
}
