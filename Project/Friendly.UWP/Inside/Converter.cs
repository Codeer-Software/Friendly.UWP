using System;
using System.Reflection;

namespace Friendly.UWP.Inside
{
    class Converter
    {
        internal static Core.ProtocolInfo Convert(Codeer.Friendly.Inside.Protocol.ProtocolInfo src)
            => new Core.ProtocolInfo(Convert(src.ProtocolType),
                Convert(src.OperationTypeInfo), Convert(src.VarAddress), src.TypeFullName, src.Operation, Convert(src.Arguments));

        internal static Codeer.Friendly.Inside.Protocol.ReturnInfo Convert(Core.ReturnInfo src)
        {
            if (src.Exception != null)
            {
                return new Codeer.Friendly.Inside.Protocol.ReturnInfo(Convert(src.Exception));
            }
            if (src.ReturnValue is Core.VarAddress)
            {
                return new Codeer.Friendly.Inside.Protocol.ReturnInfo(Convert((Core.VarAddress)src.ReturnValue));
            }
            return new Codeer.Friendly.Inside.Protocol.ReturnInfo(src.ReturnValue);
        }

        static object[] Convert(object[] src)
        {
            var dst = new object[src.Length];
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] is Codeer.Friendly.Inside.Protocol.VarAddress)
                {
                    dst[i] = Convert((Codeer.Friendly.Inside.Protocol.VarAddress)src[i]);
                }
                else
                {
                    dst[i] = src[i];
                }
            }
            return dst;
        }

        static Core.VarAddress Convert(Codeer.Friendly.Inside.Protocol.VarAddress src)
        {
            return src == null ? null : new Core.VarAddress(src.Core);
        }

        static Core.OperationTypeInfo Convert(Codeer.Friendly.OperationTypeInfo src)
        {
            return src == null ? null : new Core.OperationTypeInfo(src.Target, src.Arguments);
        }

        static Core.ProtocolType Convert(Codeer.Friendly.Inside.Protocol.ProtocolType src)
        {
            switch (src)
            {
                case Codeer.Friendly.Inside.Protocol.ProtocolType.VarInitialize: return Core.ProtocolType.VarInitialize;
                case Codeer.Friendly.Inside.Protocol.ProtocolType.VarNew: return Core.ProtocolType.VarNew;
                case Codeer.Friendly.Inside.Protocol.ProtocolType.BinOff: return Core.ProtocolType.BinOff;
                case Codeer.Friendly.Inside.Protocol.ProtocolType.GetValue: return Core.ProtocolType.GetValue;
                case Codeer.Friendly.Inside.Protocol.ProtocolType.SetValue: return Core.ProtocolType.SetValue;
                case Codeer.Friendly.Inside.Protocol.ProtocolType.GetElements: return Core.ProtocolType.GetElements;
                case Codeer.Friendly.Inside.Protocol.ProtocolType.Operation: return Core.ProtocolType.Operation;
                case Codeer.Friendly.Inside.Protocol.ProtocolType.IsEmptyVar: return Core.ProtocolType.IsEmptyVar;
                case Codeer.Friendly.Inside.Protocol.ProtocolType.AsyncResultVarInitialize: return Core.ProtocolType.AsyncResultVarInitialize;
                case Codeer.Friendly.Inside.Protocol.ProtocolType.AsyncOperation: return Core.ProtocolType.AsyncOperation;
            }
            throw new NotSupportedException();
        }

        static Codeer.Friendly.Inside.Protocol.VarAddress Convert(Friendly.Core.VarAddress returnValue)
            => new Codeer.Friendly.Inside.Protocol.VarAddress(returnValue.Core);

        static Codeer.Friendly.Inside.Protocol.ExceptionInfo Convert(Friendly.Core.ExceptionInfo src)
        {
            try
            {
                throw new NotSupportedException();
            }
            catch (Exception e)
            {
                var e2 = new Codeer.Friendly.Inside.Protocol.ExceptionInfo(e);
                var type = e2.GetType();
                type.GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(e2, src.Message);
                type.GetField("_exceptionType", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(e2, src.ExceptionType);
                type.GetField("_helpLink", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(e2, src.HelpLink);
                type.GetField("_source", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(e2, src.Source);
                type.GetField("_stackTrace", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(e2, src.StackTrace);
                return e2;
            }
        }
    }
}
