using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace eQuantic.Core.Ioc.Compiler
{
    internal interface IAssemblyLoadContext
    {
        Assembly LoadFromStream(Stream assembly);
        Assembly LoadFromAssemblyName(AssemblyName assemblyName);
        Assembly LoadFromAssemblyPath(string assemblyName);
    }
}