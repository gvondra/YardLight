using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IWorkItem
    {
        public Guid WorkItemId { get; }
        public Guid ProjectId { get; }
        public string Title { get; set; }
        public string Team { get; set; }
        public string Itteration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string Priority { get; set; }
        public string Effort { get; set; }
        public string Value { get; set; }
        public DateTime CreateTimestamp { get; }
        public DateTime UpdateTimestamp { get; }
        public Guid CreateUserId { get; }
        public Guid UpdateUserId { get; }

        Task Create(ITransactionHandler transactionHandler, Guid userId);
        Task Update(ITransactionHandler transactionHandler, Guid userId);

        Task<IWorkItemStatus> GetStatus(ISettings settings);
        void SetStatus(IWorkItemStatus status);
        Task<IWorkItemType> GetType(ISettings settings);
    }
}
