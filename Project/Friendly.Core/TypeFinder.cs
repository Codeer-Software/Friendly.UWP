using Friendly.Core;
using System;

//ここだけは、このネームスペースを守る必要がある
namespace Codeer.Friendly.DotNetExecutor
{
	/// <summary>
	/// 型に関するユーティリティー。
	/// </summary>
	public class TypeFinder
	{
        /// <summary>  
        /// タイプフルネームから型を取得する。
        /// </summary>  
        /// <param name="typeFullName">タイプフルネーム。</param>  
        /// <returns>取得した型。</returns>  
        public Type GetType(string typeFullName)
		{
            return AssemblyManager.GetType(typeFullName);
		}  
	}
}
