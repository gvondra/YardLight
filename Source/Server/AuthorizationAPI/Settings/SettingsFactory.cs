using BrassLoon.Interface.Account;
using System;
using YardLight.CommonAPI;

namespace AuthorizationAPI
{
    public class SettingsFactory : ISettingsFactory
    {
        private readonly Lazy<ITokenService> _tokenSevice;

        public SettingsFactory(Lazy<ITokenService> tokenSevice)
        {
            _tokenSevice = tokenSevice;
        }

        public YardLight.CommonCore.ISettings CreateCore(Settings settings)
        {
            return new CoreSettings
            {
                ConnectionString = settings.ConnectionString,
                EnableDatabaseAccessToken = settings.EnableDatabaseAccessToken
            };
        }

        public BrassLoon.Interface.Log.ISettings CreateLog(Settings settings)
        {
            return new LogSettings(_tokenSevice.Value,
                settings.BrassLoonLogApiBaseAddress,
                settings.BrassLoonAccountApiBaseAddress,
                settings.BrassLoonLogClientId,
                settings.BrassLoonLogClientSecret
                );
        }
    }
}
