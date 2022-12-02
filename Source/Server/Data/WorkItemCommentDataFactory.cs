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
    public class WorkItemCommentDataFactory : IWorkItemCommentDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<WorkItemCommentData> _dataFactory = new GenericDataFactory<WorkItemCommentData>();

        public WorkItemCommentDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task<IEnumerable<WorkItemCommentData>> GetByWorkItemId(ISettings settings, Guid workItemId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "workItemId", DbType.Guid, DataUtil.GetParameterValue(workItemId));
            return await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetWorkItemTypeComment_by_WorkItemId]",
                () => new WorkItemCommentData(),
                DataUtil.AssignDataStateManager,
                new IDataParameter[] { parameter }
                );
        }
    }
}
