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
    public class UserDataFactory : IUserDataFactory
    {
        private readonly IDbProviderFactory _dbProviderFactory;
        private readonly GenericDataFactory<UserData> _genericDataFactory = new GenericDataFactory<UserData>();

        public UserDataFactory(IDbProviderFactory providerFactory)
        { 
            _dbProviderFactory = providerFactory;
        }

        private UserData CreateData() => new UserData();

        public async Task<UserData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_dbProviderFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetUser]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<UserData>> GetByEmailAddress(ISqlSettings settings, string emailAddress)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_dbProviderFactory, "address", DbType.String, emailAddress);
            return await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetUserByEmailAddress]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter });
        }

        public async Task<UserData> GetByReferenceId(ISqlSettings settings, string referenceId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_dbProviderFactory, "referenceId", DbType.String, referenceId);
            return (await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetUserByReferenceId]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }
    }
}
