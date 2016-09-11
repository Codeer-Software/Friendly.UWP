using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Friendly.UWP.Core
{
#if ENG
    /// <summary>
    /// VisualTree and LogicalTree utility.
    /// In order to run inside the target process, you will need to injection the RM.Friendly.WPFStandardControls.3.dll.
    /// Use the RM.Friendly.WPFStandardControls.WPFStandardControls_3.Injection method.
    /// </summary>
#else
    /// <summary>
    /// VisualTreeとLogicalTreeのユーティリティー。
    /// 対象プロセス内部で実行するためには、RM.Friendly.WPFStandardControls.3.dllをインジェクションする必要があります。
    /// RM.Friendly.WPFStandardControls.WPFStandardControls_3.Injectionメソッドを利用してください。
    /// </summary>
#endif
    public static class TreeUtility
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
        public static IEnumerable<DependencyObject> VisualTree(this DependencyObject start, TreeRunDirection direction = TreeRunDirection.Descendants)
        {
            switch (direction)
            {
                case TreeRunDirection.Descendants:
                    return GetVisualTreeDescendants(start);
                case TreeRunDirection.Ancestors:
                    return GetVisualTreeAncestor(start);
                default:
                    throw new NotSupportedException("?");
            }
        }

        static IEnumerable<DependencyObject> GetVisualTreeDescendants(DependencyObject obj)
        {
            List<DependencyObject> list = new List<DependencyObject>();
            list.Add(obj);
            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                var item = VisualTreeHelper.GetChild(obj, i);
                var popup = item as Popup;
                if (popup != null)
                {
                    list.Add(item);
                    item = popup.Child;
                }
                list.AddRange(GetVisualTreeDescendants(item));
            }
            return list;
        }

        static IEnumerable<DependencyObject> GetVisualTreeAncestor(DependencyObject obj)
        {
            List<DependencyObject> list = new List<DependencyObject>();
            while (obj != null)
            {
                list.Add(obj);
                obj = VisualTreeHelper.GetParent(obj);
            }
            return list;
        }
    }
}
