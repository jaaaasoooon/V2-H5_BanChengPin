using BoqiangH5.BQProtocol;
using BoqiangH5Entity;
using BoqiangH5Repository;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace BoqiangH5
{
    /// <summary>
    /// UserCtrlAdjust.xaml 的交互逻辑
    /// </summary>
    public partial class UserCtrlAdjust : UserControl
    {        
        List<H5BmsInfo> ListAdjustVoltage2 = new List<H5BmsInfo>();

        System.Windows.Threading.DispatcherTimer  timerRTC;

        int nStartAddr = 0xA200;
        int nCellVoltageAddr = 0xA210;

        public UserCtrlAdjust()
        {            
            InitializeComponent();
            InitAdjustWnd();

            UpdateAdjustWndStatus();
        }

        private void InitAdjustWnd()
        { 
            string strConfigFile = XmlHelper.m_strCfgFilePath; 
            if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
            {
                XmlHelper.LoadXmlConfig(strConfigFile, "bms_info/cell_votage_info", ListAdjustVoltage2);
            }
            else
            {
                XmlHelper.LoadCellVoltageConfig(strConfigFile, ListAdjustVoltage2, "A210");
            }

            dgAdjustVoltage2.ItemsSource = ListAdjustVoltage2;

            //cbIsRefresh.IsChecked = true;
            SetTimerHandshake();
        }

         private void SetTimerHandshake()
        {
            timerRTC = new System.Windows.Threading.DispatcherTimer();
            timerRTC.Tick += new EventHandler(OnTimedHandshakeEvent);
            timerRTC.Interval = new TimeSpan(0, 0, 1);
            timerRTC.Start();
        }

         private void OnTimedHandshakeEvent(Object sender, EventArgs e)
         {
            tbCurrentTime.Text = DateTime.Now.ToString();

            if (cbIsRefresh.IsChecked == true && MainWindow.m_statusBarInfo.IsOnline == true)
            {
                if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
                    BqProtocol.BqInstance.BQ_ReadRTC();
                //else
                //    DDProtocol.DdProtocol.DdInstance.Didi_ReadRTC();
            }
        }

        private void UpdateAdjustWndStatus()
        {
            if (MainWindow.m_statusBarInfo.IsOnline)
            {
                if(SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
                {
                    btnAdjustCellVol.IsEnabled = true;
                    btnAdjustTotalVol.IsEnabled = true;
                    btnAdRtCurrent.IsEnabled = true;
                    btnAdZeroCurrent.IsEnabled = true;
                    btnAdjustTemp.IsEnabled = true;
                    btnAdjustRTC.IsEnabled = true;
                }    
            }
            else
            {
                btnAdjustCellVol.IsEnabled = false;
                btnAdjustTotalVol.IsEnabled = false;
                btnAdRtCurrent.IsEnabled = false;
                btnAdZeroCurrent.IsEnabled = false;
                btnAdjustTemp.IsEnabled = false;
                btnAdjustRTC.IsEnabled = false;
            }

        }

        public void HandleAdjustWndUpdateEvent(object sender, EventArgs e)
        {
            //if (SelectCANWnd.m_H5Protocol == H5Protocol.DI_DI)
            //{
            //    if (MainWindow.m_statusBarInfo.IsOnline)
            //    {
            //        btnAdjustRTC.IsEnabled = true;
            //    }
            //    return;
            //}

            UpdateAdjustWndStatus();
        }

        public void HandleRecvBmsInfoDataEvent(object sender, CustomRecvDataEventArgs e)
        {
            if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
            {
                if (string.IsNullOrEmpty(tbRtc.Text.Trim()))
                {
                    BqProtocol.BqInstance.BQ_ReadRTC();
                }
                BqUpdateCellVoltage(e.RecvMsg);
                BqUpdateTemp(e.RecvMsg);
            }
            else
            {
                ////if (string.IsNullOrEmpty(tbRtc.Text.Trim()))
                ////{
                ////    DDProtocol.DdProtocol.DdInstance.Didi_ReadRTC();
                ////}
                DdUpdateCellVoltage(e.RecvMsg);
            }
        }

        private void DdUpdateCellVoltage(List<byte> listRecv)
        {
            if(listRecv.Count != 0xCE)
            {
                return;
            }

            int nDdByteIndex = (nCellVoltageAddr - nStartAddr) * 2;

            for (int n = 0; n < 16; n++)  // for (int n = 16; n < 32; n++)
            {
                ListAdjustVoltage2[n].StrValue = ((listRecv[nDdByteIndex] << 8) | listRecv[nDdByteIndex + 1]).ToString();
                nDdByteIndex += 2;
            }
            
        }


        private void BqUpdateCellVoltage(List<byte> listRecv)
        {
            if (listRecv[0] != 0xA1 || listRecv.Count < 0x59)
            {
                return;
            }

            int nBqByteIndex = 1;

            for (int n = 0; n < ListAdjustVoltage2.Count; n++)
            {
                int nCellVol = 0;
                for (int m = 0; m < ListAdjustVoltage2[n].ByteCount; m++)
                {
                    nCellVol = (nCellVol << 8 | listRecv[nBqByteIndex + m]);
                }

                ListAdjustVoltage2[n].StrValue = nCellVol.ToString();

                nBqByteIndex += ListAdjustVoltage2[n].ByteCount;
            }

            tbTotalVoltage.Text = ListAdjustVoltage2[ListAdjustVoltage2.Count - 2].StrValue;
            tbCurrent.Text = ListAdjustVoltage2[ListAdjustVoltage2.Count - 1].StrValue;
        }

        private void BqUpdateTemp(List<byte> listRecv)
        {
            if (listRecv[0] != 0xA1 || listRecv.Count < 0x59)
            {
                return;
            }

            double temp = ((listRecv[41] << 8) | listRecv[42]);
            double dVal = (temp - 2731) / 10.0;
            tbTemp1.Text = dVal.ToString("F1");

            temp = ((listRecv[43] << 8) | listRecv[44]);
            dVal = (temp - 2731) / 10.0;
            tbTemp2.Text = dVal.ToString("F1");

            temp = ((listRecv[45] << 8) | listRecv[46]);
            dVal = (temp - 2731) / 10.0;
            tbTemp3.Text = dVal.ToString("F1");

            tbTemp4.Text = (listRecv[67] > 127) ? (listRecv[67] - 256).ToString() : listRecv[67].ToString();
            tbTemp5.Text = (listRecv[68] > 127) ? (listRecv[68] - 256).ToString() : listRecv[68].ToString();
            tbTemp6.Text = (listRecv[69] > 127) ? (listRecv[69] - 256).ToString() : listRecv[69].ToString();
            tbTemp7.Text = (listRecv[70] > 127) ? (listRecv[70] - 256).ToString() : listRecv[70].ToString();

            tbPowerTemp.Text = listRecv[71].ToString();
            tbAmbientTemp.Text = listRecv[72].ToString();
        }

        bool isAdjustCurrent = false;
        bool isAdjustZeroCurrent = false;
        private void btnAdjustCurrent_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
                return;

            string strPatten = @"^[\-|0-9][0-9]*$";

            if (btn.Name == "btnAdRtCurrent")
            {
                if (!Regex.IsMatch(this.tbRtCurrent.Text, strPatten))
                {
                    MessageBox.Show("请输入正确的电流值！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                isAdjustCurrent = false;
                BqProtocol.BqInstance.AdjustRtCurrent(int.Parse(tbRtCurrent.Text));
                isAdjustCurrent = true;
            }

            else if (btn.Name == "btnAdZeroCurrent")
            {
                if (!Regex.IsMatch(this.tbZeroCurrent.Text, strPatten))
                {
                    MessageBox.Show("请输入正确的电流值！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                isAdjustZeroCurrent = false;
                BqProtocol.BqInstance.AdjustZeroCurrent(int.Parse(tbZeroCurrent.Text));
                isAdjustZeroCurrent = true;
            }

        }

        public void HandleAdjustRTCurrenEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(isAdjustCurrent)
            {
                BqProtocol.bReadBqBmsResp = true;
                if (e.RecvMsg[0] == 0xC2 || e.RecvMsg.Count == 0x03)
                {
                    MessageBox.Show("校准实时电流成功！", "校准提示", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("校准实时电流失败！", "校准提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                isAdjustCurrent = false;
            }
        }

        public void HandleAdjustZeroCurrenEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(isAdjustZeroCurrent)
            {
                BqProtocol.bReadBqBmsResp = true;
                if (e.RecvMsg[0] == 0xC1 || e.RecvMsg.Count == 0x03)
                {
                    MessageBox.Show("校准零点电流成功！", "校准提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("校准零点电流失败！", "校准提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                isAdjustZeroCurrent = false;
            }
        }

        bool isAdjustRTC = false;
        private void btnAdjustRTC_Click(object sender, RoutedEventArgs e)
        {
            if(MainWindow.m_statusBarInfo.IsOnline)
            {
                try
                {
                    DateTime dt = DateTime.Parse(tbCurrentTime.Text.Trim());
                    if(null != dt)
                    {
                        //if(SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
                        //{
                            isAdjustRTC = false;
                            BqProtocol.bReadBqBmsResp = true;
                            BqProtocol.BqInstance.AdjustRTC(dt);
                            isAdjustRTC = true;
                        //}
                        //else
                        //{
                        //    isAdjustRTC = false;
                        //    DDProtocol.DdProtocol.DdInstance.AdjustDidiRTC(dt);
                        //    isAdjustRTC = true;
                        //}
                    }
                    else
                    {
                        MessageBox.Show("请检查RTC时间格式是否正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("请检查RTC时间格式是否正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("系统未连接，请连接后再进行操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void HandleAdjustRTCEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(isAdjustRTC)
            {
                //if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
                //{
                    BqProtocol.bReadBqBmsResp = true;
                    if (e.RecvMsg[0] == 0xB5)
                    {
                        MessageBox.Show("校准RTC成功！", "校准提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        BqProtocol.BqInstance.BQ_ReadRTC();
                    }
                    else
                    {
                        MessageBox.Show("校准RTC失败！", "校准提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                //}
                //else
                //{
                //    DDProtocol.DdProtocol.bReadDdBmsResp = true;
                //    if (e.RecvMsg[0] == 0x10)
                //    {
                //        MessageBox.Show("校准RTC成功！", "校准提示", MessageBoxButton.OK, MessageBoxImage.Information);
                //        DDProtocol.DdProtocol.DdInstance.Didi_ReadRTC();
                //    }
                //    else
                //    {
                //        MessageBox.Show("校准RTC失败！", "校准提示", MessageBoxButton.OK, MessageBoxImage.Information);
                //    }
                //}
                isAdjustRTC = false;
            }

        }

        public static DateTime systemStartTime = new DateTime(1970, 1, 1, 8, 0, 0);
        public void HandleReadBqRTCEvent(object sender, CustomRecvDataEventArgs e)
        {
            if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
            {
                BqProtocol.bReadBqBmsResp = true;
                if (e.RecvMsg[0] == 0xA5)
                {
                    //string str = "RTC数据读取成功！";
                    string second = (e.RecvMsg[1] & 0xFF).ToString("X2");
                    string minute = (e.RecvMsg[2] & 0xFF).ToString("X2");
                    string hour = (e.RecvMsg[3] & 0xFF).ToString("X2");
                    string week = (e.RecvMsg[4] & 0xFF).ToString("X2");
                    string day = (e.RecvMsg[5] & 0xFF).ToString("X2");
                    string month = (e.RecvMsg[6] & 0xFF).ToString("X2");
                    string year = (e.RecvMsg[7] & 0xFF).ToString("X2");

                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tbRtc.Text = string.Format("{0}/{1}/{2} {3}:{4}:{5}", "20" + year, month, day, hour, minute, second);
                    }), null);

                }
                else
                {
                    string str = "RTC数据读取失败！";
                    tbRtc.Text = str;
                    //MessageBox.Show("校准RTC失败！", "校准提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            //else
            //{
            //    DDProtocol.DdProtocol.bReadDdBmsResp = true;
            //    if (e.RecvMsg[0] == 0x03 && e.RecvMsg[1] == 0x04)
            //    {
            //        int nRegister = ((e.RecvMsg[2] << 24) | (e.RecvMsg[3] << 16) | (e.RecvMsg[4] << 8) | e.RecvMsg[5]);
            //        TimeSpan ts = new TimeSpan((long)(nRegister * Math.Pow(10, 7)));
            //        tbRtc.Text = (systemStartTime + ts).ToString("yyyy/MM/dd HH:mm:ss");
            //    };
            //}

        }
        //private void cbIsRefresh_Click(object sender, RoutedEventArgs e)
        //{
        //    if (cbIsRefresh.IsChecked == true)
        //        timerRTC.Start();
        //    else
        //        timerRTC.Stop();
        //}

    }
}
