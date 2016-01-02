using System;
using System.Collections.Generic;
using System.Reflection;

namespace Friendly.Core
{
    public static class AssemblyManager
    {
        static AssemblyFinder _finder = new AssemblyFinder();

        static AssemblyManager() { }

        public static void AddInterfaceType(Assembly assembly)
        {
            lock (_finder)
            {
                _finder.FindAssembly(assembly);
            }
        }

        public static Type GetType(string typeFullName)
        {
            lock (_finder)
            {
                return _finder.GetType(typeFullName);
            }
        }
        public static Dictionary<string, Assembly> GetAssemblies()
        {
            lock (_finder)
            {
                return new Dictionary<string, Assembly>(_finder.Assemblies);
            }
        }

        public static Type[] GetDataContractableTypes()
        {
            lock (_finder)
            {
                return _finder.DataContractableTypes.ToArray();
            }
        }
    }
}
