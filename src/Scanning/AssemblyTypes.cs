﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using eQuantic.Core.Ioc.Extensions;

namespace eQuantic.Core.Ioc.Scanning
{
    public class AssemblyScanRecord
    {
        public Exception LoadException;
        public string Name;

        public override string ToString()
        {
            return LoadException == null ? Name : $"{Name} (Failed)";
        }
    }

    public class AssemblyTypes
    {
        public readonly AssemblyShelf ClosedTypes = new AssemblyShelf();
        public readonly AssemblyShelf OpenTypes = new AssemblyShelf();
        private readonly AssemblyScanRecord _record = new AssemblyScanRecord();

        public AssemblyTypes(Assembly assembly) : this(assembly.FullName, () => assembly.ExportedTypes)
        {
        }

        public AssemblyTypes(string name, Func<IEnumerable<Type>> typeSource)
        {
            _record.Name = name;

            try
            {
                var types = typeSource();
                foreach (var type in types)
                {
                    var shelf = type.IsOpenGeneric() ? OpenTypes : ClosedTypes;
                    shelf.Add(type);
                }
            }
            catch (Exception ex)
            {
                _record.LoadException = ex;
            }
        }

        public AssemblyScanRecord Record
        {
            get { return _record; }
        }

        public IEnumerable<Type> FindTypes(TypeClassification classification)
        {
            if (classification == TypeClassification.All)
            {
                return ClosedTypes.AllTypes().Concat(OpenTypes.AllTypes());
            }

            if (classification == TypeClassification.Interfaces)
            {
                return allTypes(ClosedTypes.Interfaces, OpenTypes.Interfaces);
            }

            if (classification == TypeClassification.Abstracts)
            {
                return allTypes(ClosedTypes.Abstracts, OpenTypes.Abstracts);
            }

            if (classification == TypeClassification.Concretes)
            {
                return allTypes(ClosedTypes.Concretes, OpenTypes.Concretes);
            }

            if (classification == TypeClassification.Open)
            {
                return OpenTypes.AllTypes();
            }

            if (classification == TypeClassification.Closed)
            {
                return ClosedTypes.AllTypes();
            }

            return allTypes(selectGroups(classification).ToArray());
        }

        private IEnumerable<Type> allTypes(params IList<Type>[] shelves)
        {
            return shelves.SelectMany(x => x);
        }

        private IEnumerable<IList<Type>> selectGroups(TypeClassification classification)
        {
            return selectShelves(classification).SelectMany(x => x.SelectLists(classification));
        }

        private IEnumerable<AssemblyShelf> selectShelves(TypeClassification classification)
        {
            var open = classification.HasFlag(TypeClassification.Open);
            var closed = classification.HasFlag(TypeClassification.Closed);

            if ((open && closed) || (!open && !closed))
            {
                yield return OpenTypes;
                yield return ClosedTypes;
            }
            else if (open)
            {
                yield return OpenTypes;
            }
            else if (closed)
            {
                yield return ClosedTypes;
            }
        }
    }
}