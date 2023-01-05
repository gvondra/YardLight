using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IItterationFactory
    {
        IItteration Create(Guid projectId);
        Task<IItteration> Get(ISettings settings, Guid id);
        Task<IEnumerable<IItteration>> GetByProjectId(ISettings settings, Guid projectId);
    }
}
