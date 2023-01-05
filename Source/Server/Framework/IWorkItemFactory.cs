using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IWorkItemFactory
    {
        IWorkItem Create(Guid projectId, IWorkItemType workItemType, IWorkItemStatus workItemStatus);
        Task<IWorkItem> Get(ISettings settings, Guid id);
        Task<IEnumerable<IWorkItem>> GetByProjectId(ISettings settings, Guid projectId);
        Task<IEnumerable<IWorkItem>> GetByProjectIdTypeId(ISettings settings, Guid projectId, Guid workItemTypeId, string team = "", string itteration = "");
        Task<IEnumerable<IWorkItem>> GetByParentIds(ISettings settings, params Guid[] parentIds);
        Task<IEnumerable<string>> GetTeamsByProjectId(ISettings settings, Guid projectId);
    }
}
