using System.Runtime.Serialization;

namespace Friendly.Core
{
    /// <summary>
    /// 変数アドレス。
    /// </summary>
    [DataContract]
    public class VarAddress
	{
        /// <summary>
        /// コア。
        /// </summary>
        [DataMember]
        public int Core { get; set; }

		/// <summary>
		/// コンストラクタ。
		/// </summary>
		/// <param name="core">コア。</param>
		public VarAddress(int core)
		{
			Core = core;
		}
	}
}
