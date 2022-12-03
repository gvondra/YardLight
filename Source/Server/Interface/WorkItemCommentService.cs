using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public class WorkItemCommentService : IWorkItemCommentService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public WorkItemCommentService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<Comment> Create(ISettings settings, Guid projectId, Guid workItemId, Comment comment)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, comment)
                .AddPath("Project/{projectId}/WorkItem/{workItemId}/Comment")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddPathParameter("workItemId", workItemId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Comment>(_service, request);
        }

        public Task<List<Comment>> GetByWorkItemId(ISettings settings, Guid projectId, Guid workItemId)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Project/{projectId}/WorkItem/{workItemId}/Comment")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddPathParameter("workItemId", workItemId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<Comment>>(_service, request);
        }
    }
}
