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
        private readonly IWorkItemStatusDataFactory _dataFactory;
        private readonly IWorkItemStatusDataSaver _dataSaver;

        public WorkItemStatusFactory(IWorkItemStatusDataFactory dataFactory, IWorkItemStatusDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private WorkItemStatus Create(WorkItemStatusData data) => new WorkItemStatus(data, _dataSaver);

        public IWorkItemStatus Create(Guid projectId)
        {
            return Create(new WorkItemStatusData() { ProjectId = projectId });
        }

        public async Task<IWorkItemStatus> Get(ISettings settings, Guid id)
        {
            WorkItemStatus result = null;
            WorkItemStatusData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                result = Create(data);
            return result;
        }

        public async Task<IEnumerable<IWorkItemStatus>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null)
        {
            return (await _dataFactory.GetByProjectId(new DataSettings(settings), projectId, isActive))
                .Select<WorkItemStatusData, IWorkItemStatus>(Create);
        }
    }
}
