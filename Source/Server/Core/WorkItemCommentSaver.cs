using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Framework;

namespace YardLight.Core
{
    public class WorkItemCommentSaver : IWorkItemCommentSaver
    {
        private readonly Saver _saver;

        public WorkItemCommentSaver(Saver saver)
        {
            _saver = saver;
        }

        public Task Create(ISettings settings, IWorkItemComment workItemComment, Guid userId)
        {
            return _saver.Save(new TransactionHandler(settings), userId, workItemComment.Create);
        }
    }
}
