using YardLight.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Core.Framework
{
    public interface IUserSaver
    {
        Task Create(ISettings settings, IUser user);
        Task Update(ISettings settings, IUser user);
    }
}
