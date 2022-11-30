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
    public class WorkItemStatusDataFactory : IWorkItemStatusDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<WorkItemStatusData> _dataFactory = new GenericDataFactory<WorkItemStatusData>();

        public WorkItemStatusDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task<WorkItemStatusData> Get(ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, DataUtil.GetParameterValue(id));
            return (await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetWorkItemStatus]",
                () => new WorkItemStatusData(),
                DataUtil.AssignDataStateManager,
                new IDataParameter[] { parameter }
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<WorkItemStatusData>> GetByProjectId(ISettings settings, Guid projectId, bool? isActive = null)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "projectId", DbType.Guid, DataUtil.GetParameterValue(projectId)),
                DataUtil.CreateParameter(_providerFactory, "isActive", DbType.Boolean, DataUtil.GetParameterValue(isActive))
            };
            return await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetWorkItemStatus_by_ProjectId]",
                () => new WorkItemStatusData(),
                DataUtil.AssignDataStateManager,
                parameters
                );
        }

        public async Task<IEnumerable<WorkItemStatusData>> GetByWorkItemTypeId(ISettings settings, Guid workItemTypeId, bool? isActive = null)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "workItemTypeId", DbType.Guid, DataUtil.GetParameterValue(workItemTypeId)),
                DataUtil.CreateParameter(_providerFactory, "isActive", DbType.Boolean, DataUtil.GetParameterValue(isActive))
            };
            return await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetWorkItemStatus_by_WorkItemTypeId]",
                () => new WorkItemStatusData(),
                DataUtil.AssignDataStateManager,
                parameters
                );
        }
    }
}
