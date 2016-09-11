using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Friendly.UWP.Core
{
    public class ToggleButtonManipulator
    {
        public ToggleButton Core { get; }
        public bool IsThreeState => Core.IsThreeState;
        public bool? IsChecked => Core.IsChecked;

        public ToggleButtonManipulator(ToggleButton core)
        {
            Core = core;
        }

        public void EmulateCheck(bool? value)
        {
            Core.Focus(FocusState.Pointer);
            Core.IsChecked = value;
        }
    }
}
