Friendly.UWP_ƒ¿
======================
Friendly is a library for creating integration tests.
(The included tools can be useful, but these are only a bonus.)
It is currently designed for 
(for Desktop Applications https://github.com/Codeer-Software/Friendly.Windows).
It can be used to start up a product process and run tests on it..
However, the way of operating the target program is different from conventional automated GUI tests (capture replay tool, etc.).

## Features ...
####Invoke separate process's API.
It can invoke all methods, properties, and fields.
It's like a selenium's javascript execution.

## Getting Started
Install Friendly.UWP from NuGet

    PM> Install-Package Friendly.UWP
https://www.nuget.org/packages/Codeer.Friendly.Windows/
## Movies
https://youtu.be/YVp_hUnl1lo<br>

## Simple sample
We are developping now.
We need many feed back.
```cs  
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

                //change color
                var mainPage = app.CurrentWindow.Content.Dynamic().Content;
                var color = app.Type("Windows.UI.Colors").Blue;
                var brush = app.Type("Windows.UI.Xaml.Media.SolidColorBrush")(color);
                mainPage.Content.Background = brush;
            }
        }
    }
}
```