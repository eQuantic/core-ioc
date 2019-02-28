using System.Reflection;

namespace eQuantic.Core.Ioc.Extensions
{
    public static class MemberInfoExtensions
    {
        public static bool CanBeOverridden(this MethodInfo method)
        {
            if (method.IsAbstract) return true;

            if (method.IsVirtual && !method.IsFinal) return true;

            return false;
        }
    }
}