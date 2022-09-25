using YardLight.Authorization.Data.Framework;
using YardLight.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Data
{
    public class EmailAddressDataSaver : IEmailAddressDataSaver
    {
        private readonly IDbProviderFactory _providerFactory;

        public EmailAddressDataSaver(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, EmailAddressData emailAddress)
        {
            if (emailAddress.Manager.GetState(emailAddress) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, emailAddress);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[aut].[CreateEmailAddress]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "address", DbType.String, emailAddress.Address);

                    await command.ExecuteNonQueryAsync();
                    emailAddress.EmailAddressId = (Guid)id.Value;
                    emailAddress.CreateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
