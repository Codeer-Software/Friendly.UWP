using System.Runtime.Serialization;

namespace Friendly.Core
{
    [DataContract]
    public class VarAddress
	{
        [DataMember]
        public int Core { get; set; }

		public VarAddress(int core)
		{
			Core = core;
		}
	}
}
