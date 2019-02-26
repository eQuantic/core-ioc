using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuantic.Core.Ioc
{
    /// <summary>
    /// Use to direct Assembly type scanning to ignore this type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class IgnoreAttribute : Attribute
    {

    }
}
