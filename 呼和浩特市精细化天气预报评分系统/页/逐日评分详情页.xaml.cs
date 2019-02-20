using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace 呼和浩特市精细化天气预报评分系统
{
    /// <summary>
    /// 逐日评分详情页.xaml 的交互逻辑
    /// </summary>
    public partial class 逐日评分详情页 : Page
    {
        ObservableCollection<tjClass.ZRPF> zrpf = new ObservableCollection<tjClass.ZRPF>();
        
        public 逐日评分详情页()
        {
            InitializeComponent();
            CSH();
        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            BTLabel.Content = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd") + gwchoose.Text +
                              scchoose.Text + "时" + SXSelect.Text + "小时逐日评分详情";
            if (!(sDate.SelectedDate.ToString().Length == 0))
            {
                zrpf.Clear();
                tjClass tj = new tjClass();
                zrpf = tj.zrpftj(scchoose.Text,Convert.ToDateTime(sDate.SelectedDate),gwchoose.Text,SXSelect.Text);
                ((this.FindName("GRPFList")) as DataGrid).ItemsSource = zrpf;
            }
            else
            {
                System.Windows.MessageBox.Show("请选择查询时间");
            }
        }

        private void CSH()
        {
            try
            {
                sDate.BlackoutDates.Add(new CalendarDateRange((DateTime.Now.Date).AddDays(+1),
                    DateTime.MaxValue)); //开始时间不可选的范围，当前日期以后
                string sc = scchoose.SelectedItem.ToString();
                sc = sc.Split(':')[1].Trim();
                gwCSH(sc);
                ((this.FindName("GRPFList")) as DataGrid).ItemsSource = zrpf;
                
            }
            catch (Exception ex)
            {
               
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
            string sc = scchoose.SelectedItem.ToString();
            sc = sc.Split(':')[1].Trim();
            gwCSH(sc);
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
            tjClass.ZRPF[] dcsz = zrpf.ToArray();
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

                cellSheet.Cells[0, 0].PutValue("旗县");
                cellSheet.Cells[0, 1].PutValue("ID");
                cellSheet.Cells[0, 2].PutValue("市台高温");
                cellSheet.Cells[0, 3].PutValue("实况高温");
                cellSheet.Cells[0, 4].PutValue("指导高温");
                cellSheet.Cells[0, 5].PutValue("高温≤1");
                cellSheet.Cells[0, 6].PutValue("高温正确");
                cellSheet.Cells[0, 7].PutValue("高温错误");
                cellSheet.Cells[0, 8].PutValue("市台低温");
                cellSheet.Cells[0, 9].PutValue("实况低温");
                cellSheet.Cells[0, 10].PutValue("指导低温");
                cellSheet.Cells[0, 11].PutValue("低温≤1");
                cellSheet.Cells[0, 12].PutValue("低温正确");
                cellSheet.Cells[0, 13].PutValue("低温错误");
                cellSheet.Cells[0, 14].PutValue("市台天气");
                cellSheet.Cells[0, 15].PutValue("市台晴雨");
                cellSheet.Cells[0, 16].PutValue("实况降水量");
                cellSheet.Cells[0, 17].PutValue("指导晴雨");
                cellSheet.Cells[0, 18].PutValue("指导天气");
                for (int i = 0; i < dcsz.Length; i++)
                {
                    cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].QXName);
                    cellSheet.Cells[i + 1, 0].SetStyle(style1);
                    cellSheet.Cells[i + 1, 1].PutValue(dcsz[i].ID);
                    cellSheet.Cells[i + 1, 1].SetStyle(style1);
                    cellSheet.Cells[i + 1, 2].PutValue(Math.Round(dcsz[i].STGW,2));
                    cellSheet.Cells[i + 1, 2].SetStyle(style1);
                    cellSheet.Cells[i + 1, 3].PutValue(Math.Round(dcsz[i].SKGW,2));
                    cellSheet.Cells[i + 1, 3].SetStyle(style1);
                    cellSheet.Cells[i + 1, 4].PutValue(Math.Round(dcsz[i].ZDGW,2));
                    cellSheet.Cells[i + 1, 4].SetStyle(style1);
                    cellSheet.Cells[i + 1, 5].PutValue(dcsz[i].GW1);
                    cellSheet.Cells[i + 1, 5].SetStyle(style1);
                    cellSheet.Cells[i + 1, 6].PutValue(dcsz[i].GW2);
                    cellSheet.Cells[i + 1, 6].SetStyle(style1);
                    cellSheet.Cells[i + 1, 7].PutValue(dcsz[i].GW3);
                    cellSheet.Cells[i + 1, 7].SetStyle(style1);
                    cellSheet.Cells[i + 1, 8].PutValue(Math.Round(dcsz[i].STDW,2));
                    cellSheet.Cells[i + 1, 8].SetStyle(style1);
                    cellSheet.Cells[i + 1, 9].PutValue(Math.Round(dcsz[i].SKDW,2));
                    cellSheet.Cells[i + 1, 9].SetStyle(style1);
                    cellSheet.Cells[i + 1, 10].PutValue(Math.Round(dcsz[i].ZDDW,2));
                    cellSheet.Cells[i + 1, 10].SetStyle(style1);
                    cellSheet.Cells[i + 1, 11].PutValue(dcsz[i].DW1);
                    cellSheet.Cells[i + 1, 11].SetStyle(style1);
                    cellSheet.Cells[i + 1, 12].PutValue(dcsz[i].DW2);
                    cellSheet.Cells[i + 1, 12].SetStyle(style1);
                    cellSheet.Cells[i + 1, 13].PutValue(dcsz[i].DW3);
                    cellSheet.Cells[i + 1, 13].SetStyle(style1);
                    cellSheet.Cells[i + 1, 14].PutValue(dcsz[i].STTQ);
                    cellSheet.Cells[i + 1, 14].SetStyle(style1);
                    cellSheet.Cells[i + 1, 15].PutValue(dcsz[i].STQY);
                    cellSheet.Cells[i + 1, 15].SetStyle(style1);
                    cellSheet.Cells[i + 1, 16].PutValue(Math.Round(dcsz[i].SKJS,1));
                    cellSheet.Cells[i + 1, 16].SetStyle(style1);
                    cellSheet.Cells[i + 1, 17].PutValue(dcsz[i].ZDQY);
                    cellSheet.Cells[i + 1, 17].SetStyle(style1);
                    cellSheet.Cells[i + 1, 18].PutValue(dcsz[i].ZDTQ);
                    cellSheet.Cells[i + 1, 18].SetStyle(style1);

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


    }
}
