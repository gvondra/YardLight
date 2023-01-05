using Autofac;
using YardLight.CommonCore;
using YardLight.Framework;
namespace YardLight.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<Saver>().SingleInstance();
            builder.RegisterModule(new YardLight.Data.DataModule());
            builder.RegisterType<ItterationFactory>().As<IItterationFactory>();
            builder.RegisterType<ItterationSaver>().As<IItterationSaver>();
            builder.RegisterType<ProjectFactory>().As<IProjectFactory>();
            builder.RegisterType<ProjectSaver>().As<IProjectSaver>();
            builder.RegisterType<WorkItemCommentFactory>().As<IWorkItemCommentFactory>();
            builder.RegisterType<WorkItemCommentSaver>().As<IWorkItemCommentSaver>();
            builder.RegisterType<WorkItemFactory>().As<IWorkItemFactory>();
            builder.RegisterType<WorkItemStatusFactory>().As<IWorkItemStatusFactory>();
            builder.RegisterType<WorkItemSaver>().As<IWorkItemSaver>();
            builder.RegisterType<WorkItemTypeFactory>().As<IWorkItemTypeFactory>();
            builder.RegisterType<WorkItemTypeSaver>().As<IWorkItemTypeSaver>();
        }
    }
}
