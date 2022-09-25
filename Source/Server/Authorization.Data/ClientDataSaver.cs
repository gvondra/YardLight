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
    public class ClientDataSaver : IClientDataSaver
    {
        private readonly IDbProviderFactory _providerFactory;

        public ClientDataSaver(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, ClientData client)
        {
            if (client.Manager.GetState(client) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, client);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[aut].[CreateClient]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, client.Name);

                    await command.ExecuteNonQueryAsync();
                    client.ClientId = (Guid)id.Value;
                    client.CreateTimestamp = (DateTime)timestamp.Value;
                    client.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, ClientCredentialData clientCredentail)
        {
            if (clientCredentail.Manager.GetState(clientCredentail) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, clientCredentail);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[aut].[CreateClientCredential]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, clientCredentail.ClientId);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "secret", DbType.Binary, clientCredentail.Secret);

                    await command.ExecuteNonQueryAsync();
                    clientCredentail.ClientCredentialId = (Guid)id.Value;
                    clientCredentail.CreateTimestamp = (DateTime)timestamp.Value;
                    clientCredentail.UpdateTimestamp = (DateTime)timestamp.Value;
                    clientCredentail.IsActive = true;
                }
            }
        }

        public async Task DeactivateClient(ISqlTransactionHandler transactionHandler, Guid clientId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[aut].[UpdateClientCredentialDeactivate]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, clientId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, ClientData client)
        {
            if (client.Manager.GetState(client) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, client);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[aut].[UpdateClient]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, client.ClientId);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, client.Name);

                    await command.ExecuteNonQueryAsync();
                    client.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task AddRole(ISqlTransactionHandler transactionHandler, Guid clientId, int roleId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[aut].[CreateClientRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, clientId);
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Int32, roleId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task RemoveRole(ISqlTransactionHandler transactionHandler, Guid clientId, int roleId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[aut].[RemoveClientRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, clientId);
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Int32, roleId);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
