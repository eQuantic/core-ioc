using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuantic.Core.Ioc.Scanning
{
    [Flags]
    public enum TypeClassification : short
    {
        All = 0,
        Open = 1,
        Closed = 2,
        Interfaces = 4,
        Abstracts = 8,
        Concretes = 16
    }
}
