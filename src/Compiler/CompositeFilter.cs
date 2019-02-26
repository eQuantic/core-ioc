using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuantic.Core.Ioc.Compiler
{
    internal class CompositeFilter<T>
    {
        private readonly CompositePredicate<T> _excludes = new CompositePredicate<T>();
        private readonly CompositePredicate<T> _includes = new CompositePredicate<T>();

        internal CompositePredicate<T> Includes
        {
            get { return _includes; }
            set { }
        }

        internal CompositePredicate<T> Excludes
        {
            get { return _excludes; }
            set { }
        }

        internal bool Matches(T target)
        {
            return Includes.MatchesAny(target) && Excludes.DoesNotMatcheAny(target);
        }
    }
}
