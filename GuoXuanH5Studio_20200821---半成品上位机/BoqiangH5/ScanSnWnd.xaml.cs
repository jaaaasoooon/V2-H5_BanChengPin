using System;
using System.Collections.Generic;
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

namespace BoqiangH5
{
    /// <summary>
    /// ScanSnWnd.xaml 的交互逻辑
    /// </summary>
    public partial class ScanSnWnd : Window
    {
        public ScanSnWnd()
        {
            InitializeComponent();
        }

        private void tbSn_TextChanged(object sender, TextChangedEventArgs e)
        {
            var it = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbSn.Focus();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
