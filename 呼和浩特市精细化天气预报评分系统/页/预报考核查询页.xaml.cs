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
    /// 预报考核查询页.xaml 的交互逻辑
    /// </summary>
    public partial class 预报考核查询页 : Page
    {
        private string  strID = "",strName="";
        string con;//这里是保存连接数据库的字符串172.18.142.158 id=sa;password=134679;
        string DBconPath = Environment.CurrentDirectory + @"\config\DBconfig.txt";
        ObservableCollection<JTPF> jtpf = new ObservableCollection<JTPF>();
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        public 预报考核查询页()
        {
            InitializeComponent();
            CSH();
        }

        public void CSH()
        {
            BTLabel.Content = "预报考核查询";
            SMLabel.Content = "温度1.预报气温与实况气温相差1℃以内\n温度2.气温要素预报是正技巧且预报正确（预报气温与实况气温相差2度以内）\n温度3.每有一个站点的气温要素预报是负技巧且预报错误（气温误差超过2℃）\n晴雨1.晴雨预报技巧为正\n晴雨2.晴雨预报技巧为负";
            try
            {
                using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("sql管理员"))
                        {
                            con = line.Substring("sql管理员=".Length);
                            break;
                        }
                    }
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void sDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            eDate.BlackoutDates.Remove(dr1);//现将原来禁止的时间范围删除，否则会报错
            dr1 = new CalendarDateRange(new DateTime(), Convert.ToDateTime(sDate.Text).AddDays(-1));
            eDate.SelectedDate = null;//将已经选取的结束时间清空
            eDate.BlackoutDates.Add(dr1);//结束时间随着开始时间的改变增加新的范围
        }
        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
            {
                BTLabel.Content = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd") + "至" + Convert.ToDateTime(eDate.SelectedDate).ToString("yyyy-MM-dd") + "预报考核查询";
                jtpf.Clear();
                try
                {
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        mycon.Open(); //打开
                        string sql =
                            string.Format(
                                @"select DISTINCT PeopleID from TJ where date>='{0:yyyy-MM-dd}' and date<='{1:yyyy-MM-dd}' ",
                                Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate));
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        SqlDataReader sqlreader = sqlman.ExecuteReader();
                        if (sqlreader.HasRows)
                        {
                            strID = "";
                            while (sqlreader.Read())
                            {
                                strID += "'"+sqlreader.GetString(sqlreader.GetOrdinal("PeopleID")) + "'" + ',';
                            }

                        }
                        else
                        {
                            MessageBox.Show("指定时间范围内无记录，请重新选择起止时间或者确认数据库数据是否正常");
                            return;
                        }
                        mycon.Close();
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message+"\n指定时间范围内获取预报员信息失败，请确定数据库是否连接正常");
                    return;
                }
                strID= strID.Substring(0, strID.Length - 1);
                try
                {
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        mycon.Open(); //打开
                        string sql =
                            string.Format(
                                @"select DISTINCT ID,Name from USERID where ID in ({0}) ",
                                strID);
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        SqlDataReader sqlreader = sqlman.ExecuteReader();
                        if (sqlreader.HasRows)
                        {
                            strID = "";
                            strName = "";
                            while (sqlreader.Read())
                            {
                                strID += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + ',';
                                strName += sqlreader.GetString(sqlreader.GetOrdinal("Name")) + ',';
                            }
                            mycon.Close();
                            strID = strID.Substring(0, strID.Length - 1);
                            strName = strName.Substring(0, strName.Length - 1);
                            
                        }
                    }
                    
                    string[] szID = strID.Split(','), szName = strName.Split(',');
                    for (int i = 0; i < szID.Length; i++)
                    {

                        int gw1 = 0,
                            gw2 = 0,
                            gw3 = 0,
                            dw1 = 0,
                            dw2 = 0,
                            dw3 = 0,
                            qy1 = 0,
                            qy2 = 0,
                            allCount = 0;
                        try
                        {
                            using (SqlConnection mycon = new SqlConnection(con))
                            {
                                mycon.Open(); //打开
                                string sql = string.Format(@"select * from TJ where date>='{0:yyyy-MM-dd}' and date<='{1:yyyy-MM-dd}' and PeopleID='{2}'", Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate),szID[i]);  //SQL查询语句 

                                SqlCommand sqlman = new SqlCommand(sql, mycon);
                                SqlDataReader sqlreader = sqlman.ExecuteReader();
                                if (sqlreader.HasRows)
                                {
                                    while (sqlreader.Read())
                                    {
                                        allCount++;
                                        float floasj = 0, floazy = 0;
                                        try
                                        {
                                            floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24")));
                                            floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax24")));
                                            if (floasj < 9999 && floazy < 9999)
                                            {
                                                if (floasj <= 1)
                                                {
                                                    gw1++;
                                                }
                                                else if (floasj <= 2 && (floasj - floazy) < 0)
                                                {
                                                    gw2++;
                                                }
                                                else if (floasj > 2 && (floasj - floazy) > 0)
                                                {
                                                    gw3++;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            
                                        }
                                        try
                                        {
                                            floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24")));
                                            floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin24")));
                                            if (floasj < 9999 && floazy < 9999)
                                            {
                                                if (floasj <= 1)
                                                {
                                                    dw1++;
                                                }
                                                else if (floasj <= 2 && (floasj - floazy) < 0)
                                                {
                                                    dw2++;
                                                }
                                                else if (floasj > 2 && (floasj - floazy) > 0)
                                                {
                                                    dw3++;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            
                                        }
                                        try
                                        {
                                            floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain24")));
                                            floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain24")));
                                            if (floasj < 9999 && floazy < 9999)
                                            {
                                                if (floasj - floazy > 0)
                                                    qy1++;
                                                else if (floasj - floazy < 0)
                                                    qy2++;
                                            }
                                        }
                                        catch
                                        {
                                            
                                        }
                                        try
                                        {
                                            floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48")));
                                            floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax48")));
                                            if (floasj < 9999 && floazy < 9999)
                                            {
                                                if (floasj <= 1)
                                                {
                                                    gw1++;
                                                }
                                                else if (floasj <= 2 && (floasj - floazy) < 0)
                                                {
                                                    gw2++;
                                                }
                                                else if (floasj > 2 && (floasj - floazy) > 0)
                                                {
                                                    gw3++;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            
                                        }
                                        try
                                        {
                                            floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48")));
                                            floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin48")));
                                            if (floasj < 9999 && floazy < 9999)
                                            {
                                                if (floasj <= 1)
                                                {
                                                    dw1++;
                                                }
                                                else if (floasj <= 2 && (floasj - floazy) < 0)
                                                {
                                                    dw2++;
                                                }
                                                else if (floasj > 2 && (floasj - floazy) > 0)
                                                {
                                                    dw3++;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            
                                        }
                                        try
                                        {
                                            floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain48")));
                                            floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain48")));
                                            if (floasj < 9999 && floazy < 9999)
                                            {
                                                if (floasj - floazy > 0)
                                                    qy1++;
                                                else if (floasj - floazy < 0)
                                                    qy2++;
                                            }
                                        }
                                        catch
                                        {
                                            
                                        }
                                        try
                                        {
                                            floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72")));
                                            floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax72")));
                                            if (floasj < 9999 && floazy < 9999)
                                            {
                                                if (floasj <= 1)
                                                {
                                                    gw1++;
                                                }
                                                else if (floasj <= 2 && (floasj - floazy) < 0)
                                                {
                                                    gw2++;
                                                }
                                                else if (floasj > 2 && (floasj - floazy) > 0)
                                                {
                                                    gw3++;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            
                                        }
                                        try
                                        {
                                            floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72")));
                                            floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin72")));
                                            if (floasj < 9999&& floazy < 9999)
                                            {
                                                if (floasj <= 1)
                                                {
                                                    dw1++;
                                                }
                                                else if (floasj <= 2 && (floasj - floazy) < 0)
                                                {
                                                    dw2++;
                                                }
                                                else if (floasj > 2 && (floasj - floazy) > 0)
                                                {
                                                    dw3++;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            
                                        }
                                        try
                                        {
                                            floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain72")));
                                            floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain72")));
                                            if (floasj < 9999&& floazy < 9999)
                                            {
                                                if (floasj - floazy > 0)
                                                    qy1++;
                                                else if (floasj - floazy < 0)
                                                    qy2++;
                                            }
                                        }
                                        catch
                                        {
                                            
                                        }

                                    }

                                }
                            }
                            jtpf.Add(new JTPF()
                            {
                                Name=szName[i],
                                ID=szID[i],
                                ZDZS=allCount*3,
                                GW1=gw1,
                                GW2 = gw2,
                                GW3 = gw3,
                                DW1=dw1,
                                DW2 = dw2,
                                DW3 = dw3,
                                QY1=qy1,
                                QY2=qy2
                            });

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                ((this.FindName("JTPFList")) as DataGrid).ItemsSource = jtpf;
            }
            else
            {
                MessageBox.Show("请选择起止时间");
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
            JTPF[] dcsz = jtpf.ToArray();
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
                
                cellSheet.Cells[0, 0].PutValue("姓名");
                cellSheet.Cells[0, 1].PutValue("ID");
                cellSheet.Cells[0, 2].PutValue("总站点数");
                cellSheet.Cells[0, 3].PutValue("高温1");
                cellSheet.Cells[0, 4].PutValue("高温2");
                cellSheet.Cells[0, 5].PutValue("高温3");
                cellSheet.Cells[0, 6].PutValue("低温1");
                cellSheet.Cells[0, 7].PutValue("低温2");
                cellSheet.Cells[0, 8].PutValue("低温3");
                cellSheet.Cells[0, 9].PutValue("晴雨1");
                cellSheet.Cells[0, 10].PutValue("晴雨2");

                for (int i = 0; i < dcsz.Length; i++)
                {
                    cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].Name);
                    cellSheet.Cells[i + 1, 0].SetStyle(style1);
                    cellSheet.Cells[i + 1, 1].PutValue(dcsz[i].ID);
                    cellSheet.Cells[i + 1, 1].SetStyle(style1);
                    cellSheet.Cells[i + 1, 2].PutValue(dcsz[i].ZDZS);
                    cellSheet.Cells[i + 1, 2].SetStyle(style1);
                    cellSheet.Cells[i + 1, 3].PutValue(dcsz[i].GW1);
                    cellSheet.Cells[i + 1, 3].SetStyle(style1);
                    cellSheet.Cells[i + 1, 4].PutValue(dcsz[i].GW2);
                    cellSheet.Cells[i + 1, 4].SetStyle(style1);
                    cellSheet.Cells[i + 1, 5].PutValue(dcsz[i].GW3);
                    cellSheet.Cells[i + 1, 5].SetStyle(style1);
                    cellSheet.Cells[i + 1, 6].PutValue(dcsz[i].DW1);
                    cellSheet.Cells[i + 1, 6].SetStyle(style1);
                    cellSheet.Cells[i + 1, 7].PutValue(dcsz[i].DW2);
                    cellSheet.Cells[i + 1, 7].SetStyle(style1);
                    cellSheet.Cells[i + 1, 8].PutValue(dcsz[i].DW3);
                    cellSheet.Cells[i + 1, 8].SetStyle(style1);
                    cellSheet.Cells[i + 1, 9].PutValue(dcsz[i].QY1);
                    cellSheet.Cells[i + 1, 9].SetStyle(style1);
                    cellSheet.Cells[i + 1, 10].PutValue(dcsz[i].QY2);
                    cellSheet.Cells[i + 1, 10].SetStyle(style1);
                }
                //cellSheet.AutoFitColumns();
                int columnCount = cellSheet.Cells.MaxColumn;  //获取表页的最大列数
                cellSheet.AutoFitColumns();
                for (int col = 0; col < columnCount + 1; col++)
                {
                    cellSheet.Cells[0, col].SetStyle(style1);
                    cellSheet.Cells.SetColumnWidthPixel(col, cellSheet.Cells.GetColumnWidthPixel(col) + 30);
                }
                cellSheet.Cells[dcsz.Length + 3, 1].PutValue("温度1.预报气温与实况气温相差1℃以内");
                cellSheet.Cells[dcsz.Length + 4, 1].PutValue("温度2.气温要素预报是正技巧且预报正确（预报气温与实况气温相差2度以内）");
                cellSheet.Cells[dcsz.Length + 5, 1].PutValue("温度3.每有一个站点的气温要素预报是负技巧且预报错误（气温误差超过2℃）");
                cellSheet.Cells[dcsz.Length + 6, 1].PutValue("晴雨1.晴雨预报技巧为正");
                cellSheet.Cells[dcsz.Length + 7, 1].PutValue("晴雨2.晴雨预报技巧为负");

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

        public class JTPF//统计信息列表
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public int ZDZS { get; set; }
            public int GW1 { get; set; }
            public int GW2 { get; set; }
            public int GW3 { get; set; }
            public int DW1 { get; set; }
            public int DW2 { get; set; }
            public int DW3 { get; set; }
            public int QY1 { get; set; }
            public int QY2 { get; set; }
        }
    }
}
