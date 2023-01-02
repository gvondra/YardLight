using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using YardLight.CommonAPI;
using AuthorizationAPI = BrassLoon.Interface.Authorization;

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
            AuthorizationAPI.IUserService userService,
            AuthorizationAPI.ITokenService tokenService,
            ILogger<TokenController> logger
            ) : base(settings, settingsFactory, userService, logger)
        {
            _tokenService = tokenService;
        }

        [HttpPost()]
        [Authorize(Constants.POLICY_TOKEN_CREATE)]
        public async Task<IActionResult> Create()
        {
            DateTime start = DateTime.UtcNow;
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
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            finally
            {
                await WriteMetrics("create-token", start, result);
            }
            return result;
        }
    }
}
