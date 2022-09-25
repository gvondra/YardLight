using YardLight.Authorization.Core.Framework;
using Autofac;
namespace YardLight.Authorization.Core
{
    public class AuthorizationCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClientFactory>().As<IClientFactory>();
            builder.RegisterType<ClientSaver>().As<IClientSaver>();
            builder.RegisterType<ClientSecretProcessor>().As<IClientSecretProcessor>();
            builder.RegisterType<EmailAddressFactory>().As<IEmailAddressFactory>();
            builder.RegisterType<EmailAddressSaver>().As<IEmailAddressSaver>();
            builder.RegisterType<RoleFactory>().As<IRoleFactory>();
            builder.RegisterType<RoleSaver>().As<IRoleSaver>();
            builder.RegisterType<UserFactory>().As<IUserFactory>();
            builder.RegisterType<UserSaver>().As<IUserSaver>();
        }
    }
}
