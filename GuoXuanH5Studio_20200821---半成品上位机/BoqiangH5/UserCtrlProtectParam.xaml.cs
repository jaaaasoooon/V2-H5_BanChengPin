using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BoqiangH5.BQProtocol;
using BoqiangH5Entity;
using BoqiangH5Repository;

namespace BoqiangH5
{
    /// <summary>
    /// UserCtrlProtectParam.xaml 的交互逻辑
    /// </summary>
    public partial class UserCtrlProtectParam : UserControl
    {
        public static List<H5ProtectParamInfo> m_VoltageParamList = new List<H5ProtectParamInfo>();
        public static List<H5ProtectParamInfo> m_CurrentParamList = new List<H5ProtectParamInfo>();
        public static List<H5ProtectParamInfo> m_TemperatureParamList = new List<H5ProtectParamInfo>();
        public static List<H5ProtectParamInfo> m_WarningParamList = new List<H5ProtectParamInfo>();
        Timer timer;
        public UserCtrlProtectParam()
        {
            InitializeComponent();
            InitProtectParamWnd();
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimer;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Stop();
        }

        public void InitProtectParamWnd()
        {
            m_VoltageParamList.Clear();
            m_CurrentParamList.Clear();
            m_TemperatureParamList.Clear();
            m_WarningParamList.Clear();

            string strConfigFile = XmlHelper.m_strCfgFilePath;
            XmlHelper.LoadProtectParamXmlConfig(strConfigFile, "protect_param/protect_voltage_param", m_VoltageParamList);
            XmlHelper.LoadProtectParamXmlConfig(strConfigFile, "protect_param/protect_current_param", m_CurrentParamList);

            XmlHelper.LoadProtectParamXmlConfig(strConfigFile, "protect_param/protect_temperature_param", m_TemperatureParamList);
            XmlHelper.LoadProtectParamXmlConfig(strConfigFile, "protect_param/protect_didi_warning_param", m_WarningParamList);
        }

        private void ucProtectParam_Loaded(object sender, RoutedEventArgs e)
        {
            dgVoltageParam.ItemsSource = m_VoltageParamList;
            dgCurrentParam.ItemsSource = m_CurrentParamList;
            dgTemperatureParam.ItemsSource = m_TemperatureParamList;
            dgWarningParam.ItemsSource = m_WarningParamList;

        }

        private void OnTimer(object sender,EventArgs e)
        {
            if(MainWindow.m_statusBarInfo.IsOnline)
            {
                BqProtocol.BqInstance.BQ_ReadVoltageProtectParam();
            }
        }

        public void StartOrStopTimer(bool flag)
        {
            if (MainWindow.m_statusBarInfo.IsOnline)
            {
                if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
                {
                    if (flag)
                    {
                        timer.Stop();
                        BqProtocol.BqInstance.m_bIsStopCommunication = false;
                    }
                    else
                    {
                        BqProtocol.BqInstance.m_bIsStopCommunication = true;
                        timer.Start();
                        BqProtocol.BqInstance.BQ_ReadVoltageProtectParam();
                    }
                }
            }
        }
        public void HandleRecvReadProtectParamEvent(object sender, CustomRecvDataEventArgs e)
        {
            switch(e.RecvMsg[0])
            {
                case 0xAA:
                    UpdateVoltageParam(e.RecvMsg, m_VoltageParamList, 0x33, 0xAA);
                    BqProtocol.BqInstance.BQ_ReadCurrentProtectParam();
                    break;
                case 0xAB:
                    UpdateVoltageParam(e.RecvMsg, m_CurrentParamList, 0x1B, 0xAB);
                    BqProtocol.BqInstance.BQ_ReadTemperatureProtectParam();
                    break;
                case 0xAC:
                    UpdateVoltageParam(e.RecvMsg, m_TemperatureParamList, 0x1D, 0xAC);
                    BqProtocol.BqInstance.BQ_ReadWarningProtectParam();
                    break;
                case 0xAD:
                    UpdateVoltageParam(e.RecvMsg, m_WarningParamList, 0x3B, 0xAD);
                    break;
            }
        }
    }
}
