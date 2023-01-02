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
using YardLight.CommonAPI;
using YardLight.Interface.Models;
using AuthorizationAPI = BrassLoon.Interface.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : APIControllerBase
    {
        private readonly AuthorizationAPI.IUserService _userService;

        public UserController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            AuthorizationAPI.IUserService userService,
            ILogger<UserController> logger
            ) : base(settings, settingsFactory, userService, logger)
        {
            _userService = userService;
        }

        [HttpGet()]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<User>), 200)]
        public async Task<IActionResult> Search([FromQuery] string emailAddress)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                AuthorizationAPI.ISettings settings = _settingsFactory.CreateAuthorization(_settings.Value);
                AuthorizationAPI.Models.User innerUser = null;
                List<AuthorizationAPI.Models.User> innerUsers = null;
                if (result == null && !string.IsNullOrEmpty(emailAddress) && !UserHasRole(Constants.POLICY_USER_READ))
                {
                    result = Unauthorized();
                }
                if (result == null && innerUser == null && innerUsers == null && !string.IsNullOrEmpty(emailAddress))
                {
                    innerUsers = await _userService.Search(settings, _settings.Value.AuthorizationDomainId.Value, emailAddress);
                }
                if (result == null && innerUser == null && innerUsers == null)
                {
                    innerUser = await GetCurrentUser(settings, _settings.Value.AuthorizationDomainId.Value);
                }
                if (result == null && innerUser != null && innerUsers == null)
                {
                    innerUsers = new List<AuthorizationAPI.Models.User> { innerUser };
                }
                if (result == null && innerUsers != null)
                {                    
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    result = Ok(
                        innerUsers.Select<AuthorizationAPI.Models.User, User>(u => mapper.Map<User>(u))
                        );
                }
                if (result == null)
                {
                    result = Ok(new List<User>());
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            finally
            {
                await WriteMetrics("search-users", start, result,
                    new Dictionary<string, string>
                    {
                        { nameof(emailAddress), emailAddress ?? string.Empty }
                    });
            }
            return result;
        }

        [HttpGet("{id}")]
        [Authorize(Constants.POLICY_USER_READ)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid id parameter value");
                if (result == null)
                {
                    AuthorizationAPI.ISettings settings = _settingsFactory.CreateAuthorization(_settings.Value);
                    AuthorizationAPI.Models.User innerUser = await _userService.Get(settings, _settings.Value.AuthorizationDomainId.Value, id.Value);
                    if (innerUser == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = new Mapper(MapperConfiguration.Get());
                        result = Ok(
                        mapper.Map<User>(innerUser)
                        );
                    }                    
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            finally
            {
                await WriteMetrics("get-user", start, result);
            }
            return result;
        }

        [HttpGet("{id}/Name")]
        [Authorize(Constants.POLICY_USER_READ)]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetName([FromRoute] Guid? id)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid id parameter value");
                if (result == null)
                {
                    AuthorizationAPI.ISettings settings = _settingsFactory.CreateAuthorization(_settings.Value);
                    AuthorizationAPI.Models.User innerUser = await _userService.Get(settings, _settings.Value.AuthorizationDomainId.Value, id.Value);
                    if (innerUser == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        result = Ok(innerUser.Name);
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            finally
            {
                await WriteMetrics("get-user-name", start, result);
            }
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Constants.POLICY_USER_EDIT)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] User user)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid id parameter value");
                if (result == null && string.IsNullOrEmpty(user?.Name))
                    result = BadRequest("Missing user name value");
                if (result == null)
                {
                    AuthorizationAPI.ISettings settings = _settingsFactory.CreateAuthorization(_settings.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    AuthorizationAPI.Models.User innerUser = mapper.Map<AuthorizationAPI.Models.User>(user);
                    innerUser = await _userService.Update(settings, _settings.Value.AuthorizationDomainId.Value, id.Value, innerUser);
                    result = Ok(
                        mapper.Map<User>(innerUser)
                        );
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            finally
            {
                await WriteMetrics("update-user", start, result);
            }
            return result;
        }
    }
}
