using YardLight.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Authorization
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAll(ISettings settings);
        Task<Client> Get(ISettings settings, Guid id);
        Task<Client> Create(ISettings settings, ClientSaveRequest request);
        Task<Client> Update(ISettings settings, ClientSaveRequest request);
        Task<Client> Create(ISettings settings, Client client);
        Task<Client> Update(ISettings settings, Client client);
        Task<Client> Create(ISettings settings, Client client, string secret);
        Task<Client> Update(ISettings settings, Client client, string secret);
        Task<string> CreateSecret(ISettings settings);
    }
}
