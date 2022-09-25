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
    public class ClientDataFactory : IClientDataFactory
    {
        private readonly IDbProviderFactory _dbProviderFactory;
        private readonly GenericDataFactory<ClientData> _genericDataFactory = new GenericDataFactory<ClientData>();
        private readonly GenericDataFactory<ClientCredentialData> _clientCredentialDataFactory = new GenericDataFactory<ClientCredentialData>();

        public ClientDataFactory(IDbProviderFactory providerFactory)
        {
            _dbProviderFactory = providerFactory;
        }

        private ClientData CreateData() => new ClientData();

        public async Task<ClientData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_dbProviderFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetClient]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<ClientData>> GetAll(ISqlSettings settings)
        {
            return await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetClientAll]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter>());
        }

        public async Task<IEnumerable<ClientCredentialData>> GetClientCredentials(ISqlSettings settings, Guid clientId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_dbProviderFactory, "clientId", DbType.Guid, clientId);
            return await _clientCredentialDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetClientCredentialByClientId]",
                () => new ClientCredentialData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter });
        }
    }
}
