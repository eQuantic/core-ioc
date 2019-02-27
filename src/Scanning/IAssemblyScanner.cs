﻿using System;
using System.Reflection;
using eQuantic.Core.Ioc.Conventions;

namespace eQuantic.Core.Ioc.Scanning
{
    public interface IAssemblyScanner
    {
        /// <summary>
        /// Optional user-supplied diagnostic description of this scanning operation
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Add an Assembly to the scanning operation
        /// </summary>
        /// <param name="assembly"></param>
        void Assembly(Assembly assembly);

        /// <summary>
        /// Add an Assembly by name to the scanning operation
        /// </summary>
        /// <param name="assemblyName"></param>
        void Assembly(string assemblyName);

        /// <summary>
        /// Add the Assembly that contains type T to the scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void AssemblyContainingType<T>();

        /// <summary>
        /// Add the Assembly that contains type to the scanning operation
        /// </summary>
        /// <param name="type"></param>
        void AssemblyContainingType(Type type);

        /// <summary>
        /// Add all concrete types of the Plugin Type as Instances of Plugin Type
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        FindAllTypesFilter AddAllTypesOf<TPluginType>();

        /// <summary>
        /// Add all concrete types of the Plugin Type as Instances of Plugin Type
        /// </summary>
        /// <param name="pluginType"></param>
        FindAllTypesFilter AddAllTypesOf(Type pluginType);
        
        /// <summary>
        /// Exclude types that match the Predicate from being scanned
        /// </summary>
        /// <param name="exclude"></param>
        void Exclude(Func<Type, bool> exclude);

        /// <summary>
        /// Exclude all types in this nameSpace or its children from the scanning operation
        /// </summary>
        /// <param name="nameSpace"></param>
        void ExcludeNamespace(string nameSpace);

        /// <summary>
        /// Exclude all types in this nameSpace or its children from the scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ExcludeNamespaceContainingType<T>();

        /// <summary>
        /// Only include types matching the Predicate in the scanning operation. You can
        /// use multiple Include() calls in a single scanning operation
        /// </summary>
        /// <param name="predicate"></param>
        void Include(Func<Type, bool> predicate);

        /// <summary>
        /// Only include types from this nameSpace or its children in the scanning operation.  You can
        /// use multiple Include() calls in a single scanning operation
        /// </summary>
        /// <param name="nameSpace"></param>
        void IncludeNamespace(string nameSpace);

        /// <summary>
        /// Only include types from this nameSpace or its children in the scanning operation.  You can
        /// use multiple Include() calls in a single scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void IncludeNamespaceContainingType<T>();

        /// <summary>
        /// Exclude this specific type from the scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ExcludeType<T>();

        /// <summary>
        /// Adds a registration convention to be applied to all the types in this
        /// logical "scan" operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Convention<T>() where T : IRegistrationConvention, new();

        /// <summary>
        /// Adds a registration convention to be applied to all the types in this
        /// logical "scan" operation
        /// </summary>
        void With(IRegistrationConvention convention);

        /// <summary>
        /// Automatically registers all concrete types without primitive arguments
        /// against its first interface, if any
        /// </summary>
        void RegisterConcreteTypesAgainstTheFirstInterface();
        
        void TheCallingAssembly();
        void AssembliesFromApplicationBaseDirectory();
        void AssembliesFromApplicationBaseDirectory(Func<Assembly, bool> assemblyFilter);

        void AssembliesAndExecutablesFromApplicationBaseDirectory(Func<Assembly, bool> assemblyFilter = null);

        void AssembliesAndExecutablesFromPath(string path);
        void AssembliesFromPath(string path);

        void AssembliesAndExecutablesFromPath(string path,
            Func<Assembly, bool> assemblyFilter);

        void AssembliesFromPath(string path,
            Func<Assembly, bool> assemblyFilter);
    }
}
