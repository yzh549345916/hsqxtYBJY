using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aspose.Cells;

namespace 呼和浩特市精细化天气预报评分系统
{
    /// <summary>
    /// 登录记录查询页.xaml 的交互逻辑
    /// </summary>
    public partial class 登录记录查询页 : Page
    {
        string con;//这里是保存连接数据库的字符串
        ObservableCollection<ZBJL> zbjl = new ObservableCollection<ZBJL>();
        public 登录记录查询页()
        {
            InitializeComponent();
            CSH();
        }

        void CSH()
        {
            ((DataGrid)(this.FindName("ZBList"))).ItemsSource = zbjl;
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
                yearchoose.DisplayMemberPath= "Value";
                yearchoose.SelectedIndex = intCount - 1;
            }
            else
            {
                MessageBox.Show("时间信息获取错误，请检查与数据库连接是否正常，服务器时间是否正常");
            }
        }
        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<peopleList> plist = new ObservableCollection<peopleList>();
            zbjl.Clear();
            BTLabel.Content = yearchoose.Text + "年" + monthchoose.Text + "月值班记录";
            DateTime dtfwq=DateTime.Now;
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
            DateTime d2=dtfwq;
            if (yearchoose.Text == dtfwq.ToString("yyyy") && monthchoose.Text == dtfwq.ToString("MM"))
                d2 = dtfwq;
            else
            {
                d2=d1.AddDays(1 - d1.Day).AddMonths(1).AddDays(-1);
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
                            LB=lb,
                            ZB08=zb08,
                            ZB20=zb20
                        });
                    }
                    
                    
                    
                }
                catch (Exception e1)
                {
                    
                }
            }

        }

        private void DCButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string strPath = m_Dialog.SelectedPath + "\\" + BTLabel.Content + ".xls";
            ZBJL[] dcsz = zbjl.ToArray();
            try
            {
                Workbook workbook = new Workbook();
                Worksheet cellSheet = workbook.Worksheets[0];

                /*cellSheet.PageSetup.LeftMargin = 0.3;//左边距
                cellSheet.PageSetup.RightMargin = 0.3;//右边距
                cellSheet.PageSetup.TopMargin = 1;//上边距
                cellSheet.PageSetup.BottomMargin = 0.5;//下边距
                cellSheet.PageSetup.FooterMargin = 0.5;//页脚
                cellSheet.PageSetup.HeaderMargin = 0.5;//页眉
                cellSheet.PageSetup.Orientation = PageOrientationType.Landscape;*/
                cellSheet.PageSetup.CenterHorizontally = true;//水平居中
                cellSheet.PageSetup.CenterVertically = true;
                Aspose.Cells.Style style1 = workbook.CreateStyle();//新增样式  
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.VerticalAlignment = TextAlignmentType.Center;

                cellSheet.Cells[0, 0].PutValue("日期");
                cellSheet.Cells[0, 1].PutValue("领班");
                cellSheet.Cells[0, 2].PutValue("主班08");
                cellSheet.Cells[0, 3].PutValue("主班20");
                for (int i = 0; i < dcsz.Length; i++)
                {
                    cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].RQ);
                    cellSheet.Cells[i + 1, 0].SetStyle(style1);
                    cellSheet.Cells[i + 1, 1].PutValue(dcsz[i].LB);
                    cellSheet.Cells[i + 1, 1].SetStyle(style1);
                    cellSheet.Cells[i + 1, 2].PutValue(dcsz[i].ZB08);
                    cellSheet.Cells[i + 1, 2].SetStyle(style1);
                    cellSheet.Cells[i + 1, 3].PutValue(dcsz[i].ZB20);
                    cellSheet.Cells[i + 1, 3].SetStyle(style1);
                    

                }
                //cellSheet.AutoFitColumns();
                int columnCount = cellSheet.Cells.MaxColumn;  //获取表页的最大列数
                cellSheet.AutoFitColumns();
                for (int col = 0; col < columnCount + 1; col++)
                {
                    cellSheet.Cells[0, col].SetStyle(style1);
                    cellSheet.Cells.SetColumnWidthPixel(col, cellSheet.Cells.GetColumnWidthPixel(col) + 30);
                }

                workbook.Save(strPath);
                MessageBoxResult dr = MessageBox.Show("已成功导出数据至" + strPath + "\n是否打开？", "提示", MessageBoxButton.YesNo);
                if (dr == MessageBoxResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(strPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public class ZBJL
        {
            public string RQ { get; set; }
            public string LB { get; set; }
            public string ZB08 { get; set; }
            public string ZB20 { get; set; }
        }

        public class peopleList
        {
            public string date { get; set; }
            public string gw { get; set; }
            public string sc { get; set; }
            public string name { get; set; }
        }
    }
}
