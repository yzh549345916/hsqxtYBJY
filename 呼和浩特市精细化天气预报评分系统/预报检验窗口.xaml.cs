using System;
using System.Collections.Generic;
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

namespace 呼和浩特市精细化天气预报评分系统
{
    /// <summary>
    /// 预报检验窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 预报检验窗口 : Window
    {
        public 预报检验窗口()
        {
            InitializeComponent();
        }

        private void 预报考核查询_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/预报考核查询页.xaml", UriKind.Relative);
        }
        private void 个人评分72小时_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/个人评分72h页.xaml", UriKind.Relative);
        }
        private void 逐日详情查询_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/逐日评分详情页.xaml", UriKind.Relative);
        }

        private void 登录记录查询_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/登录记录查询页.xaml", UriKind.Relative);
        }
    }
}
