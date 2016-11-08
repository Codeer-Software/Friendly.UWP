using System;

namespace Friendly.Core
{
    public class InformationException : Exception 
    {
        public InformationException(){}
        
		public InformationException(string message) : base(message) { }
        
        public InformationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
