using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PInject
{
    class ControlPak
    {
        public static string GetVHDVolumeLabel()
        {
            DriveInfo[] drive = DriveInfo.GetDrives();
            for (int i = 0; i < drive.Length; i++)
            {
                if (drive[i].VolumeLabel == "PUBG")
                {
                    return drive[i].RootDirectory.ToString();
                }
            }
            return "E:\\";
        }
        public static void WaitForAttach()
        {
            while (true)
            {
                DriveInfo[] drive = DriveInfo.GetDrives();
                for (int i = 0; i < drive.Length; i++)
                    if (drive[i].VolumeLabel == "PUBG") return;
            }
        }
        public static void AttachVHD()
        {
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            pi.FileName = "DISKPART.EXE";
            pi.Arguments = "/S \"" + Application.StartupPath + "\\tmp\\mnt.txt" + "\"";
            Process.Start(pi);
        }
        public static void DetachVHD()
        {
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            pi.FileName = "DISKPART.EXE";
            pi.Arguments = "/S \"" + Application.StartupPath + "\\tmp\\unmnt.txt" + "\"";
            Process.Start(pi);
        }
        public static void CreateHSL(string dst, string src)
        {
            if (File.Exists(dst)) File.Delete(dst);
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            pi.FileName = "cmd";
            pi.Arguments = "/c \"mklink /h \"" + dst + "\" \"" + src + "\"\"";
            Process.Start(pi);
        }
        public static void CreateSL(string dst, string src)
        {
            if (File.Exists(dst)) File.Delete(dst);
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            pi.FileName = "cmd";
            pi.Arguments = "/c \"mklink \"" + dst + "\" \"" + src + "\"\"";
            Process.Start(pi);
        }
        public static void PreInjectPak(string pakName)
        {
            //원본넣고 초기화
            if (!Directory.Exists(Application.StartupPath + @"\tmp")) Directory.CreateDirectory(Application.StartupPath + @"\tmp");
            String org = Form1.pubgPath + @"\" + pakName,
                mid = Application.StartupPath + @"\tmp\" + pakName;
            CreateSL(org, mid);
        }
        public static void InjectPak(string pakName)
        {
            String mid = Application.StartupPath + @"\tmp\" + pakName,
                src = Form1.resPath + @"\" + pakName;
            CreateSL(mid, src);
        }
        public static void EjectPak(string pakName)
        {
            String mid = Application.StartupPath + @"\tmp\" + pakName,
                src = Application.StartupPath + @"\org." + pakName.Split('.')[1];
            CreateSL(mid, src);
        }
    }
}
