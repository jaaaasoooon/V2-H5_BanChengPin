using BoqiangH5.BQProtocol;
using BoqiangH5Entity;
using BoqiangH5Repository;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;

namespace BoqiangH5
{
    /// <summary>
    /// UserCtrlBqBmsInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UserCtrlBqBmsInfo : UserControl
    {
        public static List<H5BmsInfo> m_ListBmsInfo = new List<H5BmsInfo>();
        public static List<H5BmsInfo> m_ListCellVoltage = new List<H5BmsInfo>();

        static List<BitStatInfo> m_ListSysStatus = new List<BitStatInfo>();
        static List<BitStatInfo> m_ListBatStatus = new List<BitStatInfo>();
        static Dictionary<string, string> m_DicManufactureCode = new Dictionary<string, string>();

        //MCU消息
        List<H5BmsInfo> ListSysInfo1 = new List<H5BmsInfo>();
        List<H5BmsInfo> ListSysInfo2 = new List<H5BmsInfo>();
        List<H5BmsInfo> ListChargeInfo = new List<H5BmsInfo>();
        List<string> SnList;
        string SnRecordPath = AppDomain.CurrentDomain.BaseDirectory + "Barcode.csv";
        public UserCtrlBqBmsInfo()
        {
            InitializeComponent();
      
            InitBqBmsInfoWnd();

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(OnTimer);
        }

        private void InitBqBmsInfoWnd()
        {
            m_ListCellVoltage.Clear();
            m_ListBmsInfo.Clear();

            string strConfigFile = XmlHelper.m_strCfgFilePath; 

            XmlHelper.LoadXmlConfig(strConfigFile, "bms_info/cell_votage_info", m_ListCellVoltage);
            XmlHelper.LoadXmlConfig(strConfigFile, "bms_info/bms_info_node", m_ListBmsInfo);

            XmlHelper.LoadBqBmsStatusConfig(strConfigFile, "system_status_info/byte_status_info/bit_status_info", m_ListSysStatus);
            XmlHelper.LoadBqBmsStatusConfig(strConfigFile, "battery_status_info/byte_status_info/bit_status_info", m_ListBatStatus);

            XmlHelper.LoadManufactureCode(strConfigFile, "manufacture_code/manufacture_code_node", m_DicManufactureCode);

            //读MCU参数
            XmlHelper.LoadXmlConfig(strConfigFile, "mcu_info/system1/mcu_node_info", ListSysInfo1);
            XmlHelper.LoadXmlConfig(strConfigFile, "mcu_info/system2/mcu_node_info", ListSysInfo2);
            XmlHelper.LoadXmlConfig(strConfigFile, "mcu_info/charge_discharge/mcu_node_info", ListChargeInfo);

            //读条码参数
            SnList = CSVFileHelper.ReadSN(SnRecordPath);
        }

        //private void cbIsUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    if ((bool)cbIsUpdate.IsChecked)
        //        BqProtocol.BqInstance.m_bIsUpdateBmsInfo = true;
        //    else
        //        BqProtocol.BqInstance.m_bIsUpdateBmsInfo = false;

        //}

        private void ucBqBmsInfo_Loaded(object sender, RoutedEventArgs e)
        {
            dgBqBmsInfo.ItemsSource = m_ListBmsInfo;
            dgBqBmsCellVoltage.ItemsSource = m_ListCellVoltage;

            listBoxSysStatus.ItemsSource = m_ListSysStatus;
            listBoxBatStatus.ItemsSource = m_ListBatStatus;
        }

        public void SetOffLineUIStatus()
        {
            SetOffLineStatus(m_ListSysStatus);
            SetOffLineStatus(m_ListBatStatus);
            epTemperatureDiff.Fill = new SolidColorBrush(Colors.LightGray);
            epVoltageDiff.Fill = new SolidColorBrush(Colors.LightGray);
            epSoftwareVersion.Fill = new SolidColorBrush(Colors.LightGray);
            epHardwareVersion.Fill = new SolidColorBrush(Colors.LightGray);
            tbVoltageDiff.Text = string.Empty;
            tbTemperatureDiff.Text = string.Empty;
            tbSoftwareVersion.Text = string.Empty;
            tbHardwareVersion.Text = string.Empty;
            LabSN.Background = new SolidColorBrush(Colors.LightGray);
            LabSN.Content = string.Empty;
        }
        private void SetOffLineStatus(List<BitStatInfo> listStatInfo)
        {
            for (int n = 0; n < listStatInfo.Count; n++)
            {
                listStatInfo[n].IsSwitchOn = false;
                listStatInfo[n].BackColor = new SolidColorBrush(Colors.DarkGray);
            }
        }

        int maxVoltageCellIndex = -1;
        int minVoltageCellIndex = -1;
        public void HandleRecvBmsInfoDataEvent(object sender, CustomRecvDataEventArgs e)
        {
            DataGridRow row1 = DataGridExtension.GetRow(dgBqBmsInfo, 0);
            for(int i = 0;i < m_ListCellVoltage.Count;i++)
            {
                DataGridRow row = DataGridExtension.GetRow(dgBqBmsCellVoltage, i);
                row.Background = row1.Background;
            }
            BqUpdateBmsInfo(e.RecvMsg);
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var soc = m_ListBmsInfo.SingleOrDefault(p => p.Description == "SOC");
                if(soc != null)
                {
                    ucBattery.Battery_Glasses(100, 0, double.Parse(soc.StrValue));
                }
                var current = m_ListCellVoltage.SingleOrDefault(p => p.Description == "实时电流");
                if(current != null){
                    tbCurrent.Text = current.StrValue;
                }
                var voltage = m_ListCellVoltage.SingleOrDefault(p => p.Description == "总电压");
                if (voltage != null)
                {
                    tbVoltage.Text = voltage.StrValue;
                }

                var maxVoltageIndex = m_ListBmsInfo.FirstOrDefault(p => p.Description == "最高电压单体号");
                if(maxVoltageIndex != null)
                {
                    int index = int.Parse(maxVoltageIndex.StrValue);
                    if(index < 1 || index > 16)
                    {
                        return;
                    }
                    maxVoltageCellIndex = index;
                    DataGridRow row = DataGridExtension.GetRow(dgBqBmsCellVoltage, index - 1);
                    row.Background = new SolidColorBrush(Colors.SkyBlue);
                }

                var minVoltageIndex = m_ListBmsInfo.FirstOrDefault(p => p.Description == "最低电压单体号");
                if (minVoltageIndex != null)
                {
                    int index = int.Parse(minVoltageIndex.StrValue);
                    if (index < 1 || index > 16)
                    {
                        return;
                    }
                    minVoltageCellIndex = index;
                    DataGridRow row = DataGridExtension.GetRow(dgBqBmsCellVoltage, index - 1);
                    row.Background = new SolidColorBrush(Colors.YellowGreen);
                }
            }));
        }

        string bmsfilePath = string.Empty;
        string cellfilePath = string.Empty;
        System.Windows.Threading.DispatcherTimer timer = null;
        FileStream _fs = null;
        StreamWriter sw = null;
        //lipeng   2020.3.26,增加BMS信息记录
        private void cbIsSaveBms_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = null;
            bmsfilePath = System.AppDomain.CurrentDomain.BaseDirectory + @"Data\Bms_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            if ((bool)cbIsSaveBms.IsChecked)
            {
                BqProtocol.BqInstance.m_bIsSaveBmsInfo = true;
                //if (!(bool)cbIsUpdate.IsChecked)
                //{
                //    cbIsUpdate.IsChecked = true;
                //    BqProtocol.BqInstance.m_bIsUpdateBmsInfo = true;
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
                    CSVFileHelper.SaveBmsORCellCSVTitle(bmsfilePath,true,m_ListBmsInfo,m_ListCellVoltage,new List<H5BmsInfo>());//保存Bms数据文件头
                }

                int _interval = SelectCANWnd.m_RecordInterval;

                //msgQueue = new Queue<string>();
                timer.Interval = new TimeSpan(0, 0, _interval);
                timer.Start();

                _fs = new FileStream(bmsfilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);

                sw = new StreamWriter(_fs, System.Text.Encoding.Default);
            }
            else
            {
                BqProtocol.BqInstance.m_bIsSaveBmsInfo = false;
                if (fs != null)
                {
                    fs.Close();
                    bmsfilePath = string.Empty;
                }
                if (timer != null)
                    timer.Stop();
                sw.Close();
                _fs.Close();
            }

        }

        private void OnTimer(object sender, EventArgs e)
        {
            string str = System.DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
            string strLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47}",
                    System.DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"), m_ListBmsInfo[0].StrValue, m_ListBmsInfo[1].StrValue, m_ListBmsInfo[2].StrValue, m_ListBmsInfo[3].StrValue, m_ListBmsInfo[4].StrValue,
                    m_ListBmsInfo[5].StrValue, m_ListCellVoltage[16].StrValue, m_ListCellVoltage[17].StrValue, m_ListBmsInfo[6].StrValue, m_ListBmsInfo[7].StrValue, m_ListBmsInfo[8].StrValue, m_ListBmsInfo[9].StrValue, m_ListBmsInfo[10].StrValue, m_ListBmsInfo[11].StrValue,
                    m_ListBmsInfo[12].StrValue, m_ListBmsInfo[13].StrValue, m_ListBmsInfo[14].StrValue, m_ListBmsInfo[15].StrValue, m_ListBmsInfo[16].StrValue, m_ListBmsInfo[17].StrValue, m_ListBmsInfo[18].StrValue,
                    m_ListBmsInfo[19].StrValue, m_ListBmsInfo[20].StrValue, m_ListBmsInfo[21].StrValue, m_ListBmsInfo[22].StrValue, m_ListBmsInfo[23].StrValue, m_ListBmsInfo[24].StrValue,
                    m_ListBmsInfo[25].StrValue, m_ListBmsInfo[26].StrValue,  m_ListBmsInfo[27].StrValue, m_ListBmsInfo[28].StrValue,m_ListCellVoltage[0].StrValue,
                    m_ListCellVoltage[1].StrValue, m_ListCellVoltage[2].StrValue,m_ListCellVoltage[3].StrValue, m_ListCellVoltage[4].StrValue, m_ListCellVoltage[5].StrValue, m_ListCellVoltage[6].StrValue, m_ListCellVoltage[7].StrValue,
                    m_ListCellVoltage[8].StrValue, m_ListCellVoltage[9].StrValue,m_ListCellVoltage[10].StrValue, m_ListCellVoltage[11].StrValue, m_ListCellVoltage[12].StrValue, m_ListCellVoltage[13].StrValue, m_ListCellVoltage[14].StrValue, m_ListCellVoltage[15].StrValue);
            if (!string.IsNullOrEmpty(strLine))
            {
                sw.WriteLine(strLine); ;
            }
        }

        ////lipeng   2020.3.26,增加单体信息记录
        //private void cbIsSaveCell_Checked(object sender, RoutedEventArgs e)
        //{
        //    FileStream fs = null;
        //    cellfilePath = System.AppDomain.CurrentDomain.BaseDirectory + @"Data\Cell_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv";
        //    FileInfo fi = new FileInfo(cellfilePath);
        //    if (!fi.Directory.Exists)
        //    {
        //        fi.Directory.Create();
        //    }
        //    if (!File.Exists(cellfilePath))
        //    {
        //        fs = File.Create(cellfilePath);//创建该文件
        //        fs.Close();
        //        CSVFileHelper.SaveBmsORCellCSVTitle(cellfilePath, false);//保存Cell数据文件头
        //    }
        //    if ((bool)cbIsSaveCell.IsChecked)
        //    {
        //        BqProtocol.BqInstance.m_bIsSaveCellInfo = true;
        //        if (!(bool)cbIsUpdate.IsChecked)
        //        {
        //            cbIsUpdate.IsChecked = true;
        //            BqProtocol.BqInstance.m_bIsUpdateBmsInfo = true;
        //        }
        //    }

        //    else
        //    {
        //        BqProtocol.BqInstance.m_bIsSaveCellInfo = false;
        //        if (fs != null)
        //        {
        //            fs.Close();
        //            bmsfilePath = string.Empty;
        //        }
        //    }
        //}

        bool isDeepSleep = false;//在下发消息命令的时候增加此bool变量，拒绝总线上的其他回复消息
        public event EventHandler DeepSleepEvent;
        private void btnDeepSleep_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(MainWindow.m_statusBarInfo.IsOnline)
            {
                isDeepSleep = false;
                BqProtocol.bReadBqBmsResp = true;
                BqProtocol.BqInstance.BQ_Shutdown();
                isDeepSleep = true;
            }
            else
            {
                MessageBox.Show("系统未连接，请连接后再进行操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public event EventHandler ShallowSleepEvent;
        bool isShallowSleep = false;//在下发消息命令的时候增加此bool变量，拒绝总线上的其他回复消息
        private void btnShallowSleep_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MainWindow.m_statusBarInfo.IsOnline)
            {
                isShallowSleep = false;
                BqProtocol.bReadBqBmsResp = true;
                BqProtocol.BqInstance.BQ_Sleep();
                isShallowSleep = true;

            }
            else
            {
                MessageBox.Show("系统未连接，请连接后再进行操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void HandleDebugEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(isDeepSleep || isShallowSleep)
            {
                BqProtocol.bReadBqBmsResp = true;
                switch (e.RecvMsg[0])
                {
                    case 0xD4:
                        {
                            DeepSleepEvent?.Invoke(this, EventArgs.Empty);//设置深休眠成功，断开连接
                            MessageBox.Show("进入深休眠状态设置成功！", "进入深休眠提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        isDeepSleep = false;
                        break;
                    case 0xD5:
                        {
                            ShallowSleepEvent?.Invoke(this, EventArgs.Empty);//设置浅休眠成功，断开连接
                            MessageBox.Show("进入浅休眠状态设置成功！", "进入浅休眠提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        isShallowSleep = false;
                        //BqProtocol.BqInstance.m_bIsShallowSleep = true;
                        //BqProtocol.BqInstance.m_bShallowSleepTime = DateTime.Now;
                        //LoadingWnd wnd = new LoadingWnd(SelectCANWnd.m_WakeInterval);
                        //wnd.ShowDialog();

                        break;
                    default:
                        break;
                }
            }
        }
        //bool isRefresh = true;
        private void dgBqBmsInfo_MouseLeftDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (cbIsUpdate.IsChecked == true)
            //{
            //    MessageBox.Show("写入制造信息前请先停止数据刷新！", "写入制造信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //int index = 0;
            //foreach (var it in m_ListBmsInfo)
            //{
            //    if (it.Description == "制造信息")
            //        break;
            //    index++;
            //}
            //DataGridCell cell = DataGridExtension.GetCell(dgBqBmsInfo, index, 2);
            //cell.IsEditing = true;

            //cell.KeyDown += SelectCell_KeyDownEvent;
        }

        bool isWriteManufacture = false;//在下发消息命令的时候增加此bool变量，拒绝总线上的其他回复消息
        private void SelectCell_KeyDownEvent(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (MainWindow.m_statusBarInfo.IsOnline)
                {
                    DataGridCell cell = sender as DataGridCell;
                    string str = (cell.Content as TextBox).Text.Trim();
                    if (m_DicManufactureCode.ContainsKey(str))
                    {
                        BqProtocol.BqInstance.BQ_WriteManufacturingInformation(str);
                        isWriteManufacture = true;
                    }
                    else
                    {
                        MessageBox.Show("输入制造信息错误，请查阅相关文档！", "写入制造信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("系统未连接，请先连接设备再设置制造信息！", "写入制造信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    tbSn.Text = string.Empty;
                }
            }
        }

        public void HandleWriteManufacturingInfoEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(isWriteManufacture)
            {
                BqProtocol.bReadBqBmsResp = true;
                if (e.RecvMsg[0] == 0xDE)
                {
                    MessageBox.Show("写入 制造信息 参数成功！", "写入制造信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    isWriteManufacture = false;
                }
            }
        }

        #region  处理条码的写入问题，需先读MCU数据，再写入MCU数据
        bool isScanSn = false;
        bool isWriteSn = false;
        bool isReadMCU = false;
        bool isWriteMCU = false;
        bool isSecondReadMCU = false;
        public void HandleRecvMcuDataEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(isReadMCU)
            {
                isScanSn = false;
                isReadMCU = false;
                if (BqUpdateMcuInfo(e.RecvMsg))//读取MCU参数成功，再进行参数下发
                {
                    isWriteSn = true;
                    ListSysInfo2[9].StrValue = tbSn.Text.Trim();
                    byte[] mcuData = new byte[256];
                    int len = 0;
                    if (!GetMcuDataBuf(mcuData, ref len))
                    {
                        MessageBox.Show("写入 条码 参数失败，请检查输入数据！", "写入 MCU 提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        LabSN.Background = new SolidColorBrush(Colors.Red);
                        LabSN.Content = "FAIL";
                        return;
                    }
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        int count = 0;
                        //BqProtocol.BqInstance.m_bIsStopCommunication = true;
                        while (isWriteSn)
                        {
                            if (count > 2)
                            {
                                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    System.Windows.MessageBox.Show("写入条码数据失败，请重新操作！", "写入条码提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                    tbSn.Text = string.Empty;
                                    LabSN.Background = new SolidColorBrush(Colors.Red);
                                    LabSN.Content = "FAIL";
                                    tbSn.Focus();

                                }), null);
                                BqProtocol.BqInstance.m_bIsStopCommunication = false;
                                break;
                            }
                            isWriteMCU = false;
                            BqProtocol.BqInstance.SendMultiFrame(mcuData, len, 0xB2);
                            isWriteMCU = true;
                            System.Threading.Thread.Sleep(500);
                            count++;
                        }
                    });
                }
                else
                {
                    //MessageBox.Show("MCU参数读取失败，请查找原因！", "读取MCU参数提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            if(isSecondReadMCU)
            {
                isSecondReadMCU = false;
                if (BqUpdateMcuInfo(e.RecvMsg))
                {
                    if(ListSysInfo2[9].StrValue == tbSn.Text.Trim())
                    {
                        BqProtocol.BqInstance.m_bIsStopCommunication = false;
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            SnList.Add(tbSn.Text.Trim());
                            Thread.Sleep(500);
                            ReadDeviceInfo();
                            LabSN.Background = new SolidColorBrush(Colors.LightGreen);
                            LabSN.Content = "PASS";
                        }), null);
                    }
                    else
                    {
                        MessageBox.Show(string.Format("读取的条码 {0} 与写入的条码 {1} 不一致，条码写入失败！", ListSysInfo2[9].StrValue, tbSn.Text.Trim()), "写入 条码 提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            LabSN.Background = new SolidColorBrush(Colors.Red);
                            LabSN.Content = "FAIL";
                        }));
                    }
                }
            }
        }

        public void HandleWriteMcuDataEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(isWriteMCU)
            {
                BqProtocol.bReadBqBmsResp = true;
                if (e.RecvMsg[0] == 0xB2 || e.RecvMsg.Count == 0x03)
                {
                    isWriteSn = false;
                    isWriteMCU = false;
                    //MessageBox.Show("写入 条码 参数成功！", "写入条码提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    BqProtocol.bReadBqBmsResp = true;
                    BqProtocol.BqInstance.ReadMcuData();
                    isSecondReadMCU = true;
                }
            }
        }
        //public event EventHandler RequireCheckVersionEvent;
        private bool GetMcuDataBuf(byte[] mcuBuf, ref int nMcuIndex)
        {
            bool bRet = false;

            try
            {
                GetListBytes(mcuBuf, ListSysInfo1, ref nMcuIndex);

                GetListBytes(mcuBuf, ListSysInfo2, ref nMcuIndex);

                nMcuIndex += 5;

                GetListBytes(mcuBuf, ListChargeInfo, ref nMcuIndex);

                bRet = true;
            }
            catch (Exception ex)
            {

            }
            return bRet;

        }

        private void GetListBytes(byte[] mcuBuf, List<H5BmsInfo> ListInfo, ref int nIndex)
        {
            for (int n = 0; n < ListInfo.Count; n++)
            {
                switch (ListInfo[n].ByteCount)
                {
                    case 1:

                        mcuBuf[nIndex] = Get1BytesValue(ListInfo[n]);

                        break;
                    case 2:
                        byte[] arr2Byte = Get2BytesValue(ListInfo[n]);
                        mcuBuf[nIndex] = arr2Byte[0];
                        mcuBuf[nIndex + 1] = arr2Byte[1];
                        break;
                    case 4:
                        byte[] arr4Byte = null;

                        arr4Byte = Get4BytesValue(ListInfo[n]);
                        for (int m = 0; m < arr4Byte.Length; m++)
                        {
                            mcuBuf[nIndex + m] = arr4Byte[m];
                        }

                        break;
                    case 8:
                        break;
                    case 16:
                        if (ListInfo[n].StrValue != null)
                        {
                            byte[] arr16Byte = System.Text.Encoding.ASCII.GetBytes(ListInfo[n].StrValue);
                            for (int k = 0; k < arr16Byte.Length; k++)
                            {
                                mcuBuf[nIndex + k] = arr16Byte[k];
                            }
                        }
                        break;
                }

                nIndex += ListInfo[n].ByteCount;
            }
        }
        private byte Get1BytesValue(H5BmsInfo nodeInfo)
        {
            byte tempVal = 0;
            if (nodeInfo.Unit == "Hex")
            {
                tempVal = Convert.ToByte(nodeInfo.StrValue, 16);
            }
            else
            {
                if (nodeInfo.MinValue < 0)
                {
                    sbyte sb = 0;
                    sbyte.TryParse(nodeInfo.StrValue, out sb);
                    tempVal = Convert.ToByte(sb & 0xFF);//sbyte.Parse(ListInfo[n].StrValue);
                }
                else
                {
                    tempVal = byte.Parse(nodeInfo.StrValue);
                }
            }

            return tempVal;
        }

        private byte[] Get2BytesValue(H5BmsInfo nodeInfo)
        {
            byte[] arr2Byte = new byte[2];
            switch (nodeInfo.Description)
            {
                case "MCU配置参数":
                case "序列号":
                case "电池化学ID":

                    if (nodeInfo.StrValue.Length == 4)
                    {
                        arr2Byte[0] = Convert.ToByte(nodeInfo.StrValue.Substring(0, 2), 16);

                        arr2Byte[1] = Convert.ToByte(nodeInfo.StrValue.Substring(2, 2), 16);
                    }
                    else
                    {
                        arr2Byte[0] = 0x00;
                        arr2Byte[1] = 0x00;
                    }
                    break;

                case "软件版本":
                case "硬件版本":

                    byte tempVal2 = 0;

                    if (nodeInfo.StrValue.IndexOf('.') >= 0)
                    {
                        string[] strVer = nodeInfo.StrValue.Split('.');

                        arr2Byte[0] = Convert.ToByte(strVer[0], 16);

                        arr2Byte[1] = Convert.ToByte(strVer[1], 16);
                    }
                    else
                    {
                        arr2Byte[0] = 0x00;
                        arr2Byte[1] = 0x00;
                    }
                    break;


                default:
                    if (nodeInfo.MinValue == 0)
                    {
                        UInt16 tempVal4 = 0;
                        UInt16.TryParse(nodeInfo.StrValue, out tempVal4);

                        arr2Byte[0] = (byte)((tempVal4 & 0xFF00) >> 8);
                        arr2Byte[1] = (byte)(tempVal4 & 0x00FF);
                    }
                    else
                    {
                        Int16 tempVal4 = 0;
                        Int16.TryParse(nodeInfo.StrValue, out tempVal4);
                        arr2Byte[0] = (byte)((tempVal4 & 0xFF00) >> 8);
                        arr2Byte[1] = (byte)(tempVal4 & 0x00FF);
                    }
                    break;
            }
            return arr2Byte;

        }

        private byte[] Get4BytesValue(H5BmsInfo nodeInfo)
        {
            byte[] arr4Byte = new byte[4];
            switch (nodeInfo.Description)
            {
                case "生产日期":

                    DateTime dtVal;
                    DateTime.TryParse(nodeInfo.StrValue, out dtVal);
                    if (dtVal == null)
                    {
                        dtVal = DateTime.Now;
                    }

                    arr4Byte[0] = (byte)((dtVal.Year & 0xFF00) >> 8);
                    arr4Byte[1] = (byte)(dtVal.Year & 0x00FF);
                    arr4Byte[2] = (byte)(dtVal.Month);
                    arr4Byte[3] = (byte)(dtVal.Day);

                    break;
                default:
                    int tempVal2 = 0;
                    int.TryParse(nodeInfo.StrValue, out tempVal2);

                    arr4Byte = BitConverter.GetBytes(tempVal2);

                    Array.Reverse(arr4Byte);

                    break;
            }
            return arr4Byte;
        }
        #endregion

        private void tbSn_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                {
                    if (MainWindow.m_statusBarInfo.IsOnline)
                    {
                        string str = tbSn.Text.Trim();
                        if (str.Length == 16)
                        {
                            if(!SnList.Contains(str))
                            {
                                isScanSn = true;
                                isSecondReadMCU = false;
                                BqProtocol.BqInstance.m_bIsStopCommunication = true;
                                epTemperatureDiff.Fill = new SolidColorBrush(Colors.LightGray);
                                epVoltageDiff.Fill = new SolidColorBrush(Colors.LightGray);
                                epSoftwareVersion.Fill = new SolidColorBrush(Colors.LightGray);
                                epHardwareVersion.Fill = new SolidColorBrush(Colors.LightGray);
                                tbVoltageDiff.Text = string.Empty;
                                tbTemperatureDiff.Text = string.Empty;
                                tbSoftwareVersion.Text = string.Empty;
                                tbHardwareVersion.Text = string.Empty;
                                System.Threading.Tasks.Task.Factory.StartNew(() =>
                                {
                                    int count = 0;
                                    while (isScanSn)
                                    {
                                        if (count > 2)
                                        {
                                            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                            {
                                                System.Windows.MessageBox.Show("读取MCU数据失败，请重新操作！", "读取MCU提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                                tbSn.Text = string.Empty;
                                                tbSn.Focus();

                                            }), null);
                                            BqProtocol.BqInstance.m_bIsStopCommunication = false;
                                            break;
                                        }
                                        BqProtocol.bReadBqBmsResp = true;
                                        BqProtocol.BqInstance.ReadMcuData();
                                        isReadMCU = true;
                                        System.Threading.Thread.Sleep(500);
                                        count++;
                                    }
                                });
                            }
                            else
                            {
                                MessageBox.Show("输入条码已存在，请检查！", "输入条码提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                ReadDeviceInfo();
                            }
                        }
                        else if(str.Length < 16)
                        {
                            MessageBox.Show("输入条码长度小于16位，请检查！", "输入条码提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("输入条码长度大于16位，请检查！", "输入条码提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("系统未连接，请先连接设备再输入条码！", "输入条码提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        tbSn.Text = string.Empty;
                    }
                }
            }
        }

        private bool IsMatchBarcodeRule(string barcode)
        {
            if (barcode.Length != 16)
                return false;
            int index = 0;
            string str1 = barcode.Substring(index, 1);index++;
            if (str1 != "B") return false;

            string str2 = barcode.Substring(index, 1);index++;
            Regex reg = new Regex(@"^[A-M]+$");
            if (!reg.IsMatch(str2)) return false;

            string str3 = barcode.Substring(index, 1); index++;
            if (!reg.IsMatch(str3)) return false;

            string str4 = barcode.Substring(index, 1); index++;
            reg = new Regex(@"^[0-9A]+$");
            if (!reg.IsMatch(str4)) return false;

            string str5 = barcode.Substring(index, 1); index++;
            reg = new Regex(@"^[1-9A-Ca-l]+$");
            if (!reg.IsMatch(str5)) return false;

            //string str6 = barcode.Substring(index, 2); index += 2;
            //str6 = "20" + str6;
            //int year;
            //if(int.TryParse(str6,out year))
            //{
            //    if(year < 2015 || year > 2020)
            //    {
            //        return false;
            //    }
            //}
            //else
            //{
            //    return false;
            //}
            //string str7 = barcode.Substring(index, 2); index += 2;
            //reg = new Regex(@"^(0[1-9]|1[0-2])$");
            //if (!reg.IsMatch(str7)) return false;

            //string str8 = barcode.Substring(index, 2); index += 2;
            //reg = new Regex(@"^((0[1-9])|((1|2)[0-9])|30|31)$");
            //if (!reg.IsMatch(str8)) return false;

            //string str9 = barcode.Substring(index, 5); index += 5;
            //reg = new Regex(@"^([0-9][0-9][0-9][0-9][0-9])$");
            //if (!reg.IsMatch(str9)) return false;

            return true;
        }
        public void ShowDetection(bool isSoftwareOK,string softwareVersion,bool isHardwareOK,string hardwareVersion)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var item = m_ListBmsInfo.FirstOrDefault(p => p.Description == "最大压差");
                if (item != null)
                {
                    int val;
                    if (int.TryParse(item.StrValue, out val))
                    {
                        if (val < SelectCANWnd.m_VoltageError)
                        {
                            epVoltageDiff.Fill = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            epVoltageDiff.Fill = new SolidColorBrush(Colors.Red);
                        }
                        tbVoltageDiff.Text = item.StrValue;
                    }
                }

                var _item = m_ListBmsInfo.FirstOrDefault(p => p.Description == "温差");
                if (_item != null)
                {
                    int val;
                    if (int.TryParse(_item.StrValue, out val))
                    {
                        if (val < SelectCANWnd.m_TemperatureError)
                        {
                            epTemperatureDiff.Fill = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            epTemperatureDiff.Fill = new SolidColorBrush(Colors.Red);
                        }
                        tbTemperatureDiff.Text = _item.StrValue;
                    }
                }

                if (isSoftwareOK)
                {
                    epSoftwareVersion.Fill = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    epSoftwareVersion.Fill = new SolidColorBrush(Colors.Red);
                }
                if (isHardwareOK)
                {
                    epHardwareVersion.Fill = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    epHardwareVersion.Fill = new SolidColorBrush(Colors.Red);
                }
                tbSoftwareVersion.Text = softwareVersion;
                tbHardwareVersion.Text = hardwareVersion;
            }));
        }

        bool readDevice = false;
        private void ReadDeviceInfo()
        {
            //if (MainWindow.m_statusBarInfo.IsOnline == true)
            {
                BqProtocol.BqInstance.m_bIsStopCommunication = true;
                readDevice = true;
                Thread.Sleep(100);
                DDProtocol.DdProtocol.DdInstance.ReadDeviceInfo();
            }
            //else
            //{
            //    MessageBox.Show("系统未连接，请先连接再进行操作！", "读设备信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }
        public void HandleRecvDeviceInfoDataEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(readDevice)
            {
                BqProtocol.BqInstance.m_bIsStopCommunication = false;
                if (e.RecvMsg[1] != 0x56 ||e.RecvMsg.Count < 0x56)
                {
                    return;
                }
                List<string> deviceInfoList = new List<string>();
                if(isRequireReadDevice)
                {
                    isRequireReadDevice = false;
                    int offset = 2;
                    byte[] array = e.RecvMsg.ToArray();
                    int deviceType = ((array[offset] << 24) | (((array[offset + 1] << 16) | ((array[offset + 2] << 8) | (array[offset + 3])))));
                    if (deviceType == 3)
                        deviceInfoList.Add("BMS"); 
                    else if (deviceType == 4)
                        deviceInfoList.Add("MC");
                    else
                        deviceInfoList.Add("保留");
                    offset += 4;
                    string slaveMasterVersion = array[offset].ToString(); offset += 1;
                    string slaveVersion = array[offset].ToString(); offset += 1;
                    string slaveSamllVersion = array[offset].ToString(); offset += 1;
                    string reserve = array[offset].ToString(); offset += 1;
                    deviceInfoList.Add(string.Format("{0}.{1}.{2}.{3}", slaveMasterVersion, slaveVersion, slaveSamllVersion, reserve));
                    string slaveHardwareMasterVersion = array[offset].ToString(); offset += 1;
                    string slaveHardwareVersion = array[offset].ToString(); offset += 1;
                    deviceInfoList.Add(string.Format("{0}.{1}", slaveHardwareMasterVersion, slaveHardwareVersion));
                    byte[] manufacturerInfoArray = new byte[16];
                    Buffer.BlockCopy(array, offset, manufacturerInfoArray, 0, manufacturerInfoArray.Length);
                    offset += 16;
                    string manufacturerInfoStr = System.Text.Encoding.ASCII.GetString(manufacturerInfoArray);
                    deviceInfoList.Add(manufacturerInfoStr.Substring(0, manufacturerInfoStr.IndexOf('\0')));
                    byte[] slaveSNArray = new byte[16];
                    Buffer.BlockCopy(array, offset, slaveSNArray, 0, slaveSNArray.Length);
                    offset += 16;
                    string slaveSNStr = System.Text.Encoding.ASCII.GetString(slaveSNArray);
                    int index = slaveSNStr.IndexOf('\0');
                    if (index == -1)
                    {
                        deviceInfoList.Add(slaveSNStr);
                    }
                    else
                    {
                        deviceInfoList.Add(slaveSNStr.Substring(0, index));;
                    }

                    string hardwareTypeNum = array[offset].ToString(); offset += 1;
                    string CustomerTypeNum = array[offset].ToString(); offset += 1;
                    deviceInfoList.Add(string.Format("{0}.{1}", hardwareTypeNum, CustomerTypeNum));

                    byte[] slaveFirmwareNumber = new byte[20];
                    Buffer.BlockCopy(array, offset, slaveFirmwareNumber, 0, slaveFirmwareNumber.Length);
                    offset += 20;
                    string slaveFirmwareNumberStr = System.Text.Encoding.ASCII.GetString(slaveFirmwareNumber);
                    deviceInfoList.Add(slaveFirmwareNumberStr.Substring(0, slaveFirmwareNumberStr.IndexOf('\0')));

                    byte[] slaveHardwareNumber = new byte[20];
                    Buffer.BlockCopy(array, offset, slaveHardwareNumber, 0, slaveHardwareNumber.Length);
                    offset += 20;
                    string slaveHardwareNumberStr = System.Text.Encoding.ASCII.GetString(slaveHardwareNumber);
                    deviceInfoList.Add(slaveHardwareNumberStr.Substring(0, slaveHardwareNumberStr.IndexOf('\0')));

                    int programStatus = ((array[offset] << 8) | (((array[offset + 1]))));
                    if (programStatus == 1)
                        deviceInfoList.Add("正常应用程序，且自检完成");
                    else if (programStatus == 2)
                        deviceInfoList.Add("正常应用程序，未自检完成");
                    else
                        deviceInfoList.Add("boot loader程序");
                    GetDeviceInfoEvent?.Invoke(this, new EventArgs<List<string>>(deviceInfoList));
                    System.Windows.Forms.MessageBox.Show("读取设备信息成功！", "读取设备信息提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
                else
                {
                    int offset = 46;
                    byte[] array = e.RecvMsg.ToArray();
                    byte[] slaveFirmwareNumber = new byte[20];
                    Buffer.BlockCopy(array, offset, slaveFirmwareNumber, 0, slaveFirmwareNumber.Length);
                    offset += 20;
                    string slaveFirmwareNumberStr = System.Text.Encoding.ASCII.GetString(slaveFirmwareNumber);
                    int index = slaveFirmwareNumberStr.IndexOf('\0');
                    if (index != -1)
                    {
                        slaveFirmwareNumberStr = slaveFirmwareNumberStr.Substring(0, index);
                    }



                    byte[] slaveHardwareNumber = new byte[20];
                    Buffer.BlockCopy(array, offset, slaveHardwareNumber, 0, slaveHardwareNumber.Length);
                    offset += 20;
                    string slaveHardwareNumberStr = System.Text.Encoding.ASCII.GetString(slaveHardwareNumber);
                    index = slaveHardwareNumberStr.IndexOf('\0');
                    if (index != -1)
                    {
                        slaveHardwareNumberStr = slaveHardwareNumberStr.Substring(0, index);
                    }
                    if (slaveFirmwareNumberStr == XmlHelper.m_strSoftwareVersion)
                    {
                        if (slaveHardwareNumberStr == XmlHelper.m_strHardwareVersion)
                        {
                            ShowDetection(true, slaveFirmwareNumberStr, true, slaveHardwareNumberStr);
                        }
                        else
                        {
                            ShowDetection(true, slaveFirmwareNumberStr, false, slaveHardwareNumberStr);
                        }
                    }
                    else
                    {
                        if (slaveHardwareNumberStr == XmlHelper.m_strHardwareVersion)
                        {
                            ShowDetection(false, slaveFirmwareNumberStr, true, slaveHardwareNumberStr);
                        }
                        else
                        {
                            ShowDetection(false, slaveFirmwareNumberStr, false, slaveHardwareNumberStr);
                        }
                    }
                    CSVFileHelper.SaveSNInfomation(SnRecordPath, tbSn.Text, slaveFirmwareNumberStr, slaveHardwareNumberStr, m_ListBmsInfo, m_ListCellVoltage);
                    tbSn.Text = string.Empty;
                    tbSn.Focus();
                }
            }
            readDevice = false;
        }

        bool isRequireReadDevice = false;
        public void RequireReadDeviceInfo()
        {
            isRequireReadDevice = true;
            ReadDeviceInfo();
        }
        public event EventHandler<EventArgs<List<string>>> GetDeviceInfoEvent;

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            if(MainWindow.m_statusBarInfo.IsOnline)
            {
                epTemperatureDiff.Fill = new SolidColorBrush(Colors.LightGray);
                epVoltageDiff.Fill = new SolidColorBrush(Colors.LightGray);
                epSoftwareVersion.Fill = new SolidColorBrush(Colors.LightGray);
                epHardwareVersion.Fill = new SolidColorBrush(Colors.LightGray);
                tbVoltageDiff.Text = string.Empty;
                tbTemperatureDiff.Text = string.Empty;
                tbSoftwareVersion.Text = string.Empty;
                tbHardwareVersion.Text = string.Empty;
                BqProtocol.BqInstance.m_bIsStopCommunication = false;
                Thread.Sleep(500);
                ReadDeviceInfo();
            }
            else
            {
                MessageBox.Show("系统未连接，请先连接设备再进行检测！", "检测提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static class DataGridExtension
    {
        /// <summary>
        /// 获取DataGrid控件单元格
        /// </summary>
        /// <param name="dataGrid">DataGrid控件</param>
        /// <param name="rowIndex">单元格所在的行号</param>
        /// <param name="columnIndex">单元格所在的列号</param>
        /// <returns>指定的单元格</returns>
        public static DataGridCell GetCell(this DataGrid dataGrid, int rowIndex, int columnIndex)
        {
            DataGridRow rowContainer = GetRow(dataGrid, rowIndex);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                if (cell == null)
                {
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[columnIndex]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                }
                return cell;
            }
            return null;
        }

        /// <summary>
        /// 获取DataGrid的行
        /// </summary>
        /// <param name="dataGrid">DataGrid控件</param>
        /// <param name="rowIndex">DataGrid行号</param>
        /// <returns>指定的行号</returns>
        public static DataGridRow GetRow(this DataGrid dataGrid, int rowIndex)
        {
            DataGridRow rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (rowContainer == null)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            }
            return rowContainer;
        }

        /// <summary>
        /// 获取父可视对象中第一个指定类型的子可视对象
        /// </summary>
        /// <typeparam name="T">可视对象类型</typeparam>
        /// <param name="parent">父可视对象</param>
        /// <returns>第一个指定类型的子可视对象</returns>
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
