using Autofac;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface
{
    public class InterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance(new RestUtil());
            builder.RegisterType<Service>().As<IService>().SingleInstance();
            builder.RegisterType<ExceptionService>().As<IExceptionService>();
            builder.RegisterType<MetricService>().As<IMetricService>();
            builder.RegisterType<ProjectService>().As<IProjectService>();
            builder.RegisterType<WorkItemStatusService>().As<IWorkItemStatusService>();
            builder.RegisterType<WorkItemTypeService>().As<IWorkItemTypeService>();
        }
    }
}
