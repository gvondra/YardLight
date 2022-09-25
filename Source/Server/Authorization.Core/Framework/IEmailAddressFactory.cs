using YardLight.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Core.Framework
{
    public interface IEmailAddressFactory
    {
        Task<IEmailAddress> Get(ISettings settings, Guid id);
        Task<IEmailAddress> GetByAddress(ISettings settings, string address);
        IEmailAddress Create(string address);
    }
}
