using YardLight.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Data.Framework
{
    public interface IRoleDataFactory
    {
        Task<RoleData> Get(ISqlSettings settings, int id);
        Task<IEnumerable<RoleData>> GetAll(ISqlSettings settings);
        Task<IEnumerable<RoleData>> GetByUserId(ISqlSettings settings, Guid userId);
        Task<IEnumerable<RoleData>> GetByClientId(ISqlSettings settings, Guid clientId);
    }
}
