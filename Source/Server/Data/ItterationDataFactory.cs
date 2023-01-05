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
    public class ItterationDataFactory : IItterationDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<ItterationData> _dataFactory = new GenericDataFactory<ItterationData>();

        public ItterationDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public Task<IEnumerable<ItterationData>> GetByProjectId(ISettings settings, Guid projectId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "projectId", DbType.Guid, DataUtil.GetParameterValue(projectId));
            return _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetItterationByProjectId]",
                () => new ItterationData(),
                DataUtil.AssignDataStateManager,
                new IDataParameter[] { parameter }
                );
        }
    }
}
