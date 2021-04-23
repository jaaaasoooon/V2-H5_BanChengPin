using BoqiangH5.BQProtocol;
using BoqiangH5Entity;
using BoqiangH5Repository;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace BoqiangH5
{
    public partial class UserCtrlBqBmsInfo : UserControl
    {
        static SolidColorBrush brushGreen = new SolidColorBrush(Color.FromArgb(255, 150, 255, 150));
        static SolidColorBrush brushRed = new SolidColorBrush(Color.FromArgb(255, 255, 150, 150));
        //static SolidColorBrush brushGray = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
        static SolidColorBrush brushGray = new SolidColorBrush(Colors.LightGray);
        static SolidColorBrush brushYellow = new SolidColorBrush(Colors.Yellow);

        int nBqByteIndex = 1;
   
        static byte[] btSysStatus = new byte[2];
        static byte[] btPackStatus = new byte[2];
        static byte[] btBalanceStatus = new byte[4];
        bool isWarning = false;
        bool isProtect = false;
        public void BqProtocolUpdateBmsInfo(List<byte> listRecv)
        {
            if (listRecv.Count < 0x59 || listRecv[0] != 0xA1)
            {
                return;
            }

            for (int n = 0; n < m_ListCellVoltage.Count; n++)
            {
                m_ListCellVoltage[n].StrValue = ((listRecv[nBqByteIndex] << 8) | listRecv[nBqByteIndex + 1]).ToString();
                nBqByteIndex += 2;
            }

            m_ListBmsInfo[4].StrValue = ((listRecv[nBqByteIndex] << 24) | (listRecv[nBqByteIndex + 1] << 16) |
                                       (listRecv[nBqByteIndex + 2] << 8) | listRecv[nBqByteIndex + 3]).ToString();
            nBqByteIndex += 4;

            m_ListBmsInfo[5].StrValue = ((listRecv[nBqByteIndex] << 24) | (listRecv[nBqByteIndex + 1] << 16) |
                                       (listRecv[nBqByteIndex + 2] << 8) | listRecv[nBqByteIndex + 3]).ToString();
            nBqByteIndex += 4;

        }

        public event EventHandler<EventArgs<List<bool>>> RefreshStatusEvent;
        public void BqUpdateBmsInfo(List<byte> rdBuf)
        {
            if (rdBuf[0] != 0xA1 || rdBuf.Count < 0x59)
            {
                return;
            }

            BqProtocol.bReadBqBmsResp = true;
            isWarning = false;
            isProtect = false;
            nBqByteIndex = 1;

            UpdateCellInfo(rdBuf);
            UpdateBmsInfo(rdBuf);

            UpdateStatusInfo(btSysStatus, m_ListSysStatus);
            UpdateStatusInfo(btPackStatus, m_ListBatStatus);

            UpdateBalanceStatus(btBalanceStatus, m_ListCellVoltage);
            RefreshStatusEvent?.Invoke(this, new EventArgs<List<bool>>(new List<bool>() { isWarning, isProtect }));
        }

        //bool isVoltageDiffNG = false;
        public void UpdateCellInfo(List<byte> rdBuf)
        {
            //isVoltageDiffNG = false;
            for (int n = 0; n < m_ListCellVoltage.Count; n++)
            {
                int nCellVol = 0;
                for (int m = 0; m < m_ListCellVoltage[n].ByteCount; m++)
                {
                    nCellVol = (nCellVol << 8 | rdBuf[nBqByteIndex + m]);
                }

                m_ListCellVoltage[n].StrValue = nCellVol.ToString();
                //if (m_ListCellVoltage[n].Description != "总电压" && m_ListCellVoltage[n].Description != "实时电流")
                //{
                //    if (Math.Abs(nCellVol - SelectCANWnd.m_VoltageBase) > SelectCANWnd.m_VoltageError)
                //    {
                //        isVoltageDiffNG = true;
                //    }
                //}

                nBqByteIndex += m_ListCellVoltage[n].ByteCount;
            }
        }
        //bool isTemperatureDiffNG = false;
        public void UpdateBmsInfo(List<byte> rdBuf)
        {
            //isTemperatureDiffNG = false;
            for (int i = 0; i < m_ListBmsInfo.Count; i++)
            {
                int nBmsVal = 0;
                
                if (!m_ListBmsInfo[i].Description.Contains("状态"))
                {
                    for (int j = 0; j < m_ListBmsInfo[i].ByteCount; j++)
                    {
                        nBmsVal = (nBmsVal << 8 | rdBuf[nBqByteIndex + j]);
                    }
                    //lipeng     2020.3.26  修改电芯温度1为环境温度
                    if (m_ListBmsInfo[i].Description == "环境温度")
                    {
                        //if (nBmsVal != 0)
                        {
                            double dVal = (nBmsVal - 2731) / 10.0;
                            m_ListBmsInfo[i].StrValue = dVal.ToString("F1");
                            //if (m_ListBmsInfo[i].Description == "环境温度")
                            //{
                            //    if (Math.Abs(dVal - SelectCANWnd.m_TemperatureBase) > SelectCANWnd.m_TemperatureError)
                            //    {
                            //        isTemperatureDiffNG = true;
                            //    }
                            //}
                        }
                    }
                    else if (m_ListBmsInfo[i].Description == "满充容量" || m_ListBmsInfo[i].Description == "剩余电量")
                    {
                        UInt32 cap = (UInt32)nBmsVal;
                        m_ListBmsInfo[i].StrValue = cap.ToString();
                    }
                    else if (m_ListBmsInfo[i].Description == "最高电压单体号" || m_ListBmsInfo[i].Description == "最低电压单体号")
                    {
                        UInt32 nCellIndex = (UInt32)nBmsVal + 1;
                        m_ListBmsInfo[i].StrValue = nCellIndex.ToString();
                    }
                    else if (m_ListBmsInfo[i].Description == "电芯温度1" || m_ListBmsInfo[i].Description == "电芯温度2" || m_ListBmsInfo[i].Description == "电芯温度3"
                        || m_ListBmsInfo[i].Description == "电芯温度4" || m_ListBmsInfo[i].Description == "功率温度" 
                        || m_ListBmsInfo[i].Description == "单体最大温度" || m_ListBmsInfo[i].Description == "单体最小温度")
                    {
                        //计算一个字节为正负值的问题
                        if(nBmsVal > 127)
                            m_ListBmsInfo[i].StrValue = (nBmsVal - 256).ToString();
                        else
                            m_ListBmsInfo[i].StrValue = nBmsVal.ToString();
                        //if (Math.Abs(nBmsVal - SelectCANWnd.m_TemperatureBase) > SelectCANWnd.m_TemperatureError)
                        //{
                        //    isTemperatureDiffNG = true;
                        //}
                    }
                    else
                    {
                        m_ListBmsInfo[i].StrValue = nBmsVal.ToString();
                    }
                }

                else
                {
                    string strStatus = "";
                    for (int k = 0; k < m_ListBmsInfo[i].ByteCount; k++)
                    {
                        if (m_ListBmsInfo[i].Description == "Pack状态")
                        {
                            btSysStatus[k] = rdBuf[nBqByteIndex + k];
                        }
                        else if (m_ListBmsInfo[i].Description == "电池状态")
                        {
                            btPackStatus[k] = rdBuf[nBqByteIndex + k];
                        }
                        else if (m_ListBmsInfo[i].Description == "均衡状态")
                        {
                            btBalanceStatus[k] = rdBuf[nBqByteIndex + k];
                        }

                        strStatus += rdBuf[nBqByteIndex + k].ToString("X2") + " ";
                    }
                    m_ListBmsInfo[i].StrValue = strStatus;
                }

                nBqByteIndex += m_ListBmsInfo[i].ByteCount;
            }

            m_ListBmsInfo[m_ListBmsInfo.Count - 2].StrValue = (double.Parse(m_ListBmsInfo[18].StrValue) - double.Parse(m_ListBmsInfo[20].StrValue)).ToString();
            m_ListBmsInfo[m_ListBmsInfo.Count - 1].StrValue = (double.Parse(m_ListBmsInfo[22].StrValue) - double.Parse(m_ListBmsInfo[23].StrValue)).ToString();
        }

        private void UpdateStatusInfo(byte[] byteArr, List<BitStatInfo> listBatInfo)
        {
            for (int k = 0; k < listBatInfo.Count; k++)
            {
                if (0 == ((1 << listBatInfo[k].BitIndex) & byteArr[listBatInfo[k].ByteIndex]))
                {
                    listBatInfo[k].IsSwitchOn = false;
                    //listBatInfo[k].BackColor = brushGreen; 
                    listBatInfo[k].BackColor = brushGray;
                }
                else
                {
                    listBatInfo[k].IsSwitchOn = true;
                    //listBatInfo[k].BackColor = brushRed;       
                    if(listBatInfo[k].IsWarning)
                    {
                        isWarning = true;
                        listBatInfo[k].BackColor = brushYellow;
                    }
                    else
                    {
                        if (listBatInfo[k].IsProtect)
                        {
                            isProtect = true;
                            listBatInfo[k].BackColor = brushRed;
                        }
                        else
                        {
                            listBatInfo[k].BackColor = brushGreen;
                        }
                    }
                }

            }
        }

        private void UpdateBalanceStatus(byte[] byteArr, List<H5BmsInfo> listBmsInfo)
        {
            //if (XmlHelper.m_strBmsInfoFile.Contains("16"))
            {
                for (int n = 0; n < listBmsInfo.Count; n++)
                {
                    if (n < 8)
                    {
                        if (0 == ((1 << n) & byteArr[1]))
                        {
                            listBmsInfo[n].BalanceStat = BoqiangH5Entity.BalanceStatusEnum.No;
                        }
                        else
                        {
                            listBmsInfo[n].BalanceStat = BoqiangH5Entity.BalanceStatusEnum.Yes;
                        }
                    }
                    else
                    {
                        if (0 == ((1 << (n - 8)) & byteArr[0]))
                        {
                            listBmsInfo[n].BalanceStat = BoqiangH5Entity.BalanceStatusEnum.No;
                        }
                        else
                        {
                            listBmsInfo[n].BalanceStat = BoqiangH5Entity.BalanceStatusEnum.Yes;
                        }
                    }

                }
            }

        }


        #region
        int nMcuByteIndex = 1;
        bool isHardwareAndSoftwareNG = false;
        public bool BqUpdateMcuInfo(List<byte> listRecv)
        {
            try
            {
                if (listRecv.Count < 0x85 || listRecv[0] != 0xA2)
                {
                    return false;
                }

                BqProtocol.bReadBqBmsResp = true;

                nMcuByteIndex = 1;
                isHardwareAndSoftwareNG = false;

                BqUpdateSysInfo1(listRecv);

                BqUpdateSysInfo2(listRecv);

                BqUpdateChargeInfo(listRecv);

                //MessageBox.Show("读取 MCU 参数成功！", "读取MCU提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void BqUpdateSysInfo1(List<byte> listRecv)
        {
            for (int n = 0; n < ListSysInfo1.Count; n++)
            {
                string strVal = null;
                switch (ListSysInfo1[n].ByteCount)
                {
                    case 1:
                        strVal = listRecv[nMcuByteIndex].ToString();
                        break;

                    case 2:
                        strVal = BqUpdateMcuInfo_2Byte(ListSysInfo1[n], listRecv[nMcuByteIndex], listRecv[nMcuByteIndex + 1]);
                        break;

                    case 4:
                        strVal = BqUpdateMcuInfo_4Byte(ListSysInfo1[n], listRecv, nMcuByteIndex);
                        break;

                    case 16:
                        strVal = BqUpdateMcuInfo_16Byte(listRecv, nMcuByteIndex);
                        break;
                    default:
                        break;
                }

                ListSysInfo1[n].StrValue = strVal;

                nMcuByteIndex += ListSysInfo1[n].ByteCount;
            }
        }

        private void BqUpdateSysInfo2(List<byte> listRecv)
        {
            for (int n = 0; n < ListSysInfo2.Count; n++)
            {
                string strVal2 = null;
                switch (ListSysInfo2[n].ByteCount)
                {
                    case 1:
                        strVal2 = listRecv[nMcuByteIndex].ToString();
                        break;

                    case 2:
                        strVal2 = BqUpdateMcuInfo_2Byte(ListSysInfo2[n], listRecv[nMcuByteIndex], listRecv[nMcuByteIndex + 1]);
                        break;

                    case 4:
                        strVal2 = BqUpdateMcuInfo_4Byte(ListSysInfo2[n], listRecv, nMcuByteIndex);
                        break;

                    case 16:
                        strVal2 = BqUpdateMcuInfo_16Byte(listRecv, nMcuByteIndex);
                        break;
                    default:
                        break;
                }
                ListSysInfo2[n].StrValue = strVal2;

                nMcuByteIndex += ListSysInfo2[n].ByteCount;
            }
            
            nMcuByteIndex += 5;
        }

        private void BqUpdateChargeInfo(List<byte> listRecv)
        {
            for (int n = 0; n < ListChargeInfo.Count; n++)
            {
                string strVal3 = null;
                switch (ListChargeInfo[n].ByteCount)
                {
                    case 1:
                        strVal3 = listRecv[nMcuByteIndex].ToString();
                        break;

                    case 2:
                        strVal3 = BqUpdateMcuInfo_2Byte(ListChargeInfo[n], listRecv[nMcuByteIndex], listRecv[nMcuByteIndex + 1]);
                        break;
                    default:
                        break;
                }

                ListChargeInfo[n].StrValue = strVal3;

                nMcuByteIndex += ListChargeInfo[n].ByteCount;
            }
        }

        private string BqUpdateMcuInfo_2Byte(H5BmsInfo nodeInfo, byte bt1, byte bt2)
        {
            byte[] byteVal = new byte[2] { bt1, bt2 };

            string strVal = null;
            switch (nodeInfo.Description)
            {
                case "MCU配置参数":
                    strVal = byteVal[0].ToString("X2") + byteVal[1].ToString("X2");
                    //BqUpdateMcuCfg(byteVal[0], byteVal[1]);
                    break;
                case "软件版本":
                case "硬件版本":
                    strVal = byteVal[0].ToString("X2") + "." + byteVal[1].ToString("X2");
                    if (nodeInfo.Description == "软件版本")
                    {
                        if (strVal != XmlHelper.m_strSoftwareVersion)
                        {
                            isHardwareAndSoftwareNG = true;
                        }
                    }
                    if (nodeInfo.Description == "硬件版本")
                    {
                        if (strVal != XmlHelper.m_strHardwareVersion)
                        {
                            isHardwareAndSoftwareNG = true;
                        }
                    }
                    break;

                case "设备ID":
                    strVal = byteVal[0].ToString();
                    break;

                case "序列号":
                    strVal = byteVal[0].ToString("X2") + byteVal[1].ToString("X2");
                    break;

                case "生产厂商":
                case "电池条码":
                    strVal = System.Text.Encoding.ASCII.GetString(byteVal);
                    strVal = strVal.Trim("\0".ToCharArray());
                    break;

                default:
                    strVal = ((byteVal[0] << 8) | byteVal[1]).ToString();
                    break;
            }

            return strVal;
        }

        private string BqUpdateMcuInfo_4Byte(H5BmsInfo nodeInfo, List<byte> listRecv, int nByteIndex)
        {
            string strVal = null;

            if (nodeInfo.Description == "生产日期")
            {
                int nYear = int.Parse((listRecv[nByteIndex] << 8 | listRecv[nByteIndex + 1]).ToString());
                int nMonth = int.Parse(listRecv[nByteIndex + 2].ToString());
                int nDate = int.Parse(listRecv[nByteIndex + 3].ToString());
                //StringBuilder sb = new StringBuilder();
                //sb.Append((listRecv[nByteIndex] & 0xFF).ToString("X2"));
                //sb.Append((listRecv[nByteIndex + 1] & 0xFF).ToString("X2"));
                //string nYear = sb.ToString(); 
                //int nMonth = int.Parse((listRecv[nByteIndex + 2] & 0xFF).ToString("X2"));
                //int nDate = int.Parse((listRecv[nByteIndex + 3] & 0xFF).ToString("X2"));

                if (nYear != 0 && nMonth != 0 && nDate != 0)
                {
                    //DateTime dt = new DateTime(nYear, nMonth, nDate);
                    strVal = string.Format("{0}-{1}-{2}", nYear, nMonth.ToString(), nDate.ToString());
                }
                else
                {
                    strVal = "2020-3-1";
                }
            }
            else
            {
                strVal = ((listRecv[nByteIndex] << 24) | (listRecv[nByteIndex + 1] << 16) |
                          (listRecv[nByteIndex + 2] << 8) | (listRecv[nByteIndex + 3])).ToString();
            }

            return strVal;
        }

        private string BqUpdateMcuInfo_16Byte(List<byte> listRecv, int nByteIndex)
        {
            byte[] arr = new byte[16];

            listRecv.CopyTo(nByteIndex, arr, 0, 16);

            string strVal = null;
            strVal = System.Text.Encoding.ASCII.GetString(arr);
            strVal = strVal.Trim("\0".ToCharArray());
            return strVal;
        }

        //private void BqUpdateMcuCfg(byte byHigh0, byte byLow1)
        //{
        //    byte temp = (byte)(byLow1 & 0x01);
        //    if (temp == 0x01)
        //        cbChgEnd.IsChecked = true;

        //    temp = (byte)(byLow1 & 0x02);
        //    if (temp == 0x02)
        //        cbDsgEnd.IsChecked = true;

        //    temp = (byte)(byLow1 & 0x08);
        //    if (temp == 0x08)
        //        cbEnEeprom.IsChecked = true;

        //    temp = (byte)(byHigh0 & 0x01);
        //    if (temp == 0x01)
        //        cbIsCclb.IsChecked = true;

        //    temp = (byte)(byHigh0 & 0x02);
        //    if (temp == 0x02)
        //        cbIsPreCharge.IsChecked = true;
        //}
    }
    #endregion
}
