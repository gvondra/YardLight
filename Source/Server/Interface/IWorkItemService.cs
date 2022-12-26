using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public interface IWorkItemService
    {
        Task<List<WorkItem>> GetByProjectId(ISettings settings, Guid projectId);
        Task<List<WorkItem>> GetByParentIds(ISettings settings, Guid projectId, params Guid[] parentIds);
        Task<WorkItem> Create(ISettings settings, Guid projectId, WorkItem workItem);
        Task<WorkItem> Create(ISettings settings, WorkItem workItem);
        Task<WorkItem> Update(ISettings settings, Guid projectId, Guid id, WorkItem workItem);
        Task<WorkItem> Update(ISettings settings, WorkItem workItem);
        Task<List<string>> GetItterationsByProjectId(ISettings settings, Guid projectId);
        Task<List<string>> GetTeamsByProjectId(ISettings settings, Guid projectId);
    }
}
