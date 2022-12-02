using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            builder.RegisterType<ProjectFactory>().As<IProjectFactory>();
            builder.RegisterType<ProjectSaver>().As<IProjectSaver>();
            builder.RegisterType<WorkItemCommentFactory>().As<IWorkItemCommentFactory>();
            builder.RegisterType<WorkItemFactory>().As<IWorkItemFactory>();
            builder.RegisterType<WorkItemStatusFactory>().As<IWorkItemStatusFactory>();
            builder.RegisterType<WorkItemSaver>().As<IWorkItemSaver>();
            builder.RegisterType<WorkItemTypeFactory>().As<IWorkItemTypeFactory>();
            builder.RegisterType<WorkItemTypeSaver>().As<IWorkItemTypeSaver>();
        }
    }
}
