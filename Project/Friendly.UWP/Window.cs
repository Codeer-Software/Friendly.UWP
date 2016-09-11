using Codeer.Friendly;
using Codeer.Friendly.Dynamic;

namespace Friendly.UWP
{
    public class Window
    {
        AppVar _core;

        public AppVar Content => _core.Dynamic().Content;

        public Window(AppVar core)
        {
            _core = core;
        }
    }
}
