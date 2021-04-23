using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BoqiangH5Entity;
using BoqiangH5Repository;
using BoqiangH5.DDProtocol;
using System;
using System.IO;
using System.Windows.Media;
using System.Threading;

namespace BoqiangH5
{
    /// <summary>
    /// UserCtrlBatteryInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UserCtrlDdBmsInfo : UserControl, IHandleRecvDataEvent
    {
        List<H5BmsInfo> ListBmsInfo = new List<H5BmsInfo>();
        List<H5BmsInfo> ListCellVoltage = new List<H5BmsInfo>();
        List<H5BmsInfo> ListDeviceInfo = new List<H5BmsInfo>();
        List<H5BmsInfo> ListFeedbackInfo = new List<H5BmsInfo>();

        List<BitStatInfo> listBmsStatusInfo = new List<BitStatInfo>();
        List<BitStatInfo> listBmsErrInfo = new List<BitStatInfo>();
        List<BitStatInfo> listBmsWarnInfo = new List<BitStatInfo>();
        List<BitStatInfo> listBmsBalanceInfo = new List<BitStatInfo>();

        public UserCtrlDdBmsInfo()
        {
            InitializeComponent();

            InitDdBmsInfoWnd();

        }

        private void InitDdBmsInfoWnd()
        {
            ListBmsInfo.Clear();

            string strConfigFile = XmlHelper.m_strCfgFilePath; 
      
            XmlHelper.LoadXmlConfig(strConfigFile, "bms_info/register_node_info", ListBmsInfo);            
            XmlHelper.LoadCellVoltageConfig(strConfigFile, ListCellVoltage, "A210");
            XmlHelper.LoadXmlConfig(strConfigFile, "device_info/register_node_info", ListDeviceInfo);
            XmlHelper.LoadDdBatStatConfig(strConfigFile, "battery_status_info/register_byte/bit_status_info", listBmsStatusInfo, "内部状态");
            XmlHelper.LoadDdBatStatConfig(strConfigFile, "battery_status_info/register_byte/bit_status_info", listBmsErrInfo, "Errors");
            XmlHelper.LoadDdBatStatConfig(strConfigFile, "battery_status_info/register_byte/bit_status_info", listBmsWarnInfo, "Warring");
            XmlHelper.LoadXmlConfig(strConfigFile, "feedback_info/register_node_info", ListFeedbackInfo);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(OnTimer);

        }

        private void ucDdBmsInfo_Loaded(object sender, RoutedEventArgs e)
        {
            dgBmsRegisterStatus.ItemsSource = ListBmsInfo;
            dgBmsCellVoltage.ItemsSource = ListCellVoltage;
            dgDeviceInfo.ItemsSource = ListDeviceInfo;
            dgFeedbackInfo.ItemsSource = ListFeedbackInfo;

            listBatStatus.ItemsSource = listBmsStatusInfo;
            listBatError.ItemsSource = listBmsErrInfo;

            listBatWarning.ItemsSource = listBmsWarnInfo;
        }


        public void SetOffLineUIStatus()
        {
            SetOffLineStatus(listBmsStatusInfo);
            SetOffLineStatus(listBmsErrInfo);
            SetOffLineStatus(listBmsWarnInfo);
        }
        private void SetOffLineStatus(List<BitStatInfo> listStatInfo)
        {
            for (int n = 0; n < listStatInfo.Count; n++)
            {
                listStatInfo[n].IsSwitchOn = false;
                listStatInfo[n].BackColor = new SolidColorBrush(Colors.DarkGray);
            }
        }


        public event EventHandler<EventArgs<string>> PassSOCEvent;
        public event EventHandler<EventArgs<string>> PassUTCEvent;
        public void HandleRecvBmsInfoDataEvent(object sender, CustomRecvDataEventArgs e)
        {
            DdProtocol.bReadDdBmsResp = true;
            DdProtocolUpdateBmsInfo(e.RecvMsg);
            PassSOCEvent?.Invoke(this, new EventArgs<string>(ListBmsInfo[6].StrValue));
            PassUTCEvent?.Invoke(this, new EventArgs<string>(ListBmsInfo[24].StrValue));
        }

        public void HandleRecvDeviceInfoDataEvent(object sender,CustomRecvDataEventArgs e)
        {
            if(isReadDevice)
            {
                DdProtocol.bReadDdBmsResp = true;
                DdProtocolUpdateDeviceInfo(e.RecvMsg);
            }
        }
        //private void cbDdIsUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    if ((bool)cbDdIsUpdate.IsChecked)
        //        DdProtocol.DdInstance.m_bIsUpdateDdBmsInfo = true;
        //    else
        //        DdProtocol.DdInstance.m_bIsUpdateDdBmsInfo = false;
        //}

        public void HandleRaisePowerOnOffEvent(object sender, EventArgs e)
        {
            DdProtocol.bReadDdBmsResp = true;
            MessageBox.Show("上下电成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        string bmsfilePath = string.Empty;
        string cellfilePath = string.Empty;
        System.Windows.Threading.DispatcherTimer timer = null;
        FileStream _fs = null;
        StreamWriter sw = null;
        private void cbIsSaveBms_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = null;
            //FileStream _fs = null;
            //StreamWriter sw = null;
            bmsfilePath = System.AppDomain.CurrentDomain.BaseDirectory + @"Data\DiDiBms_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            if ((bool)cbIsSaveBms.IsChecked)
            {
                DdProtocol.DdInstance.m_bIsSaveBmsInfo = true;
                //if (!(bool)cbDdIsUpdate.IsChecked)
                //{
                //    cbDdIsUpdate.IsChecked = true;
                //    DdProtocol.DdInstance.m_bIsUpdateDdBmsInfo = true;
                //}
                FileInfo fi = new FileInfo(bmsfilePath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                if (!File.Exists(bmsfilePath))
                {
                    fs = File.Create(bmsfilePath);//创建该文件
                    fs.Close();
                    CSVFileHelper.SaveBmsORCellCSVTitle(bmsfilePath,false,ListBmsInfo,ListCellVoltage,ListDeviceInfo);//保存Bms数据文件头
                }

                //int _interval = 1000;
                //if (cbInterval.SelectedIndex == 0)
                //    _interval = 1000;
                //else if (cbInterval.SelectedIndex == 1)
                //    _interval = 2000;
                //else if (cbInterval.SelectedIndex == 2)
                //    _interval = 3000;
                //else if (cbInterval.SelectedIndex == 3)
                //    _interval = 4000;
                //else if (cbInterval.SelectedIndex == 4)
                //    _interval = 5000;
                //else
                //    _interval = 1000;

                //cbInterval.IsEnabled = false;
                //labInterval.IsEnabled = false;

                //msgQueue = new Queue<string>();
                timer.Interval = new TimeSpan(0, 0, SelectCANWnd.m_RecordInterval);
                timer.Start();

                _fs = new FileStream(bmsfilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);

                sw = new StreamWriter(_fs, System.Text.Encoding.Default);
            }
            else
            {
                DdProtocol.DdInstance.m_bIsSaveBmsInfo = false;
                if (fs != null)
                {
                    fs.Close();
                    bmsfilePath = string.Empty;
                }
                if (timer != null)
                    timer.Stop();
                sw.Close();
                _fs.Close();
                //msgQueue = null;
                //cbInterval.IsEnabled = true;
                //labInterval.IsEnabled = true;
            }

        }

        private void OnTimer(object sender, EventArgs e)
        {
            string str = System.DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
            string strLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62}",
                    System.DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"), ListBmsInfo[0].StrValue, ListBmsInfo[1].StrValue, ListBmsInfo[2].StrValue, ListBmsInfo[3].StrValue, ListBmsInfo[4].StrValue,
                    ListBmsInfo[5].StrValue, ListBmsInfo[6].StrValue, ListBmsInfo[7].StrValue, ListBmsInfo[8].StrValue, ListBmsInfo[9].StrValue, ListBmsInfo[10].StrValue, ListBmsInfo[11].StrValue,
                    ListBmsInfo[12].StrValue, ListBmsInfo[13].StrValue, ListBmsInfo[14].StrValue, ListBmsInfo[15].StrValue, ListBmsInfo[16].StrValue, ListBmsInfo[17].StrValue, ListBmsInfo[18].StrValue,
                    ListBmsInfo[19].StrValue, ListBmsInfo[20].StrValue, ListBmsInfo[21].StrValue, ListBmsInfo[22].StrValue, ListBmsInfo[23].StrValue, ListBmsInfo[24].StrValue,
                    ListBmsInfo[25].StrValue, ListBmsInfo[26].StrValue, ListBmsInfo[27].StrValue, ListBmsInfo[28].StrValue, ListBmsInfo[29].StrValue, ListBmsInfo[30].StrValue, ListBmsInfo[31].StrValue
                    , ListBmsInfo[32].StrValue, ListBmsInfo[33].StrValue, ListBmsInfo[34].StrValue, ListBmsInfo[35].StrValue, ListBmsInfo[36].StrValue, ListBmsInfo[37].StrValue, ListBmsInfo[38].StrValue,
                    ListBmsInfo[39].StrValue, ListBmsInfo[40].StrValue, ListBmsInfo[41].StrValue, ListBmsInfo[42].StrValue, ListBmsInfo[43].StrValue, ListBmsInfo[44].StrValue, ListBmsInfo[45].StrValue,
                    ListCellVoltage[0].StrValue, ListCellVoltage[1].StrValue, ListCellVoltage[2].StrValue,
                    ListCellVoltage[3].StrValue, ListCellVoltage[4].StrValue, ListCellVoltage[5].StrValue, ListCellVoltage[6].StrValue, ListCellVoltage[7].StrValue, ListCellVoltage[8].StrValue, ListCellVoltage[9].StrValue,
                    ListCellVoltage[10].StrValue, ListCellVoltage[11].StrValue, ListCellVoltage[12].StrValue, ListCellVoltage[13].StrValue, ListCellVoltage[14].StrValue, ListCellVoltage[15].StrValue);
            if (!string.IsNullOrEmpty(strLine))
            {
                sw.WriteLine(strLine); ;
            }
        }

        //bool isRequireReadDevice = false;
        //public void RequireReadDeviceMessage()
        //{
        //    isRequireReadDevice = true;
        //    Thread.Sleep(200);
        //    btnReadDevice_Clicked(null, null);
        //}
        bool isReadDevice = false;
        private void btnReadDevice_Clicked(object sender, RoutedEventArgs e)
        {
            //if(MainWindow.m_statusBarInfo.IsOnline == true)
            {
                isReadDevice = false;
                DdProtocol.DdInstance.m_bIsFlag = true;
                Thread.Sleep(1000);
                DdProtocol.DdInstance.ReadDeviceInfo();
                isReadDevice = true;
            }
            //else
            //{
            //    MessageBox.Show("系统未连接，请先连接再进行操作！", "读设备信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }
        bool isMsgVisible = true;
        public void RequireReadDeviceInfo()
        {
            isMsgVisible = false;
            btnReadDevice_Clicked(null, null);
        }
        public event EventHandler<EventArgs<List<string>>> GetDeviceInfoEvent;

        bool isReadFeedback = false;
        private void btnReadFeedback_Click(object sender, RoutedEventArgs e)
        {
            isReadFeedback = true;
            DdProtocol.DdInstance.m_bIsStopCommunication = true;
            Thread.Sleep(200);
            DdProtocol.DdInstance.DD_ReadFeedbackInfo();
        }
        public void HandleRecvDDFeedbackInfoEvent(object sender, CustomRecvDataEventArgs e)
        {
            if (isReadFeedback)
            {
                isReadFeedback = false;
                DdProtocol.DdInstance.m_bIsStopCommunication = false;
                List<byte> listRecv = e.RecvMsg;
                try
                {
                    if (listRecv.Count < 0x0A || listRecv[1] != 0x06)
                    {
                        return;
                    }
                    ListFeedbackInfo[0].StrValue = listRecv[2].ToString();
                    int value = (listRecv[4] << 8) | listRecv[5];
                    ListFeedbackInfo[1].StrValue = (value / 10.0).ToString("F1");
                    int maxValue = (listRecv[6] << 8) | listRecv[7];
                    ListFeedbackInfo[2].StrValue = (maxValue / 100.0).ToString("F2");
                    MessageBox.Show("反馈信息读取完成！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnSettingBrakeStatus_Click(object sender, RoutedEventArgs e)
        {
            byte status = Convert.ToByte(cmbBrakeStatus.SelectedIndex);

            DdProtocol.DdInstance.m_bIsStopCommunication = true;
            Thread.Sleep(200);
            DDProtocol.DdProtocol.DdInstance.DD_SettingBrakeStatus(status);
        }
        public void HandleRecvSettingBrakeStatusEvent(object sender, CustomRecvDataEventArgs e)
        {
            DdProtocol.DdInstance.m_bIsStopCommunication = false;
            MessageBox.Show("设置刹车状态成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
