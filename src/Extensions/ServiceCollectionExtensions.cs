using System;
using eQuantic.Core.Ioc.Scanning;

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
    }
}