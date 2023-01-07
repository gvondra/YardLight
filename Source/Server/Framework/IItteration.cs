using System;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IItteration
    {
        Guid? ItterationId { get; }
        Guid ProjectId { get; }
        string Name { get; set; }
        DateTime? Start { get; set; }
        DateTime? End { get; set; }
        bool Hidden { get; set; }
        bool Virtual { get; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }
        Guid? CreateUserId { get; }
        Guid? UpdateUserId { get; }

        Task Save(ITransactionHandler transactionHandler, Guid userId);
    }
}
