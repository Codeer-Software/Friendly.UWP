using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Friendly.UWP.Core
{
    public class TextBoxManipulator
    {
        public TextBox Core { get; }

        public string Text => Core.Text;

        public TextBoxManipulator(TextBox core)
        {
            Core = core;
        }

        public void EmulateChangeText(string text)
        {
            Core.Focus(FocusState.Pointer);
            Core.Text = text;
        }
    }
}
