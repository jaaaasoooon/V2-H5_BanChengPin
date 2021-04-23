using System;
using System.Windows;
using System.Windows.Controls;
//using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using BoqiangH5Entity;
using BoqiangH5Repository;
using BoqiangH5.ISO15765;


namespace BoqiangH5
{
    public partial class SelectCANWnd : Window
    {
        public event EventHandler RaiseCloseEvent;
        
        public static ObservableCollection<ZLGFuction> tabSource = new ObservableCollection<ZLGFuction> { };
        ZLGFuction m_zlgFun;

        static H5Protocol h5Protocol = H5Protocol.BO_QIANG;
        static int handShakeInterval = 3;
        //static int wakeInterval = 5;
        static int recordInterval = 1;
        static bool isClosePwd = false;
        static bool isDebugMode = false;
        static int voltageBase = 0;
        static int voltageError = 0;
        static int temperatureBase = 0;
        static int temperatureError = 0;
        public static H5Protocol m_H5Protocol
        {
            get 
            {
                if (XmlHelper.m_strProtocol == "0")
                {
                    h5Protocol = H5Protocol.BO_QIANG;
                }
                else
                {
                    h5Protocol = H5Protocol.DI_DI;
                }
                return h5Protocol; 
            }
        }

        public static int m_HandShakeTime 
        { 
            get 
            {
                int val = 0;
                Int32.TryParse(XmlHelper.m_strHandShakeInterval, out val);
                if (val != -1)
                {
                    handShakeInterval = val;
                }
                return handShakeInterval;
            } 
        }
        //public static int m_WakeInterval 
        //{ 
        //    get 
        //    {
        //        int val = 0;
        //        Int32.TryParse(XmlHelper.m_strWakeInterval, out val);
        //        if (val != -1)
        //        {
        //            wakeInterval = val;
        //        }
        //        return wakeInterval;
        //    } 
        //}
        public static int m_RecordInterval 
        { 
            get 
            {
                int val = 0;
                Int32.TryParse(XmlHelper.m_strDataRecordInterval, out val);
                if (val != -1)
                {
                    if (val == 0) recordInterval = 1;
                    else if (val == 1) recordInterval = 2;
                    else if (val == 2) recordInterval = 3;
                    else if (val == 3) recordInterval = 4;
                    else if (val == 4) recordInterval = 5;
                    else recordInterval = 1;
                }
                return recordInterval;
            } 
        }

        public static bool m_IsClosePwd
        {
            get
            {
                int val = 0;
                Int32.TryParse(XmlHelper.m_strIsClosePwd, out val);
                if (val != -1)
                {
                    if (val == 0) 
                        isClosePwd = false;
                    else
                        isClosePwd = true;
                }
                return isClosePwd;
            }
        }

        //public static bool m_IsDebugMode
        //{
        //    get
        //    {
        //        int val = 0;
        //        Int32.TryParse(XmlHelper.m_strIsDebugMode, out val);
        //        if (val != -1)
        //        {
        //            if (val == 0)
        //                isDebugMode = false;
        //            else
        //                isDebugMode = true;
        //        }
        //        return isDebugMode;
        //    }
        //}
        public static int m_VoltageBase
        {
            get
            {
                int val;
                if(int.TryParse(XmlHelper.m_strVoltageBase,out val))
                {
                    voltageBase = val;
                }
                return voltageBase;
            }
        }
        public static int m_VoltageError
        {
            get
            {
                int val;
                if (int.TryParse(XmlHelper.m_strVoltageError, out val))
                {
                    voltageError = val;
                }
                return voltageError;
            }
        }
        public static int m_TemperatureBase
        {
            get
            {
                int val;
                if (int.TryParse(XmlHelper.m_strTemperatureBase, out val))
                {
                    temperatureBase = val;
                }
                return temperatureBase;
            }
        }
        public static int m_TemperatureError
        {
            get
            {
                int val;
                if (int.TryParse(XmlHelper.m_strTemperatureError, out val))
                {
                    temperatureError = val;
                }
                return temperatureError;
            }
        }
        public SelectCANWnd()        
        {
            m_zlgFun = DataLinkLayer.DllZLGFun;

            InitializeComponent();

            InitConfigInfo();
        }

        private void InitConfigInfo()
        {
            XmlHelper.LoadConfigInfo();

            if (string.IsNullOrEmpty(XmlHelper.m_strCanType) || (string.IsNullOrEmpty(XmlHelper.m_strBaudrate)))
            {
                MessageBox.Show("请设置CAN类型和波特率！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
                        
            int nIndex = -1;

            Int32.TryParse(XmlHelper.m_strCanType, out nIndex);
            if (nIndex != -1)
            {
                cbCanType.SelectedIndex = nIndex;
            }

            Int32.TryParse(XmlHelper.m_strCanIndex, out nIndex);
            if (nIndex != -1)
            {
                cbCanIndex.SelectedIndex = nIndex;
            }
            else
            {
                cbCanIndex.SelectedIndex = 0;
            }
            
            Int32.TryParse(XmlHelper.m_strCanChannel, out nIndex);
            if (nIndex != -1)
            {
                cbCanChannel.SelectedIndex = nIndex;
            }
            else
            {
                cbCanChannel.SelectedIndex = 0;
            }

            Int32.TryParse(XmlHelper.m_strBaudrate, out nIndex);
            if (nIndex != -1)
            {
                cbBaudRate.SelectedIndex = nIndex;
            }

            Int32.TryParse(XmlHelper.m_strProtocol, out nIndex);
            if (nIndex >= 0 && nIndex < Enum.GetNames(typeof(H5Protocol)).GetLength(0))
            {
                cbProtocol.SelectedIndex = nIndex;
                if (nIndex == 0)
                {
                    h5Protocol = H5Protocol.BO_QIANG;
                }
                else
                {
                    h5Protocol = H5Protocol.DI_DI;
                }
            }

            Int32.TryParse(XmlHelper.m_strCanFD, out nIndex);
            if (nIndex != -1)
            {
                cbCANFD.SelectedIndex = nIndex;
            }

            Int32.TryParse(XmlHelper.m_strArbitration, out nIndex);
            if (nIndex != -1)
            {
                cbArbitrationBaudRate.SelectedIndex = nIndex;
            }

            Int32.TryParse(XmlHelper.m_strDataBaudRate, out nIndex);
            if (nIndex != -1)
            {
                cbDataBaudRate.SelectedIndex = nIndex;
            }

            Int32.TryParse(XmlHelper.m_strTerminalResistance, out nIndex);
            if (nIndex != -1)
            {
                if(nIndex == 1)
                {
                    chbIsTerminalResistance.IsChecked = true;
                }
                else
                {
                    chbIsTerminalResistance.IsChecked = false;
                }
            }

            Int32.TryParse(XmlHelper.m_strDataRecordInterval, out nIndex);
            if (nIndex != -1)
            {
                cbRecordInterval.SelectedIndex = nIndex;
                if (nIndex == 0) recordInterval = 1;
                else if (nIndex == 1) recordInterval = 2;
                else if (nIndex == 2) recordInterval = 3;
                else if (nIndex == 3) recordInterval = 4;
                else if (nIndex == 4) recordInterval = 5;
                else recordInterval = 1;
            }

            //Int32.TryParse(XmlHelper.m_strHandShakeInterval, out nIndex);
            //if (nIndex != -1)
            //{
            //    handShakeInterval = nIndex;
            //    tbHandShakeInterval.Text = nIndex.ToString();
            //}

            //Int32.TryParse(XmlHelper.m_strWakeInterval, out nIndex);
            //if (nIndex != -1)
            //{
            //    wakeInterval = nIndex;
            //    tbWakeInterval.Text = nIndex.ToString();
            //}

            Int32.TryParse(XmlHelper.m_strIsClosePwd, out nIndex);
            if (nIndex != -1)
            {
                if (nIndex == 1)
                {
                    cbClosePwd.IsChecked = true;
                }
                else
                {
                    cbClosePwd.IsChecked = false;
                }
            }

            //Int32.TryParse(XmlHelper.m_strIsDebugMode, out nIndex);
            //if (nIndex != -1)
            //{
            //    if (nIndex == 1)
            //    {
            //        cbDeBugMode.IsChecked = true;
            //    }
            //    else
            //    {
            //        cbDeBugMode.IsChecked = false;
            //    }
            //}

            //Int32.TryParse(XmlHelper.m_strVoltageBase, out nIndex);
            //if (nIndex != -1)
            //{
            //    tbVoltageBase.Text = nIndex.ToString();
            //}
            Int32.TryParse(XmlHelper.m_strVoltageError, out nIndex);
            if (nIndex != -1)
            {
                tbVoltageError.Text = nIndex.ToString();
            }
            //Int32.TryParse(XmlHelper.m_strTemperatureBase, out nIndex);
            //if (nIndex != -1)
            //{
            //    tbTemperatureBase.Text = nIndex.ToString();
            //}
            Int32.TryParse(XmlHelper.m_strTemperatureError, out nIndex);
            if (nIndex != -1)
            {
                tbTemperatureError.Text = nIndex.ToString();
            }
            tbHardwareVersion.Text = XmlHelper.m_strHardwareVersion;
            tbSoftwareVersion.Text = XmlHelper.m_strSoftwareVersion;
        }

        public virtual void OnRaiseCloseEvent(EventArgs e)
        {
            EventHandler handler = RaiseCloseEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (cbCanIndex.SelectedIndex == -1 || cbCanChannel.SelectedIndex == -1)
            {
                MessageBox.Show((string)Application.Current.Resources["tePromptText1"], (string)FindResource("tePrompt"), MessageBoxButton.OK, MessageBoxImage.Warning);
                
            }
            else
            {
                m_zlgFun.zlgInfo.ConnObject = cbCanType.SelectedItem.ToString();           
                tabSource.Add(m_zlgFun);

                //保存设置
                XmlHelper.m_strCanType = cbCanType.SelectedIndex.ToString();
                XmlHelper.m_strCanIndex = cbCanIndex.SelectedIndex.ToString();
                XmlHelper.m_strCanChannel = cbCanChannel.SelectedIndex.ToString();
                XmlHelper.m_strBaudrate = cbBaudRate.SelectedIndex.ToString();
                XmlHelper.m_strDataRecordInterval = cbRecordInterval.SelectedIndex.ToString();

                m_zlgFun.zlgInfo.DevIndex = uint.Parse(XmlHelper.m_strCanIndex);
                m_zlgFun.zlgInfo.DevChannel = uint.Parse(XmlHelper.m_strCanChannel);

                if (cbProtocol.SelectedIndex == 0)
                {
                    h5Protocol = H5Protocol.BO_QIANG;
                    XmlHelper.m_strProtocol = "0";
                }
                else
                {
                    h5Protocol = H5Protocol.DI_DI;
                    XmlHelper.m_strProtocol = "1";
                }

                XmlHelper.m_strCanFD = cbCANFD.SelectedIndex.ToString();
                XmlHelper.m_strArbitration = cbArbitrationBaudRate.SelectedIndex.ToString();
                XmlHelper.m_strDataBaudRate = cbDataBaudRate.SelectedIndex.ToString();
                XmlHelper.m_strTerminalResistance = chbIsTerminalResistance.IsChecked == true ? "1" : "0";

                //uint interval = 3;
                //bool ret = uint.TryParse(tbHandShakeInterval.Text.Trim(), out interval);
                //if(ret == false)
                //{
                //    MessageBox.Show("输入握手连接间隔时间有误，请重新输入！", "输入提示", MessageBoxButton.OK, MessageBoxImage.Information);
                //    return;
                //}
                //XmlHelper.m_strHandShakeInterval = interval.ToString();
                XmlHelper.m_strIsClosePwd = cbClosePwd.IsChecked == true ? "1" : "0";
                //XmlHelper.m_strIsDebugMode = cbDeBugMode.IsChecked == true ? "1" : "0";
                //interval = 5;
                //ret = uint.TryParse(tbWakeInterval.Text.Trim(), out interval);
                //if (ret == false)
                //{
                //    MessageBox.Show("输入的浅休眠唤醒间隔时间有误，请重新输入！", "输入提示", MessageBoxButton.OK, MessageBoxImage.Information);
                //    return;
                //}
                //XmlHelper.m_strWakeInterval = interval.ToString();
                int val;
                //if(int.TryParse(tbVoltageBase.Text.Trim(),out val))
                //{
                //    XmlHelper.m_strVoltageBase = val.ToString();
                //}
                //else
                //{
                //    MessageBox.Show("输入的电压基数有误，请重新输入！", "输入提示", MessageBoxButton.OK, MessageBoxImage.Information);
                //    return;
                //}
                if (int.TryParse(tbVoltageError.Text.Trim(), out val))
                {
                    XmlHelper.m_strVoltageError = val.ToString();
                }
                else
                {
                    MessageBox.Show("输入的电压误差有误，请重新输入！", "输入提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                //if (int.TryParse(tbTemperatureBase.Text.Trim(), out val))
                //{
                //    XmlHelper.m_strTemperatureBase = val.ToString();
                //}
                //else
                //{
                //    MessageBox.Show("输入的温度基数有误，请重新输入！", "输入提示", MessageBoxButton.OK, MessageBoxImage.Information);
                //    return;
                //}
                if (int.TryParse(tbTemperatureError.Text.Trim(), out val))
                {
                    XmlHelper.m_strTemperatureError = val.ToString();
                }
                else
                {
                    MessageBox.Show("输入的温度误差有误，请重新输入！", "输入提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                XmlHelper.m_strHardwareVersion = tbHardwareVersion.Text;
                XmlHelper.m_strSoftwareVersion = tbSoftwareVersion.Text;
                XmlHelper.SaveConfigInfo();
                this.Close();

                OnRaiseCloseEvent(null);
                
            }
        }

        private void Canel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        #region CAN 类型
        private void cbCanType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ZLGInfo.DevType = GetCanType(cbCanType.SelectedIndex);
            if(ZLGInfo.DevType == (uint)ZLGType.VCI_USBCANFD_100U)
            {
                cbCANFD.IsEnabled = true;
                cbArbitrationBaudRate.IsEnabled = true;
                cbDataBaudRate.IsEnabled = true;
                chbIsTerminalResistance.IsEnabled = true;
                lbCanFD.IsEnabled = true;
                lbArbitrationBaudRate.IsEnabled = true;
                lbDataBaudRate.IsEnabled = true;
                lbIsTerminalResistance.IsEnabled = true;

                lbBaudRate.IsEnabled = false;
                cbBaudRate.IsEnabled = false;
            }
            else
            {
                cbCANFD.IsEnabled = false;
                cbArbitrationBaudRate.IsEnabled = false;
                cbDataBaudRate.IsEnabled = false;
                chbIsTerminalResistance.IsEnabled = false;
                lbCanFD.IsEnabled = false;
                lbArbitrationBaudRate.IsEnabled = false;
                lbDataBaudRate.IsEnabled = false;
                lbIsTerminalResistance.IsEnabled = false;

                lbBaudRate.IsEnabled = true;
                cbBaudRate.IsEnabled = true;
            }
        }

        public static uint GetCanType(int selectedIndex)
        {
            uint unCanType = 0;
            switch (selectedIndex)
            {
                case 0:
                    unCanType = (uint)ZLGType.VCI_USBCAN_E_U;
                    break;

                case 1:
                    unCanType = (uint)ZLGType.VCI_PCI5010U; ;
                    break;

                case 2:
                    unCanType = (uint)ZLGType.VCI_PCI9810;
                    break;

                case 3:
                    unCanType = (uint)ZLGType.VCI_USBCAN1;
                    break;

                case 4:
                    unCanType = (uint)ZLGType.VCI_PCI5110;
                    break;

                case 5:
                    unCanType = (uint)ZLGType.VCI_CANLITE;
                    break;

                case 6:
                    unCanType = (uint)ZLGType.VCI_PC104CAN;
                    break;

                case 7:
                    unCanType = (uint)ZLGType.VCI_DNP9810;
                    break;

                case 8:
                    unCanType = (uint)ZLGType.VCI_USBCANFD_100U;
                    break;

                case 9:
                    unCanType = (uint)ZLGType.VCI_USBCAN_2E_U;
                    break;

                case 10:
                    unCanType = (uint)ZLGType.VCI_PCI5020U;
                    break;

                case 11:
                    unCanType = (uint)ZLGType.VCI_PCI5121;
                    break;

                case 12:
                    unCanType = (uint)ZLGType.VCI_USBCAN2;
                    break;

                case 13:
                    unCanType = (uint)ZLGType.VCI_USBCAN2A;
                    break;

                case 14:
                    unCanType = (uint)ZLGType.VCI_PCI9820;
                    break;

                case 15:
                    unCanType = (uint)ZLGType.VCI_ISA9620;
                    break;

                case 16:
                    unCanType = (uint)ZLGType.VCI_ISA5420;
                    break;

                case 17:
                    unCanType = (uint)ZLGType.VCI_PC104CAN2;
                    break;

                case 18:
                    unCanType = (uint)ZLGType.VCI_PCI9820I;
                    break;

                case 19:
                    unCanType = (uint)ZLGType.VCI_PEC9920;
                    break;

                case 20:
                    unCanType = (uint)ZLGType.VCI_PCIE9221;
                    break;

                case 21:
                    unCanType = (uint)ZLGType.VCI_PCI9840;
                    break;
                case 22:
                    unCanType = (uint)ZLGType.PCAN;
                    break;
                default:
                    break;

            }

            return unCanType;
        }

        #endregion

        #region 波特率
        private void cbBaudRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ZLGInfo.Baudrate = GetSelectBaudRate(cbBaudRate.SelectedIndex);
            SetBaudRateTimer(ZLGInfo.Baudrate);
        }

        public static BaudRate GetSelectBaudRate(int nInxex)
        {
            BaudRate br = BaudRate._500Kbps;
            switch (nInxex)
            {
                case 0:
                    br = BaudRate._5Kbps;
                    break;

                case 1:
                    br = BaudRate._10Kbps;
                    break;

                case 2:
                    br = BaudRate._20Kbps;
                    break;

                case 3:
                    br = BaudRate._40Kbps;
                    break;

                case 4:
                    br = BaudRate._50Kbps;
                    break;
                case 5:
                    br = BaudRate._80Kbps;
                    break;

                case 6:
                    br = BaudRate._100Kbps;
                    break;

                case 7:
                    br = BaudRate._125Kbps;
                    break;

                case 8:
                    br = BaudRate._200Kbps;
                    break;

                case 9:
                    br = BaudRate._250Kbps;
                    break;

                case 10:
                    br = BaudRate._400Kbps;
                    break;


                case 11:
                    br = BaudRate._500Kbps;
                    break;

                case 12:
                    br = BaudRate._666Kbps;
                    break;

                case 13:
                    br = BaudRate._800Kbps;
                    break;

                case 14:
                    br = BaudRate._1000Kbps;
                    break;
            }
            return br;
        }

        public static ArbitrationBaudRate GetSelectArbitrationBaudRate(int nInxex)
        {
            ArbitrationBaudRate abr = ArbitrationBaudRate._500Kbps;
            switch (nInxex)
            {
                case 0:
                    abr = ArbitrationBaudRate._1Mbps;
                    break;
                case 1:

                    abr = ArbitrationBaudRate._500Kbps;
                    break;

                case 2:
                    abr = ArbitrationBaudRate._500Kbps;
                    break;

                case 3:
                    abr = ArbitrationBaudRate._250Kbps;
                    break;

                case 4:
                    abr = ArbitrationBaudRate._125Kbps;
                    break;
                case 5:
                    abr = ArbitrationBaudRate._100Kbps;
                    break;

                case 6:
                    abr = ArbitrationBaudRate._50Kbps;
                    break;
            }
            return abr;
        }

        public static DataBaudRate GetSelectDataBaudRate(int nInxex)
        {
            DataBaudRate dbr = DataBaudRate._5Mbps;
            switch (nInxex)
            {
                case 0:
                    dbr = DataBaudRate._5Mbps;
                    break;

                case 1:
                    dbr = DataBaudRate._4Mbps;
                    break;

                case 2:
                    dbr = DataBaudRate._2Mbps;
                    break;

                case 3:
                    dbr = DataBaudRate._1Mbps;
                    break;
            }
            return dbr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baudRate">波特率</param>
        private void SetBaudRateTimer(BaudRate value)
        {
            switch (value)
            {
                case BaudRate._5Kbps:             
                    SetTimerBasedOnBaudRate(0xBF, 0xFF);
                    break;
                case BaudRate._10Kbps:        
                    SetTimerBasedOnBaudRate(0x31, 0x1C);
                    break;
                case BaudRate._20Kbps:                 
                    SetTimerBasedOnBaudRate(0x18, 0x1C);
                    break;
                case BaudRate._40Kbps:                     
                    SetTimerBasedOnBaudRate(0x87, 0xFF);
                    break;
                case BaudRate._50Kbps:                     
                    SetTimerBasedOnBaudRate(0x09, 0x1C);
                    break;
                case BaudRate._80Kbps:
                    SetTimerBasedOnBaudRate(0x83, 0xFF);
                    break;
                case BaudRate._100Kbps:
                    SetTimerBasedOnBaudRate(0x04, 0x1C);
                    break;
                case BaudRate._125Kbps:
                    SetTimerBasedOnBaudRate(0x03, 0x1C);
                    break;
                case BaudRate._200Kbps:
                    SetTimerBasedOnBaudRate(0x81, 0xFA);
                    break;
                case BaudRate._250Kbps:
                    SetTimerBasedOnBaudRate(0x01, 0x1C);
                    break;
                case BaudRate._400Kbps:
                    SetTimerBasedOnBaudRate(0x80, 0xFA);
                    break;
                case BaudRate._500Kbps:
                    SetTimerBasedOnBaudRate(0x00, 0x1C);
                    break;
                case BaudRate._666Kbps:
                    SetTimerBasedOnBaudRate(0x80, 0xB6);
                    break;
                case BaudRate._800Kbps:
                    SetTimerBasedOnBaudRate(0x00, 0x16);
                    break;
                case BaudRate._1000Kbps:
                    SetTimerBasedOnBaudRate(0x00, 0x14);
                    break;
                default:
                    SetTimerBasedOnBaudRate(0x00, 0x1C);
                    break;

            }
        }

        private void SetTimerBasedOnBaudRate(byte timer0, byte timer1)
        {
            ZLGInfo.Timing0 = timer0;
            ZLGInfo.Timing1 = timer1;
            teTimer0.Text = Convert.ToString(timer0, 16).ToUpper().PadLeft(2,'0');
            teTimer1.Text = Convert.ToString(timer1, 16).ToUpper().PadLeft(2, '0');
        }
        #endregion

        private void cbClosePwd_Click(object sender, RoutedEventArgs e)
        {
            PasswordWnd wnd = new PasswordWnd("666");
            wnd.ShowDialog();
            if (wnd.isOK)
            {
                if(cbClosePwd.IsChecked == true)
                {
                    cbClosePwd.IsChecked = true;
                }
                else
                {
                    cbClosePwd.IsChecked = false;
                }
            }
            else
            {
                if (cbClosePwd.IsChecked == true)
                {
                    cbClosePwd.IsChecked = false;
                }
                else
                {
                    cbClosePwd.IsChecked = true;
                }
            }
        }

        //private void cbDebugMode_Click(object sender, RoutedEventArgs e)
        //{
        //    PasswordWnd wnd = new PasswordWnd("8888");
        //    wnd.ShowDialog();
        //    if (wnd.isOK)
        //    {
        //        if (cbDeBugMode.IsChecked == true)
        //        {
        //            cbDeBugMode.IsChecked = true;
        //        }
        //        else
        //        {
        //            cbDeBugMode.IsChecked = false;
        //        }
        //    }
        //    else
        //    {
        //        if (cbDeBugMode.IsChecked == true)
        //        {
        //            cbDeBugMode.IsChecked = false;
        //        }
        //        else
        //        {
        //            cbDeBugMode.IsChecked = true;
        //        }
        //    }
        //}
    }
}
