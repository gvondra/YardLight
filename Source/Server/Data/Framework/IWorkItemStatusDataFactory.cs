using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IWorkItemStatusDataFactory
    {
        Task<WorkItemStatusData> Get(ISettings settings, Guid id);
        Task<IEnumerable<WorkItemStatusData>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null);
        Task<IEnumerable<WorkItemStatusData>> GetByWorkItemTypeId(ISettings settings, Guid workItemTypeId, bool? isActive = null);
    }
}
