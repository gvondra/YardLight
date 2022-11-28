using Autofac;
using BrassLoon.DataClient;
using YardLight.Data.Framework;

namespace YardLight.Data
{
    public sealed class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SqlClientProviderFactory>().As<ISqlDbProviderFactory>().SingleInstance();
            builder.RegisterType<ProjectDataFactory>().As<IProjectDataFactory>();
            builder.RegisterType<ProjectDataSaver>().As<IProjectDataSaver>();
            builder.RegisterType<ProjectUserDataFactory>().As<IProjectUserDataFactory>();
            builder.RegisterType<ProjectUserDataSaver>().As<IProjectUserDataSaver>();
            builder.RegisterType<WorkItemStatusDataFactory>().As<IWorkItemStatusDataFactory>();
            builder.RegisterType<WorkItemStatusDataSaver>().As<IWorkItemStatusDataSaver>();
            builder.RegisterType<WorkItemTypeDataFactory>().As<IWorkItemTypeDataFactory>();
            builder.RegisterType<WorkItemTypeDataSaver>().As<IWorkItemTypeDataSaver>();
        }
    }
}
