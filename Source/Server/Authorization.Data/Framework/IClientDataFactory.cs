using YardLight.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Data.Framework
{
    public interface IClientDataFactory
    {
        Task<ClientData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<ClientData>> GetAll(ISqlSettings settings);
        Task<IEnumerable<ClientCredentialData>> GetClientCredentials(ISqlSettings settings, Guid clientId);
    }
}
