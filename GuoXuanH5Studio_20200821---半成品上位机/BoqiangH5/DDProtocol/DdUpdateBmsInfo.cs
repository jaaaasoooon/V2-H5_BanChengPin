using BoqiangH5Entity;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls;
using BoqiangH5Repository;
using BoqiangH5.DDProtocol;
using System.Linq;

namespace BoqiangH5
{
    public partial class UserCtrlDdBmsInfo : UserControl
    {
        int nStartAddr = 0xA200;
        static SolidColorBrush brushGreen = new SolidColorBrush(Color.FromArgb(255, 150, 255, 150));
        static SolidColorBrush brushRed = new SolidColorBrush(Color.FromArgb(255, 255, 150, 150));
        //static SolidColorBrush brushGray = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
        static SolidColorBrush brushGray = new SolidColorBrush(Colors.LightGray);
        static SolidColorBrush brushYellow = new SolidColorBrush(Colors.Yellow);
        public void DdProtocolUpdateBmsInfo(List<byte> listRecv)
        {
            if (listRecv.Count < 0xD4 || listRecv[1] != 0xD2)
            {
                return;
            }
            //int nDdByteIndex = 2;
            listRecv.RemoveAt(0);
            listRecv.RemoveAt(0);

            for (int n = 0; n < ListBmsInfo.Count; n++)
            {
                switch (ListBmsInfo[n].RegisterNum)
                {
                    case 1: 
                        UpdateUIInfo_1Byte(listRecv, ref n);
                        break;
                    case 2: 
                        UpdateUIInfo_2Byte(listRecv, n);
                        break;
                        
                    case 4: 
                        UpdateUIInfo_4Byte(listRecv, n);
                        break;
                        
                    case 8: 
                        UpdateUIInfo_8Byte(listRecv, n);
                        break;

                    case 16: // 
                        UpdateUIInfo_16Byte(listRecv, n);
                        break;
                    default:
                        break;
                }
                //nDdByteIndex += ListBmsInfo[n].RegisterNum;
            }
        }

        public void DdProtocolUpdateDeviceInfo(List<byte> listRecv)
        {
            if (listRecv[1] != 0x56)
            {
                return;
            }
            try
            {
                List<string> deviceInfoList = new List<string>();
                int offset = 2;
                byte[] array = listRecv.ToArray();
                int deviceType = ((array[offset] << 24) | (((array[offset + 1] << 16) | ((array[offset + 2] << 8) | (array[offset + 3])))));
                if (deviceType == 3)
                    ListDeviceInfo[0].StrValue = "BMS";
                else if (deviceType == 4)
                    ListDeviceInfo[0].StrValue = "MC";
                else
                    ListDeviceInfo[0].StrValue = "保留";
                deviceInfoList.Add(ListDeviceInfo[0].StrValue);
                offset += 4;
                //ListDeviceInfo[0].StrValue = BitConverter.ToUInt32(array, offset).ToString(); offset += 4;
                string slaveMasterVersion = array[offset].ToString(); offset += 1;
                string slaveVersion = array[offset].ToString(); offset += 1;
                string slaveSamllVersion = array[offset].ToString(); offset += 1;
                string reserve = array[offset].ToString(); offset += 1;
                ListDeviceInfo[1].StrValue = string.Format("{0}.{1}.{2}.{3}", slaveMasterVersion, slaveVersion, slaveSamllVersion, reserve);
                deviceInfoList.Add(ListDeviceInfo[1].StrValue);
                string slaveHardwareMasterVersion = array[offset].ToString(); offset += 1;
                string slaveHardwareVersion = array[offset].ToString(); offset += 1;
                ListDeviceInfo[2].StrValue = string.Format("{0}.{1}", slaveHardwareMasterVersion, slaveHardwareVersion);
                deviceInfoList.Add(ListDeviceInfo[2].StrValue);
                byte[] manufacturerInfoArray = new byte[16];
                Buffer.BlockCopy(array, offset, manufacturerInfoArray, 0, manufacturerInfoArray.Length);
                offset += 16;
                string manufacturerInfoStr = System.Text.Encoding.ASCII.GetString(manufacturerInfoArray);
                //ListDeviceInfo[3].StrValue = manufacturerInfoStr.Substring(0, manufacturerInfoStr.IndexOf('\0')); ;
                //manufacturerInfoStr = manufacturerInfoStr.Trim("\0\b\r\n".ToCharArray());
                ListDeviceInfo[3].StrValue = manufacturerInfoStr.Substring(0, manufacturerInfoStr.IndexOf('\0'));
                deviceInfoList.Add(ListDeviceInfo[3].StrValue);
                byte[] slaveSNArray = new byte[16];
                Buffer.BlockCopy(array, offset, slaveSNArray, 0, slaveSNArray.Length);
                offset += 16;
                string slaveSNStr = System.Text.Encoding.ASCII.GetString(slaveSNArray);
                int index = slaveSNStr.IndexOf('\0');
                if (index == -1)
                {
                    ListDeviceInfo[4].StrValue = slaveSNStr;
                }
                else
                {
                    ListDeviceInfo[4].StrValue = slaveSNStr.Substring(0, index);
                }
                deviceInfoList.Add(ListDeviceInfo[4].StrValue);
                string hardwareTypeNum = array[offset].ToString(); offset += 1;
                string CustomerTypeNum = array[offset].ToString(); offset += 1;
                ListDeviceInfo[5].StrValue = string.Format("{0}.{1}", hardwareTypeNum, CustomerTypeNum);
                deviceInfoList.Add(ListDeviceInfo[5].StrValue);

                byte[] slaveFirmwareNumber = new byte[20];
                Buffer.BlockCopy(array, offset, slaveFirmwareNumber, 0, slaveFirmwareNumber.Length);
                offset += 20;
                string slaveFirmwareNumberStr = System.Text.Encoding.ASCII.GetString(slaveFirmwareNumber);
                ListDeviceInfo[6].StrValue = slaveFirmwareNumberStr.Substring(0, slaveFirmwareNumberStr.IndexOf('\0'));
                deviceInfoList.Add(ListDeviceInfo[6].StrValue);

                byte[] slaveHardwareNumber = new byte[20];
                Buffer.BlockCopy(array, offset, slaveHardwareNumber, 0, slaveHardwareNumber.Length);
                offset += 20;
                string slaveHardwareNumberStr = System.Text.Encoding.ASCII.GetString(slaveHardwareNumber);
                ListDeviceInfo[7].StrValue = slaveHardwareNumberStr.Substring(0, slaveHardwareNumberStr.IndexOf('\0'));
                deviceInfoList.Add(ListDeviceInfo[7].StrValue);

                int programStatus = ((array[offset] << 8) | (((array[offset + 1]))));
                if (programStatus == 1)
                    ListDeviceInfo[8].StrValue = "正常应用程序，且自检完成";
                else if (programStatus == 2)
                    ListDeviceInfo[8].StrValue = "正常应用程序，未自检完成";
                else
                    ListDeviceInfo[8].StrValue = "boot loader程序";
                deviceInfoList.Add(ListDeviceInfo[8].StrValue);
                GetDeviceInfoEvent?.Invoke(this,new EventArgs<List<string>>(deviceInfoList));
                if(isMsgVisible)
                {
                    System.Windows.Forms.MessageBox.Show("读取设备信息成功！", "读取设备信息提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
                isMsgVisible = true;
            }
            catch (Exception ex) { }
            finally
            {
                isReadDevice = false;
                DdProtocol.DdInstance.m_bIsFlag = false;
            }
        }
        public event EventHandler<EventArgs<bool>> ReadDeviceOverEvent;
        private void UpdateUIInfo_1Byte(List<byte> listRecv, ref int nUiIndex)
        {
            int nByteIndex_1 = (ListBmsInfo[nUiIndex].Address - nStartAddr) * 2;
        
            switch (ListBmsInfo[nUiIndex].Address)
            {
                case 0xA200:
                    ListBmsInfo[0].StrValue = listRecv[nByteIndex_1].ToString();
                    ListBmsInfo[1].StrValue = listRecv[nByteIndex_1 + 1].ToString();
                    nUiIndex++;
                    break;
                case 0xA207:
                    ListBmsInfo[nUiIndex].StrValue = listRecv[nByteIndex_1].ToString();
                    nUiIndex++;
                    ListBmsInfo[nUiIndex].StrValue = listRecv[nByteIndex_1 + 1].ToString();
                    break;
                case 0xA262:
                    string isCharging = listRecv[nByteIndex_1].ToString();
                    if (isCharging == "1")
                        ListBmsInfo[nUiIndex].StrValue = "允许";
                    else
                        ListBmsInfo[nUiIndex].StrValue = "不允许";
                    nUiIndex++;
                    string isFeedback = listRecv[nByteIndex_1 + 1].ToString();
                    if (isFeedback == "1")
                        ListBmsInfo[nUiIndex].StrValue = "允许";
                    else
                        ListBmsInfo[nUiIndex].StrValue = "不允许";
                    break;
                case 0xA236:
                case 0xA237:
                    ListBmsInfo[nUiIndex].StrValue = string.Format("{0} {1}", listRecv[nByteIndex_1].ToString("X2"), listRecv[nByteIndex_1 + 1].ToString("X2"));
                    break;
                case 0xA20E:
                    ListBmsInfo[nUiIndex].StrValue = listRecv[nByteIndex_1].ToString();
                    int svu = listRecv[nByteIndex_1 + 1] & 0x03;
                    int scu = listRecv[nByteIndex_1 + 1] & 0x0C;
                    if (svu == 0)
                    {
                        var item1 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA233");
                        item1.Unit = "mV";
                        var item2 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA235");
                        item2.Unit = "mV";
                        //var item3 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA253");
                        //item3.Unit = "mV";
                    }
                    else if (svu == 1)
                    {
                        var item1 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA233");
                        item1.Unit = "V";
                        var item2 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA235");
                        item2.Unit = "V";
                        //var item3 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA253");
                        //item3.Unit = "V";
                    }

                    if (scu == 0)
                    {
                        var item1 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA232");
                        item1.Unit = "mA";
                        var item2 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA234");
                        item2.Unit = "mA";
                        //var item3 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA255");
                        //item3.Unit = "mA";
                        var item4 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA263");
                        item4.Unit = "mA";
                    }
                    else if (scu == 1)
                    {
                        var item1 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA232");
                        item1.Unit = "A";
                        var item2 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA234");
                        item2.Unit = "A";
                        //var item3 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA255");
                        //item3.Unit = "A";
                        var item4 = ListBmsInfo.FirstOrDefault(p => p.AddressStr == "0xA263");
                        item4.Unit = "A";
                    }
                    break;
                case 0xA264:
                    string status = listRecv[nByteIndex_1 + 1].ToString("X2");
                    switch (status)
                    {
                        case "00":
                            ListBmsInfo[nUiIndex].StrValue = status + " 初始状态";
                            break;
                        case "01":
                            ListBmsInfo[nUiIndex].StrValue = status + " 还车状态";
                            break;
                        case "02":
                            ListBmsInfo[nUiIndex].StrValue = status + " 借车状态";
                            break;
                        case "03":
                            ListBmsInfo[nUiIndex].StrValue = status + " 低功耗状态";
                            break;
                        case "04":
                            ListBmsInfo[nUiIndex].StrValue = status + " 正常充电状态";
                            break;
                        case "05":
                            ListBmsInfo[nUiIndex].StrValue = status + " 充电柜搁置状态";
                            break;
                        case "06":
                            ListBmsInfo[nUiIndex].StrValue = status + " 停车桩状态";
                            break;
                        default:
                            ListBmsInfo[nUiIndex].StrValue = status;
                            break;
                    }

                    break;
                case 0xA267:
                    string supplier = string.Empty;
                    int val = (listRecv[nByteIndex_1] & 0xF0) >> 4;
                    if (val == 1) supplier = "星恒";
                    else if (val == 2) supplier = "国轩锐能";
                    else if (val == 3) supplier = "国轩博强";
                    else if (val == 4) supplier = "BYD";
                    else if (val == 5) supplier = "ATL";
                    else if (val == 6) supplier = "飞毛腿自研";
                    else supplier = listRecv[nByteIndex_1].ToString();
                    int version1 = listRecv[nByteIndex_1] & 0x0F;
                    int version2 = (listRecv[nByteIndex_1 + 1] & 0xF0) >> 4;
                    int version3 = listRecv[nByteIndex_1 + 1] & 0x0F;
                    ListBmsInfo[nUiIndex].StrValue = string.Format("{0}{1}{2}{3}", supplier, version1, version2, version3);
                    break;
                case 0xA268:
                    string isBrake = listRecv[nByteIndex_1].ToString();
                    if (isBrake == "1")
                        ListBmsInfo[nUiIndex].StrValue = "启动";
                    else
                        ListBmsInfo[nUiIndex].StrValue = "未启动";
                    break;
                default:
                    ListBmsInfo[nUiIndex].StrValue = ((listRecv[nByteIndex_1] << 8) | listRecv[nByteIndex_1 + 1]).ToString();
                    break;
            }
        }

        private void UpdateUIInfo_2Byte(List<byte> listRecv, int nUiIndex)
        {
            int nByteIndex_2 = (ListBmsInfo[nUiIndex].Address - nStartAddr) * 2;
            
            int nRegister = ((listRecv[nByteIndex_2] << 24) | (listRecv[nByteIndex_2 + 1] << 16) | (listRecv[nByteIndex_2 + 2] << 8) | listRecv[nByteIndex_2 + 3]);
            switch (ListBmsInfo[nUiIndex].Address)
            {
                case 0xA205:  
                    ListBmsInfo[nUiIndex].StrValue = nRegister.ToString();
                    break;
                case 0xA256: 
                case 0xA25C:
                    ListBmsInfo[nUiIndex].StrValue = ((UInt16)nRegister).ToString();

                    break;
                default:    
                    ListBmsInfo[nUiIndex].StrValue = ((UInt32)nRegister).ToString();
                    break;
            }

        }

        private void UpdateUIInfo_4Byte(List<byte> listRecv, int nUiIndex)
        {
            int nByteIndex_4 = (ListBmsInfo[nUiIndex].Address - nStartAddr) * 2;
            switch (ListBmsInfo[nUiIndex].Address)
            {
                case 0xA252:
                    int nRegister1 = ((listRecv[nByteIndex_4] << 8) | (listRecv[nByteIndex_4 + 1])) ;
                    int nRegister2 = ((listRecv[nByteIndex_4 + 2] << 8) | listRecv[nByteIndex_4 + 3]);
                    int _nRegister1 = ((listRecv[nByteIndex_4 + 4] << 8) | (listRecv[nByteIndex_4 + 5]));
                    int _nRegister2 = ((listRecv[nByteIndex_4 + 6] << 8) | listRecv[nByteIndex_4 + 7]);
                    //int nRegister1 = ((listRecv[nByteIndex_4] << 24) | (listRecv[nByteIndex_4 + 1] << 16) | (listRecv[nByteIndex_4 + 2] << 8) | listRecv[nByteIndex_4 + 3]);
                    //int nRegister2 = ((listRecv[nByteIndex_4 + 4] << 24) | (listRecv[nByteIndex_4 + 5] << 16) | (listRecv[nByteIndex_4 + 6] << 8) | listRecv[nByteIndex_4 + 7]); // DataFormatConvert.BytesToInt(listRecv[nByteIndex + 2], listRecv[nByteIndex + 3]);
                    ListBmsInfo[nUiIndex].StrValue = string.Format("{0}/{1}", nRegister2, _nRegister2);
                    break;
                case 0xA258:  
                case 0xA25C:
                    int nRegister3 = ((listRecv[nByteIndex_4] << 24) | (listRecv[nByteIndex_4 + 1] << 16) | (listRecv[nByteIndex_4 + 2] << 8) | listRecv[nByteIndex_4 + 3]);
                    int nRegister4 = ((listRecv[nByteIndex_4 + 4] << 24) | (listRecv[nByteIndex_4 + 5] << 16) | (listRecv[nByteIndex_4 + 6] << 8) | listRecv[nByteIndex_4 + 7]);
                    ListBmsInfo[nUiIndex].StrValue = ((UInt32)((nRegister3 << 8) | nRegister4)).ToString();
                    break;
                default:    
                    break;
            }
        }

        private void UpdateUIInfo_8Byte(List<byte> listRecv, int nUiIndex)
        {
            int nByteIndex_8 = (ListBmsInfo[nUiIndex].Address - nStartAddr) * 2;
            if (0xA238 == ListBmsInfo[nUiIndex].Address)
            {
                byte[] byteArrStat = new byte[ListBmsInfo[nUiIndex].RegisterNum * 2];
                string strStat = null;

                for (int n = 0; n < byteArrStat.Length; n++)
                {
                    byteArrStat[n] = listRecv[nByteIndex_8 + n];
                    strStat += byteArrStat[n].ToString("X2")+" ";
                }

                isProtect = false;
                isWarning = false;
                UpdateBatteryStatusInfo(byteArrStat, listBmsStatusInfo);
                UpdateBatteryStatusInfo(byteArrStat, listBmsErrInfo);
                UpdateBatteryStatusInfo(byteArrStat, listBmsWarnInfo);
                RefreshStatusEvent?.Invoke(this, new EventArgs<List<bool>>(new List<bool>() { isWarning, isProtect }));
                byte[] byteArrBalance = new byte[2] { byteArrStat[12], byteArrStat[13] };
                
                UpdateBalanceStatus(byteArrBalance);

                ListBmsInfo[nUiIndex].StrValue = strStat;
            }
            else if (0xA240 == ListBmsInfo[nUiIndex].Address)
            {
                byte[] byCellType = new byte[ListBmsInfo[nUiIndex].RegisterNum * 2];
                
                for (int i = 0; i < ListBmsInfo[nUiIndex].RegisterNum * 2; i++)
                {
                    byCellType[i] = listRecv[nByteIndex_8 + i];
                }

                string strVal = System.Text.Encoding.ASCII.GetString(byCellType);
                strVal = strVal.Substring(0, strVal.IndexOf('\0'));

                ListBmsInfo[nUiIndex].StrValue = strVal; 
            }
            else
            {
            }
        }

        bool isWarning = false;
        bool isProtect = false;
        public event EventHandler<EventArgs<List<bool>>> RefreshStatusEvent;
        private void UpdateBatteryStatusInfo(byte[] byteArr, List<BitStatInfo> listBatInfo)
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
                    if (listBatInfo[k].IsWarning)
                    {
                        isWarning = true;
                        listBatInfo[k].BackColor = brushYellow;
                    }
                    else
                    {
                        if(listBatInfo[k].IsProtect)
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

        private void UpdateBalanceStatus(byte[] byteArr)
        {

            for (int n = 0; n < 16; n++)  
            {
                if (n < 8)
                {
                    if (0 == ((1 << n) & byteArr[0]))
                    {
                        ListCellVoltage[n].BalanceStat = BoqiangH5Entity.BalanceStatusEnum.No;
                    }
                    else
                    {
                        ListCellVoltage[n].BalanceStat = BoqiangH5Entity.BalanceStatusEnum.Yes;
                    }
                }
                else
                {
                    if (0 == ((1 << (n - 8)) & byteArr[1]))
                    {
                        ListCellVoltage[n].BalanceStat = BoqiangH5Entity.BalanceStatusEnum.No;
                    }
                    else
                    {
                        ListCellVoltage[n].BalanceStat = BoqiangH5Entity.BalanceStatusEnum.Yes;
                    }
                }

            }
      
           
        }


        private void UpdateUIInfo_16Byte(List<byte> listRecv, int nUiIndex)
        {
            int nByteIndex_16 = (ListBmsInfo[nUiIndex].Address - nStartAddr) * 2;
            if (0xA210 == ListBmsInfo[nUiIndex].Address)
            {
                for (int n = 0; n < 16; n++)  // for (int n = 16; n < 32; n++)
                {
                    ListCellVoltage[n].StrValue = ((listRecv[nByteIndex_16] << 8) | listRecv[nByteIndex_16 + 1]).ToString();
                    nByteIndex_16 += 2;
                }

            }

            else
            { }

        }

    }
}
