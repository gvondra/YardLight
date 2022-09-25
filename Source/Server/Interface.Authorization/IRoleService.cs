using YardLight.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Authorization
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAll(ISettings settings);
        Task<Role> Create(ISettings settings, Role role);
        Task<Role> Update(ISettings settings, Role role);
    }
}
