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

namespace 呼和浩特市精细化天气预报评分系统
{
    /// <summary>
    /// 中央导入窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 中央导入窗口 : Window
    {
        bool SQLSuc = false;
        string BWBS = "BABJ";//报文标识
        ObservableCollection<people> peopleList = new ObservableCollection<people>();
        string con;//这里是保存连接数据库的字符串
        private string SC = "";
        string pathConfig = System.Environment.CurrentDirectory + @"\config\pathConfig.txt";
        string bdpath = "";//保存本地报文的路径
        Int16 qxCount = 0;

        public 中央导入窗口()
        {
            InitializeComponent();
            DQ10.IsChecked = true;
            sDate.BlackoutDates.Add(new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue));//开始时间不可选的范围，当前日期以后
            sDate.SelectedDate = DateTime.Now;
            DateTime dtls = DateTime.Now;
            if (dtls.Hour > 10)
            {
                scchoose.SelectedIndex = 1;
            }
            else
            {
                scchoose.SelectedIndex = 0;
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
            SFRK(Convert.ToDateTime(sDate.SelectedDate), scchoose.Text);
            RKZTXS.Text = "数据库记录存在";
        }
        //进行初始化设置




        public class people
        {
            public string QXID { get; set; }
            public float GW24 { get; set; }
            public float DW24 { get; set; }
            public string QY24 { get; set; }
            public float GW48 { get; set; }
            public float DW48 { get; set; }
            public string QY48 { get; set; }
            public float GW72 { get; set; }
            public float DW72 { get; set; }
            public string QY72 { get; set; }
            public float GW96 { get; set; }
            public float DW96 { get; set; }
            public string QY96 { get; set; }
            public float GW120 { get; set; }
            public float DW120 { get; set; }
            public string QY120 { get; set; }

        }





        private void DQBDcheck_Checked(object sender, RoutedEventArgs e)
        {
            CIMISScheck.IsChecked = false;
            DQ10.IsChecked = false;

            bdpath = "";
            AllPath.Text = "";
            try
            {
                using (StreamReader sr = new StreamReader(pathConfig, Encoding.Default))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "读取中央指导预报本地路径")
                        {
                            bdpath = line.Split('=')[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (bdpath.Length > 0)
            {
                AllPath.Text = bdpath;
            }
        }

        private void CIMISScheck_Checked(object sender, RoutedEventArgs e)
        {
            DQBDcheck.IsChecked = false;
            DQ10.IsChecked = false;
            AllPath.Text = "";
        }

        private void CX_Click(object sender, RoutedEventArgs e)
        {
            if (sDate.SelectedDate.ToString().Length != 0)
            {
                SC = scchoose.Text;
                AllPath.Text = "";
                string ss = "";
                //数据库读取旗县列表
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    try
                    {
                        mycon.Open();//打开
                        string sql = string.Format(@"select * from StationList");  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        SqlDataReader sqlreader = sqlman.ExecuteReader();
                        while (sqlreader.Read())
                        {
                            ss += sqlreader.GetString(sqlreader.GetOrdinal("Name")) + '=';
                            ss += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + '=';
                            ss += sqlreader.GetString(sqlreader.GetOrdinal("FromID")) + '\n';
                        }

                    }
                    catch (Exception ex)
                    {
                        SQLSuc = false;
                        MessageBox.Show(ex.Message);
                    }
                }
                ss = ss.Substring(0, ss.Length - 1);
                string[] szls = ss.Split('\n');
                string[,] qxSZ = new string[szls.Length, 3];//保存旗县名称、ID、所属旗县ID（该项目前没用）
                for (int i = 0; i < szls.Length; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        qxSZ[i, j] = szls[i].Split('=')[j];
                    }
                }
                qxCount = (Int16)szls.Length;
                if (DQ10.IsChecked == true || DQBDcheck.IsChecked == true)
                {
                    string strError = "";

                    BWCX(bdpath, Convert.ToDateTime(sDate.SelectedDate), BWBS, scchoose.Text, qxSZ, ref strError);

                }
            }
            else
            {
                MessageBox.Show("请选择日期");
            }

            
        }

        public void BWCX(string path,DateTime dt, string BWBS, string Time,string[,] qxSZ, ref string strError)
        {

            if (Time == "08")//滤掉10时次报文
            {
                string strParPath = "*" + BWBS + "*-SCMOC-" + dt.ToString("yyyyMMdd") + "00" + "*";
                string[] fileNameList = Directory.GetFiles(path, strParPath);
                if (fileNameList.Length == 0)
                {
                    strError += dt.ToString("yyyy年MM月dd日") + "08时预报文件不存在\n";
                    MessageBox.Show(dt.ToString("yyyy年MM月dd日") + "08时预报文件不存在");
                    return;
                }
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
                AllPath.Text = fileNameList[maxXH];
                BWTimeXS.Text = File.GetLastWriteTime(fileNameList[maxXH]).ToString("yyyy-MM-dd HH:mm:ss");
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    string line = "";
                    int lineCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (lineCount == 4)
                        {
                            intCount = Convert.ToInt32(line);
                            break;
                        }
                        lineCount++;
                    }
                }

                float[] Tmax24 = new float[qxCount], Tmin24 = new float[qxCount], Tmax48 = new float[qxCount], Tmax72 = new float[qxCount], Tmin48 = new float[qxCount], Tmin72 = new float[qxCount], Tmax96 = new float[qxCount], Tmin96 = new float[qxCount], Tmax120 = new float[qxCount], Tmin120 = new float[qxCount];
                string[] StationID = new string[qxCount], Rain24 = new string[qxCount], Rain48 = new string[qxCount], Rain72 = new string[qxCount], Rain96 = new string[qxCount], Rain120 = new string[qxCount];
                string[] FX24 = new string[qxCount], FS24 = new string[qxCount], FX48 = new string[qxCount], FS48 = new string[qxCount], FX72 = new string[qxCount], FS72 = new string[qxCount], FX96 = new string[qxCount], FS96 = new string[qxCount], FX120 = new string[qxCount], FS120 = new string[qxCount];
                string WeatherDZ = System.Environment.CurrentDirectory + @"\config\天气对照.txt";
                float WeatherLS = 0, FXLS = 0, FSLS = 0;//保存天气、风向、风速的编码临时信息，为了判断前12小时和后12小时的天气是否一致
                string FXDZ = System.Environment.CurrentDirectory + @"\config\风向对照.txt";
                string FSDZ = System.Environment.CurrentDirectory + @"\config\风速对照.txt";
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    bool bsls1 = false;
                    string line = "";
                    int lineCount = 0;
                    int k = 0;
                    Int16 xh = 0;
                    while (((line = sr.ReadLine()) != null) && k < intCount)//k代表乡镇的序号
                    {
                        string[] szLS = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (lineCount == (29 * k + 5))
                        {
                            k++;
                            string id= line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                            
                            for (int i = 0; i < qxCount; i++)
                            {
                                if (qxSZ[i, 1] == id)
                                {
                                    bsls1 = true;
                                    StationID[xh] = id;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k-1) + 9)&& bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 13) && bsls1 == true)
                        {
                            Tmax24[xh] = Convert.ToSingle(szLS[11]);
                            Tmin24[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);

                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain24[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX24[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS24[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }

                        else if (lineCount == (29 * (k - 1) + 17) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 21) && bsls1 == true)
                        {
                            Tmax48[xh] = Convert.ToSingle(szLS[11]);
                            Tmin48[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain48[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX48[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS48[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 23) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 25) && bsls1 == true)
                        {
                            
                            
                            Tmax72[xh] = Convert.ToSingle(szLS[11]);
                            Tmin72[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain72[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX72[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS72[xh] = LS1 + "转" + LS2;
                                }
                            }

                            
                        }
                        else if (lineCount == (29 * (k - 1) + 26) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 27) && bsls1 == true)
                        {
                            Tmax96[xh] = Convert.ToSingle(szLS[11]);
                            Tmin96[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain96[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX96[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS96[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 28) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 29) && bsls1 == true)
                        {
                            Tmax120[xh] = Convert.ToSingle(szLS[11]);
                            Tmin120[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            xh++;
                            bsls1 = false;
                            if (xh >= qxCount)
                                break;
                        }
                        lineCount++;
                    }
                }
                peopleList.Clear();
                for (int i = 0; i < qxCount; i++)
                {
                    if (StationID[i] != "53463")
                    {
                        peopleList.Add(new people()
                        {
                            QXID = StationID[i],
                            GW24 = Tmax24[i],
                            DW24 = Tmin24[i],
                            QY24 = Rain24[i],
                            GW48 = Tmax48[i],
                            DW48 = Tmin48[i],
                            QY48 = Rain48[i],
                            GW72 = Tmax72[i],
                            DW72 = Tmin72[i],
                            QY72 = Rain72[i],
                            GW96=Tmax96[i],
                            DW96 = Tmin96[i],
                            QY96 = Rain96[i],
                            GW120 = Tmax120[i],
                            DW120 = Tmin120[i],
                            QY120 = Rain120[i],

                        });
                    }
                }
                for (int i = 0; i < qxCount; i++)
                {
                    if (StationID[i] == "53463")
                    {
                        peopleList.Add(new people()
                        {
                            QXID = StationID[i],
                            GW24 = Tmax24[i],
                            DW24 = Tmin24[i],
                            QY24 = Rain24[i],
                            GW48 = Tmax48[i],
                            DW48 = Tmin48[i],
                            QY48 = Rain48[i],
                            GW72 = Tmax72[i],
                            DW72 = Tmin72[i],
                            QY72 = Rain72[i],
                            GW96 = Tmax96[i],
                            DW96 = Tmin96[i],
                            QY96 = Rain96[i],
                            GW120 = Tmax120[i],
                            DW120 = Tmin120[i],
                            QY120 = Rain120[i],

                        });
                        break;
                    }
                }
                ((DataGrid) (this.FindName("History"))).ItemsSource=null;
                ((DataGrid)(this.FindName("History"))).ItemsSource = peopleList;
            }
            else
            {
                string strParPath = "*" + BWBS + "*-SCMOC-" + dt.ToString("yyyyMMdd") + "12" + "*";
                string[] fileNameList = Directory.GetFiles(path, strParPath);
                if (fileNameList.Length == 0)
                {
                    strError += dt.ToString("yyyy年MM月dd日") + "20时预报文件不存在\n";
                    MessageBox.Show(dt.ToString("yyyy年MM月dd日") + "20时预报文件不存在");
                    return;
                }
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
                AllPath.Text = fileNameList[maxXH];
                BWTimeXS.Text = File.GetLastWriteTime(AllPath.Text).ToString("yyyy-MM-dd HH:mm:ss");
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    string line = "";
                    int lineCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (lineCount == 4)
                        {
                            intCount = Convert.ToInt32(line);
                            break;
                        }
                        lineCount++;
                    }
                }

                float[] Tmax24 = new float[qxCount], Tmin24 = new float[qxCount], Tmax48 = new float[qxCount], Tmax72 = new float[qxCount], Tmin48 = new float[qxCount], Tmin72 = new float[qxCount], Tmax96 = new float[qxCount], Tmin96 = new float[qxCount], Tmax120 = new float[qxCount], Tmin120 = new float[qxCount];
                string[] StationID = new string[qxCount], Rain24 = new string[qxCount], Rain48 = new string[qxCount], Rain72 = new string[qxCount], Rain96 = new string[qxCount], Rain120 = new string[qxCount];
                string[] FX24 = new string[qxCount], FS24 = new string[qxCount], FX48 = new string[qxCount], FS48 = new string[qxCount], FX72 = new string[qxCount], FS72 = new string[qxCount], FX96 = new string[qxCount], FS96 = new string[qxCount], FX120 = new string[qxCount], FS120 = new string[qxCount];
                string WeatherDZ = System.Environment.CurrentDirectory + @"\config\天气对照.txt";
                float WeatherLS = 0, FXLS = 0, FSLS = 0;//保存天气、风向、风速的编码临时信息，为了判断前12小时和后12小时的天气是否一致
                string FXDZ = System.Environment.CurrentDirectory + @"\config\风向对照.txt";
                string FSDZ = System.Environment.CurrentDirectory + @"\config\风速对照.txt";
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    bool bsls1 = false;
                    string line = "";
                    int lineCount = 0;
                    int k = 0;
                    Int16 xh = 0;
                    while (((line = sr.ReadLine()) != null) && k < intCount)//k代表乡镇的序号
                    {
                        string[] szLS = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (lineCount == (29 * k + 5))
                        {
                            k++;
                            string id = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];

                            for (int i = 0; i < qxCount; i++)
                            {
                                if (qxSZ[i, 1] == id)
                                {
                                    bsls1 = true;
                                    StationID[xh] = id;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 9) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 13) && bsls1 == true)
                        {
                            Tmax24[xh] = Convert.ToSingle(szLS[11]);
                            Tmin24[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);

                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain24[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX24[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS24[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }

                        else if (lineCount == (29 * (k - 1) + 17) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 21) && bsls1 == true)
                        {
                            Tmax48[xh] = Convert.ToSingle(szLS[11]);
                            Tmin48[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain48[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX48[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS48[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 23) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 25) && bsls1 == true)
                        {


                            Tmax72[xh] = Convert.ToSingle(szLS[11]);
                            Tmin72[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain72[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX72[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS72[xh] = LS1 + "转" + LS2;
                                }
                            }


                        }
                        else if (lineCount == (29 * (k - 1) + 26) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 27) && bsls1 == true)
                        {
                            Tmax96[xh] = Convert.ToSingle(szLS[11]);
                            Tmin96[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain96[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX96[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS96[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 28) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 29) && bsls1 == true)
                        {
                            Tmax120[xh] = Convert.ToSingle(szLS[11]);
                            Tmin120[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            xh++;
                            bsls1 = false;
                            if (xh >= qxCount)
                                break;
                        }
                        lineCount++;
                    }
                }
                peopleList.Clear();
                for (int i = 0; i < qxCount; i++)
                {
                    if (StationID[i] != "53463")
                    {
                        peopleList.Add(new people()
                        {
                            QXID = StationID[i],
                            GW24 = Tmax24[i],
                            DW24 = Tmin24[i],
                            QY24 = Rain24[i],
                            GW48 = Tmax48[i],
                            DW48 = Tmin48[i],
                            QY48 = Rain48[i],
                            GW72 = Tmax72[i],
                            DW72 = Tmin72[i],
                            QY72 = Rain72[i],
                            GW96 = Tmax96[i],
                            DW96 = Tmin96[i],
                            QY96 = Rain96[i],
                            GW120 = Tmax120[i],
                            DW120 = Tmin120[i],
                            QY120 = Rain120[i],
                        });
                    }
                }
                for (int i = 0; i < qxCount; i++)
                {
                    if (StationID[i] == "53463")
                    {
                        peopleList.Add(new people()
                        {
                            QXID = StationID[i],
                            GW24 = Tmax24[i],
                            DW24 = Tmin24[i],
                            QY24 = Rain24[i],
                            GW48 = Tmax48[i],
                            DW48 = Tmin48[i],
                            QY48 = Rain48[i],
                            GW72 = Tmax72[i],
                            DW72 = Tmin72[i],
                            QY72 = Rain72[i],
                            GW96 = Tmax96[i],
                            DW96 = Tmin96[i],
                            QY96 = Rain96[i],
                            GW120 = Tmax120[i],
                            DW120 = Tmin120[i],
                            QY120 = Rain120[i],
                        });
                        break;
                    }
                }
                ((DataGrid)(this.FindName("History"))).ItemsSource = peopleList;
            }
        }

        private void DQ10_Checked(object sender, RoutedEventArgs e)
        {
            DQBDcheck.IsChecked = false;
            CIMISScheck.IsChecked = false;
            bdpath = "";
            AllPath.Text = "";
            try
            {
                using (StreamReader sr = new StreamReader(pathConfig, Encoding.Default))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "读取中央指导预报服务器路径")
                        {
                            bdpath = line.Split('=')[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (bdpath.Length > 0)
            {
                AllPath.Text = bdpath;
            }

        }

        private void CPathBtu_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog mDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = mDialog.ShowDialog();
            string mDir = mDialog.SelectedPath.Trim();
            string[] szPathSet = File.ReadAllLines(System.Environment.CurrentDirectory + @"\config\pathConfig.txt", Encoding.Default);
            for (int i = 0; i < szPathSet.Length; i++)
            {
                if ((szPathSet[i].Split('=')[0]) == "读取中央指导预报本地路径")
                    szPathSet[i] = "读取中央指导预报本地路径=" + mDir;
            }
            File.WriteAllLines(System.Environment.CurrentDirectory + @"\config\pathConfig.txt", szPathSet, Encoding.Default);
            if (DQBDcheck.IsChecked == true)
            {
                bdpath = mDir;
                AllPath.Text = mDir;
            }
        }

        private void DR_Click(object sender, RoutedEventArgs e)
        {
            if (peopleList.Count > 0)
            {
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

                            string bjTime = "";
                            if (SC == "08")
                                bjTime = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd ") + "20:00";
                            else
                                bjTime += Convert.ToDateTime(sDate.SelectedDate).AddDays(1).ToString("yyyy-MM-dd ") + "08:00";
                            DateTime dt2 = Convert.ToDateTime(dt1.ToString("yyyy-MM-dd HH:mm"));

                            DateTime dt3 = Convert.ToDateTime(bjTime);
                            if (DateTime.Compare(dt2, dt3) != 0)
                            {
                                DRJLXS.Text = "开始导入\n";
                                try
                                {
                                    if (peopleList.Count != 0)
                                    {
                                        using (SqlConnection mycon = new SqlConnection(con))
                                        {
                                            Int16 intBS = 0;//状态标示，初始为0，不更新为1，更新为2
                                            mycon.Open();//打开
                                            people[] p1ls = peopleList.ToArray();
                                            for (int i = 0; i < p1ls.Length; i++)
                                            {
                                                int jlCount = 0;
                                                try
                                                {
                                                    string sql = string.Format(
                                                        @"insert into ZYZD values('{0}','{1:yyyy-MM-dd}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                                        p1ls[i].QXID, Convert.ToDateTime(sDate.SelectedDate), SC, p1ls[i].GW24, p1ls[i].DW24, p1ls[i].QY24,
                                                        p1ls[i].GW48, p1ls[i].DW48, p1ls[i].QY48, p1ls[i].GW72, p1ls[i].DW72,
                                                        p1ls[i].QY72, p1ls[i].GW96, p1ls[i].DW96, p1ls[i].QY96, p1ls[i].GW120, p1ls[i].DW120, p1ls[i].QY120); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                                                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                                                    jlCount = sqlman.ExecuteNonQuery();
                                                    if (jlCount != 0)
                                                    {
                                                        DRJLXS.Text += p1ls[i].QXID + "导入成功\n";
                                                    }
                                                }
                                                catch (Exception ex1)
                                                {

                                                }
                                                if (jlCount == 0) //如果插入失败，则说明已经存在，进行更新字段操作
                                                {
                                                    if (intBS == 0)
                                                    {
                                                        if (System.Windows.MessageBox.Show("中央指导预报已经入库，是否更新数据库", "提示", MessageBoxButton.YesNo) ==
                                                            MessageBoxResult.Yes)
                                                        {
                                                            intBS = 2;
                                                        }
                                                        else
                                                        {
                                                            intBS = 1;
                                                            DRJLXS.Text += "放弃更新数据库\n";
                                                        }
                                                    }
                                                    if (intBS == 2)
                                                    {
                                                        try
                                                        {
                                                            string sql = string.Format(@"update ZYZD set Tmax24='{0}' ,Tmin24='{1}',Rain24='{2}',Tmax48='{3}' ,Tmin48='{4}',Rain48='{5}',Tmax72='{6}' ,Tmin72='{7}',Rain72='{8}',Tmax96='{9}' ,Tmin96='{10}',Rain96='{11}',Tmax120='{12}' ,Tmin120='{13}',Rain120='{14}' where date='{15:yyyy-MM-dd}' and StationID='{16}'and SC='{17}'", p1ls[i].GW24, p1ls[i].DW24, p1ls[i].QY24, p1ls[i].GW48, p1ls[i].DW48, p1ls[i].QY48, p1ls[i].GW72, p1ls[i].DW72, p1ls[i].QY72, p1ls[i].GW96, p1ls[i].DW96, p1ls[i].QY96, p1ls[i].GW120, p1ls[i].DW120, p1ls[i].QY120, Convert.ToDateTime(sDate.SelectedDate), p1ls[i].QXID, SC);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                                                            SqlCommand sqlman = new SqlCommand(sql, mycon);
                                                            jlCount = sqlman.ExecuteNonQuery();
                                                        }
                                                        catch (Exception ex)
                                                        {

                                                        }
                                                        if (jlCount != 0)
                                                            DRJLXS.Text += p1ls[i].QXID + "更新成功\n";
                                                        else
                                                            DRJLXS.Text += p1ls[i].QXID + "导入失败\n";

                                                    }
                                                }
                                            }


                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            else
                            {
                                DRJLXS.Text = "入库失败：已经超过最晚入库时间 " + bjTime + "\r\n";
                            }

                        }
                        mycon1.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("无数据");
            }

                
        }

        bool SFRK(DateTime dt, string SC)
        {
            bool fhBool = false;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from ZYZD Where Date='{0}' AND SC='{1}'",dt.ToString("yyyy-MM-dd"),SC);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    if (sqlreader.HasRows)
                    {
                        fhBool = true;
                    }
                }
                catch
                {
                    
                }
            }

                return fhBool;
        }

        private void XSRK_Click(object sender, RoutedEventArgs e)
        {
            peopleList.Clear();
            ((DataGrid)(this.FindName("History"))).ItemsSource = peopleList;
            if (SFRK(Convert.ToDateTime(sDate.SelectedDate), scchoose.Text))
            {
                RKZTXS.Text = "数据库记录存在";
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    try
                    {
                        mycon.Open();
                        string sql = string.Format(@"select * from ZYZD Where Date='{0}' AND SC='{1}'", Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd"), scchoose.Text);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        SqlDataReader sqlreader = sqlman.ExecuteReader();
                        while (sqlreader.Read())
                        {
                            if (sqlreader.GetString(sqlreader.GetOrdinal("StationID")) != "53463")
                            {
                                peopleList.Add(new people()
                                {
                                    QXID = sqlreader.GetString(sqlreader.GetOrdinal("StationID")),
                                    GW24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax24")),
                                    DW24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin24")),
                                    QY24 = sqlreader.GetString(sqlreader.GetOrdinal("Rain24")),
                                    GW48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax48")),
                                    DW48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin48")),
                                    QY48 = sqlreader.GetString(sqlreader.GetOrdinal("Rain48")),
                                    GW72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax72")),
                                    DW72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin72")),
                                    QY72 = sqlreader.GetString(sqlreader.GetOrdinal("Rain72")),
                                    GW96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax96")),
                                    DW96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin96")),
                                    QY96 = sqlreader.GetString(sqlreader.GetOrdinal("Rain96")),
                                    GW120 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax120")),
                                    DW120 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin120")),
                                    QY120 = sqlreader.GetString(sqlreader.GetOrdinal("Rain120")),
                                });
                            }
                            
                            
                        }
                        sqlreader.Close();
                        sqlreader = sqlman.ExecuteReader();
                        while (sqlreader.Read())
                        {
                            if (sqlreader.GetString(sqlreader.GetOrdinal("StationID")) == "53463")
                            {
                                peopleList.Add(new people()
                                {
                                    QXID = sqlreader.GetString(sqlreader.GetOrdinal("StationID")),
                                    GW24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax24")),
                                    DW24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin24")),
                                    QY24 = sqlreader.GetString(sqlreader.GetOrdinal("Rain24")),
                                    GW48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax48")),
                                    DW48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin48")),
                                    QY48 = sqlreader.GetString(sqlreader.GetOrdinal("Rain48")),
                                    GW72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax72")),
                                    DW72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin72")),
                                    QY72 = sqlreader.GetString(sqlreader.GetOrdinal("Rain72")),
                                    GW96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax96")),
                                    DW96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin96")),
                                    QY96 = sqlreader.GetString(sqlreader.GetOrdinal("Rain96")),
                                    GW120 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax120")),
                                    DW120 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin120")),
                                    QY120 = sqlreader.GetString(sqlreader.GetOrdinal("Rain120")),
                                });
                                break;
                            }
                        }
                        ((DataGrid)(this.FindName("History"))).ItemsSource = peopleList;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                RKZTXS.Text="数据库记录不存在";
            }
        }
    }
}
