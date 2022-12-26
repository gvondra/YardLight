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

        public async Task<IEnumerable<WorkItemData>> GetByParentIds(ISettings settings, params Guid[] parentIds)
        {
            if (parentIds == null || parentIds.Length == 0) 
                throw new ArgumentNullException(nameof(parentIds));
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "parentIds", DbType.AnsiString, 
                DataUtil.GetParameterValue(string.Join(',', parentIds.Select(id => id.ToString("D"))))
                );
            return await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetWorkItem_by_ParentIds]",
                () => new WorkItemData(),
                DataUtil.AssignDataStateManager,
                new IDataParameter[] { parameter }
                );
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
            return DataUtil.ReadList<string>(_providerFactory, settings, "[yl].[GetItterationByProjectId]",
                DataUtil.CreateParameter(_providerFactory, "projectId", DbType.Guid, DataUtil.GetParameterValue(projectId))
                );
        }

        public Task<IEnumerable<string>> GetTeamsByProjectId(ISettings settings, Guid projectId)
        {
            return DataUtil.ReadList<string>(_providerFactory, settings, "[yl].[GetTeamByProjectId]",
                DataUtil.CreateParameter(_providerFactory, "projectId", DbType.Guid, DataUtil.GetParameterValue(projectId))
                );
        }
    }
}
