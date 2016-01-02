using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using EnvDTE;
using EnvDTE80;
using Friendly.UWP.Inside;
using Friendly.UWP.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using VSHTC.Friendly.PinInterface;

namespace Friendly.UWP
{
    public class ByVisualStudio : IUWPControl
    {
        public string Solution { get; private set; }
        public string Uri { get; set; } = "http://localhost:8081/";
        public string VisualStudioPath { get; set; }
        public bool ContinueDebuging { get; set; }
        public string InjectionBreakPoint { get; set; } = "App.InitializeComponent";
        public Action<WindowsAppFriend, DTE2> ChangeVisualStudioSetting { get; set; }
        static Type DTEType { get { return typeof(_DTE); } }

        WindowsAppFriend _visualStudio;
        int _uwpProcessId;
        AppVar _breaks;

        public ByVisualStudio(string solution)
        {
            Solution = solution;
        }

        public void Connected()
        {
            if (!ContinueDebuging)
            {
                var dte = GetDTE2(_visualStudio);
                dte.Debugger.DetachAll();
                _visualStudio.Type(GetType()).RestoreBreak(_breaks);
            }
            _visualStudio.Dispose();
            _visualStudio = null;
        }

        public void Stop(Action stopReceiver)
        {
            stopReceiver();
            System.Diagnostics.Process.GetProcessById(_uwpProcessId).Kill();
        }

        public void Start()
        {
            _visualStudio = FindTargetVs();

            var dte = GetDTE2(_visualStudio);

            //VisualStudioの設定変更
            if (ChangeVisualStudioSetting != null)
            {
                ChangeVisualStudioSetting(_visualStudio, dte);
            }
            
            //ブレイクを全部無効にする
            if (!ContinueDebuging)
            {
                _breaks = _visualStudio.Type(GetType()).DisableBreak(PinHelper.GetAppVar(dte));
            }

            //ブレイクを設定
            var injectionBreak = dte.Debugger.Breakpoints.Add(InjectionBreakPoint);
            dte.Debugger.Go();
            if (injectionBreak.Count == 1)
            {
                injectionBreak.Item(1).Delete();
            }
            //対象のプロセスID
            _uwpProcessId = dte.Debugger.CurrentProcess.ProcessID;

            //ファイルをコピー
            var dir = Path.GetDirectoryName(System.Diagnostics.Process.GetProcessById(_uwpProcessId).MainModule.FileName);
            DllControl.InstallDll(dir, typeof(Friendly.Core.VarAddress).Assembly);
            DllControl.InstallDll(Path.Combine(dir, "Friendly.UWP.Core.dll"), Resources.Friendly_UWP_Core);

            //インジェクション
            var exp = "System.Reflection.IntrospectionExtensions.GetTypeInfo(System.Type.GetType(\"Friendly.UWP.Core.FriendlyExecutor, Friendly.UWP.Core, Version=0.0.1.0, Culture=neutral, PublicKeyToken=e8e68ab53559cf51\")).GetDeclaredMethod(\"Start\").Invoke(null, new object[] { \"" + Uri + "\" });";
            dte.Debugger.GetExpression(exp);

            //ディタッチ
            if (ContinueDebuging)
            {
                dte.Debugger.Go(false);
            }
            else
            {
                dte.Debugger.Go(false);
            }
        }
        
        static List<Tuple<Breakpoint, bool>> DisableBreak(object obj)
        {
            DTE2 dte = (DTE2)obj;
            var breaks = new List<Tuple<Breakpoint, bool>>();
            int count = dte.Debugger.Breakpoints.Count;
            for (int i = 0; i < count; i++)
            {
                var e = dte.Debugger.Breakpoints.Item(i + 1);
                breaks.Add(new Tuple<Breakpoint, bool>(e, e.Enabled));
                e.Enabled = false;
            }
            return breaks;
        }

        static void RestoreBreak(List<Tuple<Breakpoint, bool>> breaks)
        {
            breaks.ForEach(e => e.Item1.Enabled = e.Item2);
        }

        static DTE2 GetDTE2(WindowsAppFriend app)
        {
            WindowsAppExpander.LoadAssembly(app, typeof(ByVisualStudio).Assembly);
            var dteType = app.Type(typeof(ByVisualStudio)).DTEType;
            AppVar obj = app.Type().Microsoft.VisualStudio.Shell.Package.GetGlobalService(dteType);
            return obj.Pin<DTE2>();
        }

        WindowsAppFriend FindTargetVs()
        {
            var devenvs = System.Diagnostics.Process.GetProcessesByName("devenv");
            foreach (var e in devenvs)
            {
                if (0 == e.MainWindowTitle.Length)
                {
                    continue;
                }

                /*
                //@@@
                if (e.MainWindowTitle.IndexOf("Friendly.UWP") != -1)
                {
                    continue;
                }
                */

                using (var app = new WindowsAppFriend(e))
                {
                    var dte = GetDTE2(app);
                    var solution = dte.Solution;
                    var s = solution.FileName;
                    if (string.Compare(s, Solution, true) != 0)
                    {
                        continue;
                    }
                }
                return new WindowsAppFriend(e);
            }
            return StartVisualStudio();
        }

        WindowsAppFriend StartVisualStudio()
        {
            if (string.IsNullOrEmpty(VisualStudioPath))
            {
                throw new FriendlyOperationException(Resources.ErrorNotFoundVisualStudio);
            }
            var vsProcess = System.Diagnostics.Process.Start(VisualStudioPath);
            while (0 == vsProcess.MainWindowTitle.Length)
            {
                System.Threading.Thread.Sleep(10);
                vsProcess = System.Diagnostics.Process.GetProcessById(vsProcess.Id);
            }
            var app = new WindowsAppFriend(vsProcess);
            var dte = GetDTE2(app);
            var solution = dte.Solution;
            solution.Open(Solution);
            return app;
        }
    }
}
