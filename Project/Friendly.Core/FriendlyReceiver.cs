using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Friendly.Core
{
    public class FriendlyReceiver
    {
        FriendlyControl _control;
        string _uri;
        bool _alive = false;

        public static FriendlyReceiver Instance { get; private set; }
        
        public static void StartLoop(string uri, Action<Action> invoker)
        {
            Instance = new FriendlyReceiver();
            Instance.StartLoopCore(uri, invoker);
        }

        void StartLoopCore(string uri, Action<Action> invoker)
        {
            _uri = uri;
            _control = new FriendlyControl(invoker);
            var tsk = Task.Factory.StartNew(Loop);
        }

        public void Stop()
        {
            _alive = false;
        }

        void Loop()
        {
            _alive = true;
            while (_alive)
            {
                try
                {
                    LoopCore();
                }
                catch { }
            }
        }

        void LoopCore()
        {
            var uri = new Uri(_uri);

            //要求取得
            var protocolInfo = GetProtocolInfo(uri);
            if (protocolInfo == null)
            {
                return;
            }

            //実行
            var ret = _control.Execute(protocolInfo);

            //結果をサーバーに戻す
            SendReturnInfo(uri, ret);
        }

        static void SendReturnInfo(Uri uri, ReturnInfo returnInfo)
        {
            //シリアライズ
            byte[] bin = null;
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractSerializer(returnInfo.GetType(), AssemblyManager.GetDataContractableTypes());
                serializer.WriteObject(ms, returnInfo);
                bin = ms.ToArray();
            }

            //アップロードバッファに書き込み
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var requestStream = request.GetRequestStreamAsync();
            requestStream.Wait();
            using (var reqStream = requestStream.Result)
            {
                reqStream.Write(bin, 0, bin.Length);
            }

            //送信
            var response = request.GetResponseAsync();
            response.Wait();
            ((HttpWebResponse)response.Result).Dispose();
        }

        static ProtocolInfo GetProtocolInfo(Uri uri)
        {
            var protocolGet = (HttpWebRequest)WebRequest.Create(uri);
            protocolGet.Method = "POST";
            var resProtocolGet = protocolGet.GetResponseAsync();
            resProtocolGet.Wait();

            //レスポンス読み込み
            using (var httpWebResponsex = (HttpWebResponse)resProtocolGet.Result)
            using (var reader = new StreamReader(httpWebResponsex.GetResponseStream()))
            {
                string protocol;
                protocol = reader.ReadToEnd();
                if (string.IsNullOrEmpty(protocol))
                {
                    return null;
                }
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(protocol)))
                {
                    var serializer = new DataContractSerializer(typeof(ProtocolInfo), AssemblyManager.GetDataContractableTypes());
                    return (ProtocolInfo)serializer.ReadObject(ms);
                }
            }
        }
    }
}
