using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Authorization;

namespace YardLight.Client
{
    public class SettingsFactory : ISettingsFactory
    {
        public ISettings CreateAuthorization()
        {
            return CreateAuthorization(AccessToken.Token);
        }

        public ISettings CreateAuthorization(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            return new AuthorizationSettings()
            {
                BaseAddress = Settings.AuthorizationApiBaseAddress,
                Token = token
            };
        }
    }
}
