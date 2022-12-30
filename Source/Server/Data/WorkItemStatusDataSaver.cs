using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Framework;
using YardLight.Data.Models;

namespace YardLight.Data
{
    public class WorkItemStatusDataSaver : IWorkItemStatusDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public WorkItemStatusDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, WorkItemStatusData data, Guid userId)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[yl].[CreateWorkItemStatus]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "workItemTypeId", DbType.Guid, DataUtil.GetParameterValue(data.WorkItemTypeId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "projectId", DbType.Guid, DataUtil.GetParameterValue(data.ProjectId));
                    AddCommonParameters(command.Parameters, data, userId);

                    await command.ExecuteNonQueryAsync();
                    data.WorkItemStatusId = (Guid)id.Value;
                    data.CreateTimestamp = (DateTime)timestamp.Value;
                    data.UpdateTimestamp = (DateTime)timestamp.Value;
                    data.CreateUserId = userId;
                    data.UpdateUserId = userId;
                }
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, WorkItemStatusData data, Guid userId)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[yl].[UpdateWorkItemStatus]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.WorkItemStatusId));
                    AddCommonParameters(command.Parameters, data, userId);

                    await command.ExecuteNonQueryAsync();
                    data.UpdateTimestamp = (DateTime)timestamp.Value;
                    data.UpdateUserId = userId;
                }
            }
        }

        private void AddCommonParameters(DbParameterCollection parameters, WorkItemStatusData data, Guid userId)
        {
            DataUtil.AddParameter(_providerFactory, parameters, "userId", DbType.Guid, DataUtil.GetParameterValue(userId));
            DataUtil.AddParameter(_providerFactory, parameters, "title", DbType.String, DataUtil.GetParameterValue(data.Title));
            DataUtil.AddParameter(_providerFactory, parameters, "colorCode", DbType.String, DataUtil.GetParameterValue(data.ColorCode));
            DataUtil.AddParameter(_providerFactory, parameters, "order", DbType.Int16, DataUtil.GetParameterValue(data.Order));
            DataUtil.AddParameter(_providerFactory, parameters, "isActive", DbType.Boolean, DataUtil.GetParameterValue(data.IsActive));
            DataUtil.AddParameter(_providerFactory, parameters, "isDefaultHidden", DbType.Boolean, DataUtil.GetParameterValue(data.IsDefaultHidden));
        }
    }
}
