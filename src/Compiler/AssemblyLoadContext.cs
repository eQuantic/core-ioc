using System.IO;
using System.Reflection;

#if !NET461
using System.Runtime.Loader;
#endif

namespace eQuantic.Core.Ioc.Compiler
{
#if !NET461
    public sealed class CustomAssemblyLoadContext : AssemblyLoadContext, IAssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return Assembly.Load(assemblyName);
        }

        Assembly IAssemblyLoadContext.LoadFromAssemblyName(AssemblyName assemblyName)
        {
            return Load(assemblyName);
        }
    }

    public sealed class AssemblyLoadContextWrapper : IAssemblyLoadContext
    {
        private readonly AssemblyLoadContext ctx;

        public AssemblyLoadContextWrapper(AssemblyLoadContext ctx)
        {
            this.ctx = ctx;
        }

        public Assembly LoadFromStream(Stream assembly)
        {
            return ctx.LoadFromStream(assembly);
        }

        public Assembly LoadFromAssemblyName(AssemblyName assemblyName)
        {
            return ctx.LoadFromAssemblyName(assemblyName);
        }

        public Assembly LoadFromAssemblyPath(string assemblyName)
        {
            return ctx.LoadFromAssemblyPath(assemblyName);
        }
    }
#else

    public class CustomAssemblyLoadContext : IAssemblyLoadContext
	{
		Assembly IAssemblyLoadContext.LoadFromAssemblyName(AssemblyName assemblyName)
		{
			return Assembly.Load(assemblyName);
		}

        public Assembly LoadFromAssemblyName(string assemblyName)
		{
			return Assembly.Load(assemblyName);
		}

        public Assembly LoadFromAssemblyPath(string assemblyName)
		{
			return Assembly.LoadFrom(assemblyName);
		}

        public Assembly LoadFromStream(Stream assembly)
		{
			if (assembly is MemoryStream memStream)
			{
				return Assembly.Load(memStream.ToArray());
			}

			using (var stream = new MemoryStream())
			{
				assembly.CopyTo(stream);
				return Assembly.Load(stream.ToArray());
			}
		}
	}

#endif
}