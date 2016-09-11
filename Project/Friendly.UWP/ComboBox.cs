using Codeer.Friendly;

namespace Friendly.UWP
{
    public class ComboBox : Selector
    {
        new public const string TypeFullName = "Windows.UI.Xaml.Controls.ComboBox";
        public TextBox TextBox => new TextBox(_manipulator.TextBox);
        public ComboBox(AppVar core) : base(core, "Friendly.UWP.Core.ComboBoxManipulator") { }
    }
}
