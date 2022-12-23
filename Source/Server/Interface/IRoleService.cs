using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public interface IRoleService
    {
        Task<List<Role>> Get(ISettings settings);
        Task<Role> Create(ISettings settings, Role role);
        Task<Role> Update(ISettings settings, Role role);
        Task<Role> Update(ISettings settings, Guid roleId, Role role);
    }
}
