using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Framework.Enumerations;

namespace YardLight.Framework
{
    public interface IWorkItemComment : IComment
    {
        Guid WorkItemId { get; }
        WorkItemCommentType Type { get; }

        Task Create(ITransactionHandler transactionHandler, Guid userId);
    }
}
