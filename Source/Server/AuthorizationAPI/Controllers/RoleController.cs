using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YardLight.Authorization.Core.Framework;
using YardLight.CommonAPI;
using YardLight.CommonCore;
using YardLight.Interface.Authorization.Models;
using LogAPI = BrassLoon.Interface.Log;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : AuthorizationControllerBase
    {
        private readonly IRoleFactory _roleFactory;
        private readonly IRoleSaver _roleSaver;
        private readonly IMapper _mapper;

        public RoleController(IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            LogAPI.IMetricService metricService,
            LogAPI.IExceptionService exceptionService,
            IMapper mapper,
            IRoleFactory roleFactory,
            IRoleSaver roleSaver)
            : base(settings, settingsFactory, metricService, exceptionService)
        {
            _mapper = mapper;
            _roleFactory = roleFactory;
            _roleSaver = roleSaver;
        }

        [HttpGet()]
        [Authorize()]
        [ProducesResponseType(typeof(List<Role>), 200)]
        public async Task<IActionResult> GetAll()
        {
            IActionResult result = null;
            try
            {
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                IEnumerable<IRole> innerRoles = await _roleFactory.GetAll(settings);
                result = Ok(innerRoles.Select<IRole, Role>(r => _mapper.Map<Role>(r)));
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPost()]
        [Authorize(Constants.POLICY_ROLE_EDIT)]
        [ProducesResponseType(typeof(Role), 200)]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && role == null)
                    result = BadRequest("Missing role data");
                if (result == null && string.IsNullOrEmpty(role.PolicyName))
                    result = BadRequest("Missing PolicyName value");
                if (result == null && string.IsNullOrEmpty(role.Name))
                    result = BadRequest("Missing Name value");
                if (result == null)
                {
                    ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IRole innerRole = _roleFactory.Create(role.PolicyName);
                    _mapper.Map<Role, IRole>(role, innerRole);
                    await _roleSaver.Create(settings, innerRole);
                    result = Ok(_mapper.Map<Role>(innerRole));
                }
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _ = WriteMetrics("create-role", DateTime.UtcNow.Subtract(start).TotalSeconds);
            }
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Constants.POLICY_ROLE_EDIT)]
        [ProducesResponseType(typeof(Role), 200)]
        public async Task<IActionResult> Update([FromRoute] int? id, [FromBody] Role role)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && !id.HasValue)
                    result = BadRequest("Missing role id value");
                if (result == null && role == null)
                    result = BadRequest("Missing role data");
                if (result == null && string.IsNullOrEmpty(role.PolicyName))
                    result = BadRequest("Missing PolicyName value"); 
                if (result == null)
                {
                    ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IRole innerRole = await _roleFactory.Get(settings, id.Value);
                    if (innerRole == null)
                        result = NotFound();
                    if (result == null)
                    {
                        _mapper.Map<Role, IRole>(role, innerRole);
                        await _roleSaver.Update(settings, innerRole);
                        result = Ok(_mapper.Map<Role>(innerRole));
                    }
                }
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _ = WriteMetrics("update-role", DateTime.UtcNow.Subtract(start).TotalSeconds, new Dictionary<string, string> { { nameof(id), id.ToString() } });
            }
            return result;
        }
    }
}
