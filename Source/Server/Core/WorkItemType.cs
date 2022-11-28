using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Data.Framework;
using YardLight.Data.Models;
using YardLight.Framework;

namespace YardLight.Core
{
    public class WorkItemType : IWorkItemType
    {
        private readonly WorkItemTypeData _data;
        private readonly IWorkItemTypeDataSaver _dataSaver;

        public WorkItemType(WorkItemTypeData data,
            IWorkItemTypeDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid WorkItemTypeId => _data.WorkItemTypeId;

        public Guid ProjectId => _data.ProjectId;

        public string Title { get => _data.Title; set => _data.Title = value; }
        public string ColorCode { get => _data.ColorCode; set => _data.ColorCode = value; }
        public bool IsActive { get => _data.IsActive; set => _data.IsActive = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public Guid CreateUserId => _data.CreateUserId;

        public Guid UpdateUserId => _data.UpdateUserId;

        public Task Create(ITransactionHandler transactionHandler, Guid userId)
        => _dataSaver.Create(transactionHandler, _data, userId);

        public Task Update(ITransactionHandler transactionHandler, Guid userId)
        => _dataSaver.Update(transactionHandler, _data, userId);
    }
}
