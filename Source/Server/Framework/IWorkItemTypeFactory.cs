using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IWorkItemTypeFactory
    {
        IWorkItemType Create(Guid projectId);
        Task<IWorkItemType> Get(ISettings settings, Guid id);
        Task<IEnumerable<IWorkItemType>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null);
    }
}
