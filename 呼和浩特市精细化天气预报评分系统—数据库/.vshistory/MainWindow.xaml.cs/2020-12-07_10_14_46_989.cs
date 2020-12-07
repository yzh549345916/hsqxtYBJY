using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using Timer = System.Timers.Timer;

namespace 呼和浩特市精细化天气预报评分系统_数据库
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;
        private string con; //这里是保存连接数据库的字符串172.18.142.158 id=sa;password=134679;
        private string DBconPath = Environment.CurrentDirectory + @"\config\DBconfig.txt";
        private DateTime ECdt = DateTime.Now;
        private short ECHH08, ECMM08, ECHH20, ECMM20, ECsc = 8;
        private DateTime GJZNdt = DateTime.Now;
        private short GJZNHH08, GJZNMM08, GJZNHH20, GJZNMM20, GJZNsc = 8;

        private DateTime QJZNdt = DateTime.Now;

        private short QJZNHH08, QJZNMM08, QJZNHH20, QJZNMM20, QJZNsc = 8;
        private string QXID = "", QXName = "";
        private string RKTime = "20"; //实况入库的时次,窗口初始化程序中会重新给该值从配置文件中赋值
        private short SJRK8H, SJRK8M, SJRK20H, SJRK20M, SKRK8H, SKRK20H, SKRK8M, SKRK20M; //实况入库的分钟和小时,窗口初始化程序中会重新给该值从配置文件中赋值
        private Timer t = new Timer(60000);
        private DateTime YBdt = DateTime.Now;

        private short YBHH08, YBMM08, YBHH20, YBMM20, YBsc = 8;
        private short ZYRK8H, ZYRK8M, ZYRK20H, ZYRK20M;

        public MainWindow()
        {
            InitializeComponent();
            CSH();
        }

        private void T1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                t1.TextChanged -= T1_TextChanged;
                string[] szls = t1.Text.Split('\n');
                if (szls.Length > 5000)
                {
                    string data = "";
                    for (int i = 0; i < 4000; i++)
                    {
                        data += szls[4000 - i] + '\n';
                    }

                    t1.Dispatcher.Invoke(new Action(delegate
                    {
                        t1.Text = data;
                        //将光标移至文本框最后
                        t1.Focus();
                        t1.CaretIndex = t1.Text.Length;
                    }));
                }
            }
            catch
            {
                t1.Clear();
            }
            finally
            {
                t1.TextChanged += T1_TextChanged;
            }
        }


        private void JLButton_Click(object sender, RoutedEventArgs e)
        {
            t1.Visibility = Visibility.Visible;
            errorTBox.Visibility = Visibility.Hidden;
            t1.Focus();
            t1.CaretIndex = t1.Text.Length;
        }

        private void errorJLBut_Click(object sender, RoutedEventArgs e)
        {
            t1.Visibility = Visibility.Hidden;
            errorTBox.Visibility = Visibility.Visible;
            errorTBox.Focus();
            errorTBox.CaretIndex = errorTBox.Text.Length;
        }


        private void SJHFBu_Click(object sender, RoutedEventArgs e)
        {
            数据恢复窗口 sjhfWind = new 数据恢复窗口();
            sjhfWind.Show();
        }

        private void SJYBHF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                YBdt = DateTime.Now.AddDays(-1);
                YBsc = 20;
                Thread thread = new Thread(YBRK);
                thread.Start();
               
            }
            catch (Exception ex)
            {
                try
                {
                    t1.Dispatcher.Invoke(new Action(delegate
                    {
                        t1.AppendText(ex.Message + "\r\n");
                        //将光标移至文本框最后
                        t1.Focus();
                        t1.CaretIndex = t1.Text.Length;
                    }));
                }
                catch
                {
                }
            }
        }

        #region 窗口状态改变

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Minimized)
                {
                    Visibility = Visibility.Hidden;
                }
            }
            catch
            {
            }
        }

        #endregion

        #region 托盘图标鼠标单击事件

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Visibility == Visibility.Visible)
                {
                    Visibility = Visibility.Hidden;
                }
                else
                {
                    Visibility = Visibility.Visible;
                    Activate();
                }
            }
        }

        #endregion

        public void InitialTray()
        {
            try
            {
                if (_notifyIcon == null)
                {
                    _notifyIcon = new NotifyIcon();
                    //隐藏主窗体
                    Visibility = Visibility.Hidden;
                    //设置托盘的各个属性

                    _notifyIcon.BalloonTipText = "呼和浩特市精细化天气预报评分系统数据库服务运行中..."; //托盘气泡显示内容
                    _notifyIcon.Text = "呼和浩特市精细化天气预报评分系统数据库";
                    _notifyIcon.Visible = true; //托盘按钮是否可见
                    _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                    ; //托盘中显示的图标
                    _notifyIcon.ShowBalloonTip(2000); //托盘气泡显示时间
                    _notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
                    //窗体状态改变时触发
                    StateChanged += MainWindow_StateChanged;
                }
                else
                {
                    WindowState = WindowState.Minimized;
                }
            }
            catch
            {
            }
        }

        public void HourSKRK()
        {
            try
            {
                ClassSZYB classSZYB = new ClassSZYB();
                for (int i = 0; i > -24; i--)
                {
                    string error = "";
                    bool insertBS = false;
                    classSZYB.SKRK(DateTime.Now.AddHours(i).ToString("yyyyMMdd"), DateTime.Now.AddHours(i).Hour, ref error, ref insertBS);
                    if (error.Trim().Length == 0 && insertBS)
                    {
                        error = DateTime.Now + "保存" + DateTime.Now.AddHours(i).ToString("yyyyMMddHH") + "时实况小时数据成功！\r\n";
                        SaveJL(error);
                    }
                    else if (insertBS)
                    {
                        error = DateTime.Now + "保存" + DateTime.Now.AddHours(i).ToString("yyyyMMddHH") + "时实况小时数据：\r\n" + error;
                    }

                    if (error.Trim().Length > 0 || insertBS)
                    {
                        t1.Dispatcher.Invoke(new Action(delegate
                        {
                            t1.AppendText(error);
                            //将光标移至文本框最后
                            t1.Focus();
                            t1.CaretIndex = t1.Text.Length;
                        }));
                        SaveJL(error);
                    }
                }
            }
            catch
            {
            }
        }

        public void TBBW()
        {
            市局指导预报同步 yb = new 市局指导预报同步();
            string error = yb.getFile();
            if (error.Trim().Length > 1)
            {
                t1.Dispatcher.Invoke(new Action(delegate
                {
                    t1.AppendText(error);
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }));
                SaveJL(error);
            }
        }

        public void CSH()
        {
            t1.Text = DateTime.Now + "  启动" + '\n';
            t.Elapsed += refreshTime;
            t.AutoReset = true; //设置是执行一次（false）还是一直执行(true)；  
            t.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；
            try
            {
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
                        else if (line.Split('=')[0] == "实况入库08小时")
                        {
                            SKRK8H = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "实况入库08分钟")
                        {
                            SKRK8M = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "市局预报08入库小时")
                        {
                            SJRK8H = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "市局预报08入库分钟")
                        {
                            SJRK8M = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "实况入库20小时")
                        {
                            SKRK20H = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "实况入库20分钟")
                        {
                            SKRK20M = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "市局预报20入库小时")
                        {
                            SJRK20H = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "市局预报20入库分钟")
                        {
                            SJRK20M = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "实况时次")
                        {
                            RKTime = line.Split('=')[1];
                        }
                        else if (line.Split('=')[0] == "中央指导入库08小时")
                        {
                            ZYRK8H = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "中央指导入库08分钟")
                        {
                            ZYRK8M = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "中央指导入库20小时")
                        {
                            ZYRK20H = Convert.ToInt16(line.Split('=')[1]);
                        }
                        else if (line.Split('=')[0] == "中央指导入库20分钟")
                        {
                            ZYRK20M = Convert.ToInt16(line.Split('=')[1]);
                        }
                    }
                }

                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open(); //打开
                    try
                    {
                        string sql = @"select * from QXList "; //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        SqlDataReader sqlreader = sqlman.ExecuteReader();
                        while (sqlreader.Read())
                        {
                            QXID += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + ',';
                            QXName += sqlreader.GetString(sqlreader.GetOrdinal("Name")) + ',';
                        }

                        QXID = QXID.Substring(0, QXID.Length - 1);
                        QXName = QXName.Substring(0, QXName.Length - 1);
                    }
                    catch (Exception ex)
                    {
                        t1.Dispatcher.Invoke(new Action(delegate
                        {
                            t1.AppendText(ex.Message + '\n');
                            //将光标移至文本框最后
                            t1.Focus();
                            t1.CaretIndex = t1.Text.Length;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                t1.Dispatcher.Invoke(new Action(delegate
                {
                    t1.AppendText(ex.Message + '\n');
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }));
            }

            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\智能网格设置.xml");
                QJZNHH08 = Convert.ToInt16(util.Read("QJTime", "HH08"));
                QJZNHH20 = Convert.ToInt16(util.Read("QJTime", "HH20"));
                QJZNMM08 = Convert.ToInt16(util.Read("QJTime", "MM08"));
                QJZNMM20 = Convert.ToInt16(util.Read("QJTime", "MM20"));
                GJZNHH08 = Convert.ToInt16(util.Read("GJTime", "HH08"));
                GJZNHH20 = Convert.ToInt16(util.Read("GJTime", "HH20"));
                GJZNMM08 = Convert.ToInt16(util.Read("GJTime", "MM08"));
                GJZNMM20 = Convert.ToInt16(util.Read("GJTime", "MM20"));
                ECHH08 = Convert.ToInt16(util.Read("ECTime", "HH08"));
                ECHH20 = Convert.ToInt16(util.Read("ECTime", "HH20"));
                ECMM08 = Convert.ToInt16(util.Read("ECTime", "MM08"));
                ECMM20 = Convert.ToInt16(util.Read("ECTime", "MM20"));
                YBHH08 = Convert.ToInt16(util.Read("YBTime", "HH08"));
                YBHH20 = Convert.ToInt16(util.Read("YBTime", "HH20"));
                YBMM08 = Convert.ToInt16(util.Read("YBTime", "MM08"));
                YBMM20 = Convert.ToInt16(util.Read("YBTime", "MM20"));
            }
            catch (Exception ex)
            {
                t1.Dispatcher.Invoke(new Action(delegate
                {
                    t1.AppendText(ex.Message + '\n');
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }));
            }
        }

        public void Save(string strDate, string strTime)
        {
            try
            {
                int SKRKGS = 0;
                string strError = "";
                string strSK = "";
                int rst1 = 0;

                Class1 c1 = new Class1();
                string strQXSK = c1.CIMISSHQQXSK(strDate, strTime, ref rst1, ref strError);
                if (rst1 == 0)
                {
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        mycon.Open(); //打开


                        for (int i = 0; i < strQXSK.Split('\n').Length; i++)
                        {
                            string[] szLS1 = strQXSK.Split('\n')[i].Split(' ');
                            float myTmax, myTmin, myRain;
                            try
                            {
                                myRain = Convert.ToSingle(szLS1[5]);
                            }
                            catch
                            {
                                myRain = 999999;
                            }

                            try
                            {
                                myTmax = Convert.ToSingle(szLS1[3]);
                            }
                            catch
                            {
                                myTmax = 999999;
                            }

                            try
                            {
                                myTmin = Convert.ToSingle(szLS1[4]);
                            }
                            catch
                            {
                                myTmin = 999999;
                            }

                            if (myTmin == myTmax) //如果最高最低温度相等，按照缺测处理
                            {
                                myTmin = 999999;
                                myTmax = 999999;
                            }

                            string myDate = strDate.Substring(0, 4) + '-' + strDate.Substring(4, 2) + '-' + strDate.Substring(6, 2);
                            string sql = string.Format(@"insert into SK (Name,StationID,Date,SC,Tmax,Tmin,Rain) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", QXName.Split(',')[i], QXID.Split(',')[i], myDate, strTime, myTmax, myTmin, myRain); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                            try
                            {
                                SqlCommand sqlman = new SqlCommand(sql, mycon);
                                SKRKGS += sqlman.ExecuteNonQuery(); //执行数据库语句并返回一个int值（受影响的行数）     
                            }
                            catch (Exception ex)
                            {
                                // MessageBox.Show("数据库添加失败\n" + ex.Message);
                            }
                        }
                    }
                }

                t1.Dispatcher.Invoke(new Action(delegate
                {
                    t1.AppendText(DateTime.Now + "保存" + strDate + "日" + strTime + "时" + SKRKGS + "条实况至数据库\n");
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }));
                SaveJL(DateTime.Now + "保存" + strDate + "日" + strTime + "时" + SKRKGS + "条实况至数据库\r\n");
                string error = "";
                string jltext = "";
                c1.CIMISSRain12(strDate, strTime, ref error, ref jltext);
                error += strError;
                t1.Dispatcher.Invoke(new Action(delegate
                {
                    t1.AppendText(jltext + '\n');
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }));
                SaveJL(jltext + "\r\n");
                errorTBox.Dispatcher.Invoke(new Action(delegate
                {
                    errorTBox.AppendText(error + '\n');
                    //将光标移至文本框最后
                    errorTBox.Focus();
                    errorTBox.CaretIndex = errorTBox.Text.Length;
                }));
                SaveJL(error + "\r\n");
                if (strError.Length == 0)
                {
                }
            }
            catch
            {
            }
        }

        public void SaveJL(string jtText)
        {
            try
            {
                string DicPath = Environment.CurrentDirectory + @"\日志";
                string path = DicPath + '\\' + DateTime.Now.ToString("yyyy年MM月dd日") + "日志文件.txt";
                if (!Directory.Exists(DicPath))
                {
                    Directory.CreateDirectory(DicPath);
                }

                using (StreamWriter sw = new StreamWriter(path, true, Encoding.Default))
                {
                    sw.Write(jtText);
                    sw.Flush();
                }
            }
            catch
            {
            }
        }

        public void refreshTime(object source, ElapsedEventArgs e)
        {
            try
            {
                try
                {
                    Thread mythread = new Thread(new ThreadStart(cimiss同步中央指导预报));
                    mythread.Start();
                }
                catch
                {
                }
                if (DateTime.Now.Hour == SKRK20H && DateTime.Now.Minute == SKRK20M)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        try
                        {
                            string strDate = DateTime.Now.AddDays(-1 * i - 1).ToString("yyyyMMdd");
                            Save(strDate, "20");
                            Class1 c1 = new Class1();
                            string ss = "";
                            ss += c1.TJRK(DateTime.Now.AddDays(-1 * i - 2), "20", "主班");
                            ss += c1.TJRK(DateTime.Now.AddDays(-1 * i - 2), "20", "领班");
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ss);
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                            SaveJL(ss);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                t1.Dispatcher.Invoke(new Action(delegate
                                {
                                    t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                    t1.CaretIndex = t1.Text.Length;
                                }));
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                else if (DateTime.Now.Hour == SKRK8H && DateTime.Now.Minute == SKRK8M)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        try
                        {
                            string strDate = DateTime.Now.AddDays(-1 * i).ToString("yyyyMMdd");
                            Save(strDate, "08");
                            Class1 c1 = new Class1();
                            string ss = c1.TJRK(DateTime.Now.AddDays(-1 * i - 1), "08", "主班");
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ss);
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                            SaveJL(ss);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                t1.Dispatcher.Invoke(new Action(delegate
                                {
                                    t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                    t1.CaretIndex = t1.Text.Length;
                                }));
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                if (DateTime.Now.Hour == SJRK8H && DateTime.Now.Minute == SJRK8M)
                {
                    try
                    {
                        string GWList = "";

                        using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\config\GWList.txt", Encoding.Default))
                        {
                            string line = "";
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.Split('=')[0] == "08岗位列表")
                                {
                                    GWList = line.Split('=')[1];
                                    break;
                                }
                            }
                        }

                        Class1 c1 = new Class1();
                        for (int i = 0; i < GWList.Split(',').Length; i++)
                        {
                            for (int j = 0; j < 7; j++)
                            {
                                try
                                {
                                    string strDate = DateTime.Now.AddDays(-1 * j).ToString("yyyyMMdd");
                                    string ssLs = c1.Sjyb(strDate, "08", GWList.Split(',')[i]);
                                    t1.Dispatcher.Invoke(new Action(delegate
                                    {
                                        t1.AppendText(ssLs);
                                        //将光标移至文本框最后
                                        t1.Focus();
                                        t1.CaretIndex = t1.Text.Length;
                                    }));
                                    SaveJL(ssLs);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        t1.Dispatcher.Invoke(new Action(delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                            //将光标移至文本框最后
                                            t1.Focus();
                                            t1.CaretIndex = t1.Text.Length;
                                        }));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }
                else if (DateTime.Now.Hour == SJRK20H && DateTime.Now.Minute == SJRK20M)
                {
                    try
                    {
                        string GWList = "";

                        using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\config\GWList.txt", Encoding.Default))
                        {
                            string line = "";
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.Split('=')[0] == "20岗位列表")
                                {
                                    GWList = line.Split('=')[1];
                                    break;
                                }
                            }
                        }

                        Class1 c1 = new Class1();
                        for (int i = 0; i < GWList.Split(',').Length; i++)
                        {
                            for (int j = 0; j < 7; j++)
                            {
                                try
                                {
                                    string strDate = DateTime.Now.AddDays(-1 * j).ToString("yyyyMMdd");
                                    string ssLs = c1.Sjyb(strDate, "20", GWList.Split(',')[i]);
                                    t1.Dispatcher.Invoke(new Action(delegate
                                    {
                                        t1.AppendText(ssLs);
                                        //将光标移至文本框最后
                                        t1.Focus();
                                        t1.CaretIndex = t1.Text.Length;
                                    }));
                                    SaveJL(ssLs);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        t1.Dispatcher.Invoke(new Action(delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                            //将光标移至文本框最后
                                            t1.Focus();
                                            t1.CaretIndex = t1.Text.Length;
                                        }));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }

                if (DateTime.Now.Hour == ZYRK8H && DateTime.Now.Minute == ZYRK8M)
                {
                    try
                    {
                        Class1 c1 = new Class1();
                        string ss = "";

                        if (c1.ZYZDRK(DateTime.Now, "08", ref ss))
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ss);
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                            SaveJL(ss);
                        }

                        for (int i = 0; i < 6; i++)
                        {
                            try
                            {
                                ss = c1.ZYZDCIMISS(DateTime.Now.AddDays(-1 * i - 1).ToString("yyyyMMdd"), "08");
                                t1.Dispatcher.Invoke(new Action(delegate
                                {
                                    t1.AppendText(ss);
                                    //将光标移至文本框最后
                                    t1.Focus();
                                    t1.CaretIndex = t1.Text.Length;
                                }));
                                SaveJL(ss);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    t1.Dispatcher.Invoke(new Action(delegate
                                    {
                                        t1.AppendText(ex.Message + "\r\n");
                                        //将光标移至文本框最后
                                        t1.Focus();
                                        t1.CaretIndex = t1.Text.Length;
                                    }));
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }
                else if (DateTime.Now.Hour == ZYRK20H && DateTime.Now.Minute == ZYRK20M)
                {
                    try
                    {
                        Class1 c1 = new Class1();
                        string ss = "";
                        if (c1.ZYZDRK(DateTime.Now, "20", ref ss))
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ss);
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                            SaveJL(ss);
                        }

                        for (int i = 0; i < 6; i++)
                        {
                            try
                            {
                                ss = c1.ZYZDCIMISS(DateTime.Now.AddDays(-1 * i - 1).ToString("yyyyMMdd"), "20");
                                t1.Dispatcher.Invoke(new Action(delegate
                                {
                                    t1.AppendText(ss);
                                    //将光标移至文本框最后
                                    t1.Focus();
                                    t1.CaretIndex = t1.Text.Length;
                                }));
                                SaveJL(ss);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    t1.Dispatcher.Invoke(new Action(delegate
                                    {
                                        t1.AppendText(ex.Message + "\r\n");
                                        //将光标移至文本框最后
                                        t1.Focus();
                                        t1.CaretIndex = t1.Text.Length;
                                    }));
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }

                if ((DateTime.Now.Hour == QJZNHH08 || DateTime.Now.AddHours(-1).Hour == QJZNHH08 || DateTime.Now.AddHours(-2).Hour == QJZNHH08) && DateTime.Now.Minute == QJZNMM08)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        QJZNdt = DateTime.Now;
                        QJZNsc = 8;
                        Thread th1 = new Thread(QJZNRK);
                        th1.Start();
                        Thread.Sleep(100);
                        if (DateTime.Now.AddHours(-2).Hour == QJZNHH08)
                        {
                            for (short i = -1; i > -6; i--)
                            {
                                try
                                {
                                    QJZNdt = DateTime.Now.AddDays(i);
                                    QJZNsc = 8;
                                    Thread th2 = new Thread(QJZNRK);
                                    th2.Start();
                                    Thread.Sleep(100);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        t1.Dispatcher.Invoke(new Action(delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                            //将光标移至文本框最后
                                            t1.Focus();
                                            t1.CaretIndex = t1.Text.Length;
                                        }));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }
                else if ((DateTime.Now.Hour == QJZNHH20 || DateTime.Now.AddHours(-1).Hour == QJZNHH20 || DateTime.Now.AddHours(-2).Hour == QJZNHH20) && DateTime.Now.Minute == QJZNMM20)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        QJZNdt = DateTime.Now;
                        QJZNsc = 20;
                        Thread th1 = new Thread(QJZNRK);
                        th1.Start();
                        Thread.Sleep(100);
                        if (DateTime.Now.AddHours(-2).Hour == QJZNHH20)
                        {
                            for (short i = -1; i > -6; i--)
                            {
                                try
                                {
                                    QJZNdt = DateTime.Now.AddDays(i);
                                    QJZNsc = 20;
                                    Thread th2 = new Thread(QJZNRK);
                                    th2.Start();
                                    Thread.Sleep(100);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        t1.Dispatcher.Invoke(new Action(delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                            //将光标移至文本框最后
                                            t1.Focus();
                                            t1.CaretIndex = t1.Text.Length;
                                        }));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }

                if ((DateTime.Now.Hour == GJZNHH08 || DateTime.Now.AddHours(-1).Hour == GJZNHH08 || DateTime.Now.AddHours(-2).Hour == GJZNHH08) && DateTime.Now.Minute == GJZNMM08)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        GJZNdt = DateTime.Now;
                        GJZNsc = 8;
                        Thread th1 = new Thread(GJZNRK);
                        th1.Start();
                        Thread.Sleep(100);
                        if (DateTime.Now.AddHours(-2).Hour == GJZNHH08)
                        {
                            for (short i = -1; i > -6; i--)
                            {
                                try
                                {
                                    GJZNdt = DateTime.Now.AddDays(i);
                                    GJZNsc = 8;
                                    Thread th2 = new Thread(GJZNRK);
                                    th2.Start();
                                    Thread.Sleep(100);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        t1.Dispatcher.Invoke(new Action(delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                            //将光标移至文本框最后
                                            t1.Focus();
                                            t1.CaretIndex = t1.Text.Length;
                                        }));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }
                else if ((DateTime.Now.Hour == GJZNHH20 || DateTime.Now.AddHours(-1).Hour == GJZNHH20 || DateTime.Now.AddHours(-2).Hour == GJZNHH20) && DateTime.Now.Minute == GJZNMM20)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        GJZNdt = DateTime.Now;
                        GJZNsc = 20;
                        Thread th1 = new Thread(GJZNRK);
                        th1.Start();
                        Thread.Sleep(100);
                        if (DateTime.Now.AddHours(-2).Hour == GJZNHH20)
                        {
                            for (short i = -1; i > -6; i--)
                            {
                                try
                                {
                                    GJZNdt = DateTime.Now.AddDays(i);
                                    GJZNsc = 20;
                                    Thread th2 = new Thread(GJZNRK);
                                    th2.Start();
                                    Thread.Sleep(100);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        t1.Dispatcher.Invoke(new Action(delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                            //将光标移至文本框最后
                                            t1.Focus();
                                            t1.CaretIndex = t1.Text.Length;
                                        }));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }

                if ((DateTime.Now.Hour == ECHH08 || DateTime.Now.AddHours(-1).Hour == ECHH08 || DateTime.Now.AddHours(-2).Hour == ECHH08) && DateTime.Now.Minute == ECMM08)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        ECdt = DateTime.Now;
                        ECsc = 8;
                        Thread th1 = new Thread(ECRK);
                        th1.Start();
                        Thread.Sleep(1000);
                        if (DateTime.Now.AddHours(-2).Hour == ECHH08)
                        {
                            for (short i = -1; i > -6; i--)
                            {
                                try
                                {
                                    ECdt = DateTime.Now.AddDays(i);
                                    ECsc = 8;
                                    Thread th2 = new Thread(ECRK);
                                    th2.Start();
                                    Thread.Sleep(1000);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        t1.Dispatcher.Invoke(new Action(delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                            //将光标移至文本框最后
                                            t1.Focus();
                                            t1.CaretIndex = t1.Text.Length;
                                        }));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }
                else if ((DateTime.Now.Hour == ECHH20 || DateTime.Now.AddHours(-1).Hour == ECHH20 || DateTime.Now.AddHours(-2).Hour == ECHH20) && DateTime.Now.Minute == ECMM20)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        ECdt = DateTime.Now.AddDays(-1);
                        ECsc = 20;
                        Thread th1 = new Thread(ECRK);
                        th1.Start();
                        Thread.Sleep(1000);
                        if (DateTime.Now.AddHours(-2).Hour == ECHH20)
                        {
                            for (short i = -1; i > -6; i--)
                            {
                                try
                                {
                                    ECdt = DateTime.Now.AddDays(i);
                                    ECsc = 20;
                                    Thread th2 = new Thread(ECRK);
                                    th2.Start();
                                    Thread.Sleep(1000);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        t1.Dispatcher.Invoke(new Action(delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                            //将光标移至文本框最后
                                            t1.Focus();
                                            t1.CaretIndex = t1.Text.Length;
                                        }));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }

                if ((DateTime.Now.Hour == YBHH08 || DateTime.Now.AddHours(-1).Hour == YBHH08 || DateTime.Now.AddHours(-2).Hour == YBHH08) && DateTime.Now.Minute == YBMM08)
                {
                    try
                    {
                        YBdt = DateTime.Now;
                        YBsc = 8;
                        Thread thread = new Thread(YBRK);
                        thread.Start();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }
                else if ((DateTime.Now.Hour == YBHH20 || DateTime.Now.AddHours(-1).Hour == YBHH20 || DateTime.Now.AddHours(-2).Hour == YBHH20) && DateTime.Now.Minute == YBMM20)
                {
                    try
                    {
                        YBdt = DateTime.Now;
                        YBsc = 20;
                        Thread thread = new Thread(YBRK);
                        thread.Start();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            t1.Dispatcher.Invoke(new Action(delegate
                            {
                                t1.AppendText(ex.Message + "\r\n");
                                //将光标移至文本框最后
                                t1.Focus();
                                t1.CaretIndex = t1.Text.Length;
                            }));
                        }
                        catch
                        {
                        }
                    }
                }

                if (DateTime.Now.Minute == 10) //每小时的10分入库实况小时数据
                {
                    Thread thread = new Thread(HourSKRK);
                    thread.Start();
                }

                if (DateTime.Now.Minute % 20 == 0)
                {
                    Thread thread = new Thread(获取6小时指导预报);
                    thread.Start();
                }

                //Thread thread1 = new Thread(TBBW);
                //thread1.Start();
            }
            catch (Exception ex)
            {
                try
                {
                    t1.Dispatcher.Invoke(new Action(delegate
                    {
                        t1.AppendText(ex.Message + "\r\n");
                        //将光标移至文本框最后
                        t1.Focus();
                        t1.CaretIndex = t1.Text.Length;
                    }));
                }
                catch
                {
                }
            }
        }
        public void cimiss同步中央指导预报()
        {
            try
            {
                DateTime mydate = DateTime.Now;
                if ((mydate.Hour >= 2 && mydate.Hour <= 3 && mydate.Minute % 3 == 0) || mydate.Hour == 4 ||
                    mydate.Hour == 5)
                {
                    Cimiss cimiss = new Cimiss();
                    List<CIMISS文件信息> filelists = cimiss.获取中央指导预报(DateTime.Now.Date.AddHours(8));
                    if (filelists.Count > 0)
                    {
                        if (中央指导路径.Length > 0)
                        {
                            foreach (var item in filelists)
                            {
                                string myfile = 中央指导路径 + item.fileName;
                                if (!File.Exists(myfile))
                                {
                                    try
                                    {
                                        WebClient webClient = new WebClient();
                                        webClient.Credentials = CredentialCache.DefaultCredentials;
                                        webClient.DownloadFile(item.fileUrl, myfile);
                                        string jltext = $"{DateTime.Now}同步08时起报中央指导预报报文";
                                        this.t1.Dispatcher.Invoke(
                                            new Action(
                                                delegate
                                                {
                                                    t1.AppendText(jltext);
                                                    //将光标移至文本框最后
                                                    t1.Focus();
                                                    t1.CaretIndex = (t1.Text.Length);
                                                }
                                            ));
                                        SaveJL(jltext);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }

                        }


                    }
                }
                else if (mydate.Hour >= 13 && mydate.Hour <= 16)
                {
                    Cimiss cimiss = new Cimiss();
                    List<CIMISS文件信息> filelists = cimiss.获取中央指导预报(DateTime.Now.Date.AddHours(20));
                    if (filelists.Count > 0)
                    {
                        if (中央指导路径.Length > 0)
                        {
                            foreach (var item in filelists)
                            {
                                string myfile = 中央指导路径 + item.fileName;
                                if (!File.Exists(myfile))
                                {
                                    try
                                    {
                                        WebClient webClient = new WebClient();
                                        webClient.Credentials = CredentialCache.DefaultCredentials;
                                        webClient.DownloadFile(item.fileUrl, myfile);
                                        string jltext = $"{DateTime.Now}同步20时起报中央指导预报报文";
                                        this.t1.Dispatcher.Invoke(
                                            new Action(
                                                delegate
                                                {
                                                    t1.AppendText(jltext);
                                                    //将光标移至文本框最后
                                                    t1.Focus();
                                                    t1.CaretIndex = (t1.Text.Length);
                                                }
                                            ));
                                        SaveJL(jltext);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }

                        }


                    }
                }
            }
            catch
            {
            }
        }

        private void QJZNRK()
        {
            try
            {
                int count = 0;
                智能网格类1 c1 = new 智能网格类1();
                string strtime = QJZNdt.ToString("yyyyMMdd");
                string error = c1.YBRK(strtime, QJZNsc, ref count);
                if (error.Trim().Length == 0)
                {
                    if (count != 0)
                    {
                        error = DateTime.Now + "保存" + strtime + QJZNsc.ToString().PadLeft(2, '0') + "时区局智能网格数据成功！\r\n";
                    }
                }
                else
                {
                    error = DateTime.Now + "保存" + strtime + QJZNsc.ToString().PadLeft(2, '0') + "时区局智能网格数据：\r\n" + error;
                }

                t1.Dispatcher.Invoke(new Action(delegate
                {
                    t1.AppendText(error);
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }));
                if (error.Trim().Length > 0 || count > 0)
                {
                    SaveJL(error);
                }
            }
            catch
            {
            }
        }

        private void GJZNRK()
        {
            try
            {
                int count = 0;
                智能网格类1 c1 = new 智能网格类1();
                string strtime = GJZNdt.ToString("yyyyMMdd");
                //this.t1.Dispatcher.Invoke(
                //    new Action(
                //        delegate
                //        {
                //            t1.AppendText("开始备份"+strtime+"日"+GJZNsc+"时国家智能网格数据\n");
                //            //将光标移至文本框最后
                //            t1.Focus();
                //            t1.CaretIndex = (t1.Text.Length);
                //        }
                //    ));
                string error = c1.GJYBRK(strtime, GJZNsc, ref count);
                if (error.Trim().Length == 0)
                {
                    if (count != 0)
                    {
                        error = DateTime.Now + "保存" + strtime + GJZNsc.ToString().PadLeft(2, '0') + "时国家级智能网格数据成功！\r\n";
                    }
                }
                else
                {
                    error = DateTime.Now + "保存" + strtime + GJZNsc.ToString().PadLeft(2, '0') + "时国家级智能网格数据：\r\n" + error;
                }

                t1.Dispatcher.Invoke(new Action(delegate
                {
                    t1.AppendText(error);
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }));
                if (error.Trim().Length > 0 || count > 0)
                {
                    SaveJL(error);
                }
            }
            catch
            {
            }
        }

        private void ECRK()
        {
            try
            {
                int count = 0;
                EC处理类 c1 = new EC处理类();
                string strtime = ECdt.ToString("yyyyMMdd");

                string error = c1.ECYBRK(strtime, ECsc, ref count);
                if (error.Trim().Length == 0)
                {
                    if (count != 0)
                    {
                        error = DateTime.Now + "保存" + strtime + ECsc.ToString().PadLeft(2, '0') + "时EC数据成功！\r\n";
                    }
                }
                else
                {
                    error = DateTime.Now + "保存" + strtime + ECsc.ToString().PadLeft(2, '0') + "时EC数据：\r\n" + error;
                }

                t1.Dispatcher.Invoke(new Action(delegate
                {
                    t1.AppendText(error);
                    //将光标移至文本框最后
                    t1.Focus();
                    t1.CaretIndex = t1.Text.Length;
                }));
                if (error.Trim().Length > 0 || count > 0)
                {
                    SaveJL(error);
                }
            }
            catch
            {
            }
        }

        public void YBRK()
        {
            try
            {
                ClassSZYB classSZYB = new ClassSZYB();
                string error = "";
                bool insertBS = classSZYB.CLYB(YBdt, YBsc, ref error);
                if (error.Trim().Length == 0 && insertBS)
                {
                    error = DateTime.Now + "保存" + YBdt.ToString("yyyy年MM月dd日") + YBsc + "时数值预报成功！\r\n";
                    SaveJL(error);
                }
                else if (insertBS)
                {
                    error = DateTime.Now + "保存" + YBdt.ToString("yyyy年MM月dd日") + YBsc + "时数值预报：！\r\n" + error;
                }

                if (error.Trim().Length > 0 || insertBS)
                {
                    t1.Dispatcher.Invoke(new Action(delegate
                    {
                        t1.AppendText(error);
                        //将光标移至文本框最后
                        t1.Focus();
                        t1.CaretIndex = t1.Text.Length;
                    }));
                    SaveJL(error);
                }

                if (DateTime.Now.AddHours(-2).Hour == YBHH08 || DateTime.Now.AddHours(-2).Hour == YBHH20)
                {
                    for (short i = -1; i > -3; i--)
                    {
                        try
                        {
                            insertBS = false;
                            error = "";
                            insertBS = classSZYB.CLYB(YBdt.AddDays(i), YBsc, ref error);
                            if (error.Trim().Length == 0 && insertBS)
                            {
                                error = DateTime.Now + "保存" + YBdt.AddDays(i).ToString("yyyy年MM月dd日") + YBsc + "时数值预报成功！\r\n";
                                SaveJL(error);
                            }
                            else if (insertBS)
                            {
                                error = DateTime.Now + "保存" + YBdt.AddDays(i).ToString("yyyy年MM月dd日") + YBsc + "时数值预报：！\r\n" + error;
                            }

                            if (error.Trim().Length > 0 || insertBS)
                            {
                                t1.Dispatcher.Invoke(new Action(delegate
                                {
                                    t1.AppendText(error);
                                    //将光标移至文本框最后
                                    t1.Focus();
                                    t1.CaretIndex = t1.Text.Length;
                                }));
                                SaveJL(error);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    SaveJL(ex.Message + "\r\n");
                }
                catch
                {
                }
            }
        }

        public void 获取6小时指导预报()
        {
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\智能网格设置.xml");
                string myPath = util.Read("Path6Hour");
                DateTime dateTime = DateTime.Now.AddDays(-1).Date;
                List<string> files = Directory.GetFiles(myPath, "*.txt", SearchOption.AllDirectories).ToList();
                while (dateTime.CompareTo(DateTime.Now) <= 0)
                {
                    智能网格类1 znwg = new 智能网格类1();
                    if (!files.Exists(y => y.Contains("SCMOC6H_" + dateTime.ToString("yyyyMMdd0000"))))
                    {
                        string jl = znwg.CIMISS6H(dateTime.AddHours(8));
                        if (jl.Trim().Length == 0)
                        {
                            jl = DateTime.Now.ToString("yyyyMMdd日HH:mm:ss") + "保存" + dateTime.ToString("yyyyMMdd日") + "08时6小时指导预报报文\r\n";
                        }

                        t1.Dispatcher.Invoke(new Action(delegate
                        {
                            t1.AppendText(jl);
                            //将光标移至文本框最后
                            t1.Focus();
                            t1.CaretIndex = t1.Text.Length;
                        }));
                        SaveJL(jl);
                    }

                    if (DateTime.Now.Hour > 13 && !files.Exists(y => y.Contains("SCMOC6H_" + dateTime.ToString("yyyyMMdd1200"))))
                    {
                        string jl = znwg.CIMISS6H(dateTime.AddHours(20));
                        if (jl.Trim().Length == 0)
                        {
                            jl = DateTime.Now.ToString("yyyyMMdd日HH:mm:ss") + "成功保存" + dateTime.ToString("yyyyMMdd日") + "20时6小时指导预报报文\r\n";
                        }

                        t1.Dispatcher.Invoke(new Action(delegate
                        {
                            t1.AppendText(jl);
                            //将光标移至文本框最后
                            t1.Focus();
                            t1.CaretIndex = t1.Text.Length;
                        }));
                        SaveJL(jl);
                    }

                    dateTime = dateTime.AddDays(1);
                }

                if (files.Count >= 7)
                {
                    foreach (string ff in files)
                    {
                        try
                        {
                            string[] szls = ff.Split('_');
                            string strdate = szls[szls.Length - 2];
                            DateTime myTime = Convert.ToDateTime($"{strdate.Substring(0, 4)}-{strdate.Substring(4, 2)}-{strdate.Substring(6, 2)} 00:00:00");
                            if ((DateTime.Now - myTime).TotalHours > 72)
                            {
                                File.Delete(ff);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}