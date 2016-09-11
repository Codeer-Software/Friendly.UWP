using Codeer.Friendly;

namespace Friendly.UWP
{
    public class TextBox : ManipulatorProxy
    {
        public const string TypeFullName = "Windows.UI.Xaml.Controls.TextBox";

        public string Text => _manipulator.Text;

        public TextBox(AppVar core) : base(core, "Friendly.UWP.Core.TextBoxManipulator") { }

        public void EmulateChangeText(string text)
            => _manipulator.EmulateChangeText(text);
    }
}
