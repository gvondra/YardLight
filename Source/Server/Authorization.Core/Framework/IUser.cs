using YardLight.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Core.Framework
{
    public interface IUser
    {
        Guid UserId { get; }
        string ReferenceId { get; }
        string Name { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task<IEmailAddress> GetEmailAddress(ISettings settings);
        void SetEmailAddress(IEmailAddress emailAddress);
        /// <returns>Dictionary of role names keyed on PolicyName</returns>
        Task<Dictionary<string, string>> GetRoles(ISettings settings);
        Task AddRole(ISettings settings, string policyName);
        Task RemoveRole(ISettings settings, string policyName);
        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);
    }
}
