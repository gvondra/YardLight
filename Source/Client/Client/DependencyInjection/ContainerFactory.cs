using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client.DependencyInjection
{
    public static class ContainerFactory
    {
        private static readonly IContainer _container;

        static ContainerFactory()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new SupportModule());
            _container = builder.Build();
        }

        public static IContainer Container => _container;
    }
}
