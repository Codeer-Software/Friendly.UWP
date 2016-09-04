using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Friendly.Core;

namespace Friendly.UWP.Inside
{
    class FriendlyOperationServer
    {
        string _uri;
        Task _loop;
        Exception _loopException;
        HttpListener _listener;
        Stack<ProtocolInfo> _stackProtocol = new Stack<ProtocolInfo>();
        ProtocolSerializer _serializer;

        class ProtocolAndRet
        {
            internal ProtocolInfo Info { get; set; }
            internal ReturnInfo Ret { get; set; }
        }
        List<ProtocolAndRet> _ret = new List<ProtocolAndRet>();

        internal void StartLoop(string uri)
        {
            _uri = uri;
            _loop = Task.Factory.StartNew(Loop);
        }

        internal ReturnInfo SendAndReceive(ProtocolInfo info)
        {
            lock(_stackProtocol)
            {
                _stackProtocol.Push(info);
            }

            while (true)
            {
                lock(_ret)
                {
                    var ret = _ret.Where(e => ReferenceEquals(e.Info, info)).FirstOrDefault();
                    if (ret != null)
                    {
                        _ret.Remove(ret);
                        return ret.Ret;
                    }
                    if (_loopException != null)
                    {
                        throw _loopException;
                    }
                }
            }
        }

        internal void Stop()
        {
            _listener.Abort();
            _loop.Wait();
        }

        void Loop()
        {
            try
            {
                _serializer = new ProtocolSerializer();
                LoopCore();
            }
            catch (Exception e)
            {
                _loopException = e;
            }
        }

        void LoopCore()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(_uri);
            _listener.Start();

            while (true)
            {
                ProtocolInfo info = WaitRequest();
                if (info == null)
                {
                    continue;
                }
                var obj = GetReturnInfo();
                lock (_ret)
                {
                    _ret.Add(new ProtocolAndRet() { Info = info, Ret = obj });
                }
            }
        }

        ProtocolInfo WaitRequest()
        {
            var context = _listener.GetContext();
            using (var response = context.Response)
            {
                ProtocolInfo info = null;
                lock (_stackProtocol)
                {
                    if (_stackProtocol.Count == 0)
                    {
                        return null;
                    }
                    info = _stackProtocol.Pop();
                }

                var reader = new StreamReader(context.Request.InputStream);
                var requestString = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(requestString))
                {
                    return null;
                }

                var bin = _serializer.WriteObject(info);
                response.OutputStream.Write(bin, 0, bin.Length);
                return info;
            }
        }

        ReturnInfo GetReturnInfo()
        {
            var context = _listener.GetContext();
            using (var response = context.Response)
            {
                var reader = new StreamReader(context.Request.InputStream);
                var requestString = reader.ReadToEnd();
                return (ReturnInfo)_serializer.ReadObject(Encoding.UTF8.GetBytes(requestString));
            }
        }
    }
}
