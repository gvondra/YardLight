﻿using BrassLoon.Interface.Log;
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

        public YardLight.Interface.Authorization.ISettings CreateAuthorization(Settings settings, string token)
        {
            return new YardLightAuthorizationSettings(settings.AuthorizationApiBaseAddress, token);
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
