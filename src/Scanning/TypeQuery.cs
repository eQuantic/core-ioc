﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuantic.Core.Ioc.Scanning
{
    public class TypeQuery
    {
        private readonly TypeClassification _classification;

        public TypeQuery(TypeClassification classification, Func<Type, bool> filter = null)
        {
            Filter = filter ?? (t => true);
            _classification = classification;
        }

        public readonly Func<Type, bool> Filter;

        public IEnumerable<Type> Find(AssemblyTypes assembly)
        {
            return assembly.FindTypes(_classification).Where(Filter);
        }
    }
}
