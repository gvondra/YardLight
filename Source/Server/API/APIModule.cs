using Autofac;
namespace API
{
    public class APIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            builder.RegisterModule(new YardLight.Core.CoreModule());
            builder.RegisterModule(new YardLight.Interface.Authorization.InterfaceAuthorizationModule());
            builder.RegisterType<SettingsFactory>().As<ISettingsFactory>().InstancePerLifetimeScope();
        }
    }
}
