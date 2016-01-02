using Windows.UI.Xaml;
using Windows.UI.Core;
using System;
using System.Reflection;
using Friendly.Core;

namespace Friendly.UWP.Core
{
    public static class FriendlyExecutor
    {
        public static void Start(string uri)
        {
            var c = Window.Current;
            AssemblyManager.AddInterfaceType(typeof(FriendlyExecutor).GetTypeInfo().Assembly);
            AssemblyManager.AddInterfaceType(typeof(FriendlyReceiver).GetTypeInfo().Assembly);
            AssemblyManager.AddInterfaceType(Application.Current.GetType().GetTypeInfo().Assembly);
            FriendlyReceiver.StartLoop(uri,
                (a) => c.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => a()).AsTask().Wait());
        }
    }
}
