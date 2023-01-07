using Autofac;
using BrassLoon.RestClient;

namespace YardLight.Interface
{
    public class InterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<Service>().As<IService>().SingleInstance();
            builder.RegisterType<RestUtil>().SingleInstance();
            builder.RegisterType<ExceptionService>().As<IExceptionService>();
            builder.RegisterType<ItterationService>().As<IItterationService>();
            builder.RegisterType<MetricService>().As<IMetricService>();
            builder.RegisterType<ProjectService>().As<IProjectService>();
            builder.RegisterType<RoleService>().As<IRoleService>();
            builder.RegisterType<TokenService>().As<ITokenService>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<WorkItemCommentService>().As<IWorkItemCommentService>();
            builder.RegisterType<WorkItemService>().As<IWorkItemService>();
            builder.RegisterType<WorkItemTypeService>().As<IWorkItemTypeService>();
        }
    }
}
