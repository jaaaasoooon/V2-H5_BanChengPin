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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BoqiangH5
{
    /// <summary>
    /// UcMenu.xaml 的交互逻辑
    /// </summary>
    public partial class UcMenu : UserControl
    {
        public UcMenu()
        {
            InitializeComponent();
        }

        public string MenuText
        {
            set
            {
                menuText.Text = value;
            }
            get
            {
                return null;
            }
        }

        public ImageSource MenuIcoSource
        {
            set
            {
                menuIco.Source = value;
            }
            get
            {
                return null;
            }
        }

    }
}
