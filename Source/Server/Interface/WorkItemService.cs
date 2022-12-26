using BrassLoon.RestClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
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
        private readonly static Policy _cacheItteration = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(12));
        private readonly static Policy _cacheTeam = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(12));
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
            return await _restUtil.Send<WorkItem>(_service, request);
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
            return await _restUtil.Send<List<WorkItem>>(_service, request);
        }

        public Task<List<string>> GetItterationsByProjectId(ISettings settings, Guid projectId)
        {
            return _cacheItteration.Execute((context) => InnerGetItterationsByProjectId(settings, projectId),
                new Context(projectId.ToString("N")));
        }
        
        private Task<List<string>> InnerGetItterationsByProjectId(ISettings settings, Guid projectId)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Project/{projectId}/Itteration")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<string>>(_service, request);
        }

        public Task<List<string>> GetTeamsByProjectId(ISettings settings, Guid projectId)
        {
            return _cacheTeam.Execute((context) => InnerGetTeamsByProjectId(settings, projectId),
                new Context(projectId.ToString("N")));
        }

        private Task<List<string>> InnerGetTeamsByProjectId(ISettings settings, Guid projectId)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Project/{projectId}/Team")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<string>>(_service, request);
        }

        public async Task<WorkItem> Update(ISettings settings, Guid projectId, Guid id, WorkItem workItem)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, workItem)
                .AddPath("Project/{projectId}/WorkItem/{id}")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddPathParameter("id", id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return await _restUtil.Send<WorkItem>(_service, request);
        }

        public Task<WorkItem> Update(ISettings settings, WorkItem workItem)
        {
            if (!workItem.WorkItemId.HasValue)
                throw new ArgumentNullException(nameof(workItem.WorkItemId));
            if (!workItem.ProjectId.HasValue)
                throw new ArgumentNullException(nameof(workItem.ProjectId));
            return Update(settings, workItem.ProjectId.Value, workItem.WorkItemId.Value, workItem);
        }

        public async Task<List<WorkItem>> GetByParentIds(ISettings settings, Guid projectId, params Guid[] parentIds)
        {
            if (parentIds == null || parentIds.Length == 0)
                throw new ArgumentNullException(nameof(parentIds));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Project/{projectId}/WorkItem")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            for (int i = 0; i < parentIds.Length; i += 1)
            {
                request.AddQueryParameter("parentIds", parentIds[i].ToString("D"));
            }
            return await _restUtil.Send<List<WorkItem>>(_service, request);
        }
    }
}
