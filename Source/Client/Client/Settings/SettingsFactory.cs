using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client
{
    public class SettingsFactory : ISettingsFactory
    {
        public Interface.ISettings CreateApi()
        {
            if (string.IsNullOrEmpty(AccessToken.Token))
                throw new ArgumentNullException(nameof(AccessToken.Token));
            return CreateApi(AccessToken.Token);
        }

        public Interface.ISettings CreateApi(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            return new APISettings(Settings.ApiBaseAddress, token);
        }
    }
}
