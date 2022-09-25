using YardLight.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Core.Framework
{
    public interface IRoleFactory
    {
        IRole Create(string policyName);
        Task<IRole> Get(ISettings settings, int id);
        Task<IEnumerable<IRole>> GetAll(ISettings settings);
        Task<IEnumerable<IRole>> GetByUserId(ISettings settings, Guid userId);
        Task<IEnumerable<IRole>> GetByClientId(ISettings settings, Guid clientId);
    }
}
