using System;

namespace Friendly.Core
{
    public class InternalException : Exception 
    {
        public InternalException(){}
        
		public InternalException(string message) : base(message) { }

        public InternalException(string message, Exception innerException) : base(message, innerException) { }
    }
}
