using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeer.Friendly.Dynamic;
using System.IO;
using VSHTC.Friendly.PinInterface;
using EnvDTE80;
using Codeer.Friendly;

namespace Friendly.UWP.Test
{
    [TestClass]
    public class TestAttach
    {
        [TestMethod]
        public void Test()
        {
            using (var app = new UWPAppFriend(new ByVisualStudio(Path.GetFullPath("../../../TargetApp/TargetApp.sln"))
            {
                VisualStudioPath = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe",
                ChangeVisualStudioSetting = (vs, dteSrc)=>
                {
                    var dte = dteSrc.Pin<DTE2>();
                    dte.Solution.SolutionBuild.SolutionConfigurations.Item(3).Activate();
                }
            }))
            {
                string val = app.Type("TargetApp.MyClass").Func(3);
                Assert.AreEqual("3", val);

                //背景色変更
                var current = app.Type("Windows.UI.Xaml.Window").Current;
                var x = ((AppVar)current).IsNull;

                var mainPage = current.Content.Content;
                var color = app.Type("Windows.UI.Colors").Blue;
                var brush = app.Type("Windows.UI.Xaml.Media.SolidColorBrush")(color);
                mainPage.Content.Background = brush;
            }
        }
    }
}
