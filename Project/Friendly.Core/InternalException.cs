using System;

namespace Friendly.Core
{
    /// <summary>
    /// 内部例外。
    /// </summary>
    public class InternalException : Exception 
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public InternalException(){}

		/// <summary>
        /// コンストラクタ。
		/// </summary>
        /// <param name="message">メッセージ。</param>
		public InternalException(string message) : base(message) { }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="message">メッセージ。</param>
        /// <param name="innerException">内部例外。</param>
        public InternalException(string message, Exception innerException) : base(message, innerException) { }
    }
}
