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
    public class UserDataSaver : IUserDataSaver
    {
        private readonly IDbProviderFactory _providerFactory;

        public UserDataSaver(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, UserData user)
        {
            if (user.Manager.GetState(user) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, user);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[aut].[CreateUser]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "referenceId", DbType.AnsiString, user.ReferenceId);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "emailAddressId", DbType.Guid, user.EmailAddressId);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.AnsiString, user.Name);

                    await command.ExecuteNonQueryAsync();
                    user.UserId = (Guid)id.Value;
                    user.CreateTimestamp = (DateTime)timestamp.Value;
                    user.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task AddRole(ISqlTransactionHandler transactionHandler, Guid userId, int roleId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[aut].[CreateUserRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.Guid, userId);
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Int32, roleId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, UserData user)
        {
            if (user.Manager.GetState(user) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, user);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[aut].[UpdateUser]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, user.UserId);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.AnsiString, user.Name);

                    await command.ExecuteNonQueryAsync();
                    user.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task RemoveRole(ISqlTransactionHandler transactionHandler, Guid userId, int roleId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[aut].[RemoveUserRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.Guid, userId);
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Int32, roleId);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
