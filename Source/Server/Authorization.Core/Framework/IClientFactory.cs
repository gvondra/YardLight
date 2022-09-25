using YardLight.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Core.Framework
{
    public interface IClientFactory
    {
        IClient Create(string secret);
        Task<IClient> Get(ISettings settings, Guid id);
        Task<IEnumerable<IClient>> GetAll(ISettings settings);
    }
}
