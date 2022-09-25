using YardLight.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Core.Framework
{
    public interface IClient
    {
        Guid ClientId { get; }
        string Name { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);
        void SetSecret(string secret);
        Task<bool> VerifySecret(ISettings settings, string secret);
        /// <returns>Dictionary of role names keyed on PolicyName</returns>
        Task<Dictionary<string, string>> GetRoles(ISettings settings);
        Task AddRole(ISettings settings, string policyName);
        Task RemoveRole(ISettings settings, string policyName);
    }
}
