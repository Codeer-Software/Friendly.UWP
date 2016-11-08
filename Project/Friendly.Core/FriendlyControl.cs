using Codeer.Friendly.DotNetExecutor;
using System;

namespace Friendly.Core
{
	public class FriendlyControl
	{
		VarPool _pool = new VarPool();
		TypeFinder _typeFinder = new TypeFinder();
        Action<Action> _invoke;

        public FriendlyControl(Action<Action> invoke)
        {
            _invoke = invoke;
        }
        
        public ReturnInfo Execute(ProtocolInfo info)
		{
            try
            {
                return FriendlyInvoker.Execute(_invoke, _pool, _typeFinder, info);
            }
            catch (Exception e)
            {
                return new ReturnInfo(new ExceptionInfo(e));
            }
        }
    }
}
