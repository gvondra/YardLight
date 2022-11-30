using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
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
    public class WorkItemTypeController : APIControllerBase
    {
        private readonly IProjectFactory _projectFactory;
        private readonly IWorkItemTypeFactory _typeFactory;
        private readonly IWorkItemTypeSaver _typeSaver;
        private readonly IWorkItemStatusFactory _statusFactory;

        public WorkItemTypeController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            Log.IMetricService metricService,
            Log.IExceptionService exceptionService,
            AuthorizationAPI.IUserService userService,
            IProjectFactory projectFactory,
            IWorkItemStatusFactory statusFactory,
            IWorkItemTypeFactory typeFactory,
            IWorkItemTypeSaver typeSaver
            ) : base(settings, settingsFactory, metricService, exceptionService, userService)
        {
            _projectFactory = projectFactory;
            _statusFactory = statusFactory;
            _typeFactory = typeFactory;
            _typeSaver = typeSaver;
        }

        [HttpGet("/api/Project/{projectId}/WorkItemType")]
        [Authorize()]
        public async Task<IActionResult> Get([FromRoute] Guid? projectId, [FromQuery] bool? isActive = null)
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
                    IEnumerable<IWorkItemStatus> statuses = await _statusFactory.GetByProjectId(settings, projectId.Value);                        
                    IEnumerable<WorkItemType> types = (await _typeFactory.GetByProjectId(settings, projectId.Value, isActive))
                        .GroupJoin(statuses, t => t.WorkItemTypeId, s => s.WorkItemTypeId, (t, s) => 
                        {
                            WorkItemType result = mapper.Map<WorkItemType>(t);  
                            result.Statuses = (s ?? new List<IWorkItemStatus>()).Select(x => mapper.Map<WorkItemStatus>(x)).ToList();    
                            return result;
                        })
                        ;
                    
                    result = Ok(types);
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-project-types", DateTime.UtcNow.Subtract(start).TotalSeconds,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty },
                        { "isAtive", isActive.HasValue ? isActive.Value.ToString() : string.Empty }
                    }
                    );
            }
            return result;
        }

        [NonAction]
        private IActionResult Validate(WorkItemType type)
        {
            IActionResult result = null;
            if (result == null && type == null)
                result = BadRequest("Missing type body");
            if (result == null && string.IsNullOrEmpty(type.Title))
                result = BadRequest("Missing type title value");
            if (result == null && type.Statuses != null && type.Statuses.Any(s => string.IsNullOrEmpty(s.Title)))
                result = BadRequest("Missing status title value(s)");
            return result;
        }

        [HttpPost("/api/Project/{projectId}/WorkItemType")]
        [Authorize()]
        public async Task<IActionResult> Create([FromRoute] Guid? projectId, [FromBody] WorkItemType type)
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
                    result = Validate(type);
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
                    IWorkItemType innerType = _typeFactory.Create(project.ProjectId);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    mapper.Map<WorkItemType, IWorkItemType>(type, innerType);
                    IEnumerable<IWorkItemStatus> innerStatuses = (type.Statuses ?? new List<WorkItemStatus>())
                        .Select(s =>
                        {
                            IWorkItemStatus innerStatus = innerType.CreateStatus();
                            return mapper.Map<WorkItemStatus, IWorkItemStatus>(s, innerStatus);
                        })
                        .ToList();
                    await _typeSaver.Create(settings, innerType, innerStatuses, currentUserId.Value);
                    type = mapper.Map<WorkItemType>(innerType);
                    type.Statuses = innerStatuses.Select(s => mapper.Map<WorkItemStatus>(s)).ToList();
                    result = Ok(type);
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("create-project-type", DateTime.UtcNow.Subtract(start).TotalSeconds,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [HttpPut("/api/Project/{projectId}/WorkItemType/{id}")]
        [Authorize()]
        public async Task<IActionResult> Update([FromRoute] Guid? projectId, [FromRoute] Guid? id, [FromBody] WorkItemType type)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                Guid? currentUserId = null;
                IProject project = null;
                IWorkItemType innerType = null;
                IWorkItemStatus innerStatus;
                List<IWorkItemStatus> innerStatuses = null;
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
                if (result == null && (!id.HasValue || Guid.Empty.Equals(id.Value)))
                    result = BadRequest("Missing or invalid id route parameter value");
                if (result == null)
                    result = Validate(type);
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
                    innerType = await _typeFactory.Get(settings, id.Value);
                    if (innerType == null)
                        result = NotFound();
                }
                if (result == null && innerType != null)
                {
                    innerStatuses = ((await innerType.GetStatuses(settings)) ?? new List<IWorkItemStatus>()).ToList();
                }
                if (result == null && innerType != null && innerStatuses != null)
                {
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    mapper.Map<WorkItemType, IWorkItemType>(type, innerType);
                    foreach (WorkItemStatus status in type.Statuses ?? new List<WorkItemStatus>())
                    {
                        innerStatus = innerStatuses.FirstOrDefault(s => status.WorkItemStatusId.HasValue && s.WorkItemStatusId.Equals(status.WorkItemStatusId.Value));
                        if (innerStatus == null)
                        {
                            innerStatus = innerType.CreateStatus();
                            innerStatuses.Add(innerStatus);
                        }
                        mapper.Map(status, innerStatus);
                    }
                    await _typeSaver.Update(settings, innerType, innerStatuses, currentUserId.Value);
                    type = mapper.Map<WorkItemType>(innerType);
                    type.Statuses = innerStatuses.Select(s => mapper.Map<WorkItemStatus>(s)).ToList();
                    result = Ok(type);
                }
            }
            catch (System.Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("update-project-type", DateTime.UtcNow.Subtract(start).TotalSeconds,
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
