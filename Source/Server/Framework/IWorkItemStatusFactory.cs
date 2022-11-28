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
        IWorkItemStatus Create(Guid projectId);
        Task<IWorkItemStatus> Get(ISettings settings, Guid id);
        Task<IEnumerable<IWorkItemStatus>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null);
    }
}
