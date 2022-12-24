using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using YardLight.CommonAPI;
using YardLight.Interface.Models;
using AuthorizationAPI = BrassLoon.Interface.Authorization;
using Log = BrassLoon.Interface.Log;
using System.Linq;

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
            Log.IMetricService metricService,
            Log.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService
            ) : base(settings, settingsFactory, metricService, exceptionService, userService)
        {
            _userService = userService;
        }

        [HttpGet()]
        [Authorize(Constants.POLICY_USER_READ)]
        [ProducesResponseType(typeof(List<User>), 200)]
        public async Task<IActionResult> Search([FromQuery] string emailAddress)
        {
            IActionResult result = null;
            try
            {
                if (result == null && string.IsNullOrEmpty(emailAddress))
                    result = BadRequest("Missing emailAddress parameter value");
                if (result == null)
                {
                    AuthorizationAPI.ISettings settings = _settingsFactory.CreateAuthorization(_settings.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    IEnumerable<AuthorizationAPI.Models.User> innerUsers = await _userService.Search(settings, _settings.Value.AuthorizationDomainId.Value, emailAddress);
                    result = Ok(
                        innerUsers.Select<AuthorizationAPI.Models.User, User>(u => mapper.Map<User>(u))
                        );
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpGet()]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> GetCurrentUser()
        {
            IActionResult result = null;
            try
            {
                if (result == null)
                {
                    AuthorizationAPI.ISettings settings = _settingsFactory.CreateAuthorization(_settings.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    AuthorizationAPI.Models.User innerUser = await _userService.Get(settings, _settings.Value.AuthorizationDomainId.Value);
                    result = Ok(                       
                        mapper.Map<User>(innerUser) 
                        );
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpGet("{id}")]
        [Authorize(Constants.POLICY_USER_READ)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
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
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpGet("{id}/Name")]
        [Authorize(Constants.POLICY_USER_READ)]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetName([FromRoute] Guid? id)
        {
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
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Constants.POLICY_USER_EDIT)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] User user)
        {
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
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }
    }
}
