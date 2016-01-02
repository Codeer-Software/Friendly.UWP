using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly;
using System.IO;

namespace Friendly.UWP.Test
{
    [TestClass]
    public class TestAttach
    {
        /*
        [TestMethod]
        public void TestMethod1()
        {
            using (var app = new UWPAppFriend(new UWPControlByVisualStudio(@"C:\tfs\codeer\Research\UWP\UWP.sln")
            { VisualStudioPath = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe",
                ContinueDebuging = false}))
            {
                app.Type("UWPApp.MyClass").Y = 3;

                string ret = app.Type("UWPApp.MyClass").Func(1, "a");
                Assert.AreEqual("xxx", ret);

                var w = app.Type("Windows.UI.Xaml.Window").Current;

                var x = w.Content.Content.button.Content;
                var xx = x.ToString();

                AppVar a = w;
                bool b1 = a.IsNull;
                a = w.Content;
                b1 = a.IsNull;
                a = w.Content.Content;
                b1 = a.IsNull;
                a = w.Content.Content.button;
                b1 = a.IsNull;
                a = w.Content.Content.button.Content;
                b1 = a.IsNull;
                //       var x = w.Content.Content.button.Content;
                w.Content.Content.button.Content = "xxx";
            }
        }
        */

        [TestMethod]
        public void Test()
        {
            using (var app = new UWPAppFriend(new ByVisualStudio(Path.GetFullPath("../../../TargetApp/TargetApp.sln"))
            {
                VisualStudioPath = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe",
                ContinueDebuging = false
            }))
            {
                string val = app.Type("TargetApp.MyClass").Func(3);
                Assert.AreEqual("3", val);
            }
        }
    }
}
