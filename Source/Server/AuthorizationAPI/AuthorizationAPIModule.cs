using Autofac;
using AutoMapper;

namespace AuthorizationAPI
{
    public class AuthorizationAPIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            builder.RegisterModule(new YardLight.Authorization.Core.AuthorizationCoreModule());
            builder.RegisterModule(new YardLight.Authorization.Data.AuthorizationDataModule());
            builder.RegisterModule(new YardLight.Interface.Authorization.InterfaceAuthorizationModule());
            builder.Register<IMapper>((context) => MapperConfiguration.CreateMapper());
            builder.RegisterType<SettingsFactory>().As<ISettingsFactory>();
        }
    }
}
