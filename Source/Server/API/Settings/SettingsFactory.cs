using BrassLoon.Interface.Log;
using YardLight.CommonAPI;
using BlAccount = BrassLoon.Interface.Account;

namespace API
{
    public class SettingsFactory : ISettingsFactory
    {
        private readonly BlAccount.ITokenService _tokenService;

        public SettingsFactory(BlAccount.ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public BrassLoon.Interface.Authorization.ISettings CreateAuthorization(Settings settings, string token)
        {
            return new AuthorizationSettings(settings.AuthorizationApiBaseAddress, token);
        }

        public BrassLoon.Interface.Authorization.ISettings CreateAuthorization(Settings settings)
        {
            return new AuthorizationSettings(_tokenService, 
                settings.AuthorizationApiBaseAddress, 
                settings.BrassLoonAccountApiBaseAddress, 
                settings.BrassLoonLogClientId,
                settings.BrassLoonLogClientSecret);
        }

        public YardLight.CommonCore.ISettings CreateCore(Settings settings)
        {
            return new CoreSettings()
            {
                ConnectionString = settings.ConnectionString,
                EnableDatabaseAccessToken = settings.EnableDatabaseAccessToken
            };
        }

        public ISettings CreateLog(Settings settings)
        {
            return new LogSettings(_tokenService,
                settings.BrassLoonLogApiBaseAddress,
                settings.BrassLoonAccountApiBaseAddress,
                settings.BrassLoonLogClientId,
                settings.BrassLoonLogClientSecret);
        }
    }
}
