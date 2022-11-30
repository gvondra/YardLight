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
        private readonly IWorkItemDataFactory _dataFactory;
        private readonly IWorkItemDataSaver _dataSaver;
        private readonly IWorkItemStatusFactory _statusFactory;
        private readonly IWorkItemTypeFactory _typeFatory;

        public WorkItemFactory(IWorkItemDataFactory dataFactory, 
            IWorkItemDataSaver dataSaver,
            IWorkItemStatusFactory statusFactory,
            IWorkItemTypeFactory typeFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _statusFactory = statusFactory;
            _typeFatory = typeFactory;
        }

        private WorkItem Create(WorkItemData data, IWorkItemType workItemType, IWorkItemStatus workItemStatus) => new WorkItem(data, _dataSaver, _statusFactory, _typeFatory, workItemStatus, workItemType);
        private WorkItem Create(WorkItemData data) => new WorkItem(data, _dataSaver, _statusFactory, _typeFatory);

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
    }
}
