using YardLight.Authorization.Data.Framework;
using YardLight.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Data
{
    public class RoleDataFactory : IRoleDataFactory
    {
        private readonly IDbProviderFactory _dbProviderFactory;
        private readonly GenericDataFactory<RoleData> _genericDataFactory = new GenericDataFactory<RoleData>();

        public RoleDataFactory(IDbProviderFactory providerFactory)
        {
            _dbProviderFactory = providerFactory;
        }

        private RoleData CreateData() => new RoleData();

        public async Task<RoleData> Get(ISqlSettings settings, int id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_dbProviderFactory, "id", DbType.Int32, id);
            return (await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetRole]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<RoleData>> GetAll(ISqlSettings settings)
        {
            return (await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetRoleAll]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter>()));
        }

        public async Task<IEnumerable<RoleData>> GetByUserId(ISqlSettings settings, Guid userId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_dbProviderFactory, "userId", DbType.Guid, userId);
            return (await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetRoleByUserId]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }));
        }

        public async Task<IEnumerable<RoleData>> GetByClientId(ISqlSettings settings, Guid clientId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_dbProviderFactory, "clientId", DbType.Guid, clientId);
            return (await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetRoleByClientId]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }));
        }
    }
}
