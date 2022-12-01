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
        Guid WorkItemId { get; }
        Guid ProjectId { get; }
        Guid? ParentWorkItemId { get; set; }
        string Title { get; set; }
        string Team { get; set; }
        string Itteration { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? TargetDate { get; set; }
        DateTime? CloseDate { get; set; }
        string Priority { get; set; }
        string Effort { get; set; }
        string Value { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }
        Guid CreateUserId { get; }
        Guid UpdateUserId { get; }

        Task Create(ITransactionHandler transactionHandler, Guid userId);
        Task Update(ITransactionHandler transactionHandler, Guid userId);

        Task<IWorkItemStatus> GetStatus(ISettings settings);
        void SetStatus(IWorkItemStatus status);
        Task<IWorkItemType> GetType(ISettings settings);
    }
}
