using Codeer.Friendly;
using Friendly.UWP.Properties;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Friendly.UWP.Inside
{
    static class DllControl
    {
        /// <summary>
        /// DLLのインストール。
        /// </summary>
        /// <param name="dllPath">DLLのパス。</param>
        /// <param name="dllData">DLLのバイナリデータ。</param>
        internal static void InstallDll(string dir, Assembly asm)
        {
            var dllPath = Path.Combine(dir, Path.GetFileName(asm.Location));
            InstallDll(dllPath, File.ReadAllBytes(asm.Location));
        }

        /// <summary>
        /// DLLのインストール。
        /// </summary>
        /// <param name="dllPath">DLLのパス。</param>
        /// <param name="dllData">DLLのバイナリデータ。</param>
        internal static void InstallDll(string dllPath, byte[] dllData)
        {
            string dir = Path.GetDirectoryName(dllPath);
            string name = Path.GetFileNameWithoutExtension(dllPath);
            Mutex mutex = new Mutex(false, name);
            try
            {
                try
                {
                    mutex.WaitOne();
                }
                catch { }
                try
                {
                    byte[] buf = File.ReadAllBytes(dllPath);
                    if (IsMatchBinary(buf, dllData))
                    {
                        return;//インストール済み
                    }
                }
                catch { }

                //ディレクトリ作成               
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch
                {
                    throw new FriendlyOperationException(Resources.ErrorFriendlySystem);
                }
                //古いファイルを削除
                try
                {
                    if (File.Exists(dllPath))
                    {
                        File.Delete(dllPath);
                    }
                }
                catch
                {
                    throw new FriendlyOperationException(Resources.ErrorBinaryInstall
                        + Environment.NewLine + dllPath);
                }
                //書き込み
                try
                {
                    File.WriteAllBytes(dllPath, dllData);
                }
                catch
                {
                    throw new FriendlyOperationException(Resources.ErrorFriendlySystem);
                }
            }
            finally
            {
                //ミューテックスを解放する
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// バイナリの一致チェック。
        /// </summary>
        /// <param name="buf1">バイナリ1。</param>
        /// <param name="buf2">バイナリ2。</param>
        /// <returns>一致するか。</returns>
        private static bool IsMatchBinary(byte[] buf1, byte[] buf2)
        {
            if (buf1.Length != buf2.Length)
            {
                return false;
            }
            for (int i = 0; i < buf1.Length; i++)
            {
                if (buf1[i] != buf2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
