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
    public class WorkItemStatus : IWorkItemStatus
    {
        private readonly WorkItemStatusData _data;
        private readonly IWorkItemStatusDataSaver _dataSaver;
        private readonly IWorkItemType _parentType;

        public WorkItemStatus(WorkItemStatusData data,
            IWorkItemStatusDataSaver dataSaver,
            IWorkItemType parentType)
        {
            _data = data;
            _dataSaver = dataSaver;
            _parentType = parentType;
        }

        public WorkItemStatus(WorkItemStatusData data,
            IWorkItemStatusDataSaver dataSaver)
            : this(data, dataSaver, null)
        {}

        public Guid WorkItemStatusId => _data.WorkItemStatusId;
        public Guid WorkItemTypeId { get => _data.WorkItemTypeId; private set => _data.WorkItemTypeId = value; }
        public Guid ProjectId => _data.ProjectId;

        public string Title { get => _data.Title; set => _data.Title = value; }
        public string ColorCode { get => _data.ColorCode; set => _data.ColorCode = value; }
        public short Order { get => _data.Order; set => _data.Order = value; }
        public bool IsActive { get => _data.IsActive; set => _data.IsActive = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public Guid CreateUserId => _data.CreateUserId;

        public Guid UpdateUserId => _data.UpdateUserId;

        public async Task Create(ITransactionHandler transactionHandler, Guid userId)
        {
            WorkItemTypeId = _parentType.WorkItemTypeId;
            await _dataSaver.Create(transactionHandler, _data, userId);
        }

        public Task Update(ITransactionHandler transactionHandler, Guid userId)
        => _dataSaver.Update(transactionHandler, _data, userId);
    }
}
