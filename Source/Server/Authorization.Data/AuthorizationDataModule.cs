using YardLight.Authorization.Data.Framework;
using Autofac;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Data
{
    public class AuthorizationDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register<IDbProviderFactory>((context) => new BrassLoon.DataClient.SqlClientProviderFactory());
            builder.RegisterType<ClientDataFactory>().As<IClientDataFactory>();
            builder.RegisterType<ClientDataSaver>().As<IClientDataSaver>();
            builder.RegisterType<EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            builder.RegisterType<EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            builder.RegisterType<RoleDataFactory>().As<IRoleDataFactory>();
            builder.RegisterType<RoleDataSaver>().As<IRoleDataSaver>();
            builder.RegisterType<UserDataFactory>().As<IUserDataFactory>();
            builder.RegisterType<UserDataSaver>().As<IUserDataSaver>();
        }
    }
}
