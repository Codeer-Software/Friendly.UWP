using Codeer.Friendly;
using Codeer.Friendly.Dynamic;

namespace Friendly.UWP
{
#if ENG
    /// <summary>
    /// VisualTree and LogicalTree utility.
    /// </summary>
#else
    /// <summary>
    /// VisualTreeとLogicalTreeのユーティリティー。
    /// </summary>
#endif
    public static class TreeUtilityExtensions
    {

#if ENG
        /// <summary>
        /// Enumerate DependencyObject continuing to VisualTree.
        /// </summary>
        /// <param name="start">Start DependencyObject.</param>
        /// <param name="direction">Run direction.</param>
        /// <returns>Enumerated DependencyObject.</returns>
#else
        /// <summary>
        /// VisualTreeに連なるDependencyObjectを列挙。
        /// </summary>
        /// <param name="start">列挙を開始するDependencyObject。</param>
        /// <param name="direction">走査方向。</param>
        /// <returns>列挙されたDependencyObject。</returns>
#endif
        public static IDependencyObjectCollection VisualTree(this AppVar start, TreeRunDirection directionSrc = TreeRunDirection.Descendants)
        {
            var direction = start.App["Friendly.UWP.Core.TreeRunDirection." + directionSrc.ToString()]();
            return new DependencyObjectCollection(start.App.Type("Friendly.UWP.Core.TreeUtility").VisualTree(start, direction));
        }

#if ENG
        /// <summary>
        /// Enumerate DependencyObject continuing to VisualTree.
        /// </summary>
        /// <param name="start">Start DependencyObject.</param>
        /// <param name="direction">Run direction.</param>
        /// <returns>Enumerated DependencyObject.</returns>
#else
        /// <summary>
        /// VisualTreeに連なるDependencyObjectを列挙。
        /// </summary>
        /// <param name="start">列挙を開始するDependencyObject。</param>
        /// <param name="direction">走査方向。</param>
        /// <returns>列挙されたDependencyObject。</returns>
#endif
        public static IDependencyObjectCollection VisualTree(this IAppVarOwner start, TreeRunDirection directionSrc = TreeRunDirection.Descendants)
            => start.AppVar.VisualTree(directionSrc);
    }
}
