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
        private readonly IWorkItemTypeDataFactory _dataFactory;
        private readonly IWorkItemTypeDataSaver _dataSaver;

        public WorkItemTypeFactory(IWorkItemTypeDataFactory dataFactory, IWorkItemTypeDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private WorkItemType Create(WorkItemTypeData data) => new WorkItemType(data, _dataSaver);

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

        public async Task<IEnumerable<IWorkItemType>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null)
        {
            return (await _dataFactory.GetByProjectId(new DataSettings(settings), projectId, isActive))
                .Select<WorkItemTypeData, IWorkItemType>(Create);
        }
    }
}
