using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Friendly.UWP.Core
{
    public class SelectorManipulator
    {
        public Selector Core { get; }

        public SelectorManipulator(Selector core)
        {
            Core = core;
        }

        public int ItemCount => Core.Items.Count;

        public void EmulateChangeSelectedIndex(int index)
        {
            Core.Focus(FocusState.Pointer);
            Core.SelectedIndex = index;
        }
    }
}
