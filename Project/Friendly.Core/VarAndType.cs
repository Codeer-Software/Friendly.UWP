using System;

namespace Friendly.Core
{
    class VarAndType
    {
        object _core;
        Type _type;
        
        internal object Core { get { return _core; } }

        internal Type Type { get { return _type; } }

        internal VarAndType(object core)
        {
            _core = core;
            if (core != null)
            {
                _type = core.GetType();
            }
        }

        internal VarAndType(object core, Type type)
        {
            _core = core;
            _type = type;
        }
    }
}
