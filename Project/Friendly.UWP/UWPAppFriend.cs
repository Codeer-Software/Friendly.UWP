using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Inside.Protocol;
using Friendly.UWP.Inside;
using System;

namespace Friendly.UWP
{
    public class UWPAppFriend : AppFriend, IDisposable
    {
        Core _core = new Core();
        IUWPControl _starter;

        protected override IFriendlyConnector FriendlyConnector { get { return _core; } }

        public UWPAppFriend(IUWPControl starter)
        {
            _core.Server.StartLoop(starter.Uri);
            _starter = starter;
            starter.Start();
            Friendly.Core.ResourcesLocal.Initialize();
            this.Type<Friendly.Core.ResourcesLocal>().Instance = Friendly.Core.ResourcesLocal.Instance;
            starter.Connected();
        }

        class Core : IFriendlyConnector
        {
            public FriendlyOperationServer Server { get; private set; } = new FriendlyOperationServer();
            public AppFriend App { get; private set; }
            public object Identity { get { return App; } }

            public ReturnInfo SendAndReceive(ProtocolInfo info)
            {
                return Converter.Convert(Server.SendAndReceive(Converter.Convert(info)));
            }
        }

        public void Dispose()
        {
            _starter.Stop(()=>
            {
                try
                {
                    this.Type<Friendly.Core.FriendlyReceiver>().Instance.Stop();
                }
                catch { }
            });
            _core.Server.Stop();
        }
    }
}
