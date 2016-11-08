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
        /// Install dll.
        /// </summary>
        /// <param name="dllPath">path of dll.</param>
        /// <param name="asm">assembly.</param>
        internal static void InstallDll(string dir, Assembly asm)
        {
            var dllPath = Path.Combine(dir, Path.GetFileName(asm.Location));
            InstallDll(dllPath, File.ReadAllBytes(asm.Location));
        }

        /// <summary>
        /// Install dll.
        /// </summary>
        /// <param name="dllPath">path of dll.</param>
        /// <param name="dllData">binary of dll.</param>
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
                        return;//installed.
                    }
                }
                catch { }

                //make directory.        
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch
                {
                    throw new FriendlyOperationException(Resources.ErrorFriendlySystem);
                }
                //delete old file.
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
                //write.
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
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// compare binary.
        /// </summary>
        /// <param name="buf1">binary1.</param>
        /// <param name="buf2">binary2.</param>
        /// <returns>is match.</returns>
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
