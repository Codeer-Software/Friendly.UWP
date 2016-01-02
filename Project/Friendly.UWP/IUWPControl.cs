using System;

namespace Friendly.UWP
{
    public interface IUWPControl
    {
        string Uri { get; }
        void Start();
        void Connected();
        void Stop(Action stopReceiver);
    }
}
