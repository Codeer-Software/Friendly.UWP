using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeer.Friendly.Dynamic;
using System.IO;
using VSHTC.Friendly.PinInterface;
using EnvDTE80;

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
             /*   ChangeVisualStudioSetting = (vs, dteSrc)=>
                {
                    var dte = dteSrc.Pin<DTE2>();
                    int count = dte.Solution.SolutionBuild.SolutionConfigurations.Count;

                }*/
            }))
            {
                string val = app.Type("TargetApp.MyClass").Func(3);
                Assert.AreEqual("3", val);
            }
        }
    }
}
