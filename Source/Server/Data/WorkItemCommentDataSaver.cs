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
    public class WorkItemCommentDataSaver : IWorkItemCommentDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public WorkItemCommentDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, WorkItemCommentData data, Guid userId)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[yl].[CreateWorkItemComment]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter commentId = DataUtil.CreateParameter(_providerFactory, "commentId", DbType.Guid);
                    commentId.Direction = ParameterDirection.Output;
                    command.Parameters.Add(commentId);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "workItemId", DbType.Guid, DataUtil.GetParameterValue(data.WorkItemId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "text", DbType.String, DataUtil.GetParameterValue(data.Text));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "type", DbType.Int16, DataUtil.GetParameterValue(data.Type));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.Guid, DataUtil.GetParameterValue(userId));

                    await command.ExecuteNonQueryAsync();
                    data.CommentId = (Guid)commentId.Value;
                    data.CreateTimestamp = (DateTime)timestamp.Value;
                    data.CreateUserId = userId;
                }
            }
        }
    }
}
