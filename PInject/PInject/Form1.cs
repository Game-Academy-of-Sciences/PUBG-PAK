using PInject;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PInject
{
    public partial class Form1 : Form
    {
        public static string pubgPath, resPath;
        public Form1()
        {
            InitializeComponent();
            pubgPath = textBox1.Text + @"\TslGame\Content\Paks";
            resPath = textBox2.Text;
            File.WriteAllText(Application.StartupPath + "\\tmp\\mnt.txt", "SELECT VDISK FILE = \"" + Application.StartupPath + "\\res.vhd" + "\"\r\nATTACH VDISK\r\nEXIT", System.Text.Encoding.ASCII);
            File.WriteAllText(Application.StartupPath + "\\tmp\\unmnt.txt", "SELECT VDISK FILE = \"" + Application.StartupPath + "\\res.vhd" + "\"\r\nDETACH VDISK\r\nEXIT", System.Text.Encoding.ASCII);
        }
        async Task WaitnSec(int n)
        {
            await Task.Delay(n, new CancellationTokenSource().Token);
        }
        public void SetPUBG()
        {
            //FileSystem.CopyDirectory(textBox1.Text, textBox1.Text + "2",UIOption.AllDialogs);
            foreach (string eachDirectory in Directory.EnumerateDirectories(textBox1.Text, "*.*", System.IO.SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(eachDirectory.Replace(@"\PUBG\", @"\PUBG2\"));
            }
            foreach (string eachFile in Directory.EnumerateFiles(textBox1.Text, "*.*", System.IO.SearchOption.AllDirectories))
            {
                ControlPak.CreateHSL(eachFile.Replace(@"\PUBG\", @"\PUBG2\"), eachFile);
            }
            File.Delete(textBox1.Text + @"2\TslGame\Content\Paks\pakList.json");
            File.Delete(textBox1.Text + @"2\TslGame\Binaries\Win64\TslGame.exe");
            File.Copy(Application.StartupPath + @"\org.json", textBox1.Text + @"2\TslGame\Content\Paks\pakList.json", true);
            File.Copy(Application.StartupPath + @"\org.exe", textBox1.Text + @"2\TslGame\Binaries\Win64\TslGame.exe", true);
            ControlPak.CreateSL(textBox1.Text + @"\TslGame\Binaries\Win64\TslGame.exe", textBox1.Text + @"2\TslGame\Binaries\Win64\TslGame.exe");
            MessageBox.Show("Done");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ControlPak.AttachVHD();
            ControlPak.WaitForAttach();
            ControlSecureFolder.SecureFolderState(false);
            foreach (FileInfo eachPak in new DirectoryInfo(resPath).GetFiles("*.pak"))
            {
                ControlPak.PreInjectPak(eachPak.Name);
                ControlPak.EjectPak(eachPak.Name);
                ControlSecureFolder.SecureFolderAdd(pubgPath + "\\" + eachPak.Name);
            }
            if (chkUsePList.Checked)
            {
                ControlPak.PreInjectPak("pakList.json");
                ControlPak.EjectPak("pakList.json");
                ControlSecureFolder.SecureFolderAdd(pubgPath + "\\" + "pakList.json");
            }
            tmrCheckPak.Start();
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            pubgPath = textBox1.Text + @"\TslGame\Content\Paks";
        }

        private async void TmrCheckPUBG_Tick(object sender, EventArgs e)
        {
            Text = "Pak 적용 대기중";
            if (Process.GetProcessesByName("TslGame").Length >= 2)
            {
                tmrCheckPak.Stop();
                await WaitnSec(int.Parse(textBox3.Text));
                if (chkUsePList.Checked)
                    ControlPak.InjectPak("pakList.json");
                foreach (FileInfo eachPak in new DirectoryInfo(resPath).GetFiles("*.pak"))
                    ControlPak.InjectPak(eachPak.Name);
                Text = "적용완료";
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            tmrCheckPak.Stop();
            ControlSecureFolder.SecureFolderState(true);
            foreach (FileInfo eachPak in new DirectoryInfo(resPath).GetFiles("*.pak"))
                ControlPak.EjectPak(eachPak.Name);
            if (chkUsePList.Checked)
                ControlPak.EjectPak("pakList.json");
            ControlPak.DetachVHD();
            Text = "우회 완료";
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            resPath = textBox2.Text;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = (int.Parse(textBox3.Text) + 100).ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ControlPak.AttachVHD();
            ControlPak.WaitForAttach();
            textBox2.Text = ControlPak.GetVHDVolumeLabel() + "res";
            ControlSecureFolder.InstallSecureFolder();
        }

        private void LinkLabel1_Click(object sender, EventArgs e)
        {
            if(Directory.Exists(textBox1.Text.Replace(@"\PUBG", @"\PUBG2")))
                Directory.Delete(textBox1.Text.Replace(@"\PUBG", @"\PUBG2"),true);
            SetPUBG();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ControlSecureFolder.SecureFolderState(false);
            ControlPak.AttachVHD();
            ControlPak.WaitForAttach();
            foreach (FileInfo eachPak in new DirectoryInfo(resPath).GetFiles("*.pak"))
                ControlPak.EjectPak(eachPak.Name);
            ControlPak.EjectPak("pakList.json");
            ControlPak.DetachVHD();
        }
    }
}