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
using YardLight.Framework.Enumerations;
using YardLight.Interface.Models;
using AuthorizationAPI = BrassLoon.Interface.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkItemController : APIControllerBase
    {
        private readonly IItterationFactory _itterationFactory;
        private readonly IProjectFactory _projectFactory;
        private readonly IWorkItemFactory _workItemFactory;
        private readonly IWorkItemSaver _workItemSaver;
        private readonly IWorkItemStatusFactory _statusFactory;
        private readonly IWorkItemTypeFactory _typeFactory;

        public WorkItemController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            AuthorizationAPI.IUserService userService,
            IItterationFactory itterationFactory,
            IProjectFactory projectFactory,
            IWorkItemFactory workItemFactory,
            IWorkItemSaver workItemSaver,
            IWorkItemStatusFactory statusFactory,
            IWorkItemTypeFactory typeFactory,
            ILogger<WorkItemController> logger
            ) : base(settings, settingsFactory, userService, logger)
        {
            _itterationFactory = itterationFactory;
            _projectFactory = projectFactory;
            _workItemFactory = workItemFactory;
            _workItemSaver = workItemSaver;
            _statusFactory = statusFactory;
            _typeFactory = typeFactory;
        }

        [HttpGet("/api/Project/{projectId}/WorkItem")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Search(
            [FromRoute] Guid? projectId, 
            [FromQuery] Guid[] parentIds,
            [FromQuery] Guid? workItemTypeId,
            [FromQuery] string team,
            [FromQuery] string itteration)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                Guid? currentUserId = null;
                IProject project = null;
                IEnumerable<IWorkItem> innerWorkItems = null;
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
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
                if (result == null && project != null && innerWorkItems == null && workItemTypeId.HasValue && !workItemTypeId.Value.Equals(Guid.Empty))
                {
                    innerWorkItems = await _workItemFactory.GetByProjectIdTypeId(settings, projectId.Value, workItemTypeId.Value, team: team, itteration: itteration);
                }
                if (result == null && project != null && innerWorkItems == null && parentIds != null && parentIds.Length > 0)
                {
                    innerWorkItems = await _workItemFactory.GetByParentIds(settings, parentIds);
                }
                if (result == null && project != null && innerWorkItems == null)
                {
                    innerWorkItems = (await _workItemFactory.GetByProjectId(settings, projectId.Value))
                        .Where(i => !i.ParentWorkItemId.HasValue);
                }
                if (result == null && project != null && innerWorkItems != null)
                {
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    result = Ok(await Task.WhenAll(
                        innerWorkItems.Select<IWorkItem, Task<WorkItem>>(i => Map(settings, mapper, i))
                        ));
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-project-items", start, result,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty },
                        { "parentIdLength", parentIds != null ? parentIds.Length.ToString() : string.Empty }
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
            workItem.Description = await innerWorkItem.GetComment(settings, WorkItemCommentType.Description);
            workItem.Criteria = await innerWorkItem.GetComment(settings, WorkItemCommentType.Criteria);
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
        [Authorize(Constants.POLICY_BL_AUTH)]
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
                    currentUserId = await GetCurrentUserId(_settingsFactory.CreateAuthorization(_settings.Value));
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
                    await innerWorkItem.SetComment(settings, WorkItemCommentType.Description, workItem.Description ?? string.Empty);
                    await innerWorkItem.SetComment(settings, WorkItemCommentType.Criteria, workItem.Criteria ?? string.Empty);
                    await _workItemSaver.Create(settings, innerWorkItem, currentUserId.Value);
                    result = Ok(await Map(settings, mapper, innerWorkItem));
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("create-project-item", start, result,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [HttpPut("/api/Project/{projectId}/WorkItem/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
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
                    await innerWorkItem.SetComment(settings, WorkItemCommentType.Description, workItem.Description ?? string.Empty);
                    await innerWorkItem.SetComment(settings, WorkItemCommentType.Criteria, workItem.Criteria ?? string.Empty);
                    await _workItemSaver.Update(settings, innerWorkItem, currentUserId.Value);
                    result = Ok(await Map(settings, mapper, innerWorkItem));
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("update-project-item", start, result,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [HttpGet("/api/Project/{projectId}/Itteration")]
        [ProducesResponseType(typeof(string[]), 200)]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [Obsolete()]
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
                        (await _itterationFactory.GetByProjectId(settings, projectId.Value))
                        .Select<IItteration, string>(i => i.Name)
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
                await WriteMetrics("get-project-itterations", start, result,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [HttpGet("/api/Project/{projectId}/Team")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> GetTeams([FromRoute] Guid? projectId)
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
                    result = Ok(
                        await _workItemFactory.GetTeamsByProjectId(settings, project.ProjectId)
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
                await WriteMetrics("get-project-teams", start, result,
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
