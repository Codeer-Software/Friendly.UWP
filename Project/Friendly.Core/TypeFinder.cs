using Friendly.Core;
using System;

//keep namespace. don't change it.
namespace Codeer.Friendly.DotNetExecutor
{
	public class TypeFinder
	{
        public Type GetType(string typeFullName)
		{
            return AssemblyManager.GetType(typeFullName);
		}  
	}
}
