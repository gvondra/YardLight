﻿using AutoMapper;
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
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<User>), 200)]
        public async Task<IActionResult> Search([FromQuery] string emailAddress)
        {
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
                    innerUser = await _userService.Get(settings, _settings.Value.AuthorizationDomainId.Value);
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
