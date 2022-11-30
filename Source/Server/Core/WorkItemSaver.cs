using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Framework;

namespace YardLight.Core
{
    public class WorkItemSaver : IWorkItemSaver
    {
        private readonly Saver _saver;

        public WorkItemSaver(Saver saver)
        {
            _saver = saver;
        }

        public Task Create(ISettings settings, IWorkItem workItem, Guid userId)
        {
            return _saver.Save(new TransactionHandler(settings), userId, workItem.Create);
        }

        public Task Update(ISettings settings, IWorkItem workItem, Guid userId)
        {
            return _saver.Save(new TransactionHandler(settings), userId, workItem.Update);
        }
    }
}
