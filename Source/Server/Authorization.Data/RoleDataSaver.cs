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
    public class RoleDataSaver : IRoleDataSaver
    {
        private readonly IDbProviderFactory _providerFactory;

        public RoleDataSaver(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, RoleData role)
        {
            if (role.Manager.GetState(role) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, role);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[aut].[CreateRole]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Int32);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.AnsiString, role.Name);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "policyName", DbType.AnsiString, role.PolicyName);

                    await command.ExecuteNonQueryAsync();
                    role.RoleId = (int)id.Value;
                    role.CreateTimestamp = (DateTime)timestamp.Value;
                    role.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, RoleData role)
        {
            if (role.Manager.GetState(role) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, role);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[aut].[UpdateRole]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Int32, role.RoleId);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.AnsiString, role.Name);

                    await command.ExecuteNonQueryAsync();
                    role.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
