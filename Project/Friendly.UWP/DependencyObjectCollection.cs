using Codeer.Friendly;
using System;

namespace Friendly.UWP
{
#if ENG
    /// <summary>
    /// Collection of T in target app.
    /// </summary>
#else
    /// <summary>
    /// Tのコレクションです。
    /// </summary>
#endif
    class DependencyObjectCollection : IDependencyObjectCollection, IAppVarOwner
    {
#if ENG
        /// <summary>
        /// List＜T＞ in target app.
        /// </summary>
#else
        /// <summary>
        /// 対象プロセス内のList＜T＞ です。
        /// </summary>
#endif
        public AppVar AppVar { get; private set; }

#if ENG
        /// <summary>
        /// Count.
        /// </summary>
#else
        /// <summary>
        /// コレクションの数。
        /// </summary>
#endif
        public int Count { get { return (int)AppVar["Count"]().Core; } }

#if ENG
        /// <summary>
        /// T of index in target app .
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>T of index in target app .</returns>
#else
        /// <summary>
        /// 対象プロセス内での指定のインデックスのT。
        /// </summary>
        /// <param name="index">インデックス。</param>
        /// <returns>対象プロセス内での指定のインデックスのT。</returns>
#endif
        public AppVar this[int index]
        {
            get { return AppVar["[]"](index); }
        }

#if ENG
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appVar">List＜T＞ in target app.</param>
#else
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appVar">対象プロセス内のList＜T＞ </param>
#endif
        public DependencyObjectCollection(AppVar appVar)
        {
            AppVar = appVar;
        }

#if ENG
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appVar">List＜T＞ in target app.</param>
#else
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appVar">対象プロセス内のList＜T＞ </param>
#endif
        public DependencyObjectCollection(IAppVarOwner appVar)
        {
            AppVar = appVar.AppVar;
        }

#if ENG
        /// <summary>
        /// Get only one item.
        /// </summary>
        /// <returns></returns>
#else
        /// <summary>
        /// コレクションの要素が一つであることを確認してそれを取得する。
        /// </summary>
        /// <returns></returns>
#endif
        public AppVar Single()
        {
            if (Count != 1)
            {
                //TODO
                throw new NotSupportedException(string.Format("NotSingle", Count));
            }
            return AppVar["[]"](0);
        }
    }
}
