using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client.DependencyInjection
{
    public class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new YardLight.Interface.Authorization.InterfaceAuthorizationModule());
            builder.RegisterType<SettingsFactory>().As<ISettingsFactory>();
        }
    }
}
