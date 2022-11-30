using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IWorkItemTypeSaver
    {
        Task Create(ISettings settings, IWorkItemType workItemType, Guid userId);
        Task Create(ISettings settings, IWorkItemType workItemType, IEnumerable<IWorkItemStatus> statuses, Guid userId);
        Task Update(ISettings settings, IWorkItemType workItemType, Guid userId);
        Task Update(ISettings settings, IWorkItemType workItemType, IEnumerable<IWorkItemStatus> statuses, Guid userId);
    }
}
