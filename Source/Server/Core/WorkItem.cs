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
    public class WorkItem : IWorkItem
    {
        private readonly WorkItemData _data;
        private readonly IWorkItemDataSaver _dataSaver;
        private readonly IWorkItemStatusFactory _statusFactory;
        private readonly IWorkItemTypeFactory _typeFactory;
        private IWorkItemStatus _status;
        private IWorkItemType _type;

        public WorkItem(WorkItemData data,
            IWorkItemDataSaver dataSaver,
            IWorkItemStatusFactory statusFactory,
            IWorkItemTypeFactory typeFactory,
            IWorkItemStatus status,
            IWorkItemType type)
        {
            _data = data;
            _dataSaver = dataSaver;
            _statusFactory = statusFactory;
            _typeFactory = typeFactory;
            _status = status;
            _type = type;
        }

        public WorkItem(WorkItemData data,
            IWorkItemDataSaver dataSaver,
            IWorkItemStatusFactory statusFactory,
            IWorkItemTypeFactory typeFactory)
            : this(data, dataSaver, statusFactory, typeFactory, null, null)
        { }

        public Guid WorkItemId => _data.WorkItemId;

        public Guid ProjectId => _data.ProjectId;

        public string Title { get => _data.Title; set => _data.Title = (value ?? string.Empty).Trim(); }
        public string Team { get => _data.Team; set => _data.Team = (value ?? string.Empty).Trim(); }
        public string Itteration { get => _data.Itteration; set => _data.Itteration = (value ?? string.Empty).Trim(); }
        public DateTime? StartDate { get => _data.StartDate; set => _data.StartDate = value; }
        public DateTime? TargetDate { get => _data.TargetDate; set => _data.TargetDate = value; }
        public DateTime? CloseDate { get => _data.CloseDate; set => _data.CloseDate = value; }
        public string Priority { get => _data.Priority; set => _data.Priority = (value ?? string.Empty).Trim(); }
        public string Effort { get => _data.Effort; set => _data.Effort = (value ?? string.Empty).Trim(); }
        public string Value { get => _data.Value; set => _data.Value = (value ?? string.Empty).Trim(); }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public Guid CreateUserId => _data.CreateUserId;

        public Guid UpdateUserId => _data.UpdateUserId;

        private Guid StatusId { get => _data.StatusId; set => _data.StatusId = value; }
        private Guid TypeId { get => _data.TypeId; set => _data.TypeId = value; }
        public Guid? ParentWorkItemId { get => _data.ParentWorkItemId; set => _data.ParentWorkItemId = value; }

        public async Task Create(ITransactionHandler transactionHandler, Guid userId)
        {
            TypeId = _type.WorkItemTypeId;
            StatusId = _status.WorkItemStatusId;
            await _dataSaver.Create(transactionHandler, _data, userId);
        }

        public async Task<IWorkItemStatus> GetStatus(ISettings settings)
        {
            if (_status == null)
                _status = await _statusFactory.Get(settings, StatusId);
            return _status;
        }

        public async Task<IWorkItemType> GetType(ISettings settings)
        {
            if (_type == null)
                _type = await _typeFactory.Get(settings, TypeId);
            return _type;
        }

        public void SetStatus(IWorkItemStatus status)
        {
            _status = status;
        }

        public async Task Update(ITransactionHandler transactionHandler, Guid userId)
        {
            if (_status != null)
                StatusId = _status.WorkItemStatusId;
            await _dataSaver.Update(transactionHandler, _data, userId);
        }
    }
}
