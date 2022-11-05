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
            builder.RegisterType<SettingsFactory>().As<ISettingsFactory>().InstancePerLifetimeScope();
        }
    }
}
