using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Framework;

namespace YardLight.Core
{
    public class WorkItemTypeSaver : IWorkItemTypeSaver
    {
        private readonly Saver _saver;

        public WorkItemTypeSaver(Saver saver)
        {
            _saver = saver;
        }

        public Task Create(ISettings settings, IWorkItemType workItemType, Guid userId)
        {
            return Create(settings, workItemType, new List<IWorkItemStatus>(), userId); 
        }

        public async Task Create(ISettings settings, IWorkItemType workItemType, IEnumerable<IWorkItemStatus> statuses, Guid userId)
        {
            await _saver.Save(new TransactionHandler(settings), userId, async (th, uid) =>
            {
                await workItemType.Create(th, uid);
                await SaveStatuses(th, uid, statuses);
            });
        }

        public Task Update(ISettings settings, IWorkItemType workItemType, Guid userId)
        {
            return Update(settings, workItemType, new List<IWorkItemStatus>(), userId);
        }

        public async Task Update(ISettings settings, IWorkItemType workItemType, IEnumerable<IWorkItemStatus> statuses, Guid userId)
        {
            await _saver.Save(new TransactionHandler(settings), userId, async (th, uid) =>
            {
                await workItemType.Update(th, uid);
                await SaveStatuses(th, uid, statuses);
            });
        }

        private async Task SaveStatuses(ITransactionHandler transactionHandler, Guid userId, IEnumerable<IWorkItemStatus> statuses)
        {
            foreach (IWorkItemStatus status in statuses ?? new List<IWorkItemStatus>())
            {
                if (status.WorkItemStatusId.Equals(Guid.Empty))
                    await status.Create(transactionHandler, userId);
                else 
                    await status.Update(transactionHandler, userId);
            }
        }
    }
}
