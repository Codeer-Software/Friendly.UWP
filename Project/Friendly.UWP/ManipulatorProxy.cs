using Codeer.Friendly;
using Codeer.Friendly.Dynamic;

namespace Friendly.UWP
{
    public class ManipulatorProxy : IAppVarOwner
    {
        public AppVar AppVar { get; }
        protected dynamic _manipulator { get; }

        protected ManipulatorProxy(AppVar core, string fullName)
        {
            AppVar = core;
            _manipulator = core.App.Type(fullName)(core);
        }
    }
}
