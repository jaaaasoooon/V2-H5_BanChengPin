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
    /// PasswordWnd.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordWnd : Window
    {
        string password = string.Empty;
        public PasswordWnd(string _password)
        {
            InitializeComponent();
            password = _password;
            isOK = false;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            isOK = false;
            this.Close();

        }

        public bool isOK = false;
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string psw = pswBox.Password.Trim();
            if(psw == password)
            {
                isOK = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("输入密码错误！请重新输入！","输入密码提示",MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }
    }
}
