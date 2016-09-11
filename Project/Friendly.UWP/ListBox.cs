using Codeer.Friendly;

namespace Friendly.UWP
{
    public class ListBox : Selector
    {
        new public const string TypeFullName = "Windows.UI.Xaml.Controls.ListBox";

        public ListBox(AppVar core) : base(core, "Friendly.UWP.Core.ListBoxManipulator") { }

        public ListBoxItem GetItem(int index)
            => new ListBoxItem(_manipulator.GetItem(index));

        public void EnsureVisible(int index)
            => _manipulator.EnsureVisible(index);
    }
}
