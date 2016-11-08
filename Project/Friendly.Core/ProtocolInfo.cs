using System.Runtime.Serialization;

namespace Friendly.Core
{
    [DataContract]
    public class ProtocolInfo
	{
        [DataMember]
        public ProtocolType ProtocolType { get; set; }
        
        [DataMember]
        public OperationTypeInfo OperationTypeInfo { get; set; }

        [DataMember]
        public VarAddress VarAddress { get; set; }

        [DataMember]
        public string TypeFullName { get; set; }

        [DataMember]
        public string Operation { get; set; }

        [DataMember]
        public object[] Arguments { get; set; }

        public ProtocolInfo(ProtocolType protocolType, OperationTypeInfo operationTypeInfo, VarAddress varAddress, string typeFullName, string operation, object[] arguments)
		{
            ProtocolType = protocolType;
            OperationTypeInfo = operationTypeInfo;
            VarAddress = varAddress;
			TypeFullName = typeFullName;
			Operation = operation;
			Arguments = arguments;
		}
	}
}