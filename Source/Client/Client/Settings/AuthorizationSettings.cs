using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Authorization;

namespace YardLight.Client
{
    public class AuthorizationSettings : ISettings
    {
        public string BaseAddress { get; set; }
        public string Token { get; set; }

        public Task<string> GetToken()
        {
            return Task.FromResult(Token);
        }
    }
}
