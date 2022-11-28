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
    public class WorkItemTypeService : IWorkItemTypeService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public WorkItemTypeService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<WorkItemType> Create(ISettings settings, WorkItemType type)
        {
            if (!type.ProjectId.HasValue)
                throw new ArgumentNullException(nameof(type.ProjectId));
            return Create(settings, type.ProjectId.Value, type);
        }

        public async Task<WorkItemType> Create(ISettings settings, Guid projectId, WorkItemType type)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, type)
                .AddPath("Project/{projectId}/WorkItemType")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<WorkItemType> response = await _service.Send<WorkItemType>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<List<WorkItemType>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Project/{projectId}/WorkItemType")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            if (isActive.HasValue)
                request.AddQueryParameter("isActive", isActive.Value.ToString());
            IResponse<List<WorkItemType>> response = await _service.Send<List<WorkItemType>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<WorkItemType> Update(ISettings settings, WorkItemType type)
        {
            if (!type.ProjectId.HasValue)
                throw new ArgumentNullException(nameof(type.ProjectId));
            if (!type.WorkItemTypeId.HasValue)
                throw new ArgumentNullException(nameof(type.WorkItemTypeId));
            return Update(settings, type.ProjectId.Value, type.WorkItemTypeId.Value, type);
        }

        public async Task<WorkItemType> Update(ISettings settings, Guid projectId, Guid id, WorkItemType type)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, type)
                .AddPath("Project/{projectId}/WorkItemType/{id}")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddPathParameter("id", id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<WorkItemType> response = await _service.Send<WorkItemType>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }
    }
}
