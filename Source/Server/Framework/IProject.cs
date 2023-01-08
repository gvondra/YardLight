using System;
using System.Threading.Tasks;
using YardLight.CommonCore;
namespace YardLight.Framework
{
    public interface IProject
    {
        Guid ProjectId { get; }
        string Title { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task Create(ITransactionHandler transactionHandler, string userEmailAddress);
        Task Update(ITransactionHandler transactionHandler);
    }
}
