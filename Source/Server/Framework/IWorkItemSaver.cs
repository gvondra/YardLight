using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IWorkItemSaver
    {
        Task Create(ISettings settings, IWorkItem workItem, Guid userId);
        Task Update(ISettings settings, IWorkItem workItem, Guid userId);
    }
}
