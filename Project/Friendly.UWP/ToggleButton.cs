using Codeer.Friendly;

namespace Friendly.UWP
{
    public class ToggleButton : ManipulatorProxy
    {
        public const string TypeFullName = "Windows.UI.Xaml.Controls.Primitives.ToggleButton";

        public bool IsThreeState => _manipulator.IsThreeState;
        public bool? IsChecked => _manipulator.IsChecked;

        public ToggleButton(AppVar core) : base(core, "Friendly.UWP.Core.ToggleButtonManipulator") { }

        public void EmulateCheck(bool? value)
            => _manipulator.EmulateCheck(value);
    }
}
