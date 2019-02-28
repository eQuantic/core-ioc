using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using eQuantic.Core.Ioc.Compiler;
using eQuantic.Core.Ioc.Conventions;
using eQuantic.Core.Ioc.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace eQuantic.Core.Ioc.Scanning
{
    [Ignore]
    public class AssemblyScanner : IAssemblyScanner
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly CompositeFilter<Type> _filter = new CompositeFilter<Type>();
        private readonly IServiceCollection _services;
        private Task<TypeSet> _typeFinder;

        public AssemblyScanner(IServiceCollection services)
        {
            _services = services;
            Exclude(type => type.HasAttribute<IgnoreAttribute>());
        }

        public List<IRegistrationConvention> Conventions { get; } = new List<IRegistrationConvention>();
        public string Description { get; set; }
        public Task<TypeSet> TypeFinder => _typeFinder;

        public FindAllTypesFilter AddAllTypesOf<TPluginType>()
        {
            return AddAllTypesOf(typeof(TPluginType));
        }

        public FindAllTypesFilter AddAllTypesOf(Type pluginType)
        {
            var filter = new FindAllTypesFilter(pluginType);
            With(filter);

            return filter;
        }

        public void ApplyRegistrations(IServiceCollection services)
        {
            foreach (var convention in Conventions)
            {
                convention.ScanTypes(_typeFinder.Result, services);
            }
        }

        public void AssembliesAndExecutablesFromApplicationBaseDirectory(Func<Assembly, bool> assemblyFilter = null)
        {
            var assemblies = AssemblyFinder.FindAssemblies(assemblyFilter, txt =>
            {
                Console.WriteLine("Could not load assembly from file " + txt);
            }, includeExeFiles: true);

            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        public void AssembliesAndExecutablesFromPath(string path)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("Could not load assembly from file " + txt);
            }, includeExeFiles: true);

            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        public void AssembliesAndExecutablesFromPath(string path,
            Func<Assembly, bool> assemblyFilter)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("Could not load assembly from file " + txt);
            }, includeExeFiles: true).Where(assemblyFilter);

            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        public void AssembliesFromApplicationBaseDirectory()
        {
            AssembliesFromApplicationBaseDirectory(a => true);
        }

        public void AssembliesFromApplicationBaseDirectory(Func<Assembly, bool> assemblyFilter)
        {
            var assemblies = AssemblyFinder.FindAssemblies(assemblyFilter, txt =>
            {
                Console.WriteLine("Could not load assembly from file " + txt);
            }, includeExeFiles: false);

            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        public void AssembliesFromPath(string path)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("Could not load assembly from file " + txt);
            }, includeExeFiles: false);

            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        public void AssembliesFromPath(string path,
            Func<Assembly, bool> assemblyFilter)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("Could not load assembly from file " + txt);
            }, includeExeFiles: false).Where(assemblyFilter);

            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        public void Assembly(Assembly assembly)
        {
            if (!_assemblies.Contains(assembly))
                _assemblies.Add(assembly);
        }

        public void Assembly(string assemblyName)
        {
            Assembly(AssemblyLoader.ByName(assemblyName));
        }

        public void AssemblyContainingType<T>()
        {
            AssemblyContainingType(typeof(T));
        }

        public void AssemblyContainingType(Type type)
        {
            _assemblies.Add(type.GetTypeInfo().Assembly);
        }

        public void ConnectImplementationsToTypesClosing(Type openGenericType)
        {
            var convention = new GenericConnectionScanner(openGenericType);
            With(convention);
        }

        public bool Contains(string assemblyName)
        {
            return _assemblies
                .Select(assembly => new AssemblyName(assembly.FullName))
                .Any(aName => aName.Name == assemblyName);
        }

        public void Convention<T>() where T : IRegistrationConvention, new()
        {
            var previous = Conventions.FirstOrDefault(scanner => scanner is T);
            if (previous == null)
                With(new T());
        }

        public void Describe(StringWriter writer)
        {
            writer.WriteLine(Description);
            writer.WriteLine("Assemblies");
            writer.WriteLine("----------");

            _assemblies.OrderBy(x => x.FullName).Each(x => writer.WriteLine("* " + x));
            writer.WriteLine();

            writer.WriteLine("Conventions");
            writer.WriteLine("--------");
            Conventions.Each(x => writer.WriteLine("* " + x));
        }

        public void Exclude(Func<Type, bool> exclude)
        {
            _filter.Excludes += exclude;
        }

        public void ExcludeNamespace(string nameSpace)
        {
            Exclude(type => type.IsInNamespace(nameSpace));
        }

        public void ExcludeNamespaceContainingType<T>()
        {
            ExcludeNamespace(typeof(T).Namespace);
        }

        public void ExcludeType<T>()
        {
            Exclude(type => type == typeof(T));
        }

        public void Include(Func<Type, bool> predicate)
        {
            _filter.Includes += predicate;
        }

        public void IncludeNamespace(string nameSpace)
        {
            Include(type => type.IsInNamespace(nameSpace));
        }

        public void IncludeNamespaceContainingType<T>()
        {
            IncludeNamespace(typeof(T).Namespace);
        }

        public void RegisterConcreteTypesAgainstTheFirstInterface()
        {
            var convention = new FirstInterfaceConvention();
            With(convention);
        }

        public void Start()
        {
            if (!Conventions.Any())
            {
                throw new InvalidOperationException($"There are no {nameof(IRegistrationConvention)}'s in this scanning operation. ");
            }
            _typeFinder = TypeRepository.FindTypes(_assemblies, type => _filter.Matches(type));
        }

        public void TheCallingAssembly()
        {
            if (_services.GetType().Assembly != typeof(AssemblyScanner).Assembly)
            {
                Assembly(_services.GetType().Assembly);
                return;
            }

            var callingAssembly = CallingAssembly.Find();

            if (callingAssembly != null)
            {
                Assembly(callingAssembly);
            }
            else
            {
                throw new InvalidOperationException("Could not determine the calling assembly, you may need to explicitly call IAssemblyScanner.Assembly()");
            }
        }

        public void With(IRegistrationConvention convention)
        {
            Conventions.Fill(convention);
        }
    }
}