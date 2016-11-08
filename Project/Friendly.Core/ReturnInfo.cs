using System.Runtime.Serialization;

namespace Friendly.Core
{
    [DataContract]
    public class ReturnInfo
	{
        [DataMember]
        public object ReturnValue { get; set; }
        
        [DataMember]
        public ExceptionInfo Exception { get; set; }

        public ReturnInfo() { }

		public ReturnInfo(object returnValue)
		{
			ReturnValue = returnValue;
		}

        public ReturnInfo(ExceptionInfo exception)
		{
            Exception = exception;
		}
	}
}
