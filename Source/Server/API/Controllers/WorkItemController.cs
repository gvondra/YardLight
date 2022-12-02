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
    public class WorkItemController : APIControllerBase
    {
        private readonly IProjectFactory _projectFactory;
        private readonly IWorkItemFactory _workItemFactory;
        private readonly IWorkItemSaver _workItemSaver;
        private readonly IWorkItemStatusFactory _statusFactory;
        private readonly IWorkItemTypeFactory _typeFactory;

        public WorkItemController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService,
            Log.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService,
            IProjectFactory projectFactory,
            IWorkItemFactory workItemFactory,
            IWorkItemSaver workItemSaver,
            IWorkItemStatusFactory statusFactory,
            IWorkItemTypeFactory typeFactory
            ) : base(settings, settingsFactory, metricService, exceptionService, userService)
        {
            _projectFactory = projectFactory;
            _workItemFactory = workItemFactory;
            _workItemSaver = workItemSaver;
            _statusFactory = statusFactory;
            _typeFactory = typeFactory;
        }

        [HttpGet("/api/Project/{projectId}/WorkItem")]
        [Authorize()]
        public async Task<IActionResult> GetByProjectId([FromRoute] Guid? projectId)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                Guid? currentUserId = null;
                IProject project = null;
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
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
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    result = Ok(await Task.WhenAll(
                        (await _workItemFactory.GetByProjectId(settings, projectId.Value))
                        .Select<IWorkItem, Task<WorkItem>>(i => Map(settings, mapper, i))
                        ));
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-project-items", DateTime.UtcNow.Subtract(start).TotalSeconds,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [NonAction]
        private async Task<WorkItem> Map(ISettings settings, IMapper mapper, IWorkItem innerWorkItem)
        {
            WorkItem workItem = mapper.Map<WorkItem>(innerWorkItem);
            IWorkItemType workItemType = await innerWorkItem.GetType(settings);
            workItem.Type = mapper.Map<WorkItemType>(workItemType);
            workItem.Type.Statuses = (await workItemType.GetStatuses(settings))
                .Select<IWorkItemStatus, WorkItemStatus>(s => mapper.Map<WorkItemStatus>(s))
                .ToList()
                ;
            workItem.Status = mapper.Map<WorkItemStatus>(await innerWorkItem.GetStatus(settings));
            return workItem;
        }

        [NonAction]
        private IActionResult Validate(WorkItem workItem)
        {
            IActionResult result = null;
            if (result == null && workItem == null)
                result = BadRequest("Missing work item body");
            if (result == null && string.IsNullOrEmpty(workItem.Title))
                result = BadRequest("Missing work item title value");
            if (result == null && workItem.Type?.WorkItemTypeId == null)
                result = BadRequest("Missing work item type");
            if (result == null && workItem.Status?.WorkItemStatusId == null)
                result = BadRequest("Missing work item status");
            return result;
        }

        [HttpPost("/api/Project/{projectId}/WorkItem")]
        [Authorize()]
        public async Task<IActionResult> Create([FromRoute] Guid? projectId, [FromBody] WorkItem workItem)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = null;
                IProject project = null;
                IWorkItemStatus innerStatus = null;
                IWorkItemType innerType = null;
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
                if (result == null)
                    result = Validate(workItem);
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
                    innerType = await _typeFactory.Get(settings, workItem.Type.WorkItemTypeId.Value);
                    if (innerType == null)
                        result = BadRequest($"Work item type \"{workItem?.Type?.Title}\" not found for id {workItem?.Type?.WorkItemTypeId}");
                }
                if (result == null && project != null && innerType != null)
                {
                    innerStatus = await _statusFactory.Get(settings, workItem.Status.WorkItemStatusId.Value);
                    if (innerStatus == null)
                        result = BadRequest($"Work item status \"{workItem?.Status?.Title}\" not found for id {workItem?.Status?.WorkItemStatusId}");
                }
                if (result == null && project != null && innerType != null && innerStatus != null)
                {
                    IWorkItem innerWorkItem = _workItemFactory.Create(project.ProjectId, innerType, innerStatus);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    mapper.Map(workItem, innerWorkItem);
                    await _workItemSaver.Create(settings, innerWorkItem, currentUserId.Value);
                    result = Ok(await Map(settings, mapper, innerWorkItem));
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("create-project-item", DateTime.UtcNow.Subtract(start).TotalSeconds,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [HttpPut("/api/Project/{projectId}/WorkItem/{id}")]
        [Authorize()]
        public async Task<IActionResult> Update([FromRoute] Guid? projectId, [FromRoute] Guid? id, [FromBody] WorkItem workItem)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = null;
                IProject project = null;
                IWorkItem innerWorkItem = null;
                IWorkItemStatus innerStatus = null;
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
                if (result == null && (!id.HasValue || Guid.Empty.Equals(id.Value)))
                    result = BadRequest("Missing or invalid id route parameter value");
                if (result == null)
                    result = Validate(workItem);
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
                if (result == null)
                {
                    innerWorkItem = await _workItemFactory.Get(settings, id.Value);
                    if (innerWorkItem == null)
                        result = NotFound();
                }
                if (result == null)
                {
                    innerStatus = await _statusFactory.Get(settings, workItem.Status.WorkItemStatusId.Value);
                    if (innerStatus == null)
                        result = BadRequest($"Work item status \"{workItem?.Status?.Title}\" not found for id {workItem?.Status?.WorkItemStatusId}");
                }
                if (result == null && innerWorkItem != null && innerStatus != null)
                {
                    innerWorkItem.SetStatus(innerStatus);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    mapper.Map(workItem, innerWorkItem);
                    await _workItemSaver.Update(settings, innerWorkItem, currentUserId.Value);
                    result = Ok(await Map(settings, mapper, innerWorkItem));
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("update-project-item", DateTime.UtcNow.Subtract(start).TotalSeconds,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [HttpGet("/api/Project/{projectId}/Itteration")]
        [Authorize()]
        public async Task<IActionResult> GetItterations([FromRoute] Guid? projectId)
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
                    result = Ok(
                        await _workItemFactory.GetItterationsByProjectId(settings, projectId.Value)
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
                await WriteMetrics("get-project-itterations", DateTime.UtcNow.Subtract(start).TotalSeconds,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [HttpGet("/api/Project/{projectId}/Team")]
        [Authorize()]
        public async Task<IActionResult> GetTeams([FromRoute] Guid? projectId)
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
                    result = Ok(
                        await _workItemFactory.GetTeamsByProjectId(settings, projectId.Value)
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
                await WriteMetrics("get-project-teams", DateTime.UtcNow.Subtract(start).TotalSeconds,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }
    }
}
