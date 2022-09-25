using YardLight.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Authorization
{
    public interface ITokenService
    {
        Task<string> Create(ISettings settings);
        Task<string> Create(ISettings settings, ClientCredential clientCredential);
    }
}
