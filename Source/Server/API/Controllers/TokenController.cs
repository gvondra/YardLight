using BrassLoon.Interface.Authorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using YardLight.CommonAPI;
using AuthorizationAPI = BrassLoon.Interface.Authorization;
using Log = BrassLoon.Interface.Log;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : APIControllerBase
    {
        private readonly AuthorizationAPI.ITokenService _tokenService;
        public TokenController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService,
            Log.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService,
            AuthorizationAPI.ITokenService tokenService
            ) : base(settings, settingsFactory, metricService, exceptionService, userService)
        {
            _tokenService = tokenService;
        }

        [HttpPost()]
        [Authorize(Constants.POLICY_TOKEN_CREATE)]
        public async Task<IActionResult> Create()
        {
            IActionResult result = null;
            try
            {
                if (result == null)
                {
                    AuthorizationAPI.ISettings settings = _settingsFactory.CreateAuthorization(_settings.Value, GetUserToken());
                    result = Content(await _tokenService.Create(settings, _settings.Value.AuthorizationDomainId.Value),
                        "text/plain");
                }
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }
    }
}
