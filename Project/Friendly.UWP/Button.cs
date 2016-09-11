using Codeer.Friendly;

namespace Friendly.UWP
{
    public class Button : ManipulatorProxy
    {
        public const string TypeFullName = "Windows.UI.Xaml.Controls.Button";
        public Button(AppVar core): base(core, "Friendly.UWP.Core.ButtonManipulator") { }
        public void EmulateClick() => _manipulator.EmulateClick();
    }
}
