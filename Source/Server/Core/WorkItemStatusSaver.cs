using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Framework;

namespace YardLight.Core
{
    public class WorkItemStatusSaver : IWorkItemStatusSaver
    {
        private readonly Saver _saver;

        public WorkItemStatusSaver(Saver saver)
        {
            _saver = saver;
        }

        public Task Create(ISettings settings, IWorkItemStatus workItemStatus, Guid userId)
        => _saver.Save(new TransactionHandler(settings), userId, workItemStatus.Create);

        public Task Update(ISettings settings, IWorkItemStatus workItemStatus, Guid userId)
        => _saver.Save(new TransactionHandler(settings), userId, workItemStatus.Create);
    }
}
