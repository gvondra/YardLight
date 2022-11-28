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
        => _saver.Save(new TransactionHandler(settings), userId, workItemType.Create);

        public Task Update(ISettings settings, IWorkItemType workItemType, Guid userId)
        => _saver.Save(new TransactionHandler(settings), userId, workItemType.Update);
    }
}
