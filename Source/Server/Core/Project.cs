using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Data.Framework;
using YardLight.Data.Models;
using YardLight.Framework;

namespace YardLight.Core
{
    public class Project : IProject
    {
        private readonly ProjectData _data;
        private readonly IProjectDataSaver _dataSaver;
        private readonly IProjectFactory _projectFactory;

        public Project(ProjectData data,
            IProjectDataSaver dataSaver,
            IProjectFactory projectFactory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _projectFactory = projectFactory;
        }

        public Guid ProjectId => _data.ProjectId;

        public string Title { get => _data.Title; set => _data.Title = (value ?? string.Empty).Trim(); }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public Task Create(ITransactionHandler transactionHandler, string userEmailAddress) 
            => _dataSaver.Create(transactionHandler, _data, userEmailAddress);

        public Task<IEnumerable<string>> GetUsers(ISettings settings)
            => _projectFactory.GetUsers(settings, ProjectId);

        public Task Update(ITransactionHandler transactionHandler) => _dataSaver.Update(transactionHandler, _data);
    }
}
