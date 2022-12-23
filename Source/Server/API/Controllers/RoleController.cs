using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using YardLight.CommonAPI;
using YardLight.Interface.Models;
using AuthorizationAPI = BrassLoon.Interface.Authorization;
using Log = BrassLoon.Interface.Log;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : APIControllerBase
    {
        private readonly AuthorizationAPI.IRoleService _roleService;
        public RoleController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService,
            Log.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService,
            AuthorizationAPI.IRoleService roleService
            ) : base(settings, settingsFactory, metricService, exceptionService, userService)
        {
            _roleService = roleService;
        }

        [HttpGet()]
        [Authorize(Constants.POLICY_ROLE_EDIT)]
        [ProducesResponseType(typeof(List<Role>), 200)]
        public async Task<IActionResult> GetByDomainId()
        {
            IActionResult result = null;
            try
            {
                if (result == null)
                {
                    AuthorizationAPI.ISettings settings = _settingsFactory.CreateAuthorization(_settings.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    result = Ok(
                        (await _roleService.GetByDomainId(settings, _settings.Value.AuthorizationDomainId.Value))
                        .Select<AuthorizationAPI.Models.Role, Role>(r => mapper.Map<Role>(r))
                        );
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        private IActionResult ValidateCreate(Role role)
        {
            IActionResult result = Validate(role);
            if (result == null && string.IsNullOrEmpty(role.PolicyName))
                result = BadRequest("Missing role policy name value");
            return result;
        }

        [NonAction]
        private IActionResult Validate(Role role)
        {
            IActionResult result = null;
            if (result == null && role == null)
                result = BadRequest("Missing role data body");
            if (result == null && string.IsNullOrEmpty(role.Name))
                result = BadRequest("Missing role name value");
            return result;
        }

        [HttpPost()]
        [Authorize(Constants.POLICY_ROLE_EDIT)]
        [ProducesResponseType(typeof(Role), 200)]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            IActionResult result = null;
            try
            {
                if (result == null)
                    result = ValidateCreate(role);
                if (result == null)
                {
                    AuthorizationAPI.ISettings settings = _settingsFactory.CreateAuthorization(_settings.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    AuthorizationAPI.Models.Role innerRole = mapper.Map<AuthorizationAPI.Models.Role>(role);
                    innerRole = await _roleService.Create(settings, _settings.Value.AuthorizationDomainId.Value, innerRole);
                    result = Ok(
                        mapper.Map<Role>(innerRole)
                        );
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Constants.POLICY_ROLE_EDIT)]
        [ProducesResponseType(typeof(Role), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] Role role)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid role id parameter value");
                if (result == null)
                    result = Validate(role);
                if (result == null)
                {
                    AuthorizationAPI.ISettings settings = _settingsFactory.CreateAuthorization(_settings.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    AuthorizationAPI.Models.Role innerRole = mapper.Map<AuthorizationAPI.Models.Role>(role);
                    innerRole = await _roleService.Update(settings, _settings.Value.AuthorizationDomainId.Value, id.Value, innerRole);
                    result = Ok(
                        mapper.Map<Role>(innerRole)
                        );
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
