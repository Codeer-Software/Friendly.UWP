using Friendly.Core;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Friendly.UWP.Inside
{
    class ProtocolSerializer
    {
        AssemblyFinder _finder = new AssemblyFinder();

        internal ProtocolSerializer()
        {
            FindAllDataContractableTypes();
        }

        internal byte[] WriteObject(object obj)
        {
            Func<byte[]> func = () =>
            {
                var serializer = new DataContractSerializer(obj.GetType(), _finder.DataContractableTypes.ToArray());
                using (var m = new MemoryStream())
                {
                    serializer.WriteObject(m, obj);
                    return m.ToArray();
                }
            };

            try
            {
                return func();
            }
            catch { }
            FindAllDataContractableTypes();
            return func();
        }

        internal object ReadObject(byte[] bin)
        {
            Func<object> func = () =>
            {
                var serializer = new DataContractSerializer(typeof(Friendly.Core.ReturnInfo), _finder.DataContractableTypes.ToArray());
                return serializer.ReadObject(new MemoryStream(bin));
            };
            try
            {
                return func();
            }
            catch { }
            FindAllDataContractableTypes();
            return func();
        }

        internal void FindAllDataContractableTypes()
        {
            foreach (var e in AppDomain.CurrentDomain.GetAssemblies())
            {
                _finder.FindAssembly(e);
            }
        }
    }
}
