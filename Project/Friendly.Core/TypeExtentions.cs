using System;
using System.Linq;
using System.Reflection;

namespace Friendly.Core
{
    static class TypeExtentions
    {
        internal static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }
        internal static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        internal static ConstructorInfo[] GetConstructors(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors.ToArray();
        }

        internal static FieldInfo GetField(this Type type, string operation, BindingFlags bind)
        {
            var f = type.GetTypeInfo().GetDeclaredField(operation);
            if (f == null)
            {
                return null;
            }
            if (bind.IsStatic != f.IsStatic)
            {
                return null;
            }
            return f;
        }

        internal static PropertyInfo GetProperty(this Type type, string operation, BindingFlags bind)
        {
            var f = type.GetTypeInfo().GetDeclaredProperty(operation);
            if (f == null)
            {
                return null;
            }
            bool isStatic = false;
            if (f.GetMethod != null && f.GetMethod.IsStatic)
            {
                isStatic = true;
            }
            if (f.SetMethod != null && f.SetMethod.IsStatic)
            {
                isStatic = true;
            }
            if (bind.IsStatic != isStatic)
            {
                return null;
            }
            return f;
        }

        internal static Type BaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        internal static bool IsAssignableFrom(this Type type, Type dst)
        {
            return type.GetTypeInfo().IsAssignableFrom(dst.GetTypeInfo());
        }

        internal static MethodInfo[] GetMethods(this Type type, string operation, BindingFlags bind)
        {
            var ms = type.GetTypeInfo().GetDeclaredMethods(operation).Where(e => e.IsStatic == bind.IsStatic).ToList();
            var p = type.GetTypeInfo().GetDeclaredProperty(operation);
            if (p != null && p.GetMethod != null)
            {
                if (bind.IsStatic == p.GetMethod.IsStatic)
                {
                    ms.Add(p.GetMethod);
                }
            }
            if (p != null && p.SetMethod != null)
            {
                if (bind.IsStatic == p.SetMethod.IsStatic)
                {
                    ms.Add(p.SetMethod);
                }
            }
            return ms.ToArray();
        }
    }
}
