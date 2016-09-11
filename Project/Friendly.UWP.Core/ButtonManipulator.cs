using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;

namespace Friendly.UWP.Core
{
    public class ButtonManipulator
    {
        public Button Core { get; }

        public ButtonManipulator(Button core)
        {
            Core = core;
        }

        public void EmulateClick()
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(Core);
            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();
        }
    }
}
