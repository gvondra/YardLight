using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public interface IWorkItemTypeService
    {
        Task<List<WorkItemType>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null);
        Task<WorkItemType> Create(ISettings settings, WorkItemType type);
        Task<WorkItemType> Create(ISettings settings, Guid projectId, WorkItemType type);
        Task<WorkItemType> Update(ISettings settings, WorkItemType type);
        Task<WorkItemType> Update(ISettings settings, Guid projectId, Guid id, WorkItemType type);
    }
}
