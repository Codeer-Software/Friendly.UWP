using System;
using System.Runtime.Serialization;

namespace Friendly.Core
{
    [DataContract]
    public class OperationTypeInfo
    {
        [DataMember]
        public string Target { get; set; }

        [DataMember]
        public string[] Arguments { get; set; }

        public OperationTypeInfo(string target, params string[] arguments)
        {
            if (string.IsNullOrEmpty(target))
            {
                throw new ArgumentNullException("target");
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            for (int i = 0; i < arguments.Length; i++)
            {
                if (string.IsNullOrEmpty(arguments[i]))
                {
                    throw new ArgumentException("arguments is invalid");
                }
            }
            Target = target;
            Arguments = arguments;
        }
    }
}
