using Codeer.Friendly;

namespace Friendly.UWP
{
    public class Selector : ManipulatorProxy
    {
        public const string TypeFullName = "Windows.UI.Xaml.Controls.Primitives.Selector";

        public Selector(AppVar core) : base(core, "Friendly.UWP.Core.SelectorManipulator") { }
        protected Selector(AppVar core, string fullName) : base(core, fullName) { }

        public int ItemCount => _manipulator.Count;

        public void EmulateChangeSelectedIndex(int index)
            => _manipulator.EmulateChangeSelectedIndex(index);
    }
}
