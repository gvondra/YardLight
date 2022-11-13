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
    public class ProjectUserDataFactory : IProjectUserDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<ProjectUserData> _dataFactory = new GenericDataFactory<ProjectUserData>();

        public ProjectUserDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task<ProjectUserData> Get(ISettings settings, Guid projectId, Guid userId)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "projectId", DbType.Guid, DataUtil.GetParameterValue(projectId)),
                DataUtil.CreateParameter(_providerFactory, "userId", DbType.Guid, DataUtil.GetParameterValue(userId))
            };
            return (await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetProjectUser]",
                () => new ProjectUserData(), 
                DataUtil.AssignDataStateManager,
                parameters
                )).FirstOrDefault();
        }
    }
}
