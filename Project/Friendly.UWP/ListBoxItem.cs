using Codeer.Friendly;
using Codeer.Friendly.Dynamic;

namespace Friendly.UWP
{
    public class ListBoxItem : ManipulatorProxy
    {
        public const string TypeFullName = "Windows.UI.Xaml.Controls.ListBoxItem";

        public bool IsSelected => _manipulator.IsSelected;

        public ListBoxItem(AppVar core) : base(core, "Friendly.UWP.Core.ListBoxManipulator") { }

        public void EmulateChangeSelected(bool isSelected)
            => _manipulator.EmulateChangeSelected(isSelected);
    }
}
