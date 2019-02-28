using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using eQuantic.Core.Ioc.Compiler;

namespace eQuantic.Core.Ioc.Scanning
{
    public static class AssemblyFinder
    {
        public static IEnumerable<Assembly> FindAssemblies(Action<string> logFailure, bool includeExeFiles)
        {
            string path;
            try
            {
                path = AppContext.BaseDirectory;
            }
            catch (Exception)
            {
                path = System.IO.Directory.GetCurrentDirectory();
            }

            return FindAssemblies(path, logFailure, includeExeFiles);
        }

        public static IEnumerable<Assembly> FindAssemblies(string assemblyPath, Action<string> logFailure, bool includeExeFiles)
        {
            var dllFiles = Directory.EnumerateFiles(assemblyPath, "*.dll", SearchOption.AllDirectories);
            var files = dllFiles;

            if (includeExeFiles)
            {
                var exeFiles = Directory.EnumerateFiles(assemblyPath, "*.exe", SearchOption.AllDirectories);
                files = dllFiles.Concat(exeFiles);
            }

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Assembly assembly = null;

                try
                {
                    assembly = AssemblyContext.Loader.LoadFromAssemblyName(new AssemblyName(name));
                }
                catch (Exception)
                {
                    try
                    {
                        assembly = AssemblyContext.Loader.LoadFromAssemblyPath(file);
                    }
                    catch (Exception)
                    {
                        logFailure(file);
                    }
                }

                if (assembly != null)
                {
                    yield return assembly;
                }
            }
        }

        internal static IEnumerable<Assembly> FindAssemblies(Func<Assembly, bool> filter,
            Action<string> onDirectoryFound = null, bool includeExeFiles = false)
        {
            if (filter == null)
            {
                filter = a => true;
            }

            if (onDirectoryFound == null)
            {
                onDirectoryFound = dir => { };
            }

            return FindAssemblies(file => { }, includeExeFiles: includeExeFiles).Where(filter);
        }

        internal static class AssemblyContext
        {
#if NET461
		    public static readonly IAssemblyLoadContext Loader = new CustomAssemblyLoadContext();
#else
            public static readonly IAssemblyLoadContext Loader = new AssemblyLoadContextWrapper(System.Runtime.Loader.AssemblyLoadContext.Default);
#endif
        }
    }
}