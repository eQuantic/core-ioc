using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace eQuantic.Core.Ioc.Extensions
{
    internal static class AssemblyExtensions
    {
        public static void ForAttribute<T>(this Assembly provider, Action<T> action) where T : Attribute
        {
            foreach (T attribute in provider.GetAllAttributes<T>())
            {
                action(attribute);
            }
        }

        public static void ForAttribute<T>(this Assembly provider, Action<T> action, Action elseDo)
            where T : Attribute
        {
            var found = false;
            foreach (T attribute in provider.GetAllAttributes<T>())
            {
                action(attribute);
                found = true;
            }

            if (!found) elseDo();
        }

        public static IEnumerable<T> GetAllAttributes<T>(this Assembly provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T)).OfType<T>();
        }

        public static T GetAttribute<T>(this Assembly provider) where T : Attribute
        {
            var atts = provider.GetCustomAttributes(typeof(T));
            return atts.FirstOrDefault() as T;
        }

        public static bool HasAttribute<T>(this Assembly provider) where T : Attribute
        {
            return provider.IsDefined(typeof(T));
        }
    }
}