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
    public class WorkItemFactory : IWorkItemFactory
    {
        private readonly static Policy _cacheTeam = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(2));
        private readonly IWorkItemDataFactory _dataFactory;
        private readonly IWorkItemDataSaver _dataSaver;
        private readonly IWorkItemStatusFactory _statusFactory;
        private readonly IWorkItemTypeFactory _typeFatory;
        private readonly IWorkItemCommentFactory _commentFactory;

        public WorkItemFactory(IWorkItemDataFactory dataFactory, 
            IWorkItemDataSaver dataSaver,
            IWorkItemStatusFactory statusFactory,
            IWorkItemTypeFactory typeFactory,
            IWorkItemCommentFactory commentFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _statusFactory = statusFactory;
            _typeFatory = typeFactory;
            _commentFactory = commentFactory;
        }

        private WorkItem Create(WorkItemData data, IWorkItemType workItemType, IWorkItemStatus workItemStatus) => new WorkItem(data, _dataSaver, _statusFactory, _typeFatory, _commentFactory, workItemStatus, workItemType);
        private WorkItem Create(WorkItemData data) => new WorkItem(data, _dataSaver, _statusFactory, _typeFatory, _commentFactory);

        public IWorkItem Create(Guid projectId, IWorkItemType workItemType, IWorkItemStatus workItemStatus)
        {
            return Create(new WorkItemData { ProjectId = projectId },
                workItemType, workItemStatus);
        }

        public async Task<IEnumerable<IWorkItem>> GetByProjectId(ISettings settings, Guid projectId)
        {
            return (await _dataFactory.GetByProjectId(new DataSettings(settings), projectId))
                .Select<WorkItemData, IWorkItem>(Create);
        }

        public async Task<IWorkItem> Get(ISettings settings, Guid id)
        {
            WorkItem result = null;
            WorkItemData data = await _dataFactory.Get(new DataSettings(settings), id); 
            if (data != null) 
                result = Create(data);
            return result;
        }

        public Task<IEnumerable<string>> GetTeamsByProjectId(ISettings settings, Guid projectId)
        {
            return _cacheTeam.Execute((context) => InnerGetTeamsByProjectId(settings, projectId),
                new Context(projectId.ToString("N")));
        }

        private Task<IEnumerable<string>> InnerGetTeamsByProjectId(ISettings settings, Guid projectId)
        {
            return _dataFactory.GetTeamsByProjectId(new DataSettings(settings), projectId);
        }

        public async Task<IEnumerable<IWorkItem>> GetByParentIds(ISettings settings, params Guid[] parentIds)
        {
            return (await _dataFactory.GetByParentIds(new DataSettings(settings), parentIds))
                .Select<WorkItemData, IWorkItem>(Create);
        }

        public async Task<IEnumerable<IWorkItem>> GetByProjectIdTypeId(ISettings settings, Guid projectId, Guid workItemTypeId, string team = "", string itteration = "")
        {
            return (await _dataFactory.GetByProjectIdTypeId(new DataSettings(settings), projectId, workItemTypeId))
                .Select<WorkItemData, IWorkItem>(Create)
                .Where(item => WorkItemFilter(item, team, itteration));
        }

        // return true to include items
        private static bool WorkItemFilter(IWorkItem workItem, string team, string itteration)
        {
            bool include = true;
            include = (include && (string.IsNullOrEmpty(team) || string.IsNullOrEmpty(workItem.Team) || string.Equals(team, workItem.Team, StringComparison.OrdinalIgnoreCase)));
            include = (include && (string.IsNullOrEmpty(itteration) || string.IsNullOrEmpty(workItem.Itteration) || string.Equals(itteration, workItem.Itteration, StringComparison.OrdinalIgnoreCase)));
            return include;
        }
    }
}
