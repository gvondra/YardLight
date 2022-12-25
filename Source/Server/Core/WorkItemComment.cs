using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Data.Framework;
using YardLight.Data.Models;
using YardLight.Framework;
using YardLight.Framework.Enumerations;

namespace YardLight.Core
{
    public class WorkItemComment : Comment, IWorkItemComment
    {
        private readonly WorkItemCommentData _data;
        private readonly IWorkItemCommentDataSaver _dataSaver;
        private readonly IWorkItem _parentWorkItem;

        public WorkItemComment(WorkItemCommentData data,
            IWorkItemCommentDataSaver dataSaver,
            IWorkItem parentWorkItem)
            : base(data)
        {
            _data = data;
            _dataSaver = dataSaver;
            _parentWorkItem = parentWorkItem;
        }

        public Guid WorkItemId { get => _data.WorkItemId; private set => _data.WorkItemId = value; }

        public WorkItemCommentType Type => (WorkItemCommentType)_data.Type;

        public async Task Create(ITransactionHandler transactionHandler, Guid userId)
        {
            WorkItemId = _parentWorkItem.WorkItemId;
            await _dataSaver.Create(transactionHandler, _data, userId);
        }        
    }
}
