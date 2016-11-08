using System;
using System.Runtime.Serialization;

namespace Friendly.Core
{
    [DataContract]
    public class ExceptionInfo
    {
        [DataMember]
        public string ExceptionType { get; set; }
        
        [DataMember]
        public string HelpLink { get; set; }
        
        [DataMember]
        public string Message { get; set; }
        
        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        public ExceptionInfo(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            //library's error.
            if (exception is InformationException)
            {
                Message = exception.Message;
                return;
            }

            //others.
            //if it has InternalError, use it.
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            Message = exception.Message;
            ExceptionType = exception.GetType().FullName;
            HelpLink = exception.HelpLink;
            Source = exception.Source;
            StackTrace = exception.StackTrace;
        }
    }
}
