using System;
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

        public Project(ProjectData data,
            IProjectDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid ProjectId => _data.ProjectId;

        public string Title { get => _data.Title; set => _data.Title = (value ?? string.Empty).Trim(); }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public Task Create(ITransactionHandler transactionHandler, Guid userId, string userEmailAddress) 
            => _dataSaver.Create(transactionHandler, _data, userId, userEmailAddress);

        public Task Update(ITransactionHandler transactionHandler) => _dataSaver.Update(transactionHandler, _data);
    }
}
