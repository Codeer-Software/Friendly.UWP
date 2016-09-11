using Codeer.Friendly;
using Codeer.Friendly.Dynamic;

namespace Friendly.UWP
{
#if ENG
    /// <summary>
    /// Search by binding.
    /// </summary>
#else
    /// <summary>
    /// Binding情報から要素を取得するためのユーティリティー
    /// </summary>
#endif
    public static class UIElementSearcherExtensions
    {
#if ENG
        /// <summary>
        /// Search by binding from DependencyObject collection.
        /// </summary>
        /// <param name="collection">DependencyObject collection.</param>
        /// <param name="path">Binding path.</param>
        /// <param name="dataItem">DataItem.</param>
        /// <returns>Hit elements.</returns>
#else
        /// <summary>
        /// Binding情報から要素を検索。
        /// </summary>
        /// <param name="collection">DependencyObjectのコレクション。</param>
        /// <param name="path">バインディングパス。</param>
        /// <param name="dataItem">DataItem。</param>
        /// <returns>ヒットした要素。</returns>
#endif
        public static IDependencyObjectCollection ByBinding(this IDependencyObjectCollection collection, string path, ExplicitAppVar dataItem = null)
        {
            var app = ((IAppVarOwner)collection).AppVar.App;
            return new DependencyObjectCollection(app.Type("Friendly.UWP.Core.UIElementSearcher").ByBinding(collection, path, dataItem));
        }

#if ENG
        /// <summary>
        /// Search by Type from DependencyObject collection.
        /// </summary>
        /// <param name="collection">DependencyObject collection.</param>
        /// <param name="typeFullName">Target type.</param>
        /// <returns>Hit elements.</returns>
#else
        /// <summary>
        /// タイプから要素を検索。
        /// </summary>
        /// <param name="collection">DependencyObjectのコレクション。</param>
        /// <param name="typeFullName">検索対象のタイプ。</param>
        /// <returns>ヒットした要素。</returns>
#endif
        public static IDependencyObjectCollection ByType(this IDependencyObjectCollection collection, string typeFullName)
        {
            var app = ((IAppVarOwner)collection).AppVar.App;
            return new DependencyObjectCollection(app.Type("Friendly.UWP.Core.UIElementSearcher").ByType(collection, typeFullName));
        }
    }
}
