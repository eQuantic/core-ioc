using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace eQuantic.Core.Ioc.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly List<Type> _enumerableTypes = new List<Type>
        {
            typeof (IEnumerable<>),
            typeof (IList<>),
            typeof (IReadOnlyList<>),
            typeof (List<>)
        };

        public static readonly Dictionary<Type, string> Aliases = new Dictionary<Type, string>
        {
            {typeof(int), "int"},
            {typeof(void), "void"},
            {typeof(string), "string"},
            {typeof(long), "long"},
            {typeof(double), "double"},
            {typeof(bool), "bool"},
            {typeof(Task), "Task"},
            {typeof(object), "object"},
            {typeof(object[]), "object[]"}
        };

        public static bool CanBeCreated(this Type type)
        {
            return type.IsConcrete() && type.GetConstructors().Any();
        }

        public static bool CanBeCastTo<T>(this Type type)
        {
            if (type == null) return false;
            var destinationType = typeof(T);

            return CanBeCastTo(type, destinationType);
        }

        public static bool CanBeCastTo(this Type type, Type destinationType)
        {
            if (type == null) return false;
            if (type == destinationType) return true;

            return destinationType.IsAssignableFrom(type);
        }

        public static bool CouldCloseTo(this Type openConcretion, Type closedInterface)
        {
            var openInterface = closedInterface.GetGenericTypeDefinition();
            var arguments = closedInterface.GetGenericArguments();

            var concreteArguments = openConcretion.GetGenericArguments();
            return arguments.Length == concreteArguments.Length && openConcretion.CanBeCastTo(openInterface);
        }

        public static bool IsEnumerable(this Type type)
        {
            if (type.IsArray) return true;

            return type.IsGenericType && _enumerableTypes.Contains(type.GetGenericTypeDefinition());
        }

        public static Type DetermineElementType(this Type serviceType)
        {
            if (serviceType.IsArray)
            {
                return serviceType.GetElementType();
            }

            return serviceType.GetGenericArguments().First();
        }

        public static bool IsInNamespace(this Type type, string nameSpace)
        {
            if (type == null) return false;

            return type.Namespace.StartsWith(nameSpace);
        }

        public static bool IsOpenGeneric(this Type type)
        {
            if (type == null) return false;
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericTypeDefinition || typeInfo.ContainsGenericParameters;
        }

        public static bool IsConcrete(this Type type)
        {
            if (type == null) return false;

            var typeInfo = type.GetTypeInfo();

            return !typeInfo.IsAbstract && !typeInfo.IsInterface;
        }

        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetTypeInfo().GetCustomAttributes<T>().Any();
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetTypeInfo().GetCustomAttributes<T>().FirstOrDefault();
        }

        public static Type FindFirstInterfaceThatCloses(this Type TPluggedType, Type templateType)
        {
            return TPluggedType.FindInterfacesThatClose(templateType).FirstOrDefault();
        }

        public static IEnumerable<Type> FindInterfacesThatClose(this Type TPluggedType, Type templateType)
        {
            return RawFindInterfacesThatCloses(TPluggedType, templateType).Distinct();
        }

        /// <summary>
        /// Derives the full type name *as it would appear in C# code*
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string FullNameInCode(this Type type)
        {
            if (Aliases.ContainsKey(type)) return Aliases[type];

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var cleanName = type.Name.Split('`').First();
                if (type.IsNested && type.DeclaringType?.IsGenericTypeDefinition == true)
                {
                    cleanName = $"{type.ReflectedType.NameInCode(type.GetGenericArguments())}.{cleanName}";
                    return $"{type.Namespace}.{cleanName}";
                }

                if (type.IsNested)
                {
                    cleanName = $"{type.ReflectedType.NameInCode()}.{cleanName}";
                }

                var args = type.GetGenericArguments().Select(x => x.FullNameInCode()).Join(", ");

                return $"{type.Namespace}.{cleanName}<{args}>";
            }

            if (type.FullName == null)
            {
                return type.Name;
            }

            return type.FullName.Replace("+", ".");
        }

        /// <summary>
        /// Derives the type name *as it would appear in C# code*
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string NameInCode(this Type type)
        {
            if (Aliases.ContainsKey(type)) return Aliases[type];

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var cleanName = type.Name.Split('`').First().Replace("+", ".");
                if (type.IsNested)
                {
                    cleanName = $"{type.ReflectedType.NameInCode()}.{cleanName}";
                }

                var args = type.GetGenericArguments().Select(x => x.FullNameInCode()).Join(", ");

                return $"{cleanName}<{args}>";
            }

            if (type.MemberType == MemberTypes.NestedType)
            {
                return $"{type.ReflectedType.NameInCode()}.{type.Name}";
            }

            return type.Name.Replace("+", ".").Replace("`", "_");
        }

        /// <summary>
        /// Derives the type name *as it would appear in C# code* for a type with generic parameters
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericParameterTypes"></param>
        /// <returns></returns>
        public static string NameInCode(this Type type, Type[] genericParameterTypes)
        {
            var cleanName = type.Name.Split('`').First().Replace("+", ".");
            var args = genericParameterTypes.Select(x => x.FullNameInCode()).Join(", ");
            return $"{cleanName}<{args}>";
        }

        public static string ShortNameInCode(this Type type)
        {
            if (Aliases.ContainsKey(type)) return Aliases[type];

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var cleanName = type.Name.Split('`').First().Replace("+", ".");
                if (type.IsNested)
                {
                    cleanName = $"{type.ReflectedType.NameInCode()}.{cleanName}";
                }

                var args = type.GetGenericArguments().Select(x => x.ShortNameInCode()).Join(", ");

                return $"{cleanName}<{args}>";
            }

            if (type.MemberType == MemberTypes.NestedType)
            {
                return $"{type.ReflectedType.NameInCode()}.{type.Name}";
            }

            return type.Name.Replace("+", ".");
        }

        private static IEnumerable<Type> RawFindInterfacesThatCloses(Type TPluggedType, Type templateType)
        {
            if (!TPluggedType.IsConcrete()) yield break;

            if (templateType.GetTypeInfo().IsInterface)
            {
                foreach (
                    var interfaceType in
                    TPluggedType.GetInterfaces()
                        .Where(type => type.GetTypeInfo().IsGenericType && (type.GetGenericTypeDefinition() == templateType)))
                {
                    yield return interfaceType;
                }
            }
            else if (TPluggedType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
                     (TPluggedType.GetTypeInfo().BaseType.GetGenericTypeDefinition() == templateType))
            {
                yield return TPluggedType.GetTypeInfo().BaseType;
            }

            if (TPluggedType.GetTypeInfo().BaseType == typeof(object)) yield break;

            foreach (var interfaceType in RawFindInterfacesThatCloses(TPluggedType.GetTypeInfo().BaseType, templateType))
            {
                yield return interfaceType;
            }
        }
    }
}
