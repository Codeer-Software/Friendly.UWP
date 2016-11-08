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
                ChangeVisualStudioSetting = (vs, dteSrc)=>
                {
                    var dte = dteSrc.Pin<DTE2>();
                    dte.Solution.SolutionBuild.SolutionConfigurations.Item(3).Activate();
                }
            }))
            {
                //get visual tree.
                var tree = app.CurrentWindow.Content.VisualTree();

                //textbox
                var textBox = new TextBox(tree.ByType(TextBox.TypeFullName).Single());
                textBox.EmulateChangeText("abc");

                //button
                var button = new Button(tree.ByBinding("Execute").Single());
                button.EmulateClick();

                //combobox
                var comboBox = new ComboBox(tree.ByType(ComboBox.TypeFullName).Single());
                comboBox.EmulateChangeSelectedIndex(2);

                //listbox
                var listBox = new ListBox(tree.ByType(ListBox.TypeFullName).Single());
                listBox.EmulateChangeSelectedIndex(2);

                //radiobutton
                var radioButton = new RadioButton(tree.ByType(RadioButton.TypeFullName).Single());
                radioButton.EmulateCheck(true);

                //check button
                var check = new CheckBox(tree.ByType(CheckBox.TypeFullName).Single());
                check.EmulateCheck(true);

                //it can access all .net api 
                //invoke static property.
                var mainPage = app.Type("Windows.UI.Xaml.Window").Current.Content.Content;

                //invoke method defined by user.
                string result = mainPage.MyFunc(5);
                Assert.AreEqual("5", result);

                //change color
                var color = app.Type("Windows.UI.Colors").Blue;
                var brush = app.Type("Windows.UI.Xaml.Media.SolidColorBrush")(color);
                mainPage.Content.Background = brush;
            }
        }
    }
}
