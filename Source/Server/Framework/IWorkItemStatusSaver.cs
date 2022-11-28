using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IWorkItemStatusSaver
    {
        Task Create(ISettings settings, IWorkItemStatus workItemStatus, Guid userId);
        Task Update(ISettings settings, IWorkItemStatus workItemStatus, Guid userId);
    }
}
