using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Framework;
using YardLight.Data.Models;

namespace YardLight.Data
{
    public class WorkItemDataFactory : IWorkItemDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<WorkItemData> _dataFactory = new GenericDataFactory<WorkItemData>();

        public WorkItemDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task<WorkItemData> Get(ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, DataUtil.GetParameterValue(id));
            return (await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetWorkItem]",
                () => new WorkItemData(),
                DataUtil.AssignDataStateManager,
                new IDataParameter[] { parameter }
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<WorkItemData>> GetByProjectId(ISettings settings, Guid projectId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "projectId", DbType.Guid, DataUtil.GetParameterValue(projectId));
            return await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetWorkItem_by_ProjectId]",
                () => new WorkItemData(),
                DataUtil.AssignDataStateManager,
                new IDataParameter[] { parameter }
                );
        }

        public Task<IEnumerable<string>> GetItterationsByProjectId(ISettings settings, Guid projectId)
        {
            return GetList<string>(settings,
                DataUtil.CreateParameter(_providerFactory, "projectId", DbType.Guid, DataUtil.GetParameterValue(projectId)),
                "[yl].[GetItterationByProjectId]"
                );
        }

        public Task<IEnumerable<string>> GetTeamsByProjectId(ISettings settings, Guid projectId)
        {
            return GetList<string>(settings,
                DataUtil.CreateParameter(_providerFactory, "projectId", DbType.Guid, DataUtil.GetParameterValue(projectId)),
                "[yl].[GetTeamByProjectId]"
                );
        }

        private async Task<IEnumerable<T>> GetList<T>(
            ISettings settings, 
            IDataParameter dataParameter,
            string storedProcedureName)
        {
            List<T> items = new List<T>();
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(dataParameter);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(await reader.GetFieldValueAsync<T>(0));
                        }
                    }
                }
            }
            return items;
        }
    }
}
