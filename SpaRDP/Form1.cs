using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;
using System.IO;

namespace SpaRDP
{
    
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hwnd, int cmd);
        //public static extern bool ShowWindow(IntPtr hwnd, int cmd);
        int iter = 0, nip = 0, tiktak=0;
        string fileName;
        string path_prom = @"C:\\a1\\AnyDesk.exe";
        public Form1()
        {
            TopMost = true;  // окно почерх остальных       
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {                }
        public void ip_search()
        {
            string pid1_;
            using (var p = new Process())
            {//узнаем PID сервиса AnyDesk
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = " /c \"tasklist.exe /fi \"imagename eq AnyDesk.exe\" /NH /FO CsV | findstr \"Ser\"\"";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("CP866");
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                string[] pid1 = output.Split(',');//переводим ответ в массив
                pid1_ = pid1[1].Replace("\"", "");//берем 2й єлемент без кавычек
            }
            string resip = "";
            using (var p = new Process())
            {//ищем открытый сеанс указанного PIDa
                p.StartInfo.FileName = "cmd.exe ";
                //p.StartInfo.Arguments = "/c \" netstat  -n -o | findstr /I " + pid1_ + "\"";
                p.StartInfo.Arguments = "/c \" netstat  -n -o | findstr /I " + pid1_ + " | findstr \"ESTABLISHED\"\"";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("CP866");
                p.Start();
                string output = p.StandardOutput.ReadToEnd();

                if (output.Trim() != "")
                {
                    if (output != " ")
                    {
                        output = output.Trim();
                        //output
                        while (output.Contains("  ")) { output = output.Replace("  ", " "); }
                        string[] pid1 = output.Split(' ');
                        string res = pid1[2];
                        resip = res.Substring(0, res.IndexOf(":"));
                        System.IO.File.AppendAllText(fileName, resip + Environment.NewLine);
                        // MessageBox.Show(resip);
                        textBox2.Text += "+";
                    }
                }
                else
                { textBox2.Text += "."; }
            }
            if (resip != "")
            {
                textBox1.Text += DateTime.Now.ToString("HH:mm:ss") + ",+," + resip + Environment.NewLine;
                nip++;
                label4.Text = nip.ToString();
                //добавляем маршрут вникуда
                using (var p = new Process())
                {
                    p.StartInfo.FileName = "cmd.exe ";
                    p.StartInfo.Arguments = "/c route add " + resip + " mask 255.255.255.255 192.168.1.191 if 1 -p";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                }
               using (var p = new Process())
                {
                    p.StartInfo.FileName = "cmd.exe ";
                    //p.StartInfo.Arguments = " /fi \"imagename eq AnyDesk.exe\" /NH | findstr \"Servic\"";
                    //p.StartInfo.Arguments = " -n -o | find /I " + pid1_;
                    p.StartInfo.Arguments = "/c taskkill /PID " + pid1_ + " /F";
                    //  MessageBox.Show("/k route add " + resip + " mask 255.255.255.255 192.168.1.191 if 1 -p");
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.CreateNoWindow = true;                  
                    p.Start();
                }
                //ProcessStartInfo startInfo = new ProcessStartInfo("C:\\a1\\AnyDesk.exe");
                //startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                //Process.Start(startInfo);
                //минимизируем прогу 
                if (File.Exists(path_prom)){ 
                Process p1 = Process.Start(path_prom);
                ShowWindow(p1.MainWindowHandle, 0);
                }
                //ShowWindow(1124, 3);
                //IntPtr hWnd = p1;
                //ShowWindow(hWnd, 0);
                //ShowWindow(p1.MainWindowHandle.,0)
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000 * 30; //1 minutes = 60 sec
            timer1.Enabled = true;
            fileName = System.IO.Path.Combine(Environment.CurrentDirectory, "ip.txt");
            label7.Text = DateTime.Now.ToString();
            textBox3.Text = "AnyDesk.exe";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ip_search();
            iter++;
            label2.Text = iter.ToString();
            label8.Text = DateTime.Now.ToString();
        }
    }
}
    

