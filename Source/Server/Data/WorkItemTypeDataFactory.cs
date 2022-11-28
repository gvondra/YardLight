using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Framework;
using YardLight.Data.Models;

namespace YardLight.Data
{
    public class WorkItemTypeDataFactory : IWorkItemTypeDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<WorkItemTypeData> _dataFactory = new GenericDataFactory<WorkItemTypeData>();

        public WorkItemTypeDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task<WorkItemTypeData> Get(ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, DataUtil.GetParameterValue(id));
            return (await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetWorkItemType]",
                () => new WorkItemTypeData(),
                DataUtil.AssignDataStateManager,
                new IDataParameter[] { parameter }
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<WorkItemTypeData>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "projectId", DbType.Guid, DataUtil.GetParameterValue(projectId)),
                DataUtil.CreateParameter(_providerFactory, "isActive", DbType.Guid, DataUtil.GetParameterValue(isActive))
            };
            return await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetWorkItemType_by_ProjectId]",
                () => new WorkItemTypeData(),
                DataUtil.AssignDataStateManager,
                parameters
                );
        }
    }
}
