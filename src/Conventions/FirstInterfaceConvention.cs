using System;
using System.Linq;
using eQuantic.Core.Ioc.Extensions;
using eQuantic.Core.Ioc.Scanning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace eQuantic.Core.Ioc.Conventions
{
    public class FirstInterfaceConvention : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, IServiceCollection services)
        {
            foreach (var type in types.FindTypes(TypeClassification.Concretes).Where(x => x.GetConstructors().Any()))
            {
                var interfaceType = type.GetInterfaces().FirstOrDefault(x => x != typeof(IDisposable));
                if (interfaceType != null && !interfaceType.HasAttribute<IgnoreAttribute>() && !type.IsOpenGeneric())
                {
                    services.TryAddTransient(interfaceType, type);
                }
            }

        }

        public override string ToString()
        {
            return "Register all concrete types against the first interface (if any) that they implement";
        }
    }
}