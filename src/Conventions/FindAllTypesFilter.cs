using System;
using System.Linq;
using System.Reflection;
using eQuantic.Core.Ioc.Extensions;
using eQuantic.Core.Ioc.Scanning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace eQuantic.Core.Ioc.Conventions
{
    public class FindAllTypesFilter : IRegistrationConvention
    {
        private readonly Type _serviceType;

        public FindAllTypesFilter(Type serviceType)
        {
            _serviceType = serviceType;
        }

        public bool Matches(Type type)
        {
            return CanBeCastTo(type, _serviceType) && type.GetConstructors().Any() && type.CanBeCreated();
        }

        public void ScanTypes(TypeSet types, IServiceCollection services)
        {
            if (_serviceType.IsOpenGeneric())
            {
                var scanner = new GenericConnectionScanner(_serviceType);
                scanner.ScanTypes(types, services);
            }
            else
            {
                types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed).Where(Matches).Each(type =>
                {
                    var serviceType = DetermineLeastSpecificButValidType(_serviceType, type);

                    services.TryAddTransient(serviceType, type);
                });
            }
        }

        public override string ToString()
        {
            return "Find and register all types implementing " + _serviceType.FullName;
        }

        private static bool CanBeCast(Type serviceType, Type implementationType)
        {
            try
            {
                return CheckGenericType(implementationType, serviceType);
            }
            catch (Exception e)
            {
                var message =
                    string.Format("Could not Determine Whether Type '{0}' plugs into Type '{1}'",
                        serviceType.Name,
                        implementationType.Name);
                throw new ArgumentException(message, e);
            }
        }

        private static bool CanBeCastTo(Type implementationType, Type serviceType)
        {
            if (implementationType == null) return false;

            if (implementationType == serviceType) return true;

            if (serviceType.IsOpenGeneric())
            {
                return CanBeCast(serviceType, implementationType);
            }

            if (implementationType.IsOpenGeneric())
            {
                return false;
            }

            return serviceType.GetTypeInfo().IsAssignableFrom(implementationType.GetTypeInfo());
        }

        private static bool CheckGenericType(Type pluggedType, Type pluginType)
        {
            if (pluggedType == null || pluginType == null) return false;

            if (pluginType.GetTypeInfo().IsAssignableFrom(pluggedType.GetTypeInfo())) return true;

            // check interfaces
            foreach (var type in pluggedType.GetInterfaces())
            {
                if (!type.GetTypeInfo().IsGenericType)
                {
                    continue;
                }

                if (type.GetGenericTypeDefinition() == pluginType)
                {
                    return true;
                }
            }

            var baseType = pluggedType.GetTypeInfo().BaseType;
            if (baseType != null && baseType.GetTypeInfo().IsGenericType)
            {
                var baseTypeGenericDefinition = baseType.GetGenericTypeDefinition();

                if (baseTypeGenericDefinition == pluginType)
                {
                    return true;
                }
                else
                {
                    return CanBeCast(pluginType, baseTypeGenericDefinition);
                }
            }

            return false;
        }

        private static Type DetermineLeastSpecificButValidType(Type pluginType, Type type)
        {
            if (pluginType.IsGenericTypeDefinition && !type.IsOpenGeneric())
                return type.FindFirstInterfaceThatCloses(pluginType);

            return pluginType;
        }
    }
}