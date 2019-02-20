using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace 呼和浩特市精细化天气预报评分系统
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void BUFB_Click(object sender, RoutedEventArgs e)
        {
            登录窗口 dlwindow = new 登录窗口();
            dlwindow.Show();
        }

        private void UpdataBu_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("更新将关闭当前客户端，请确认是否继续", "请注意", MessageBoxButton.YesNo);
            if (dr == MessageBoxResult.Yes)
            {
                try
                {
                    string updatePath = Environment.CurrentDirectory + @"\QX-update.exe";
                    string strML = Environment.CurrentDirectory;
                    Process pr = new Process();//声明一个进程类对象
                    pr.StartInfo.WorkingDirectory = strML;
                    pr.StartInfo.FileName = updatePath;//指定运行的程序
                    pr.Start();//运行
                    System.Environment.Exit(0);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void JYBu_Click(object sender, RoutedEventArgs e)
        {
            预报检验窗口 ybjyck = new 预报检验窗口();
            ybjyck.Show();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ZYDR_Click(object sender, RoutedEventArgs e)
        {
            中央导入窗口 dlwindow = new 中央导入窗口();
            dlwindow.Show();
        }
    }
}
