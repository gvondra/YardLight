﻿using BrassLoon.DataClient;
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
    public class ProjectDataFactory : IProjectDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<ProjectData> _dataFactory = new GenericDataFactory<ProjectData>();

        public ProjectDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task<ProjectData> Get(ISettings settings, Guid projectId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "projectId", DbType.Guid, DataUtil.GetParameterValue(projectId));
            return (await _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetProject]",
                () => new ProjectData(),
                DataUtil.AssignDataStateManager,
                new IDataParameter[] { parameter }
                )).FirstOrDefault();
        }

        public Task<IEnumerable<ProjectData>> GetByEmailAddress(ISettings settings, string emailAddress)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "emailAddress", DbType.AnsiString, DataUtil.GetParameterValue(emailAddress));
            return _dataFactory.GetData(settings, _providerFactory,
                "[yl].[GetProject_by_EmailAddress]",
                () => new ProjectData(),
                DataUtil.AssignDataStateManager,
                new IDataParameter[] { parameter }
                );
        }
    }
}
