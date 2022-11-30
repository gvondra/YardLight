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
    public class WorkItemDataSaver : IWorkItemDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public WorkItemDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, WorkItemData data, Guid userId)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[yl].[CreateWorkItem]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "projectId", DbType.Guid, DataUtil.GetParameterValue(data.ProjectId));
                    AddCommonParameters(command.Parameters, data, userId);

                    await command.ExecuteNonQueryAsync();
                    data.WorkItemId = (Guid)id.Value;
                    data.CreateTimestamp = (DateTime)timestamp.Value;
                    data.UpdateTimestamp = (DateTime)timestamp.Value;
                    data.CreateUserId = userId;
                    data.UpdateUserId = userId;
                }
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, WorkItemData data, Guid userId)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[yl].[UpdateWorkItem]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.WorkItemId));
                    AddCommonParameters(command.Parameters, data, userId);

                    await command.ExecuteNonQueryAsync();
                    data.UpdateTimestamp = (DateTime)timestamp.Value;
                    data.UpdateUserId = userId;
                }
            }
        }

        private void AddCommonParameters(DbParameterCollection parameters, WorkItemData data, Guid userId)
        {
            DataUtil.AddParameter(_providerFactory, parameters, "userId", DbType.Guid, DataUtil.GetParameterValue(userId));
            DataUtil.AddParameter(_providerFactory, parameters, "title", DbType.String, DataUtil.GetParameterValue(data.Title));
            DataUtil.AddParameter(_providerFactory, parameters, "typeId", DbType.Guid, DataUtil.GetParameterValue(data.TypeId));
            DataUtil.AddParameter(_providerFactory, parameters, "statusId", DbType.Guid, DataUtil.GetParameterValue(data.StatusId));
            DataUtil.AddParameter(_providerFactory, parameters, "team", DbType.String, DataUtil.GetParameterValue(data.Team));
            DataUtil.AddParameter(_providerFactory, parameters, "itteration", DbType.String, DataUtil.GetParameterValue(data.Itteration));
            DataUtil.AddParameter(_providerFactory, parameters, "startDate", DbType.Date, DataUtil.GetParameterValue(data.StartDate));
            DataUtil.AddParameter(_providerFactory, parameters, "targetDate", DbType.Date, DataUtil.GetParameterValue(data.TargetDate));
            DataUtil.AddParameter(_providerFactory, parameters, "closeDate", DbType.Date, DataUtil.GetParameterValue(data.CloseDate));
            DataUtil.AddParameter(_providerFactory, parameters, "priority", DbType.String, DataUtil.GetParameterValue(data.Priority));
            DataUtil.AddParameter(_providerFactory, parameters, "effort", DbType.String, DataUtil.GetParameterValue(data.Effort));
            DataUtil.AddParameter(_providerFactory, parameters, "value", DbType.String, DataUtil.GetParameterValue(data.Value));
        }
    }
}
