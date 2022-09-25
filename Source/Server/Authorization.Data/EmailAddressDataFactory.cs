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
    public class EmailAddressDataFactory : IEmailAddressDataFactory
    {
        private readonly IDbProviderFactory _dbProviderFactory;
        private readonly GenericDataFactory<EmailAddressData> _genericDataFactory = new GenericDataFactory<EmailAddressData>();

        public EmailAddressDataFactory(IDbProviderFactory providerFactory)
        {
            _dbProviderFactory = providerFactory;
        }

        private EmailAddressData CreateData() => new EmailAddressData();

        public async Task<EmailAddressData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_dbProviderFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetEmailAddress]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<EmailAddressData> GetByAddress(ISqlSettings settings, string address)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_dbProviderFactory, "address", DbType.String, address);
            return (await _genericDataFactory.GetData(settings,
                _dbProviderFactory,
                "[aut].[GetEmailAddressByAddress]",
                CreateData,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }
    }
}
