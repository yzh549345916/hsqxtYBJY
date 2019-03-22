using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// 城镇预报检验72小时页.xaml 的交互逻辑
    /// </summary>
    public partial class 数值预报检验 : Page
    {
        List<GRPF> grpf = new List<GRPF>();
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        public 数值预报检验()
        {
            InitializeComponent();
            DateTime dt = DateTime.Now;
            sDate.SelectedDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
            GSCSH();
        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            CX();
        }
        public void CX()
        {
            try
            {
                if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
                {
                    BTLabel.Content = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy年MM月dd日") + "至" + Convert.ToDateTime(eDate.SelectedDate).ToString("yyyy年MM月dd日") + SXSelect.Text + "小时数值预报准确率";
                    ConfigClass1 configClass1 = new ConfigClass1();
                    string IDNameStr = configClass1.IDName(GSSelect.Text);

                    string[] szIDName = IDNameStr.Split('\n');
                    Int16 minsx = 0, maxsx = 21;
                    if (SXSelect.SelectedIndex == 0)
                    {
                        minsx = 0;
                        maxsx = 21;
                    }
                    else if (SXSelect.SelectedIndex == 1)
                    {
                        minsx = 24;
                        maxsx = 45;
                    }
                    else
                    {
                        minsx = 48;
                        maxsx = 69;
                    }
                    List<ConfigClass1.YBList> temLists = configClass1.GetTemListsbyGSandDate(Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate), GSSelect.Text, Convert.ToInt16(SCSelect.Text), minsx, maxsx);
                    temLists = temLists.OrderBy(y => y.dateTime).ToList();
                    if (szIDName.Length > 0)
                    {
                        if (temLists.Count > 0)
                        {
                            List<ConfigClass1.SKList> skLists = configClass1.GetSKListsbyGSandDate(temLists[0].dateTime, temLists[temLists.Count - 1].dateTime);
                            GRPFList.ItemsSource = null;
                            grpf.Clear();
                            for (int i = 0; i < szIDName.Length; i++)
                            {
                                string id = szIDName[i].Split(',')[0], name = szIDName[i].Split(',')[1];
                                ;
                                grpf.Add(new GRPF
                                {
                                    id = id,
                                    Name = name,
                                    Zql0 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, id, minsx),
                                    Zql3 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, id, minsx + 3),
                                    Zql6 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, id, minsx + 6),
                                    Zql9 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, id, minsx + 9),
                                    Zql12 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, id, minsx + 12),
                                    Zql15 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, id, minsx + 15),
                                    Zql18 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, id, minsx + 18),
                                    Zql21 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, id, minsx + 21),
                                    ZqlPJ = configClass1.GetTemPJZQLbyIDandSX(temLists, skLists, id, minsx, maxsx),

                                });
                            }
                            grpf.Add(new GRPF
                            {
                                id = "00000",
                                Name = "平均",
                                Zql0 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, minsx),
                                Zql3 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, minsx + 3),
                                Zql6 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, minsx + 6),
                                Zql9 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, minsx + 9),
                                Zql12 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, minsx + 12),
                                Zql15 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, minsx + 15),
                                Zql18 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, minsx + 18),
                                Zql21 = configClass1.GetTemZqLbyIDandSx(temLists, skLists, minsx + 21),
                                ZqlPJ = configClass1.GetTemPjzqLbyIDandSx(temLists, skLists, minsx, maxsx),

                            });
                            temLists = configClass1.GetTaxListsbyGSandDate(Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate), GSSelect.Text, Convert.ToInt16(SCSelect.Text), Convert.ToInt16(SXSelect.Text.Replace("小时", "")));
                            for (int i = 0; i < szIDName.Length; i++)
                            {
                                string id = szIDName[i].Split(',')[0], name = szIDName[i].Split(',')[1];
                                grpf.Find(y => y.id == id).ZqlGW = configClass1.GetTaxZqLbyIDandSx(temLists, skLists, id);
                            }
                            grpf.Find(y => y.id == "00000").ZqlGW = configClass1.GetTaxZqLbyIDandSx(temLists, skLists);
                            temLists = configClass1.GetTminListsbyGSandDate(Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate), GSSelect.Text, Convert.ToInt16(SCSelect.Text), Convert.ToInt16(SXSelect.Text.Replace("小时", "")));
                            for (int i = 0; i < szIDName.Length; i++)
                            {
                                string id = szIDName[i].Split(',')[0], name = szIDName[i].Split(',')[1];
                                grpf.Find(y => y.id == id).ZqlDW = configClass1.GetTminZqLbyIDandSx(temLists, skLists, id);
                            }
                            grpf.Find(y => y.id == "00000").ZqlDW = configClass1.GetTminZqLbyIDandSx(temLists, skLists);
                            ((this.FindName("GRPFList")) as DataGrid).ItemsSource = grpf;
                            temLists.Clear();
                            skLists.Clear();

                        }

                        else
                        {
                            MessageBox.Show("所选时间段没有登录记录，请重新选择起止时间");
                        }
                    }
                    else
                    {
                        MessageBox.Show("站点信息获取失败");
                    }


                }
                else
                {
                    MessageBox.Show("请选择起止时间");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DCButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private bool GSCSH()
        {
            try
            {
                ConfigClass1 configClass1 = new ConfigClass1();
                string gsStr=configClass1.getGS();
                if(gsStr.Trim().Length>0)
                {
                    int count = 0;
                    Dictionary<int, string> mydic = new Dictionary<int, string>();
                    string[] szls = gsStr.Split(',');
                    foreach(string ss in szls)
                    {
                        mydic.Add(count++, ss);
                    }
                    GSSelect.ItemsSource = mydic;
                    GSSelect.SelectedValuePath = "Key";
                    GSSelect.DisplayMemberPath = "Value";
                    GSSelect.SelectedIndex = 0;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        private void sDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            eDate.BlackoutDates.Remove(dr1);//现将原来禁止的时间范围删除，否则会报错
            dr1 = new CalendarDateRange(new DateTime(), Convert.ToDateTime(sDate.Text).AddDays(-1));
            eDate.SelectedDate = null;//将已经选取的结束时间清空
            eDate.BlackoutDates.Add(dr1);//结束时间随着开始时间的改变增加新的范围
            try
            {
                DateTime dt1 = Convert.ToDateTime(sDate.SelectedDate);
                DateTime dt = dt1.AddDays(1 - dt1.Day);
                dt = dt.AddMonths(1).AddDays(-1);
                eDate.SelectedDate = dt;
            }
            catch (Exception ex)
            {
            }
        }

        private void SXSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            int count = comboBox.SelectedIndex;
            if(count==0)
            {
                Dgc0.Header = "0小时";
                Dgc3.Header = "3小时";
                Dgc6.Header = "6小时";
                Dgc9.Header = "9小时";
                Dgc12.Header = "12小时";
                Dgc15.Header = "15小时";
                Dgc18.Header = "18小时";
                Dgc21.Header = "21小时";
            }
            else if (count == 1)
            {
                Dgc0.Header = "24小时";
                Dgc3.Header = "27小时";
                Dgc6.Header = "30小时";
                Dgc9.Header = "33小时";
                Dgc12.Header = "36小时";
                Dgc15.Header = "39小时";
                Dgc18.Header = "42小时";
                Dgc21.Header = "45小时";
            }
            else if (count == 2)
            {
                Dgc0.Header = "48小时";
                Dgc3.Header = "51小时";
                Dgc6.Header = "54小时";
                Dgc9.Header = "57小时";
                Dgc12.Header = "60小时";
                Dgc15.Header = "63小时";
                Dgc18.Header = "66小时";
                Dgc21.Header = "69小时";
            }
        }

        public class GRPF//统计信息列表
        {
            public string Name { get; set; }
            public string id { get; set; }
            public float Zql0 { get; set; }
            public float Zql3 { get; set; }
            public float Zql6 { get; set; }
            public float Zql9 { get; set; }
            public float Zql12 { get; set; }
            public float Zql15 { get; set; }
            public float Zql18 { get; set; }
            public float Zql21 { get; set; }
            public float ZqlGW { get; set; }
            public float ZqlDW { get; set; }
            public float ZqlPJ { get; set; }
        }
    }
}
