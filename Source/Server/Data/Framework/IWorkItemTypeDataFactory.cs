using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IWorkItemTypeDataFactory
    {
        Task<WorkItemTypeData> Get(ISettings settings, Guid id);
        Task<IEnumerable<WorkItemTypeData>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null);
    }
}
