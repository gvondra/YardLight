using YardLight.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Core.Framework
{
    public interface IRoleSaver
    {
        Task Create(ISettings settings, IRole role);
        Task Update(ISettings settings, IRole role);
    }
}
