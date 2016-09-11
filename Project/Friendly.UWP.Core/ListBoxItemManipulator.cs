using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Friendly.UWP.Core
{
    class ListBoxItemManipulator
    {
        public ListBoxItem Core { get; }

        public bool IsSelected => Core.IsSelected;

        public ListBoxItemManipulator(ListBoxItem core)
        {
            Core = core;
        }

        public void EmulateChangeSelected(bool isSelected)
        {
            Core.Focus(FocusState.Pointer);
            Core.IsSelected = isSelected;
        }
    }
}
