using System;
using System.Linq;
using eQuantic.Core.Ioc.Conventions;
using eQuantic.Core.Ioc.Scanning;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Scan(this IServiceCollection services, Action<IAssemblyScanner> scan)
        {
            var finder = new AssemblyScanner(services);
            scan(finder);

            finder.Start();

            var descriptor = ServiceDescriptor.Singleton(finder);
            services.Add(descriptor);
            return services;
        }

        public static ConnectedConcretions ConnectedConcretions(this IServiceCollection services)
        {
            var concretions = services.FirstOrDefault(x => x.ServiceType == typeof(ConnectedConcretions))
                ?.ImplementationInstance as ConnectedConcretions;

            if (concretions == null)
            {
                concretions = new ConnectedConcretions();
                services.AddSingleton(concretions);
            }

            return concretions;
        }
    }
}