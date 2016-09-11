using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;

namespace Friendly.UWP.Core
{
    public class ListBoxManipulator : SelectorManipulator
    {
        public new ListBox Core { get; }

        public ListBoxManipulator(ListBox core) : base(core)
        {
            Core = core;
        }

        public ListBoxItem GetItem(int index)
        {
            EnsureVisible(index);
            return (ListBoxItem)Core.ContainerFromIndex(index);
        }

        public void EnsureVisible(int index)
        {
            Core.Focus(FocusState.Pointer);
            Core.ScrollIntoView(Core.Items[index]);
            Core.UpdateLayout();
            if (Core.ContainerFromIndex(index) == null)
            {
                ListBoxAutomationPeer peer = new ListBoxAutomationPeer(Core);
                var scroll = peer.GetPattern(PatternInterface.Scroll) as IScrollProvider;
                scroll.SetScrollPercent(scroll.HorizontalScrollPercent, 0);
                Core.UpdateLayout();
                while (Core.ContainerFromIndex(index) == null)
                {
                    scroll.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
                    Core.UpdateLayout();
                }
            }
        }
    }
}
