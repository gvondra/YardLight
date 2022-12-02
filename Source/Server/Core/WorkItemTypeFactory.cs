using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
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
    public class WorkItemTypeFactory : IWorkItemTypeFactory
    {
        private readonly static Policy _cacheByProjectId = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(3));
        private readonly IWorkItemTypeDataFactory _dataFactory;
        private readonly IWorkItemTypeDataSaver _dataSaver;
        private readonly IWorkItemStatusFactory _statusFactory;

        public WorkItemTypeFactory(IWorkItemTypeDataFactory dataFactory, 
            IWorkItemTypeDataSaver dataSaver,
            IWorkItemStatusFactory statusFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _statusFactory = statusFactory;
        }

        private WorkItemType Create(WorkItemTypeData data) => new WorkItemType(data, _dataSaver, _statusFactory);

        public IWorkItemType Create(Guid projectId)
        {
            return Create(new WorkItemTypeData() { ProjectId = projectId });
        }

        public async Task<IWorkItemType> Get(ISettings settings, Guid id)
        {
            WorkItemType result = null;
            WorkItemTypeData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null) 
                result = Create(data);
            return result;
        }

        public Task<IEnumerable<IWorkItemType>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null)
        {
            return _cacheByProjectId.Execute((context) => InnerGetByProjectId(settings, projectId, isActive),
                new Context($"{projectId.ToString("N")};{isActive}"));
        }

        private async Task<IEnumerable<IWorkItemType>> InnerGetByProjectId(ISettings settings, Guid projectId, bool? isActive = null)
        {
            return (await _dataFactory.GetByProjectId(new DataSettings(settings), projectId, isActive))
                .Select<WorkItemTypeData, IWorkItemType>(Create);
        }
    }
}
