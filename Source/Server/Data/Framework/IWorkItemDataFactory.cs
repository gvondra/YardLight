using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IWorkItemDataFactory
    {
        Task<WorkItemData> Get(ISettings settings, Guid id);
        Task<IEnumerable<WorkItemData>> GetByProjectId(ISettings settings, Guid projectId);
        Task<IEnumerable<WorkItemData>> GetByProjectIdTypeId(ISettings settings, Guid projectId, Guid workItemTypeId);
        Task<IEnumerable<WorkItemData>> GetByParentIds(ISettings settings, params Guid[] parentIds);
        Task<IEnumerable<string>> GetTeamsByProjectId(ISettings settings, Guid projectId);
    }
}
