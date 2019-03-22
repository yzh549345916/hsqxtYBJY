using System;
using System.Text;
using System.Windows;
using System.IO;
using System.Data.SqlClient;
using System.Threading;

using System.Data.SqlClient;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Forms;

namespace 呼和浩特市精细化天气预报评分系统_数据库
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon = null;
        System.Timers.Timer t = new System.Timers.Timer(60000);
        string con;//这里是保存连接数据库的字符串172.18.142.158 id=sa;password=134679;
        private Int16 ZYRK8H = 0, ZYRK8M = 0, ZYRK20H = 0, ZYRK20M = 0;
        Int16  SJRK8H = 0, SJRK8M = 0, SJRK20H = 0, SJRK20M = 0,SKRK8H=0,SKRK20H=0,SKRK8M=0,SKRK20M=0;//实况入库的分钟和小时,窗口初始化程序中会重新给该值从配置文件中赋值
        private string QXID = "",QXName="";

        Int16 QJZNHH08=0, QJZNMM08 = 0, QJZNHH20 = 0,QJZNMM20=0,QJZNsc=8;
        Int16 GJZNHH08 = 0, GJZNMM08 = 0, GJZNHH20 = 0, GJZNMM20 = 0, GJZNsc = 8;
        Int16 ECHH08 = 0,ECMM08 = 0, ECHH20 = 0, ECMM20 = 0, ECsc = 8;

        private void T1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                t1.TextChanged -= T1_TextChanged;
                string[] szls = t1.Text.Split('\n');
                if(szls.Length>5000)
                {
                    string data = "";
                    for(int i=0;i<4000;i++)
                    {
                        data += szls[4000 - i] + '\n';
                    }
                    this.t1.Dispatcher.Invoke(
                           new Action(
                               delegate
                               {
                                   t1.Text=data;
                                   //将光标移至文本框最后
                                   t1.Focus();
                                   t1.CaretIndex = (t1.Text.Length);
                               }
                           ));
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

        Int16 YBHH08 = 0, YBMM08 = 0, YBHH20 = 0, YBMM20 = 0, YBsc = 8;

        private DateTime QJZNdt = DateTime.Now;
        private DateTime GJZNdt = DateTime.Now;
        private DateTime ECdt = DateTime.Now;
        private DateTime YBdt = DateTime.Now;


        private void JLButton_Click(object sender, RoutedEventArgs e)
        {
            t1.Visibility = Visibility.Visible;
            errorTBox.Visibility = Visibility.Hidden;
            t1.Focus();
            t1.CaretIndex = (t1.Text.Length);
        }

        private void errorJLBut_Click(object sender, RoutedEventArgs e)
        {
            t1.Visibility = Visibility.Hidden;
            errorTBox.Visibility = Visibility.Visible;
            errorTBox.Focus();
            errorTBox.CaretIndex = (errorTBox.Text.Length);
        }



        private void SJHFBu_Click(object sender, RoutedEventArgs e)
        {

            数据恢复窗口 sjhfWind = new 数据恢复窗口();
            sjhfWind.Show();
        }

        private void SJYBHF_Click(object sender, RoutedEventArgs e)
        {
            ClassSZYB classSZYB = new ClassSZYB();
            string error = "";
            classSZYB.temByGJZNSK("53463", DateTime.Now, 8, ref error);

        }
        #region 窗口状态改变
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.WindowState == WindowState.Minimized)
                {
                    this.Visibility = Visibility.Hidden;
                }
            }
            catch { }
        }
        #endregion

        #region 托盘图标鼠标单击事件
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.Activate();
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
                    _notifyIcon = new System.Windows.Forms.NotifyIcon();
                    //隐藏主窗体
                    this.Visibility = Visibility.Hidden;
                    //设置托盘的各个属性

                    _notifyIcon.BalloonTipText = "呼和浩特市精细化天气预报评分系统数据库服务运行中...";//托盘气泡显示内容
                    _notifyIcon.Text = "呼和浩特市精细化天气预报评分系统数据库";
                    _notifyIcon.Visible = true;//托盘按钮是否可见
                    _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); ;//托盘中显示的图标
                    _notifyIcon.ShowBalloonTip(2000);//托盘气泡显示时间
                    _notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
                    //窗体状态改变时触发
                    this.StateChanged += MainWindow_StateChanged;
                }
                else
                {
                    this.WindowState = WindowState.Minimized;
                }

            }
            catch { }
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
                    classSZYB.SKRK(DateTime.Now.AddHours(i).ToString("yyyyMMdd"), DateTime.Now.AddHours(i).Hour, ref error,ref insertBS);
                   if(error.Trim().Length == 0&&insertBS)
                   {
                        error = DateTime.Now.ToString() + "保存" + DateTime.Now.AddHours(i).ToString("yyyyMMddHH") + "时实况小时数据成功！\r\n";
                        SaveJL(error);
                    }
                   else if(insertBS)
                   {
                        error = DateTime.Now.ToString() + "保存" + DateTime.Now.AddHours(i).ToString("yyyyMMddHH") + "时实况小时数据：\r\n" + error;
                    }
                    if (error.Trim().Length > 0 || insertBS )
                    {
                        this.t1.Dispatcher.Invoke(
                          new Action(
                              delegate
                              {
                                  t1.AppendText(error);
                              //将光标移至文本框最后
                              t1.Focus();
                                  t1.CaretIndex = (t1.Text.Length);
                              }
                          ));
                        SaveJL(error);
                    }
                }

                
                
            }
            catch
            {
            }

        }
        string RKTime = "20";//实况入库的时次,窗口初始化程序中会重新给该值从配置文件中赋值
        string DBconPath = Environment.CurrentDirectory + @"\config\DBconfig.txt";
        public MainWindow()
        {
            InitializeComponent();
            CSH();
        }

        public void CSH()
        {
            t1.Text = DateTime.Now.ToString() + "  启动"+'\n';
            t.Elapsed += new System.Timers.ElapsedEventHandler(refreshTime);
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；  
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
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
                    mycon.Open();//打开
                    try
                    {
                        string sql = string.Format(@"select * from QXList ");  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
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
                        this.t1.Dispatcher.Invoke(
                             new Action(
                                 delegate
                                 {
                                     t1.AppendText(ex.Message + '\n');
                                      //将光标移至文本框最后
                                      t1.Focus();
                                     t1.CaretIndex = (t1.Text.Length);
                                 }
                             ));
                    }
                }
            }
            catch (Exception ex)
            {
                this.t1.Dispatcher.Invoke(
                             new Action(
                                 delegate
                                 {
                                     t1.AppendText(ex.Message + '\n');
                                      //将光标移至文本框最后
                                      t1.Focus();
                                     t1.CaretIndex = (t1.Text.Length);
                                 }
                             ));
            }

            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\智能网格设置.xml");
                QJZNHH08=Convert.ToInt16(util.Read("QJTime", "HH08"));
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
            catch(Exception ex)
            {
                this.t1.Dispatcher.Invoke(
                              new Action(
                                  delegate
                                  {
                                      t1.AppendText(ex.Message+'\n');
                                      //将光标移至文本框最后
                                      t1.Focus();
                                      t1.CaretIndex = (t1.Text.Length);
                                  }
                              ));
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
                if ((rst1 == 0))
                {
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        mycon.Open();//打开


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

                            if (myTmin == myTmax)//如果最高最低温度相等，按照缺测处理
                            {
                                myTmin = 999999;
                                myTmax = 999999;
                            }
                            string myDate = strDate.Substring(0, 4) + '-' + strDate.Substring(4, 2) + '-' + strDate.Substring(6, 2);
                            string sql = string.Format(@"insert into SK (Name,StationID,Date,SC,Tmax,Tmin,Rain) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", QXName.Split(',')[i], QXID.Split(',')[i], myDate, strTime, myTmax, myTmin, myRain);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                            try
                            {
                                SqlCommand sqlman = new SqlCommand(sql, mycon);
                                SKRKGS += sqlman.ExecuteNonQuery();                            //执行数据库语句并返回一个int值（受影响的行数）     



                            }
                            catch (Exception ex)
                            {
                                // MessageBox.Show("数据库添加失败\n" + ex.Message);
                            }
                        }
                    }
                }
                this.t1.Dispatcher.Invoke(
                    new Action(
                        delegate
                        {
                            t1.AppendText(DateTime.Now.ToString() + "保存" + strDate + "日" + strTime + "时" + SKRKGS.ToString() + "条实况至数据库\n");
                        //将光标移至文本框最后
                        t1.Focus();
                            t1.CaretIndex = (t1.Text.Length);
                        }
                    ));
                SaveJL(DateTime.Now.ToString() + "保存" + strDate + "日" + strTime + "时" + SKRKGS.ToString() + "条实况至数据库\r\n");
                string error = "";
                string jltext = "";
                c1.CIMISSRain12(strDate, strTime, ref error, ref jltext);
                error += strError;
                this.t1.Dispatcher.Invoke(
                    new Action(
                        delegate
                        {
                            t1.AppendText(jltext + '\n');
                        //将光标移至文本框最后
                        t1.Focus();
                            t1.CaretIndex = (t1.Text.Length);
                        }
                    ));
                SaveJL(jltext + "\r\n");
                this.errorTBox.Dispatcher.Invoke(
                    new Action(
                        delegate
                        {
                            errorTBox.AppendText(error + '\n');
                        //将光标移至文本框最后
                        errorTBox.Focus();
                            errorTBox.CaretIndex = (errorTBox.Text.Length);
                        }
                    ));
                SaveJL(error + "\r\n");
                if (strError.Length == 0)
                {

                }
                else
                {
                    //MessageBox.Show("CIMISS出错，返回代码为：" + strError);
                }
            }
            catch { }

            
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
            catch { }
        }
        public void refreshTime(object source, System.Timers.ElapsedEventArgs e)
        {


            try
            {
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
                            this.t1.Dispatcher.Invoke(
                                new Action(
                                    delegate
                                    {
                                        t1.AppendText(ss);
                                    //将光标移至文本框最后
                                    t1.Focus();
                                        t1.CaretIndex = (t1.Text.Length);
                                    }
                                ));
                            SaveJL(ss);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                this.t1.Dispatcher.Invoke(
                                        new Action(
                                            delegate
                                            {
                                                t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                t1.CaretIndex = (t1.Text.Length);
                                            }
                                        ));
                            }
                            catch { }
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
                            this.t1.Dispatcher.Invoke(
                                new Action(
                                    delegate
                                    {
                                        t1.AppendText(ss);
                                    //将光标移至文本框最后
                                    t1.Focus();
                                        t1.CaretIndex = (t1.Text.Length);
                                    }
                                ));
                            SaveJL(ss);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                this.t1.Dispatcher.Invoke(
                                        new Action(
                                            delegate
                                            {
                                                t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                t1.CaretIndex = (t1.Text.Length);
                                            }
                                        ));
                            }
                            catch { }
                        }
                    }

                }
                if (DateTime.Now.Hour == SJRK8H && DateTime.Now.Minute == SJRK8M)
                {
                   try
                   {
                        string GWList = "";

                        using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\config\GWList.txt",
                            Encoding.Default))
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
                                    this.t1.Dispatcher.Invoke(
                                        new Action(
                                            delegate
                                            {
                                                t1.AppendText(ssLs);
                                            //将光标移至文本框最后
                                            t1.Focus();
                                                t1.CaretIndex = (t1.Text.Length);
                                            }
                                        ));
                                    SaveJL(ssLs);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        this.t1.Dispatcher.Invoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                        t1.CaretIndex = (t1.Text.Length);
                                                    }
                                                ));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
                    }



                }
                else if (DateTime.Now.Hour == SJRK20H && DateTime.Now.Minute == SJRK20M)
                {
                    try
                    {
                        string GWList = "";

                        using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\config\GWList.txt",
                            Encoding.Default))
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
                                    this.t1.Dispatcher.Invoke(
                                        new Action(
                                            delegate
                                            {
                                                t1.AppendText(ssLs);
                                            //将光标移至文本框最后
                                            t1.Focus();
                                                t1.CaretIndex = (t1.Text.Length);
                                            }
                                        ));
                                    SaveJL(ssLs);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        this.t1.Dispatcher.Invoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                        t1.CaretIndex = (t1.Text.Length);
                                                    }
                                                ));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
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
                            this.t1.Dispatcher.Invoke(
                                new Action(
                                    delegate
                                    {
                                        t1.AppendText(ss);
                                    //将光标移至文本框最后
                                    t1.Focus();
                                        t1.CaretIndex = (t1.Text.Length);
                                    }
                                ));
                            SaveJL(ss);
                        }

                        for (int i = 0; i < 6; i++)
                        {
                           try
                           {
                                ss = c1.ZYZDCIMISS(DateTime.Now.AddDays(-1 * i - 1).ToString("yyyyMMdd"), "08");
                                this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ss);
                                        //将光标移至文本框最后
                                        t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                                SaveJL(ss);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    this.t1.Dispatcher.Invoke(
                                            new Action(
                                                delegate
                                                {
                                                    t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                    t1.CaretIndex = (t1.Text.Length);
                                                }
                                            ));
                                }
                                catch { }
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
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
                            this.t1.Dispatcher.Invoke(
                                new Action(
                                    delegate
                                    {
                                        t1.AppendText(ss);
                                    //将光标移至文本框最后
                                    t1.Focus();
                                        t1.CaretIndex = (t1.Text.Length);
                                    }
                                ));
                            SaveJL(ss);
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            try
                            {
                                ss = c1.ZYZDCIMISS(DateTime.Now.AddDays(-1 * i - 1).ToString("yyyyMMdd"), "20");
                                this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ss);
                                        //将光标移至文本框最后
                                        t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                                SaveJL(ss);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    this.t1.Dispatcher.Invoke(
                                            new Action(
                                                delegate
                                                {
                                                    t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                    t1.CaretIndex = (t1.Text.Length);
                                                }
                                            ));
                                }
                                catch { }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
                    }
                }
                if ((DateTime.Now.Hour == QJZNHH08 || DateTime.Now.AddHours(-1).Hour == QJZNHH08 || DateTime.Now.AddHours(-2).Hour == QJZNHH08) && DateTime.Now.Minute == QJZNMM08)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        QJZNdt = DateTime.Now;
                        QJZNsc = 8;
                        Thread th1 = new Thread(new ThreadStart(QJZNRK));
                        th1.Start();
                        Thread.Sleep(100);
                        if (DateTime.Now.AddHours(-2).Hour == QJZNHH08)
                        {
                            for (Int16 i = -1; i > -6; i--)
                            {
                               try
                               {
                                    QJZNdt = DateTime.Now.AddDays(i);
                                    QJZNsc = 8;
                                    Thread th2 = new Thread(new ThreadStart(QJZNRK));
                                    th2.Start();
                                    Thread.Sleep(100);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        this.t1.Dispatcher.Invoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                        t1.CaretIndex = (t1.Text.Length);
                                                    }
                                                ));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
                    }
                }
                else if ((DateTime.Now.Hour == QJZNHH20 || DateTime.Now.AddHours(-1).Hour == QJZNHH20 || DateTime.Now.AddHours(-2).Hour == QJZNHH20) && DateTime.Now.Minute == QJZNMM20)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        QJZNdt = DateTime.Now;
                        QJZNsc = 20;
                        Thread th1 = new Thread(new ThreadStart(QJZNRK));
                        th1.Start();
                        Thread.Sleep(100);
                        if (DateTime.Now.AddHours(-2).Hour == QJZNHH20)
                        {
                            for (Int16 i = -1; i > -6; i--)
                            {
                              try
                              {
                                    QJZNdt = DateTime.Now.AddDays(i);
                                    QJZNsc = 20;
                                    Thread th2 = new Thread(new ThreadStart(QJZNRK));
                                    th2.Start();
                                    Thread.Sleep(100);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        this.t1.Dispatcher.Invoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                        t1.CaretIndex = (t1.Text.Length);
                                                    }
                                                ));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
                    }
                }
                if ((DateTime.Now.Hour == GJZNHH08 || DateTime.Now.AddHours(-1).Hour == GJZNHH08 || DateTime.Now.AddHours(-2).Hour == GJZNHH08) && DateTime.Now.Minute == GJZNMM08)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        GJZNdt = DateTime.Now;
                        GJZNsc = 8;
                        Thread th1 = new Thread(new ThreadStart(GJZNRK));
                        th1.Start();
                        Thread.Sleep(100);
                        if (DateTime.Now.AddHours(-2).Hour == GJZNHH08)
                        {
                            for (Int16 i = -1; i > -6; i--)
                            {
                                try
                                {
                                    GJZNdt = DateTime.Now.AddDays(i);
                                    GJZNsc = 8;
                                    Thread th2 = new Thread(new ThreadStart(GJZNRK));
                                    th2.Start();
                                    Thread.Sleep(100);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        this.t1.Dispatcher.Invoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                        t1.CaretIndex = (t1.Text.Length);
                                                    }
                                                ));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
                    }
                }
                else if ((DateTime.Now.Hour == GJZNHH20 || DateTime.Now.AddHours(-1).Hour == GJZNHH20 || DateTime.Now.AddHours(-2).Hour == GJZNHH20) && DateTime.Now.Minute == GJZNMM20)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        GJZNdt = DateTime.Now;
                        GJZNsc = 20;
                        Thread th1 = new Thread(new ThreadStart(GJZNRK));
                        th1.Start();
                        Thread.Sleep(100);
                        if (DateTime.Now.AddHours(-2).Hour == GJZNHH20)
                        {
                            for (Int16 i = -1; i > -6; i--)
                            {
                               try
                               {
                                    GJZNdt = DateTime.Now.AddDays(i);
                                    GJZNsc = 20;
                                    Thread th2 = new Thread(new ThreadStart(GJZNRK));
                                    th2.Start();
                                    Thread.Sleep(100);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        this.t1.Dispatcher.Invoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                        t1.CaretIndex = (t1.Text.Length);
                                                    }
                                                ));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
                    }
                }
                if ((DateTime.Now.Hour == ECHH08 || DateTime.Now.AddHours(-1).Hour == ECHH08 || DateTime.Now.AddHours(-2).Hour == ECHH08) && DateTime.Now.Minute == ECMM08)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        ECdt = DateTime.Now;
                        ECsc = 8;
                        Thread th1 = new Thread(new ThreadStart(ECRK));
                        th1.Start();
                        Thread.Sleep(1000);
                        if (DateTime.Now.AddHours(-2).Hour == ECHH08)
                        {
                            for (Int16 i = -1; i > -6; i--)
                            {
                               try
                               {
                                    ECdt = DateTime.Now.AddDays(i);
                                    ECsc = 8;
                                    Thread th2 = new Thread(new ThreadStart(ECRK));
                                    th2.Start();
                                    Thread.Sleep(1000);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        this.t1.Dispatcher.Invoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                        t1.CaretIndex = (t1.Text.Length);
                                                    }
                                                ));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
                    }
                }
                else if ((DateTime.Now.Hour == ECHH20 || DateTime.Now.AddHours(-1).Hour == ECHH20 || DateTime.Now.AddHours(-2).Hour == ECHH20) && DateTime.Now.Minute == ECMM20)
                {
                    try
                    {
                        //指定备份时间未来三个小时都对当天的预报进行入库，对过去5天的预报进行重新入库
                        ECdt = DateTime.Now.AddDays(-1);
                        ECsc = 20;
                        Thread th1 = new Thread(new ThreadStart(ECRK));
                        th1.Start();
                        Thread.Sleep(1000);
                        if (DateTime.Now.AddHours(-2).Hour == ECHH20)
                        {
                            for (Int16 i = -1; i > -6; i--)
                            {
                                try
                                {
                                    ECdt = DateTime.Now.AddDays(i);
                                    ECsc = 20;
                                    Thread th2 = new Thread(new ThreadStart(ECRK));
                                    th2.Start();
                                    Thread.Sleep(1000);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        this.t1.Dispatcher.Invoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                                        t1.CaretIndex = (t1.Text.Length);
                                                    }
                                                ));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
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
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
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
                            this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                        }
                        catch { }
                    }
                }

                if (DateTime.Now.Minute == 10)//每小时的10分入库实况小时数据
                {
                    Thread thread = new Thread(HourSKRK);
                    thread.Start();
                }
            }
            catch(Exception ex)
            {
                try
                {
                    this.t1.Dispatcher.Invoke(
                            new Action(
                                delegate
                                {
                                    t1.AppendText(ex.Message + "\r\n");
                                    //将光标移至文本框最后
                                    t1.Focus();
                                    t1.CaretIndex = (t1.Text.Length);
                                }
                            ));
                }
                catch { }
            }
        }

        private void QJZNRK()
        {
            try
            {
                int count = 0;
                智能网格类1 c1 = new 智能网格类1();
                string strtime = QJZNdt.ToString("yyyyMMdd");
                string error = c1.YBRK(strtime, QJZNsc,ref count);
                if (error.Trim().Length == 0)
                {
                    if (count != 0)
                    {
                        error = DateTime.Now.ToString() + "保存" + strtime + QJZNsc.ToString().PadLeft(2, '0') + "时区局智能网格数据成功！\r\n";
                    }
                    

                }
                else
                {
                    error = DateTime.Now.ToString() + "保存" + strtime + QJZNsc.ToString().PadLeft(2, '0') + "时区局智能网格数据：\r\n" + error;
                }

                this.t1.Dispatcher.Invoke(
                    new Action(
                        delegate
                        {
                            t1.AppendText(error);
                            //将光标移至文本框最后
                            t1.Focus();
                            t1.CaretIndex = (t1.Text.Length);
                        }
                    ));
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
                        error = DateTime.Now.ToString() + "保存" + strtime + GJZNsc.ToString().PadLeft(2, '0') + "时国家级智能网格数据成功！\r\n";
                    }


                }
                else
                {
                    error = DateTime.Now.ToString() + "保存" + strtime + GJZNsc.ToString().PadLeft(2, '0') + "时国家级智能网格数据：\r\n" + error;
                }

                this.t1.Dispatcher.Invoke(
                    new Action(
                        delegate
                        {
                            t1.AppendText(error);
                            //将光标移至文本框最后
                            t1.Focus();
                            t1.CaretIndex = (t1.Text.Length);
                        }
                    ));
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

                string error = c1.ECYBRK(strtime,ECsc, ref count);
                if (error.Trim().Length == 0)
                {
                    if (count != 0)
                    {
                        error = DateTime.Now.ToString() + "保存" + strtime + ECsc.ToString().PadLeft(2, '0') + "时EC数据成功！\r\n";
                    }


                }
                else
                {
                    error = DateTime.Now.ToString() + "保存" + strtime + ECsc.ToString().PadLeft(2, '0') + "时EC数据：\r\n" + error;
                }

                this.t1.Dispatcher.Invoke(
                    new Action(
                        delegate
                        {
                            t1.AppendText(error);
                            //将光标移至文本框最后
                            t1.Focus();
                            t1.CaretIndex = (t1.Text.Length);
                        }
                    ));
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
                bool insertBS= classSZYB.CLYB(YBdt, YBsc, ref error);
                if (error.Trim().Length == 0 && insertBS)
                {
                    error = DateTime.Now.ToString() + "保存" + YBdt.ToString("yyyyMMdd") +YBsc+ "时数值预报成功！\r\n";
                    SaveJL(error);
                }
                else if (insertBS)
                {
                    error = DateTime.Now.ToString() + "保存" + YBdt.ToString("yyyyMMdd") + YBsc + "时数值预报：！\r\n" + error;
                }
                if (error.Trim().Length > 0 || insertBS)
                {
                    this.t1.Dispatcher.Invoke(
                      new Action(
                          delegate
                          {
                              t1.AppendText(error);
                                  //将光标移至文本框最后
                                  t1.Focus();
                              t1.CaretIndex = (t1.Text.Length);
                          }
                      ));
                    SaveJL(error);
                }

                if (DateTime.Now.AddHours(-2).Hour == YBHH08|| DateTime.Now.AddHours(-2).Hour == YBHH20)
                {
                    for (Int16 i = -1; i > -3; i--)
                    {
                       try
                       {
                            insertBS = false;
                            error = "";
                            insertBS = classSZYB.CLYB(YBdt.AddDays(i), YBsc, ref error);
                            if (error.Trim().Length == 0 && insertBS)
                            {
                                error = DateTime.Now.ToString() + "保存" + YBdt.ToString("yyyyMMdd") + YBsc + "时数值预报成功！\r\n";
                                SaveJL(error);
                            }
                            else if (insertBS)
                            {
                                error = DateTime.Now.ToString() + "保存" + YBdt.ToString("yyyyMMdd") + YBsc + "时数值预报：！\r\n" + error;
                            }
                            if (error.Trim().Length > 0 || insertBS)
                            {
                                this.t1.Dispatcher.Invoke(
                                  new Action(
                                      delegate
                                      {
                                          t1.AppendText(error);
                                      //将光标移至文本框最后
                                      t1.Focus();
                                          t1.CaretIndex = (t1.Text.Length);
                                      }
                                  ));
                                SaveJL(error);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch(Exception ex)
            {
                SaveJL(ex.Message+"\r\n");
            }

        }
    }

    
}

