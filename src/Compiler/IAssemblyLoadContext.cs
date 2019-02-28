using System.IO;
using System.Reflection;

namespace eQuantic.Core.Ioc.Compiler
{
    internal interface IAssemblyLoadContext
    {
        Assembly LoadFromAssemblyName(AssemblyName assemblyName);

        Assembly LoadFromAssemblyPath(string assemblyName);

        Assembly LoadFromStream(Stream assembly);
    }
}