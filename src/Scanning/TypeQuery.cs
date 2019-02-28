using System;
using System.Collections.Generic;
using System.Linq;

namespace eQuantic.Core.Ioc.Scanning
{
    public class TypeQuery
    {
        public readonly Func<Type, bool> Filter;
        private readonly TypeClassification _classification;

        public TypeQuery(TypeClassification classification, Func<Type, bool> filter = null)
        {
            Filter = filter ?? (t => true);
            _classification = classification;
        }

        public IEnumerable<Type> Find(AssemblyTypes assembly)
        {
            return assembly.FindTypes(_classification).Where(Filter);
        }
    }
}