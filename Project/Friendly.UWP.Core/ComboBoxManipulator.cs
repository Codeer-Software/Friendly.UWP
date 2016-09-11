using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Friendly.UWP.Core
{
    public class ComboBoxManipulator : SelectorManipulator
    {
        public new ComboBox Core { get; }

        public TextBox TextBox => Core.VisualTree().ByType<TextBox>().Single();

        public ComboBoxManipulator(ComboBox core) : base(core)
        {
            Core = core;
        }
    }
}
