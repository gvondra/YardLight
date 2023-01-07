﻿using AutoMapper;
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
using YardLight.CommonCore;
using YardLight.Framework;
using YardLight.Interface.Models;
using AuthorizationAPI = BrassLoon.Interface.Authorization;

namespace API.Controllers
{
    [Route("api/v2/Project/{projectId}/[controller]")]
    [ApiController]
    public class ItterationController : APIControllerBase
    {
        private readonly IItterationFactory _itterationFactory;
        private readonly IItterationSaver _itterationSaver;
        private readonly IProjectFactory _projectFactory;

        public ItterationController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            AuthorizationAPI.IUserService userService,
            IItterationFactory itterationFactory,
            IItterationSaver itterationSaver,
            IProjectFactory projectFactory,
            ILogger<WorkItemController> logger
            ) : base(settings, settingsFactory, userService, logger)
        {
            _itterationFactory = itterationFactory;
            _itterationSaver = itterationSaver;
            _projectFactory = projectFactory;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(Itteration[]), 200)]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Search([FromRoute] Guid? projectId)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = null;
                IProject project = null;
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                if (!projectId.HasValue || projectId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing or invalid project id parameter value");
                if (result == null)
                    currentUserId = await GetCurrentUserId(_settingsFactory.CreateAuthorization(_settings.Value));
                if (result == null && !currentUserId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "UserNotFound");
                if (result == null)
                {
                    project = await _projectFactory.Get(settings, currentUserId.Value, projectId.Value);
                    if (project == null)
                        result = NotFound();
                }
                if (result == null)
                {
                    IEnumerable<IItteration> innerItterations = await _itterationFactory.GetByProjectId(settings, projectId.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    result = Ok(
                        innerItterations.Select<IItteration, Itteration>(i => mapper.Map<Itteration>(i))
                        );
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-project-itterations-v2", start, result,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        private IActionResult Validate(Itteration itteration)
        {
            IActionResult result = null;
            if (result == null && string.IsNullOrEmpty(itteration?.Name))
                result = BadRequest("Missing itteration name value");
            return result;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Itteration[]), 200)]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Save([FromRoute] Guid? projectId, [FromRoute] Guid? id, [FromBody] Itteration itteration)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = null;
                IProject project = null;
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                if (!projectId.HasValue || projectId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing or invalid project id parameter value");
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    id = Guid.NewGuid();
                if (result == null)
                    result = Validate(itteration);
                if (result == null)
                    currentUserId = await GetCurrentUserId(_settingsFactory.CreateAuthorization(_settings.Value));
                if (result == null && !currentUserId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "UserNotFound");
                if (result == null)
                {
                    project = await _projectFactory.Get(settings, currentUserId.Value, projectId.Value);
                    if (project == null)
                        result = NotFound();
                }
                if (result == null)
                {
                    Guid itterationId = id.Value;
                    IItteration innerItteration = _itterationFactory.Create(project.ProjectId, ref itterationId);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    mapper.Map<Itteration, IItteration>(itteration, innerItteration);
                    await _itterationSaver.Save(settings, innerItteration, currentUserId.Value);
                    result = Ok(
                        mapper.Map<Itteration>(innerItteration)
                        );
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("save-project-itteration", start, result,
                    new Dictionary<string, string>
                    {
                        { nameof(projectId), projectId.HasValue ? projectId.Value.ToString("D") : string.Empty },
                        { nameof(id), id.HasValue ? id.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }
    }
}