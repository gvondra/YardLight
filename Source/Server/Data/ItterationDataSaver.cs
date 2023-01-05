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
    public class ItterationDataSaver : IItterationDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public ItterationDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Save(ISqlTransactionHandler transactionHandler, ItterationData data, Guid userId)
        {
            if (data.Manager.GetState(data) != DataState.Unchanged)
            {
                if (!data.ItterationId.HasValue)
                    data.ItterationId = Guid.NewGuid();
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[yl].[SaveItteration]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.ItterationId.Value));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "projectId", DbType.Guid, DataUtil.GetParameterValue(data.ProjectId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, DataUtil.GetParameterValue(data.Name));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "start", DbType.Date, DataUtil.GetParameterValue(data.Start));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "end", DbType.Date, DataUtil.GetParameterValue(data.End));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "hidden", DbType.Boolean, DataUtil.GetParameterValue(data.Hidden));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.Guid, DataUtil.GetParameterValue(userId));

                    await command.ExecuteNonQueryAsync();
                    data.CreateTimestamp = (DateTime)timestamp.Value;
                    data.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
