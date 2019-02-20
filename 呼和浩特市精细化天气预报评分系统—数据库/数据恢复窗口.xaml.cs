using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace 呼和浩特市精细化天气预报评分系统_数据库
{
    /// <summary>
    /// 数据恢复窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 数据恢复窗口 : Window
    {
        private string  con = "";
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        public 数据恢复窗口()
        {
            InitializeComponent();
            CSH();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
            {
                string ss = "";
                double douLS; //赋值保存进度条的进度数
                UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(progressBar1.SetValue);
                UpdateProgressBarDelegate updateText = new UpdateProgressBarDelegate(txtboxJD.SetValue);
                UpdateProgressBarDelegate updatejlText = new UpdateProgressBarDelegate(tHistory.SetValue);
                DateTime dateStartDate = Convert.ToDateTime(sDate.SelectedDate),
                    dateEndDate = Convert.ToDateTime(eDate.SelectedDate); //获取选择的起止时间
                DateTime dateLS = dateStartDate, dateLS2 = dateLS;
                int intLS = 0;
                for (int i = 0; DateTime.Compare(dateLS2, dateEndDate) <= 0; i++) //判断总共需要循环的次数，决定进度条的进度
                {
                    intLS++;
                    dateLS2 = dateLS2.AddDays(1);
                }
                //bool bsbool1 = false, bsbool2= false; //bsbool1如果数据库已有数据是否覆盖标识，true为覆盖;bsbool2 数据库已有数据时，是否弹框提醒标识，true则不再弹框
                Class1 c1 = new Class1();
                for (int i = 0; DateTime.Compare(dateLS, dateEndDate) <= 0; i++) //临时日期初始值为开始日期，每个循环加1天，一直到大于截止日期
                {
                    douLS = (i + 1) * 100 / intLS;
                    string strDate = dateLS.ToString("yyyyMMdd");
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, Math.Ceiling(douLS) });//委托更新显示进度条
                    Dispatcher.Invoke(updateText, System.Windows.Threading.DispatcherPriority.Background, new object[] { TextBox.TextProperty, strDate });//委托更新显示文本
                   
                    try
                    {
                        if (sjChoose.Text=="市台预报")
                        {
                            
                            ss += c1.Sjyb(strDate,scchoose.Text,gwchoose.Text);
                        }
                        else if (sjChoose.Text == "中央指导预报")
                        {
                            ss += c1.ZYZDCIMISS(strDate, scchoose.Text);
                        }
                        else if (sjChoose.Text == "实况")
                        {
                            ss += c1.SKRK(strDate, scchoose.Text);
                        }
                        else if (sjChoose.Text == "统计信息")
                        {
                            string sls= c1.TJRK(dateLS, scchoose.Text, gwchoose.Text);
                            if (sls.Length == 0)
                                ss +=  strDate +"日"+scchoose.Text+"时"+gwchoose.Text+ "统计信息入库成功\n";
                            else
                                ss += sls;
                        }
                    }
                    catch (Exception ex)
                    {
                        ss += ex.Message + '\n';
                    }
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, Math.Ceiling(douLS) });//委托更新显示进度条
                    Dispatcher.Invoke(updateText, System.Windows.Threading.DispatcherPriority.Background, new object[] { TextBox.TextProperty, strDate });//委托更新显示文本
                    this.tHistory.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                tHistory.Text = ss;
                                //将光标移至文本框最后
                                tHistory.Focus();
                                tHistory.CaretIndex = (tHistory.Text.Length);
                            }
                        ));
                    dateLS = dateLS.AddDays(1);
                    
                }
                Dispatcher.Invoke(updateText, System.Windows.Threading.DispatcherPriority.Background, new object[] { TextBox.TextProperty, "完成" });
                tHistory.Text = ss;
            }
            else
            {
                MessageBox.Show("请选择起止时间");
            }
        }

        
        void CSH()
        {
            try
            {
                scchoose.SelectedIndex = 0;
                sDate.BlackoutDates.Add(new CalendarDateRange((DateTime.Now.Date).AddDays(+1),
                    DateTime.MaxValue)); //开始时间不可选的范围，当前日期以后
                eDate.BlackoutDates.Add(dr2); //结束时间不可选的范围
                progressBar1.Maximum = 100; //进度条最大值为100
                string DBconPath = System.Environment.CurrentDirectory + @"\config\DBconfig.txt";
                using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("sql管理员"))
                        {
                            con = line.Substring("sql管理员=".Length);
                        }
                    }
                }
                //初始化岗位选择框
                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

        }

        void gwCSH(string sc)
        {
            try
            {
                int gwchooseCount = 0;
                Dictionary<int, string> mydic = new Dictionary<int, string>();
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\config\GWList.txt",
                    Encoding.Default))
                {
                    string line = "";

                    Int16 intCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 0 && line.Split('=')[0] == sc + "岗位列表")
                        {
                            string[] szls = line.Split('=')[1].Split(',');
                            foreach (string ssls in szls)
                            {
                                mydic.Add(intCount++, ssls);

                            }
                            break;
                        }

                    }

                    gwchoose.ItemsSource = mydic;
                    gwchoose.SelectedValuePath = "Key";
                    gwchoose.DisplayMemberPath = "Value";
                }
                gwchoose.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void scchoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sc= scchoose.SelectedItem.ToString();
            sc=sc.Split(':')[1].Trim();
            gwCSH(sc);
        }

        private void sDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            eDate.BlackoutDates.Remove(dr1);//现将原来禁止的时间范围删除，否则会报错
            dr1 = new CalendarDateRange(new DateTime(), Convert.ToDateTime(sDate.Text).AddDays(-1));
            eDate.SelectedDate = null;//将已经选取的结束时间清空
            eDate.BlackoutDates.Add(dr1);//结束时间随着开始时间的改变增加新的范围

        }
    }
}
