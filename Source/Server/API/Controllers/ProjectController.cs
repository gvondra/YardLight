﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Framework;
using YardLight.Interface.Models;
using AuthorizationAPI = BrassLoon.Interface.Authorization;
using Log = BrassLoon.Interface.Log;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : APIControllerBase
    {
        private readonly IProjectFactory _projectFactory;
        private readonly IProjectSaver _projectSaver;

        public ProjectController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService,
            Log.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService,
            IProjectFactory projectFactory,
            IProjectSaver projectSaver
            ) : base(settings, settingsFactory, metricService, exceptionService, userService)
        {
            _projectFactory = projectFactory;
            _projectSaver = projectSaver;
        }

        [HttpGet()]
        [Authorize()]
        [ProducesResponseType(typeof(Project[]), 200)]
        public async Task<IActionResult> Search()
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = await GetCurrentUserId(_settingsFactory.CreateAuthorization(_settings.Value));
                if (result == null && !currentUserId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "UserNotFound");
                if (result == null)
                {
                    IEnumerable<IProject> projects = await _projectFactory.GetByUserId(_settingsFactory.CreateCore(_settings.Value), currentUserId.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    result = Ok(
                        projects.Select<IProject, Project>(p => mapper.Map<Project>(p))
                        );
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("search-project", DateTime.UtcNow.Subtract(start).TotalSeconds);
            }
            return result;
        }

        [HttpGet("{id}")]
        [Authorize()]
        [ProducesResponseType(typeof(Project), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = null;
                if (result == null && !id.HasValue || id.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing id parameter value");
                if (result == null)
                    currentUserId = await GetCurrentUserId(_settingsFactory.CreateAuthorization(_settings.Value));
                if (result == null && !currentUserId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "UserNotFound");                
                if (result == null)
                {
                    IProject project = await _projectFactory.Get(_settingsFactory.CreateCore(_settings.Value), currentUserId.Value, id.Value);
                    if (project == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = new Mapper(MapperConfiguration.Get());
                        result = Ok(mapper.Map<Project>(project));
                    }
                }

            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-project", DateTime.UtcNow.Subtract(start).TotalSeconds, new Dictionary<string, string> { { nameof(id), id?.ToString("D") } });
            }
            return result;
        }

        [HttpPost()]
        [Authorize()]
        [ProducesResponseType(typeof(Project), 200)]
        public async Task<IActionResult> Create([FromBody] Project project)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = null;
                if (result == null && project == null)
                    result = BadRequest("Missing project data body");
                if (result == null && string.IsNullOrEmpty(project.Title))
                    result = BadRequest("Missing project title value");
                if (result == null)
                    currentUserId = await GetCurrentUserId(_settingsFactory.CreateAuthorization(_settings.Value));
                if (result == null && !currentUserId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "UserNotFound");
                if (result == null)
                {
                    ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IProject innerProject = _projectFactory.Create(project.Title);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    mapper.Map<Project, IProject>(project, innerProject);
                    await _projectSaver.Create(settings, innerProject, currentUserId.Value);
                    result = Ok(mapper.Map<Project>(innerProject));
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("create-project", DateTime.UtcNow.Subtract(start).TotalSeconds);
            }
            return result;
        }

        [HttpPut("{id}")]
        [Authorize()]
        [ProducesResponseType(typeof(Project), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] Project project)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = null;
                if (result == null && !id.HasValue || id.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing id parameter value");
                if (result == null && project == null)
                    result = BadRequest("Missing project data body");
                if (result == null && string.IsNullOrEmpty(project.Title))
                    result = BadRequest("Missing project title value");
                if (result == null)
                    currentUserId = await GetCurrentUserId(_settingsFactory.CreateAuthorization(_settings.Value));
                if (result == null && !currentUserId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "UserNotFound");
                if (result == null)
                {
                    ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IProject innerProject = await _projectFactory.Get(settings, currentUserId.Value, id.Value);
                    if (innerProject == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = new Mapper(MapperConfiguration.Get());
                        mapper.Map<Project, IProject>(project, innerProject);
                        await _projectSaver.Update(settings, innerProject);
                        result = Ok(mapper.Map<Project>(innerProject));
                    }
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("update-project", DateTime.UtcNow.Subtract(start).TotalSeconds);
            }
            return result;
        }
    }
}
