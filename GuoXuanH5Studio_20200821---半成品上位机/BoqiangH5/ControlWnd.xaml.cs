using BoqiangH5.BQProtocol;
using BoqiangH5.DDProtocol;
using BoqiangH5Entity;
using System.Text.RegularExpressions;
using System.Windows;

namespace BoqiangH5
{
    /// <summary>
    /// ControlWnd.xaml 的交互逻辑
    /// </summary>
    public partial class ControlWnd : Window
    {
        public static bool m_bIsNotUpdateBmsInfo = false;
        public ControlWnd()
        {
            InitializeComponent();

            InitCtrlWnd();
        }

        private void InitCtrlWnd()
        {
            if (!MainWindow.m_statusBarInfo.IsOnline)
            {
                contrlWnd.IsEnabled = false;
            }

            if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
            {
                gbDidi.IsEnabled = false;            
            }
            else
            {
                gbBoQiang.IsEnabled = false;

            }
        }


        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            InitCtrlWnd();
        }

        /// <summary>
        /// 上电
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPowerOn_Click(object sender, RoutedEventArgs e)
        {
            DdProtocol.DdInstance.DD_PowerOn();
        }


        /// <summary>
        /// 下电
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPowerOff_Click(object sender, RoutedEventArgs e)
        {
            DdProtocol.DdInstance.DD_PowerOff();

        }

        private void btnSleep_Click(object sender, RoutedEventArgs e)
        {
            BqProtocol.BqInstance.BQ_Sleep();
        }

        private void btnSoftReset_Click(object sender, RoutedEventArgs e)
        {
            BqProtocol.BqInstance.BQ_Reset();
        }

        private void btnReinitialize_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnFactoryReset_Click(object sender, RoutedEventArgs e)
        {
            BqProtocol.BqInstance.BQ_FactoryReset();
        }

        private void btnAlterSOC_Click(object sender, RoutedEventArgs e)
        {
            string str = @"^[0-9]{1,3}$";
            if (!Regex.IsMatch(tbSOC.Text, str))    // if (string.IsNullOrEmpty(txtBoxBarcode.Text))
            {
                MessageBox.Show("请输入正确的 SOC 值！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            byte socVal = byte.Parse(tbSOC.Text);

            if (socVal < 0 || socVal > 100)
            {
                MessageBox.Show("请输入正确的 SOC 值！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            BqProtocol.BqInstance.BQ_AlterSOC(socVal);
        }

        
    }
}
