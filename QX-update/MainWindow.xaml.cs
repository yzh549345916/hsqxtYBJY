using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace QX_update
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

                Process[] processList = Process.GetProcesses();
                foreach (Process process in processList)
                {
                    //如果程序启动了，则杀死
                    if (process.ProcessName == "呼和浩特市精细化天气预报评分系统")
                    {
                        process.Kill();
                    }
                }
            string updatePath = "";
            string pathConfig = System.Environment.CurrentDirectory + @"\config\pathConfig.txt";
            using (StreamReader sr = new StreamReader(pathConfig, Encoding.Default))
            {
                string line = "";
                while((line=sr.ReadLine())!=null)
                {
                    if(line.Split('=')[0]== "升级文件路径")
                    {
                        updatePath = line.Split('=')[1];
                    }
                }
            }
            if(Directory.Exists(updatePath))
            {
                try
                {
                    string[] file = Directory.GetFiles(updatePath, "*", SearchOption.AllDirectories);
                    foreach (string ff in file)
                    {
                        string[] szls = ff.Split('\\');
                        string destinationFile = Environment.CurrentDirectory + '\\' + ff.Substring(updatePath.Length, ff.Length - updatePath.Length);
                        string dirStr = destinationFile.Substring(0, destinationFile.Length - szls[szls.Length - 1].Length);
                        if (!Directory.Exists(dirStr))
                        {
                            Directory.CreateDirectory(dirStr);
                        }
                        File.Copy(ff, destinationFile, true);
                    }
                    MessageBoxResult dr = MessageBox.Show("更新完成,是否重新打开呼和浩特市精细化天气预报评分系统", "请注意", MessageBoxButton.YesNo);
                    if (dr == MessageBoxResult.Yes)
                    {
                        string dlPath = Environment.CurrentDirectory + @"\呼和浩特市精细化天气预报评分系统.exe";
                        string strML = Environment.CurrentDirectory;
                        Process pr = new Process();//声明一个进程类对象
                        pr.StartInfo.WorkingDirectory = strML;
                        pr.StartInfo.FileName = dlPath;//指定运行的程序
                        pr.Start();//运行
                    }

                    this.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                MessageBox.Show("请确认升级文件的路径是否正确配置，配置文件在" + pathConfig + "中。");
                this.Close();

            }


        }
    }
}
