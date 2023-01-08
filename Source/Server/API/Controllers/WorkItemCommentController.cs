using AutoMapper;
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
    [Route("api/Project/{projectId}/WorkItem/{workItemId}/Comment")]
    [ApiController]
    public class WorkItemCommentController : APIControllerBase
    {
        private readonly IProjectFactory _projectFactory;
        private readonly IWorkItemFactory _workItemFactory;
        private readonly IWorkItemCommentSaver _commentSaver;

        public WorkItemCommentController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            AuthorizationAPI.IUserService userService,
            IProjectFactory projectFactory,
            IWorkItemFactory workItemFactory,
            IWorkItemCommentSaver commentSaver,
            ILogger<WorkItemCommentController> logger
            ) : base(settings, settingsFactory, userService, logger)
        {
            _projectFactory = projectFactory;
            _workItemFactory = workItemFactory;
            _commentSaver = commentSaver;
        }

        [HttpGet()]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> GetByWorkItemId([FromRoute] Guid? projectId, [FromRoute] Guid? workItemId)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                ISettings settings = GetCoreSettings();
                IWorkItem workItem = null;
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
                if (result == null && (!workItemId.HasValue || Guid.Empty.Equals(workItemId.Value)))
                    result = BadRequest("Missing or invalid workItemId route parameter value");
                if (result == null)
                {
                    ValueTuple<IActionResult, IProject> userProject = await GetProjectForCurrentUser(_projectFactory, projectId.Value);
                    result = userProject.Item1;
                }
                if (result == null)
                {
                    workItem = await _workItemFactory.Get(settings, workItemId.Value);
                    if (workItem == null)
                        result = NotFound();
                }
                if (result == null && workItem != null)
                {
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    result = Ok((await workItem.GetComments(settings, WorkItemCommentType.Comment))
                        .Select<IWorkItemComment, Comment>(i => mapper.Map<Comment>(i))
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
                await WriteMetrics("get-project-item-comments", start, result,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty },
                        { "workItemId", workItemId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }

        [HttpPost()]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Create([FromRoute] Guid? projectId, [FromRoute] Guid? workItemId, [FromBody] Comment comment)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                ISettings settings = GetCoreSettings();
                IWorkItem workItem = null;
                if (result == null && (!projectId.HasValue || Guid.Empty.Equals(projectId.Value)))
                    result = BadRequest("Missing or invalid projectId route parameter value");
                if (result == null && (!workItemId.HasValue || Guid.Empty.Equals(workItemId.Value)))
                    result = BadRequest("Missing or invalid workItemId route parameter value");
                if (result == null && string.IsNullOrEmpty(comment?.Text))
                    result = BadRequest("Missing comment text");
                if (result == null)
                {
                    ValueTuple<IActionResult, IProject> userProject = await GetProjectForCurrentUser(_projectFactory, projectId.Value);
                    result = userProject.Item1;
                }
                if (result == null)
                {
                    workItem = await _workItemFactory.Get(settings, workItemId.Value);
                    if (workItem == null)
                        result = NotFound();
                }
                if (result == null && workItem != null)
                {                    
                    IWorkItemComment innerComment = workItem.CreateComment(comment.Text, WorkItemCommentType.Comment);
                    IMapper mapper = new Mapper(MapperConfiguration.Get());
                    await _commentSaver.Create(settings, innerComment, 
                        (await GetCurrentUserId()).Value
                        );
                    result = Ok(
                        mapper.Map<Comment>(innerComment)
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
                await WriteMetrics("create-project-item-comment", start, result,
                    new Dictionary<string, string>
                    {
                        { "projectId", projectId.HasValue ? projectId.Value.ToString("D") : string.Empty },
                        { "workItemId", workItemId.HasValue ? projectId.Value.ToString("D") : string.Empty }
                    }
                    );
            }
            return result;
        }
    }
}
