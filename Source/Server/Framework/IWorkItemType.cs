using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IWorkItemType
    {
        Guid WorkItemTypeId { get; }
        Guid ProjectId { get; }
        string Title { get; set; }
        string ColorCode { get; set; }
        bool IsActive { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }
        Guid CreateUserId { get; }
        Guid UpdateUserId { get; }

        Task Create(ITransactionHandler transactionHandler, Guid userId);
        Task Update(ITransactionHandler transactionHandler, Guid userId);
    }
}
