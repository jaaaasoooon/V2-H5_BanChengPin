using BoqiangH5.BQProtocol;
using BoqiangH5Entity;
using BoqiangH5Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BoqiangH5
{
    public partial class UserCtrlMCU : UserControl
    {

        int nMcuByteIndex = 1;
        public event EventHandler<EventArgs<string>> ExportMCUMsgEvent;
        StringBuilder sb = new StringBuilder();
        public void BqUpdateMcuInfo(List<byte> listRecv)
        {
            try
            {
                if (listRecv.Count < 0x85 || listRecv[0] != 0xA2)
                {
                    return;
                }

                BqProtocol.bReadBqBmsResp = true;

                nMcuByteIndex = 1;
                sb.Clear();

                BqUpdateSysInfo1(listRecv);

                BqUpdateSysInfo2(listRecv);

                BqUpdateChargeInfo(listRecv);

                if(isRequireReadMcu)
                {
                    isRequireReadMcu = false;
                    ExportMCUMsgEvent?.Invoke(this, new EventArgs<string>(sb.ToString()));
                }
                else
                    MessageBox.Show("读取 MCU 参数成功！", "读取MCU提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {                
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
                        if(ListSysInfo1[n].Description == "自放电率")
                        {
                            strVal = (listRecv[nMcuByteIndex] & 0xFF).ToString();
                        }
                        else
                        {
                            strVal = listRecv[nMcuByteIndex].ToString();
                        }
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

        private string BqUpdateMcuInfo_2Byte(H5BmsInfo nodeInfo,byte bt1, byte bt2)
        {
            byte[] byteVal = new byte[2] { bt1, bt2};

            string strVal = null;
            switch (nodeInfo.Description)
            {
                case "MCU配置参数":
                    strVal = byteVal[0].ToString("X2") + byteVal[1].ToString("X2");
                    BqUpdateMcuCfg(byteVal[0], byteVal[1]);
                    break;
                case "软件版本":
                case "硬件版本":
                    strVal = byteVal[0].ToString("X2") + "." + byteVal[1].ToString("X2");
                    sb.Append(strVal);
                    sb.Append("$");
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
                    strVal = string.Format("{0}-{1}-{2}",nYear, nMonth.ToString(),nDate.ToString());
                }
                sb.Append(strVal);
            }
            else
            {
                strVal = ((listRecv[nByteIndex] << 24) | (listRecv[nByteIndex + 1] << 16) |
                          (listRecv[nByteIndex + 2] << 8) | (listRecv[nByteIndex + 3] )).ToString();
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

        private void BqUpdateMcuCfg(byte byHigh0, byte byLow1)
        {
            byte temp = (byte)(byLow1 & 0x01);
            if (temp == 0x01)
                cbChgEnd.IsChecked = true;

            temp = (byte)(byLow1 & 0x02);
            if (temp == 0x02)
                cbDsgEnd.IsChecked = true;

            temp = (byte)(byLow1 & 0x08);
            if (temp == 0x08)
                cbEnEeprom.IsChecked = true;

            temp = (byte)(byHigh0 & 0x01);
            if (temp == 0x01)
                cbIsCclb.IsChecked = true;

            temp = (byte)(byHigh0 & 0x02);
            if (temp == 0x02)
                cbIsPreCharge.IsChecked = true;
        }
    }
}
