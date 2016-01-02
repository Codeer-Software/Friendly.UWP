using System;

namespace Friendly.Core
{
    /// <summary>
    /// 情報通知用例外。
    /// </summary>
    public class InformationException : Exception 
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public InformationException(){}

		/// <summary>
        /// コンストラクタ。
		/// </summary>
        /// <param name="message">メッセージ。</param>
		public InformationException(string message) : base(message) { }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="message">メッセージ。</param>
        /// <param name="innerException">内部例外。</param>
        public InformationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
