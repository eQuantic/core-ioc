﻿using System.Reflection;

namespace eQuantic.Core.Ioc.Scanning
{
    public static class AssemblyLoader
    {
        public static Assembly ByName(string assemblyName)
        {
            return Assembly.Load(new AssemblyName(assemblyName));
        }
    }
}