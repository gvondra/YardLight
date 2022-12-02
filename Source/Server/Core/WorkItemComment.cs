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

        public WorkItemComment(WorkItemCommentData data,
            IWorkItemCommentDataSaver dataSaver)
            : base(data)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid WorkItemId => _data.WorkItemId;

        public WorkItemCommentType Type => (WorkItemCommentType)_data.Type;

        public Task Create(ITransactionHandler transactionHandler, Guid userId)
        => _dataSaver.Create(transactionHandler, _data, userId);
    }
}
