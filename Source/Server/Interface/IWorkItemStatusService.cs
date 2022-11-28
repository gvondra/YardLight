using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public interface IWorkItemStatusService
    {
        Task<List<WorkItemStatus>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null);
        Task<WorkItemStatus> Create(ISettings settings, WorkItemStatus status);
        Task<WorkItemStatus> Create(ISettings settings, Guid projectId, WorkItemStatus status);
        Task<WorkItemStatus> Update(ISettings settings, WorkItemStatus status);
        Task<WorkItemStatus> Update(ISettings settings, Guid projectId, Guid id, WorkItemStatus status);
    }
}
