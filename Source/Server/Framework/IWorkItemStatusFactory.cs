using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IWorkItemStatusFactory
    {
        IWorkItemStatus Create(Guid projectId, IWorkItemType parentType);
        Task<IWorkItemStatus> Get(ISettings settings, Guid id);
        Task<IEnumerable<IWorkItemStatus>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null, bool skipCache = false);
        Task<IEnumerable<IWorkItemStatus>> GetByWorkItemTypeId(ISettings settings, Guid workItemTypeId, bool? isActive = null);
    }
}
