using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static 呼和浩特市精细化天气预报评分系统.登录记录查询页;

namespace 呼和浩特市精细化天气预报评分系统
{
    /// <summary>
    /// 登录窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 登录窗口 : Window
    {
        bool SQLSuc = false;
        ObservableCollection<people> peopleList = new ObservableCollection<people>();
        int intCount = 0;//记录当前旗县人员个数
        string con;//这里是保存连接数据库的字符串
        string GWconfigPath = System.Environment.CurrentDirectory + @"\config\GWList.txt";
        string DQGW = "";
        string basePath = System.Environment.CurrentDirectory + @"\config\user";
        ObservableCollection<ZBJL> zbjl = new ObservableCollection<ZBJL>();

        public 登录窗口()
        {
            InitializeComponent();
            CSH();
            
            HQUserID(ref SQLSuc);
            /*if (SQLSuc)
            {
                HQDL();
            }*/


        }
        //进行初始化设置
        public void CSH()
        {
            ((DataGrid)(this.FindName("ZBList"))).ItemsSource = zbjl;
            sDate.BlackoutDates.Add(new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue));//开始时间不可选的范围，当前日期以后
            sDate.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue,DateTime.Now.Date.AddDays(-2)));//开始时间不可选的范围，当前日期以后
            sDate.SelectedDate = DateTime.Now;
            try
            {
                using (StreamReader sr = new StreamReader(GWconfigPath, Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "当前岗位")
                            DQGW = line.Split('=')[1];
                    }
                }

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
                int gwchooseCount = 0;
                Dictionary<int, string> mydic = new Dictionary<int, string>();
                using (StreamReader sr = new StreamReader(GWconfigPath, Encoding.Default))
                {
                    string line = "";
                    intCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 0 && line.Split('=')[0] == "岗位列表")
                        {
                            string[] szls = line.Split('=')[1].Split(',');
                            foreach (string ssls in szls)
                            {
                                if (ssls == DQGW)
                                    gwchooseCount = intCount;
                                mydic.Add(intCount++, ssls);

                            }
                        }
                    }
                    gwchoose.ItemsSource = mydic;
                    gwchoose.SelectedValuePath = "Key";
                    gwchoose.DisplayMemberPath = "Value";
                }
                gwchoose.SelectedValue = gwchooseCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            gwchoose.SelectionChanged += gwchoose_SelectionChanged;
            DateTime dtls = DateTime.Now;
            if (dtls.Hour > 10)
            {
                scchoose.SelectedIndex = 1;
            }
            else
            {
                scchoose.SelectedIndex = 0;
            }
            try
            {
                Int16 yearInt = 2018;
                Int16 yeartd = 2018;
                Int16 monthtd = 0;
                using (SqlConnection mycon1 = new SqlConnection(con))
                {
                    try
                    {
                        mycon1.Open();
                        string sql1 = string.Format(@"select getdate()");
                        SqlCommand sqlCommand1 = new SqlCommand(sql1, mycon1);
                        SqlDataReader sqlDataReader1 = sqlCommand1.ExecuteReader();
                        DateTime dt1;
                        if (sqlDataReader1.Read())
                        {
                            dt1 = (DateTime)sqlDataReader1[0];
                            mycon1.Close();
                            yeartd = Convert.ToInt16(dt1.ToString("yyyy"));
                            monthtd = Convert.ToInt16(dt1.ToString("MM"));
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                if (yeartd >= yearInt && monthtd > 0)
                {
                    Dictionary<int, string> mydic = new Dictionary<int, string>();
                    monthchoose.SelectedIndex = monthtd - 1;
                    Int16 intCount = 0;
                    for (; yearInt <= yeartd; yearInt++)
                    {
                        mydic.Add(intCount++, yearInt.ToString());
                    }
                    yearchoose.ItemsSource = mydic;
                    yearchoose.SelectedValuePath = "Key";
                    yearchoose.DisplayMemberPath = "Value";
                    yearchoose.SelectedIndex = intCount - 1;
                    CXzb();
                }
                else
                {
                    MessageBox.Show("时间信息获取错误，请检查与数据库连接是否正常，服务器时间是否正常");
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void HQUserID(ref bool SQLSuc)
        {
            string idPath = System.Environment.CurrentDirectory + @"\config\user\" + DQGW + ".txt";
            string ss = "";
            SQLSuc = true;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from USERID where GW='{0}'", DQGW);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("Name")) + '=';
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + '=';
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("Password")) + '=';
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("admin")) + '\n';
                    }
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    using (FileStream fs = new FileStream(idPath, FileMode.Create))
                    {
                        StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                        sw.Write(ss);
                        sw.Flush();
                        sw.Close();
                    }

                }
                catch (Exception ex)
                {
                    SQLSuc = false;
                    MessageBox.Show(ex.Message + "\n如果错误为数据库连接失败将连接本地人员名单");
                }
            }
            Dictionary<int, string> mydic = new Dictionary<int, string>();
            using (StreamReader sr = new StreamReader(idPath, Encoding.Default))
            {
                string line = "";
                intCount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        string[] szls = line.Split('=');
                        mydic.Add(intCount++, szls[0]+ ' '+szls[1]);
                    }
                }
                userchoose.ItemsSource = mydic;
                userchoose.SelectedValuePath = "Key";
                userchoose.DisplayMemberPath = "Value";
            }
            userchoose.SelectedValue = 0;
        }

        private void DL_Click(object sender, RoutedEventArgs e)
        {
            DQGW = gwchoose.Text;
            //自动保存当前岗位选择信息
            string configString = "";
            bool bsLS = false;
            using (StreamReader sr = new StreamReader(GWconfigPath, Encoding.Default))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "当前岗位"&& line.Split('=')[0]!= DQGW)
                    {
                        bsLS = true;
                        configString += "当前岗位=" + DQGW+ "\r\n";
                    }
                    else
                        configString += line + "\r\n";

                }
                configString = configString.Substring(0, configString.Length - 2);
                sr.Close();
                using (FileStream fs = new FileStream(GWconfigPath, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.Write(configString);
                    sw.Flush();
                    sw.Close();
                }

            }


            string idPath = System.Environment.CurrentDirectory + @"\config\user\" + DQGW + ".txt";
            using (StreamReader sr = new StreamReader(idPath, Encoding.Default))
            {
                int i = 0;
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        string[] xxsz = line.Split('=');//0姓名  1ID  2 密码  3管理权限
                        if (xxsz[0]== userchoose.Text.Split()[0]&& xxsz[1] == userchoose.Text.Split()[1])
                        {
                            string passStr = xxsz[2];
                            if (passStr == passWord.Password)
                            {
                                using (SqlConnection mycon = new SqlConnection(con))
                                {
                                    int jlCount = 0;
                                    try
                                    {

                                        mycon.Open();//打开
                                        string sql = string.Format(@"insert into USERJL values('{0}','{1:yyyy-MM-dd}','{2}','{3}','{4}')", xxsz[1], Convert.ToDateTime(sDate.SelectedDate), DQGW, xxsz[0], scchoose.Text);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                                        jlCount = sqlman.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    if (jlCount == 0)//如果插入失败，则说明已经存在，进行更新字段操作
                                    {
                                        try
                                        {
                                            string sql = string.Format(@"update USERJL set userID='{0}' ,Name='{1}' where date='{2}' and GW='{3}'and SC='{4}'", xxsz[1], xxsz[0], Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd"), DQGW,scchoose.Text);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                                            SqlCommand sqlman = new SqlCommand(sql, mycon);
                                            jlCount = sqlman.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message);
                                        }
                                        if (jlCount == 0)//如果更新字段失败，说明连接数据库失败，则保存至登陆信息至发报文件夹
                                        {
                                            string pathconfig = System.Environment.CurrentDirectory + @"\config\pathConfig.txt";
                                            using (StreamReader sr1 = new StreamReader(pathconfig, Encoding.Default))
                                            {
                                                string line1;
                                                string pathLS = "";

                                                // 从文件读取并显示行，直到文件的末尾 
                                                while ((line1 = sr1.ReadLine()) != null)
                                                {
                                                    if (line1.Split('=')[0] == "登陆信息保存路径")
                                                    {
                                                        pathLS = line1.Split('=')[1];
                                                    }
                                                }
                                                pathLS += DQGW+ Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd")+scchoose.Text+"时登陆.txt";
                                                
                                                    string ss = "";
                                                    ss += Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd")+'=' +scchoose.Text+ '=' + DQGW + '=' + xxsz[1]+ '='+xxsz[0] + '\n';
                                                try
                                                {
                                                    using (FileStream fs = new FileStream(pathLS, FileMode.Create))
                                                    {
                                                        StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                                                        sw.Write(ss);
                                                        sw.Flush();
                                                        sw.Close();
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show(pathLS + "保存失败，将保存至本地");
                                                    pathLS = Environment.CurrentDirectory + @"\本地登陆记录\";
                                                    if (!Directory.Exists(pathLS))
                                                        Directory.CreateDirectory(pathLS);
                                                    pathLS += DQGW + Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd") + scchoose.Text + "时登陆.txt";
                                                    try
                                                    {
                                                        using (FileStream fs = new FileStream(pathLS, FileMode.Create))
                                                        {
                                                            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                                                            sw.Write(ss);
                                                            sw.Flush();
                                                            sw.Close();
                                                        }
                                                        MessageBox.Show("成功将登陆信息保存至"+pathLS);
                                                    }
                                                    catch (Exception e1)
                                                    {
                                                        MessageBox.Show(e1.Message);
                                                    }
                                                }
                                                

                                            }
                                        }

                                    }
                                }
                                ZTXS.Text = gwchoose.Text+ "登陆成功!";
                                CXzb();


                            }
                            else
                            {
                                MessageBox.Show("密码错误，请重新输入");
                            }
                            break;
                        }
                    }
                }

            }
            

        }

        private void HQDL()
        {
            peopleList.Clear();
            string ss = "";
            DateTime d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);//本月第一天
            string sd1 = d1.ToString("yyyy-MM-dd");
            string sd2 = DateTime.Now.ToString("yyyy-MM-dd");
            using (SqlConnection mycon = new SqlConnection(con))
            {

                mycon.Open();//打开
                try
                {
                    string sql = string.Format(@"select * from USERJL where date between '{0}' AND '{1}' order by date,SC",  sd1, sd2);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        peopleList.Add(new people()
                        {
                            DLdate = sqlreader.GetDateTime(sqlreader.GetOrdinal("date")).ToString("yyyy年MM月dd日"),
                            GW = sqlreader.GetString(sqlreader.GetOrdinal("GW")),
                            SC  = sqlreader.GetString(sqlreader.GetOrdinal("SC")),
                            userName = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                        });
                    }
                    ((DataGrid) (this.FindName("History"))).ItemsSource = peopleList;
                    

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void OpenFB()
        {
            string pathConfig = System.Environment.CurrentDirectory + @"\config\pathConfig.txt";
            string YBpath = "";
            using (StreamReader sr1 = new StreamReader(pathConfig, Encoding.Default))
            {
                string line1 = "";
                while ((line1 = sr1.ReadLine()) != null)
                {
                    if (line1.Split('=')[0] == DQGW+"发报软件路径")
                    {
                        YBpath = line1.Split('=')[1];
                    }
                }

            }
            if (File.Exists(YBpath))
            {
                string[] szLS = YBpath.Split('\\');
                string strML = YBpath.Substring(0, YBpath.Length - szLS[szLS.Length - 1].Length);
                Process pr = new Process();//声明一个进程类对象
                pr.StartInfo.WorkingDirectory = strML;
                pr.StartInfo.FileName = YBpath;//指定运行的程序
                pr.Start();//运行
            }
            else
            {
                MessageBox.Show("发报软件路径有误，请设置发报软件路径");
                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "可执行文件|*"
                };
                var result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    using (FileStream fs = new FileStream(pathConfig, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                        {
                            sw.Write("发报软件路径=" + openFileDialog.FileName);
                            sw.Flush();
                        }
                    }
                    using (StreamReader sr1 = new StreamReader(pathConfig, Encoding.Default))
                    {
                        string line1 = "";
                        while ((line1 = sr1.ReadLine()) != null)
                        {
                            if (line1.Split('=')[0] == "发报软件路径")
                            {
                                YBpath = line1.Split('=')[1];
                            }
                        }

                    }
                    string[] szLS = YBpath.Split('\\');
                    string strML = YBpath.Substring(0, YBpath.Length - szLS[szLS.Length - 1].Length);
                    Process pr = new Process();//声明一个进程类对象
                    pr.StartInfo.WorkingDirectory = strML;
                    pr.StartInfo.FileName = YBpath;//指定运行的程序
                    pr.Start();//运行
                }
            }
        }

        private void update_Click(object sender, RoutedEventArgs e)
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
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }



        public class people
        {
            public string DLdate { get; set; }
            public string GW { get; set; }
            public string SC { get; set; }
            public string userName { get; set; }
        }

        private void gwchoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DQGW = gwchoose.SelectedItem.ToString();
            DQGW = DQGW.Split(',')[1].Trim().TrimEnd(']');
            HQUserID(ref SQLSuc);
        }

        private void SJYBTBBtu_Click(object sender, RoutedEventArgs e)
        {
            string BWBS = "";
            string pathConfig = Environment.CurrentDirectory + @"\config\pathConfig.txt";
            string BSConfig = Environment.CurrentDirectory + @"\config\报文标识.txt";
            string qjPath = "", sjPath = "";
            using (StreamReader sr = new StreamReader(pathConfig, Encoding.Default))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "市台指导区局路径")
                    {
                        qjPath = line.Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "市台预报市局路径")
                    {
                        sjPath = line.Split('=')[1];
                    }
                }
            }
            using (StreamReader sr = new StreamReader(BSConfig, Encoding.Default))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "呼市气象台")
                    {
                        BWBS = line.Split('=')[1];
                    }
                    break;
                }
            }
            if (qjPath.Length > 0 && qjPath.Length > 0&&BWBS.Length>0)
            {
                if (scchoose.Text == "08")
                {
                    qjPath += Convert.ToDateTime(sDate.SelectedDate).AddDays(-1). ToString("yyyyMMdd") + "\\";
                    string strParPath = "*" + BWBS + "_" + Convert.ToDateTime(sDate.SelectedDate).AddDays(-1).ToString("yyyyMMdd2245") + "*-SPCC-" + Convert.ToDateTime(sDate.SelectedDate).ToString("yyyyMMdd") + "00" + "*";
                    string[] fileNameList = Directory.GetFiles(qjPath, strParPath);
                    int intCount = 0;//记录该报文中的站点数
                    Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                    for (Int16 j = 0; j < fileNameList.Length; j++)
                    {
                        string strLS = fileNameList[j].Split('_')[4];
                        intLS = Convert.ToInt16(strLS.Substring(strLS.Length - 2));
                        if (j == 0)
                        {
                            maxLS = intLS;
                            minLS = intLS;
                        }
                        else
                        {
                            if (intLS > maxLS)
                            {
                                maxLS = intLS;
                                maxXH = j;
                            }
                            if (intLS < minLS)
                            {
                                minLS = intLS;
                                minXH = j;
                            }
                        }
                    }
                    try
                    {
                        sjPath += System.IO.Path.GetFileName(fileNameList[maxXH]);
                        File.Copy(fileNameList[maxXH], sjPath, true);
                        ZTXS.Text = sjPath + "同步成功";
                    }
                    catch(Exception ex)
                    {
                        ZTXS.Text =  "同步失败："+ex.Message;
                    }
                }
                else
                {
                    qjPath += Convert.ToDateTime(sDate.SelectedDate).ToString("yyyyMMdd") + "\\";
                    string strParPath = "*" + BWBS + "_" + Convert.ToDateTime(sDate.SelectedDate).ToString("yyyyMMdd0830") + "*-SPCC-" + Convert.ToDateTime(sDate.SelectedDate).ToString("yyyyMMdd") + "12" + "*";
                    string[] fileNameList = Directory.GetFiles(qjPath, strParPath);
                    int intCount = 0;//记录该报文中的站点数
                    Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                    for (Int16 j = 0; j < fileNameList.Length; j++)
                    {
                        string strLS = fileNameList[j].Split('_')[4];
                        intLS = Convert.ToInt16(strLS.Substring(strLS.Length - 2));
                        if (j == 0)
                        {
                            maxLS = intLS;
                            minLS = intLS;
                        }
                        else
                        {
                            if (intLS > maxLS)
                            {
                                maxLS = intLS;
                                maxXH = j;
                            }
                            if (intLS < minLS)
                            {
                                minLS = intLS;
                                minXH = j;
                            }
                        }
                    }
                    try
                    {
                        sjPath += System.IO.Path.GetFileName(fileNameList[maxXH]);
                        File.Copy(fileNameList[maxXH], sjPath, true);
                        ZTXS.Text = sjPath + "同步成功";
                    }
                    catch (Exception ex)
                    {
                        ZTXS.Text = "同步失败：" + ex.Message;
                    }
                }
            }
        }

        private void FBBtu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes ==
                                  MessageBox.Show("是否更改报文。\r\n如果选是，则自动进入发报软件。\r\n如果选否，则直接生成报文。", "请选择",
                                      MessageBoxButton.YesNo))
            {
                OpenFB();
            }
            else
            {
                string GWBwbs = "";
                string BWBS = "";
                string BSPath = Environment.CurrentDirectory + @"\config\报文标识.txt";
                using (StreamReader sr2 = new StreamReader(BSPath, Encoding.Default))
                {
                    string line2 = "";
                    while ((line2 = sr2.ReadLine()) != null)
                    {
                        if (line2.Split('=')[0] == "呼市气象台")
                        {
                            BWBS = line2.Split('=')[1];

                        }
                        else if (line2.Split('=')[0] == gwchoose.Text)
                        {
                            GWBwbs = line2.Split('=')[1];
                        }
                    }
                }
                string pathConfig = Environment.CurrentDirectory + @"\config\pathConfig.txt";
                string BWPath = "";
                using (StreamReader sr2 = new StreamReader(pathConfig, Encoding.Default))
                {
                    string line2 = "";
                    while ((line2 = sr2.ReadLine()) != null)
                    {
                        if (line2.Split('=')[0] == "市台预报市局路径")
                        {
                            BWPath = line2.Split('=')[1];
                        }
                    }
                }
                if (scchoose.Text == "08")
                {
                    string strParPath = "*" + BWBS + "*-SPCC-" + Convert.ToDateTime(sDate.SelectedDate).ToString("yyyyMMdd") + "00" + "*";
                    string[] fileNameList = Directory.GetFiles(BWPath, strParPath);
                    if (fileNameList.Length == 0)
                    {

                        MessageBox.Show(Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy年MM月dd日") + "08时市台预报不存在，请同步");
                        return;
                    }
                    else
                    {
                        int intCount = 0;//记录该报文中的站点数
                        Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                        Int64 maxLS = 0, minLS = 0, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                        for (Int16 j = 0; j < fileNameList.Length; j++)
                        {
                            string strLS = fileNameList[j].Split('_')[4];
                            intLS = Convert.ToInt64(strLS);
                            if (j == 0)
                            {
                                maxLS = intLS;
                                minLS = intLS;
                            }
                            else
                            {
                                if (intLS > maxLS)
                                {
                                    maxLS = intLS;
                                    maxXH = j;
                                }
                                if (intLS < minLS)
                                {
                                    minLS = intLS;
                                    minXH = j;
                                }
                            }
                        }

                        string gwbwpath = fileNameList[maxXH].Replace(BWBS, GWBwbs);
                        File.Copy(fileNameList[maxXH], gwbwpath, true);

                    }
                }
                else if (scchoose.Text == "20")
                {
                    string strParPath = "*" + BWBS + "*-SPCC-" + Convert.ToDateTime(sDate.SelectedDate).ToString("yyyyMMdd") + "12" + "*";
                    string[] fileNameList = Directory.GetFiles(BWPath, strParPath);
                    if (fileNameList.Length == 0)
                    {

                        MessageBox.Show(Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy年MM月dd日") + "20时市台预报不存在，请同步");
                        return;
                    }
                    else
                    {
                        int intCount = 0;//记录该报文中的站点数
                        Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                        Int64 maxLS = 0, minLS = 0, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                        for (Int16 j = 0; j < fileNameList.Length; j++)
                        {
                            string strLS = fileNameList[j].Split('_')[4];
                            intLS = Convert.ToInt64(strLS);
                            if (j == 0)
                            {
                                maxLS = intLS;
                                minLS = intLS;
                            }
                            else
                            {
                                if (intLS > maxLS)
                                {
                                    maxLS = intLS;
                                    maxXH = j;
                                }
                                if (intLS < minLS)
                                {
                                    minLS = intLS;
                                    minXH = j;
                                }
                            }
                        }

                        string gwbwpath = fileNameList[maxXH].Replace(BWBS, GWBwbs);
                        File.Copy(fileNameList[maxXH], gwbwpath, true);

                    }
                }
                try
                {
                    ZTXS.Text = gwchoose.Text + Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy年MM月dd日") + scchoose.Text + "时报文发送成功!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void CX_Click(object sender, RoutedEventArgs e)
        {
            CXzb();
        }

        public void CXzb()
        {
            ObservableCollection<peopleList> plist = new ObservableCollection<peopleList>();
            zbjl.Clear();
            DateTime dtfwq = DateTime.Now;
            using (SqlConnection mycon1 = new SqlConnection(con))
            {
                try
                {
                    mycon1.Open();
                    string sql1 = string.Format(@"select getdate()");
                    SqlCommand sqlCommand1 = new SqlCommand(sql1, mycon1);
                    SqlDataReader sqlDataReader1 = sqlCommand1.ExecuteReader();
                    DateTime dt1;
                    if (sqlDataReader1.Read())
                    {
                        dtfwq = (DateTime)sqlDataReader1[0];
                        mycon1.Close();
                    }
                }
                catch (Exception)
                {

                }
            }

            DateTime d1 = new DateTime(Convert.ToInt16(yearchoose.Text), Convert.ToInt16(monthchoose.Text), 1);//月第一天
            string sd1 = d1.ToString("yyyy-MM-dd");
            DateTime d2 = dtfwq;
            if (yearchoose.Text == dtfwq.ToString("yyyy") && monthchoose.Text == dtfwq.ToString("MM"))
                d2 = dtfwq;
            else
            {
                d2 = d1.AddDays(1 - d1.Day).AddMonths(1).AddDays(-1);
            }
            string sd2 = d2.ToString("yyyy-MM-dd");
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open();//打开
                try
                {
                    string sql = string.Format(@"select * from USERJL where date between '{0}' AND '{1}' order by date,GW,SC", sd1, sd2);
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {

                        plist.Add(new peopleList()
                        {
                            date = sqlreader.GetDateTime(sqlreader.GetOrdinal("date")).ToString("yyyy年MM月dd日"),
                            gw = sqlreader.GetString(sqlreader.GetOrdinal("GW")),
                            sc = sqlreader.GetString(sqlreader.GetOrdinal("SC")),
                            name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                        });
                    }
                    for (DateTime dtls = d1; DateTime.Compare(dtls, d2) <= 0; dtls = dtls.AddDays(1))
                    {
                        string lb = "", zb08 = "", zb20 = "";
                        var p1 = plist.Where(p => p.date == dtls.ToString("yyyy年MM月dd日"));
                        var p2 = p1.Where(p => p.gw == "领班");
                        foreach (var pp in p2)
                            lb = pp.name;
                        var p3 = p1.Where(p => p.gw == "主班").Where(p => p.sc == "08");
                        foreach (var pp in p3)
                            zb08 = pp.name;
                        p3 = p1.Where(p => p.gw == "主班").Where(p => p.sc == "20");
                        foreach (var pp in p3)
                            zb20 = pp.name;
                        zbjl.Add(new ZBJL()
                        {
                            RQ = dtls.ToString("yyyy年MM月dd日"),
                            LB = lb,
                            ZB08 = zb08,
                            ZB20 = zb20
                        });
                        ZBList.ScrollIntoView(zbjl.Last());
                    }



                }
                catch (Exception e1)
                {

                }
            }
        }
    }
}
