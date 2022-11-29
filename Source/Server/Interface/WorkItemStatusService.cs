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
    public class WorkItemStatusService : IWorkItemStatusService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public WorkItemStatusService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<WorkItemStatus> Create(ISettings settings, WorkItemStatus status)
        {
            if (!status.ProjectId.HasValue)
                throw new ArgumentNullException(nameof(status.ProjectId));
            return Create(settings, status.ProjectId.Value, status);
        }

        public async Task<WorkItemStatus> Create(ISettings settings, Guid projectId, WorkItemStatus status)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, status)
                .AddPath("Project/{projectId}/WorkItemStatus")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<WorkItemStatus> response = await _service.Send<WorkItemStatus>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<List<WorkItemStatus>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Project/{projectId}/WorkItemStatus")
                .AddPathParameter("projectId", projectId.ToString("N"))                
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            if (isActive.HasValue)
                request.AddQueryParameter("isActive", isActive.Value.ToString());
            IResponse<List<WorkItemStatus>> response = await _service.Send<List<WorkItemStatus>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<WorkItemStatus> Update(ISettings settings, WorkItemStatus status)
        {
            if (!status.ProjectId.HasValue)
                throw new ArgumentNullException(nameof(status.ProjectId));
            if (!status.WorkItemStatusId.HasValue)
                throw new ArgumentNullException(nameof(status.WorkItemStatusId));
            return Update(settings, status.ProjectId.Value, status.WorkItemStatusId.Value, status);
        }

        public async Task<WorkItemStatus> Update(ISettings settings, Guid projectId, Guid id, WorkItemStatus status)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, status)
                .AddPath("Project/{projectId}/WorkItemStatus/{id}")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddPathParameter("id", id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<WorkItemStatus> response = await _service.Send<WorkItemStatus>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }
    }
}
