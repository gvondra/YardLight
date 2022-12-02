using Microsoft.Extensions.Caching.Memory;
using Polly.Caching.Memory;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Data.Framework;
using YardLight.Data.Models;
using YardLight.Framework;

namespace YardLight.Core
{
    public class WorkItemStatusFactory : IWorkItemStatusFactory
    {
        private readonly static Policy _cacheByProjectId = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(3));
        private readonly static Policy _cacheByWorkItemTypeId = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(3));
        private readonly IWorkItemStatusDataFactory _dataFactory;
        private readonly IWorkItemStatusDataSaver _dataSaver;

        public WorkItemStatusFactory(IWorkItemStatusDataFactory dataFactory, IWorkItemStatusDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private WorkItemStatus Create(WorkItemStatusData data) => new WorkItemStatus(data, _dataSaver);
        private WorkItemStatus Create(WorkItemStatusData data, IWorkItemType parentType) => new WorkItemStatus(data, _dataSaver, parentType);

        public IWorkItemStatus Create(Guid projectId, IWorkItemType parentType)
        {
            return Create(new WorkItemStatusData { ProjectId = projectId }, parentType);
        }

        public async Task<IWorkItemStatus> Get(ISettings settings, Guid id)
        {
            WorkItemStatus result = null;
            WorkItemStatusData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                result = Create(data);
            return result;
        }

        public Task<IEnumerable<IWorkItemStatus>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null)
        {
            return _cacheByProjectId.Execute((context) => InnerGetByProjectId(settings, projectId, isActive),
                new Context($"{projectId.ToString("N")};{isActive}"));
        }

        private async Task<IEnumerable<IWorkItemStatus>> InnerGetByProjectId(ISettings settings, Guid projectId, bool? isActive = null)
        {
            return (await _dataFactory.GetByProjectId(new DataSettings(settings), projectId, isActive))
                .Select<WorkItemStatusData, IWorkItemStatus>(Create);
        }

        public Task<IEnumerable<IWorkItemStatus>> GetByWorkItemTypeId(ISettings settings, Guid workItemTypeId, bool? isActive = null)
        {
            return _cacheByWorkItemTypeId.Execute((context) => InnerGetByWorkItemTypeId(settings, workItemTypeId, isActive),
                new Context($"{workItemTypeId.ToString("N")};{isActive}"));
        }

        private async Task<IEnumerable<IWorkItemStatus>> InnerGetByWorkItemTypeId(ISettings settings, Guid workItemTypeId, bool? isActive = null)
        {
            return (await _dataFactory.GetByWorkItemTypeId(new DataSettings(settings), workItemTypeId, isActive))
                .Select<WorkItemStatusData, IWorkItemStatus>(Create);
        }
    }
}
