using Codeer.Friendly;
using Codeer.Friendly.Dynamic;

namespace Friendly.UWP
{
    /// <summary>
    /// Search by binding.
    /// </summary>
    public static class UIElementSearcherExtensions
    {
        /// <summary>
        /// Search by binding from DependencyObject collection.
        /// </summary>
        /// <param name="collection">DependencyObject collection.</param>
        /// <param name="path">Binding path.</param>
        /// <param name="dataItem">DataItem.</param>
        /// <returns>Hit elements.</returns>
        public static IDependencyObjectCollection ByBinding(this IDependencyObjectCollection collection, string path, ExplicitAppVar dataItem = null)
        {
            var app = ((IAppVarOwner)collection).AppVar.App;
            return new DependencyObjectCollection(app.Type("Friendly.UWP.Core.UIElementSearcher").ByBinding(collection, path, dataItem));
        }

        /// <summary>
        /// Search by Type from DependencyObject collection.
        /// </summary>
        /// <param name="collection">DependencyObject collection.</param>
        /// <param name="typeFullName">Target type.</param>
        /// <returns>Hit elements.</returns>
        public static IDependencyObjectCollection ByType(this IDependencyObjectCollection collection, string typeFullName)
        {
            var app = ((IAppVarOwner)collection).AppVar.App;
            return new DependencyObjectCollection(app.Type("Friendly.UWP.Core.UIElementSearcher").ByType(collection, typeFullName));
        }
    }
}
