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
    public class WorkItemService : IWorkItemService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public WorkItemService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<WorkItem> Create(ISettings settings, Guid projectId, WorkItem workItem)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, workItem)
                .AddPath("Project/{projectId}/WorkItem")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<WorkItem> response = await _service.Send<WorkItem>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<WorkItem> Create(ISettings settings, WorkItem workItem)
        {
            if (!workItem.ProjectId.HasValue)
                throw new ArgumentNullException(nameof(workItem.ProjectId));
            return Create(settings, workItem.ProjectId.Value, workItem);
        }

        public async Task<List<WorkItem>> GetByProjectId(ISettings settings, Guid projectId)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Project/{projectId}/WorkItem")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<List<WorkItem>> response = await _service.Send<List<WorkItem>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<WorkItem> Update(ISettings settings, Guid projectId, Guid id, WorkItem workItem)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, workItem)
                .AddPath("Project/{projectId}/WorkItem/{id}")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddPathParameter("id", id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<WorkItem> response = await _service.Send<WorkItem>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<WorkItem> Update(ISettings settings, WorkItem workItem)
        {
            if (!workItem.WorkItemId.HasValue)
                throw new ArgumentNullException(nameof(workItem.WorkItemId));
            if (!workItem.ProjectId.HasValue)
                throw new ArgumentNullException(nameof(workItem.ProjectId));
            return Update(settings, workItem.ProjectId.Value, workItem.WorkItemId.Value, workItem);
        }
    }
}
