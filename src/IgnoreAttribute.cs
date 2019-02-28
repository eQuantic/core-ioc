using System;

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