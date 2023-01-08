using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using YardLight.CommonAPI;
using YardLight.CommonCore;
using YardLight.Framework;
using YardLight.Interface.Models;
using AuthorizationAPI = BrassLoon.Interface.Authorization;

namespace API.Controllers
{
    [Route("api/Project/{projectId}/[controller]")]
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
            AuthorizationAPI.IUserService userService,
            IProjectFactory projectFactory,
            IWorkItemStatusFactory statusFactory,
            IWorkItemTypeFactory typeFactory,
            IWorkItemTypeSaver typeSaver,
            ILogger<WorkItemTypeController> logger
            ) : base(settings, settingsFactory, userService, logger)
        {
            _projectFactory = projectFactory;
            _statusFactory = statusFactory;
            _typeFactory = typeFactory;
            _typeSaver = typeSaver;
        }

        [HttpGet()]
        [Authorize(Constants.POLICY_BL_AUTH)]
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
                    ValueTuple<IActionResult, IProject> userProject = await GetProjectForCurrentUser(_projectFactory, projectId.Value);
                    result = userProject.Item1;
                }
                if (result == null)
                {
                    ISettings settings = GetCoreSettings();
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    IEnumerable<IWorkItemStatus> statuses = await _statusFactory.GetByProjectId(settings, projectId.Value, skipCache: true);                        
                    IEnumerable<WorkItemType> types = (await _typeFactory.GetByProjectId(settings, projectId.Value, isActive))
                        .GroupJoin(statuses, t => t.WorkItemTypeId, s => s.WorkItemTypeId, (t, s) => 
                        {
                            WorkItemType result = mapper.Map<WorkItemType>(t);  
                            result.Statuses = (s ?? new List<IWorkItemStatus>()).Select(x => mapper.Map<WorkItemStatus>(x)).ToList();    
                            return result;
                        })
                        .ToList();
                    
                    result = Ok(types);
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("get-project-types", start, result,
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

        [HttpPost()]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Create([FromRoute] Guid? projectId, [FromBody] WorkItemType type)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                IProject project = null;
                ISettings settings = GetCoreSettings();
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
                if (result == null)
                    result = Validate(type);
                if (result == null)
                {
                    ValueTuple<IActionResult, IProject> userProject = await GetProjectForCurrentUser(_projectFactory, projectId.Value);
                    result = userProject.Item1;
                    project = userProject.Item2;
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
                    await _typeSaver.Create(settings, innerType, innerStatuses, 
                        (await GetCurrentUserId()).Value
                        );
                    type = mapper.Map<WorkItemType>(innerType);
                    type.Statuses = innerStatuses.Select(s => mapper.Map<WorkItemStatus>(s)).ToList();
                    result = Ok(type);
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("create-project-type", start, result,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Update([FromRoute] Guid? projectId, [FromRoute] Guid? id, [FromBody] WorkItemType type)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                IProject project = null;
                IWorkItemType innerType = null;
                IWorkItemStatus innerStatus;
                List<IWorkItemStatus> innerStatuses = null;
                ISettings settings = GetCoreSettings();
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
                if (result == null && (!id.HasValue || Guid.Empty.Equals(id.Value)))
                    result = BadRequest("Missing or invalid id route parameter value");
                if (result == null)
                    result = Validate(type);
                if (result == null)
                {
                    ValueTuple<IActionResult, IProject> userProject = await GetProjectForCurrentUser(_projectFactory, projectId.Value);
                    result = userProject.Item1;
                    project = userProject.Item2;
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
                    await _typeSaver.Update(settings, innerType, innerStatuses, 
                        (await GetCurrentUserId()).Value
                        
                        );
                    type = mapper.Map<WorkItemType>(innerType);
                    type.Statuses = innerStatuses.Select(s => mapper.Map<WorkItemStatus>(s)).ToList();
                    result = Ok(type);
                }
            }
            catch (System.Exception ex)
            {
                WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                await WriteMetrics("update-project-type", start, result,
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
