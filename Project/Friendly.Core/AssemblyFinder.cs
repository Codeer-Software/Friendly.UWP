using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Friendly.Core
{
    public class AssemblyFinder
    {
        public Dictionary<string, Assembly> Assemblies { get; } = new Dictionary<string, Assembly>();
        public Dictionary<string, TypeInfo> Types { get; } = new Dictionary<string, TypeInfo>();
        public List<Type> DataContractableTypes { get; } = new List<Type>();

        public Assembly[] GetAssemblies()
        {
            return Assemblies.Values.ToArray();
        }

        public void FindAssembly(Assembly assembly)
        {
            if (Assemblies.ContainsKey(assembly.FullName))
            {
                return;
            }
            Assemblies.Add(assembly.FullName, assembly);
            foreach (var e in assembly.DefinedTypes)
            {
                FindAllType(e);
            }
            foreach (var e in Types.Select(e => e.Value.Assembly).ToArray())
            {
                FindAssembly(e);
            }
        }

        internal Type GetType(string typeFullName)
        {
            TypeInfo t;
            return Types.TryGetValue(typeFullName, out t) ? t.AsType() : null;
        }

        void FindAllType(TypeInfo typeInfo)
        {
            if (string.IsNullOrEmpty(typeInfo.FullName))
            {
                return;
            }
            if (Types.ContainsKey(typeInfo.FullName))
            {
                return;
            }

            //Generic
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() != typeInfo.AsType())
            {
                FindAllType(typeInfo.GetGenericTypeDefinition().GetTypeInfo());
                foreach (var e in typeInfo.GenericTypeArguments)
                {
                    FindAllType(e.GetTypeInfo());
                }
                return;
            }
            Types.Add(typeInfo.FullName, typeInfo);
            if (!typeInfo.IsGenericType && typeInfo.GetCustomAttribute(typeof(DataContractAttribute)) != null)
            {
                DataContractableTypes.Add(typeInfo.AsType());
            }

            //ベースクラス
            if (typeInfo.BaseType != null)
            {
                FindAllType(typeInfo.BaseType.GetTypeInfo());
            }

            //インターフェイス
            foreach (var e in typeInfo.ImplementedInterfaces)
            {
                FindAllType(e.GetTypeInfo());
            }

            //フィールド
            foreach (var e in typeInfo.DeclaredFields)
            {
                FindAllType(e.FieldType.GetTypeInfo());
            }

            //プロパティー
            foreach (var e in typeInfo.DeclaredProperties)
            {
                FindAllType(e.PropertyType.GetTypeInfo());
            }

            //メソッド
            foreach (var e in typeInfo.DeclaredMethods)
            {
                FindAllType(e.ReturnType.GetTypeInfo());
                foreach (var ee in e.GetParameters())
                {
                    FindAllType(ee.ParameterType.GetTypeInfo());
                }
            }

            //コンストラクタ
            foreach (var e in typeInfo.DeclaredConstructors)
            {
                foreach (var ee in e.GetParameters())
                {
                    FindAllType(ee.ParameterType.GetTypeInfo());
                }
            }

            //イベント
            foreach (var e in typeInfo.DeclaredEvents)
            {
                FindAllType(e.GetType().GetTypeInfo());
            }
        }
    }
}
