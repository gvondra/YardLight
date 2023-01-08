using BrassLoon.DataClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using YardLight.Data.Framework;
using YardLight.Data.Models;

namespace YardLight.Data
{
    public class ProjectUserDataSaver : IProjectUserDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public ProjectUserDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory= providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, ProjectUserData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[yl].[CreateProjectUser]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    AddCommonParameters(command.Parameters, data);

                    await command.ExecuteNonQueryAsync();
                    data.CreateTimestamp = (DateTime)timestamp.Value;
                    data.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, ProjectUserData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[yl].[UpdateProjectUser]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    AddCommonParameters(command.Parameters, data);

                    await command.ExecuteNonQueryAsync();
                    data.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        private void AddCommonParameters(DbParameterCollection parameters, ProjectUserData data)
        {
            DataUtil.AddParameter(_providerFactory, parameters, "projectId", DbType.Guid, DataUtil.GetParameterValue(data.ProjectId));
            DataUtil.AddParameter(_providerFactory, parameters, "emailAddress", DbType.AnsiString, DataUtil.GetParameterValue(data.EmailAddress));
            DataUtil.AddParameter(_providerFactory, parameters, "isActive", DbType.Boolean, DataUtil.GetParameterValue(data.IsActive));
        }
    }
}
