using AutoMapper;
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
using AuthorizationAPI = YardLight.Interface.Authorization;
using Log = BrassLoon.Interface.Log;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkItemStatusController : APIControllerBase
    {
        private readonly IProjectFactory _projectFactory;
        private readonly IWorkItemStatusFactory _statusFactory;
        private readonly IWorkItemStatusSaver _statusSaver;

        public WorkItemStatusController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService,
            Log.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService,
            IProjectFactory projectFactory,
            IWorkItemStatusFactory statusFactory,
            IWorkItemStatusSaver statusSaver
            ) : base(settings, settingsFactory, metricService, exceptionService, userService)
        { 
            _projectFactory = projectFactory;
            _statusFactory = statusFactory;
            _statusSaver = statusSaver;
        }

        [HttpGet("/api/Project/{projectId}/WorkItemStatus")]
        [Authorize()]
        public async Task<IActionResult> Get([FromRoute] Guid? projectId, [FromQuery] bool? isAtive = null)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
                if (result == null)
                {
                    ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    result = Ok(
                        (await _statusFactory.GetByProjectId(settings, projectId.Value, isAtive))
                        .Select<IWorkItemStatus, WorkItemStatus>(s => mapper.Map<WorkItemStatus>(s))
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
                await WriteMetrics("get-project-statuses", DateTime.UtcNow.Subtract(start).TotalSeconds, 
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty },
                        { "isAtive", isAtive.HasValue ? isAtive.Value.ToString() : string.Empty }
                    }
                    );
            }
            return result;
        }

        [NonAction]
        private IActionResult Validate(WorkItemStatus status)
        {
            IActionResult result = null;
            if (result == null && status == null)
                result = BadRequest("Missing status body");
            if (result == null && string.IsNullOrEmpty(status.Title))
                result = BadRequest("Missing status title value");
            return result;
        }

        [HttpPost("/api/Project/{projectId}/WorkItemStatus")]
        [Authorize()]
        public async Task<IActionResult> Create([FromRoute] Guid? projectId, [FromBody] WorkItemStatus status)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = null;
                IProject project = null;
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
                if (result == null)
                    result = Validate(status);
                if (result == null)
                    currentUserId = await GetCurrentUserId(_settingsFactory.CreateAuthorization(_settings.Value, GetUserToken()));
                if (result == null && !currentUserId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "UserNotFound");
                if (result == null)
                {                    
                    project = await _projectFactory.Get(settings, currentUserId.Value, projectId.Value);
                    if (project == null)
                        result = NotFound();
                }
                if (result == null && project != null)
                {
                    IWorkItemStatus innerStatus = _statusFactory.Create(project.ProjectId);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    mapper.Map<WorkItemStatus, IWorkItemStatus>(status, innerStatus);
                    await _statusSaver.Create(settings, innerStatus, currentUserId.Value);
                    result = Ok(
                        mapper.Map<WorkItemStatus>(innerStatus)
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
                await WriteMetrics("create-project-status", DateTime.UtcNow.Subtract(start).TotalSeconds,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [HttpPut("/api/Project/{projectId}/WorkItemStatus/{id}")]
        [Authorize()]
        public async Task<IActionResult> Update([FromRoute] Guid? projectId, [FromRoute] Guid? id, [FromBody] WorkItemStatus status)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = null;
                IProject project = null;
                IWorkItemStatus innerStatus = null;
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
                if (result == null && (!id.HasValue || Guid.Empty.Equals(id.Value)))
                    result = BadRequest("Missing or invalid id route parameter value");
                if (result == null)
                    result = Validate(status);
                if (result == null)
                    currentUserId = await GetCurrentUserId(_settingsFactory.CreateAuthorization(_settings.Value, GetUserToken()));
                if (result == null && !currentUserId.HasValue)
                    result = StatusCode(StatusCodes.Status500InternalServerError, "UserNotFound");
                if (result == null)
                {
                    project = await _projectFactory.Get(settings, currentUserId.Value, projectId.Value);
                    if (project == null)
                        result = NotFound();
                }
                if (result == null && project != null)
                {
                    innerStatus = await _statusFactory.Get(settings, id.Value);
                    if (innerStatus == null)
                        result = NotFound();
                }
                if (result == null && innerStatus != null)
                {
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    mapper.Map<WorkItemStatus, IWorkItemStatus>(status, innerStatus);
                    await _statusSaver.Update(settings, innerStatus, currentUserId.Value);
                    result = Ok(
                        mapper.Map<WorkItemStatus>(innerStatus)
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
                await WriteMetrics("update-project-status", DateTime.UtcNow.Subtract(start).TotalSeconds,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty },
                        { "id", id.HasValue ? id.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

    }
}
