using Codeer.Friendly.DotNetExecutor;
using System;

namespace Friendly.Core
{
	/// <summary>
	/// .NetのFriendly処理制御。
	/// </summary>
	public class FriendlyControl
	{
		VarPool _pool = new VarPool();
		TypeFinder _typeFinder = new TypeFinder();
        Action<Action> _invoke;

        public FriendlyControl(Action<Action> invoke)
        {
            _invoke = invoke;
        }

        /// <summary>
        /// 処理呼び出し。
        /// </summary>
        /// <param name="info">呼び出し情報。</param>
        /// <returns>戻り値情報。</returns>
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
