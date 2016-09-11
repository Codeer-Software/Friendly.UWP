using Codeer.Friendly.DotNetExecutor;
using Friendly.Core;
using Friendly.UWP.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace CheckInTarget
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            AssemblyManager.AddInterfaceType(typeof(FriendlyExecutor).GetTypeInfo().Assembly);
            AssemblyManager.AddInterfaceType(typeof(FriendlyReceiver).GetTypeInfo().Assembly);
            AssemblyManager.AddInterfaceType(Application.Current.GetType().GetTypeInfo().Assembly);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var xx = Window.Current.Content.VisualTree().ByType<Button>().ToArray();

            var tree = this.VisualTree();
            var text1 = tree.ByBinding("Text1").Single();
            var buttons = tree.ByType<Button>();
            var button = buttons.ByBinding("Value2").Single();
            new ButtonManipulator(button).EmulateClick();
            var time = new DispatcherTimer();
            time.Interval = new TimeSpan(2000);
            time.Tick += (_,__)=>
            {
                /*
                var p = this.VisualTree().ByType<Popup>().Single();
                var tt = p.VisualTree().ToArray();
                var bs = p.VisualTree().ByType<Button>();
                var x = this.IsEnabled;
                var y = Window.Current;
                y.Close();*/
                var x = Window.Current.Content.VisualTree().ByType<Button>().ToArray();
                int dmy = 0;
            };
            time.Start();
        }

        async void button2_Click(object sender, RoutedEventArgs e)
        {
            /*
            var tree = TreeUtilityInTarget.VisualTree(this);
            var text1 = SearcherInTarget.ByBinding(tree, "Text1").Single();
            new TextBoxManipulator((TextBox)text1).EmulateChangeText("xxx");*/
            var dlg = new MessageDialog("アイテムを削除しますか？");
            dlg.Commands.Add(new UICommand("はい"));
            dlg.Commands.Add(new UICommand("いいえ"));
            dlg.DefaultCommandIndex = 1; // 「いいえ」をデフォルトボタンにする
            var cmd = await dlg.ShowAsync();
        }
    }
}
