using BoqiangH5.BQProtocol;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BoqiangH5
{
    public partial class UserCtrlEEPROM : UserControl
    {
        int nRomStartIndex = 1;
        public void BqUpdateRomInfo(List<byte> listRecv)
        {
            if (listRecv[0] != 0xA3 || listRecv.Count < 0x1A)
            {
                return;
            }

            BqProtocol.bReadBqBmsResp = true;

            listReadRom = listRecv;

            nRomStartIndex = 1;

            SetEepromBitStatus(listRecv);

            UpdateEepromComboBox(listRecv);

            SetTxtBox_VoltageAndTemp(listRecv);

            //BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "recv", "showEEPROM");

            MessageBox.Show("读取 EEPROM 参数成功！", "读取EEPROM提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetEepromBitStatus(List<byte> listRecv)
        {
            SetBitStatus(listRecv[nRomStartIndex], 7, cbEnpch);

            SetBitStatus(listRecv[nRomStartIndex], 6, cbEnmos);

            SetBitStatus(listRecv[nRomStartIndex], 5, cbOcpm);

            SetBitStatus(listRecv[nRomStartIndex], 4, cbBal);

            SetBitStatus(listRecv[nRomStartIndex + 1], 7, cbE0VB);

            SetBitStatus(listRecv[nRomStartIndex + 1], 5, cbUV_OP);

            SetBitStatus(listRecv[nRomStartIndex + 1], 4, cbDIS_PF);

            SetBitStatus(listRecv[nRomStartIndex + 1], 1, cbOCRA);

            SetBitStatus(listRecv[nRomStartIndex + 1], 0, cbEUVR);

        }

        private void SetBitStatus(byte byteVal, int bitIndex, ComboBox cbBit)
        {
            byte byteStatus = (byte)(byteVal & (1 << bitIndex));
            if (byteStatus == 0x00)
            {
                cbBit.SelectedIndex = 0;
            }
            else
            {
                cbBit.SelectedIndex = 1;
            }
        }

        private void UpdateEepromComboBox(List<byte> listRecv)
        {

            SetComboBoxVal(listRecv[nRomStartIndex + 1], 0x0C, cbCTL, 2);

            SetComboBoxVal(listRecv[nRomStartIndex + 2], 0x0C, cbLDRT, 2);

            SetComboBoxVal(listRecv[nRomStartIndex + 2], 0xF0, cbOVT_2_74, 4);

            SetComboBoxVal(listRecv[nRomStartIndex + 4], 0xF0, cbUVT_4_74, 4);

            SetComboBoxVal(listRecv[nRomStartIndex + 12], 0xF0, cbOCD1V_C_74, 4);

            SetComboBoxVal(listRecv[nRomStartIndex + 12], 0x0F, cbOCD1T_C_30, 0);

            SetComboBoxVal(listRecv[nRomStartIndex + 13], 0xF0, cbOCD2V_D_74, 4);

            SetComboBoxVal(listRecv[nRomStartIndex + 13], 0x0F, cbOCD2T_D_30, 0);

            SetComboBoxVal(listRecv[nRomStartIndex + 14], 0xF0, cbSCV74, 4);

            SetComboBoxVal(listRecv[nRomStartIndex + 14], 0x0F, cbSCT30, 0);

            SetComboBoxVal(listRecv[nRomStartIndex + 15], 0xF0, cbOCCV74, 4);

            SetComboBoxVal(listRecv[nRomStartIndex + 15], 0x0F, cbOCCT30, 0);

            SetComboBoxVal(listRecv[nRomStartIndex + 16], 0xC0, cbCHS76, 6);

            SetComboBoxVal(listRecv[nRomStartIndex + 16], 0x30, cbMOST54, 4);

            SetComboBoxVal(listRecv[nRomStartIndex + 16], 0x0C, cbOCRT32, 2);
        }

        private void SetComboBoxVal(byte byteVal, byte twoBitVal, ComboBox combox, byte rightShift)
        {
            byte bitVal = (byte)((byteVal & twoBitVal) >> rightShift);
            switch (bitVal)
            {
                case 0x00:
                    combox.SelectedIndex = 0;
                    break;
                case 0x01:
                    combox.SelectedIndex = 1;
                    break;
                case 0x02:
                    combox.SelectedIndex = 2;
                    break;
                case 0x03:
                    combox.SelectedIndex = 3;
                    break;
                case 0x04:
                    combox.SelectedIndex = 4;
                    break;
                case 0x05:
                    combox.SelectedIndex = 5;
                    break;
                case 0x06:
                    combox.SelectedIndex = 6;
                    break;
                case 0x07:
                    combox.SelectedIndex = 7;
                    break;
                case 0x08:
                    combox.SelectedIndex = 8;
                    break;
                case 0x09:
                    combox.SelectedIndex = 9;
                    break;
                case 0x0A:
                    combox.SelectedIndex = 10;
                    break;
                case 0x0B:
                    combox.SelectedIndex = 11;
                    break;
                case 0x0C:
                    combox.SelectedIndex = 12;
                    break;
                case 0x0D:
                    combox.SelectedIndex = 13;
                    break;
                case 0x0E:
                    combox.SelectedIndex = 14;
                    break;
                case 0x0F:
                    combox.SelectedIndex = 15;
                    break;
                default:
                    break;
            }
        }



        private void SetTxtBox_VoltageAndTemp(List<byte> listRecv)
        {
            tbOV_23.Text = ((((listRecv[nRomStartIndex + 2] & 0x03) << 8) | listRecv[nRomStartIndex + 3]) * 5).ToString();

            tbOVR_45.Text = ((((listRecv[nRomStartIndex + 4] & 0x03) << 8) | listRecv[nRomStartIndex + 5]) * 5).ToString();

            tbUV_6.Text = (listRecv[nRomStartIndex + 6] * 20).ToString();

            tbUVR_7.Text = (listRecv[nRomStartIndex + 7] * 20).ToString();

            tbBALV_8.Text = (listRecv[nRomStartIndex + 8] * 20).ToString();

            tbPREV_9.Text = (listRecv[nRomStartIndex + 9] * 20).ToString();

            tbL0V_A.Text = (listRecv[nRomStartIndex + 10] * 20).ToString();

            tbPFV_B.Text = (listRecv[nRomStartIndex + 11] * 20).ToString();

            double nRref = GetRrefVal(listReadRom[26]);

            int nRt1 = GetHighTempRt1(listRecv[nRomStartIndex + 17], nRref);
            tbOTC_11.Text = GetTempFromTable(nRt1).ToString();

            nRt1 = GetHighTempRt1(listRecv[nRomStartIndex + 18], nRref);
            tbOTCR_12.Text = GetTempFromTable(nRt1).ToString();

            nRt1 = GetLowTempRt1(listRecv[nRomStartIndex + 19], nRref);
            tbUTC_13.Text = GetTempFromTable(nRt1).ToString();    

            nRt1 = GetLowTempRt1(listRecv[nRomStartIndex + 20], nRref);
            tbUTCR_14.Text = GetTempFromTable(nRt1).ToString();   

            nRt1 = GetHighTempRt1(listRecv[nRomStartIndex + 21], nRref);
            tbOTD_15.Text = GetTempFromTable(nRt1).ToString();    

            nRt1 = GetHighTempRt1(listRecv[nRomStartIndex + 22], nRref);
            tbOTDR_16.Text = GetTempFromTable(nRt1).ToString();   

            nRt1 = GetLowTempRt1(listRecv[nRomStartIndex + 23], nRref);
            tbUTD_17.Text = GetTempFromTable(nRt1).ToString();    

            nRt1 = GetLowTempRt1(listRecv[nRomStartIndex + 24], nRref);
            tbUTDR_18.Text = GetTempFromTable(nRt1).ToString();    
        }

        private void WriteEepromData()
        {
            byte[] wrBuf = new byte[0x1A];

        }

        private void GetByteValFromComboBox(ComboBox comBox)
        {
            byte byteVal = 0x00;
            switch (comBox.SelectedIndex)
            {
                case 0:
                    byteVal = 0x00;
                    break;
                case 1:
                    byteVal = 0x01;
                    break;
                case 2:
                    byteVal = 0x02;
                    break;
                case 3:
                    byteVal = 0x03;
                    break;
                case 4:
                    byteVal = 0x04;
                    break;
                case 5:
                    byteVal = 0x05;
                    break;
                case 6:
                    byteVal = 0x06;
                    break;
                case 7:
                    byteVal = 0x07;
                    break;
                case 8:
                    byteVal = 0x08;
                    break;

                case 9:
                    byteVal = 0x09;
                    break;

                case 10:
                    byteVal = 0x0A;
                    break;
                case 11:
                    byteVal = 0x0B;
                    break;
                case 12:
                    byteVal = 0x0C;
                    break;
                case 13:
                    byteVal = 0x0D;
                    break;
                case 14:
                    byteVal = 0x0E;
                    break;
                case 15:
                    byteVal = 0x0F;
                    break;
            }
        }

        private void GetEepromByte1()
        {
            byte enpch = 0;
            if (cbEnpch.SelectedIndex == 0)
            {
                enpch = 0;
            }
            else
            {
                enpch = (0x01 << 7);
            }

            byte enmos = 0;
            if (cbEnmos.SelectedIndex == 0)
            {
                enmos = 0;
            }
            else
            {
                enmos = (0x01 << 6);
            }
        }

        private double GetRrefVal(byte TRValue)
        {
            double nRref = 6.8 + 0.05 * TRValue;
            return nRref;
        }

        private int GetHighTempRt1(byte byteVal, double dRref)
        {
            double nTemp = 0;

            nTemp = byteVal * dRref / (512 - byteVal);

            return (int)(nTemp * 1000);
        }

        private int GetLowTempRt1(byte byteVal, double dRref)
        {
            double nTemp = 0;

            nTemp = (byteVal + 256) * dRref / (256 - byteVal);

            return (int)(nTemp * 1000);

        }

        private int GetTempFromTable(int nRt1)
        {
            int nTemp = -100;

            int nAbs = Math.Abs((int)(DicTempTable[0] - nRt1));

            int low = -40;
            int high = 85;

            if (nRt1 < DicTempTable[high])
            {
                nTemp = high;
            }
            else if (nRt1 > DicTempTable[low])
            {
                nTemp = low;
            }
            else
            {
                while (low <= high)
                {
                    int middle = (low + high) >> 1;
                    int midNext = middle + 1;

                    if (nRt1 == middle)
                    {
                        nTemp = middle;
                        break;
                    }
                    else if (nRt1 < DicTempTable[middle] && nRt1 > DicTempTable[midNext])
                    {
                        int nAbs1 = Math.Abs((int)(DicTempTable[middle] - nRt1));
                        int nAbs2 = Math.Abs((int)(nRt1 - DicTempTable[middle + 1]));
                        if (nAbs1 > nAbs2)
                        {
                            nTemp = middle + 1;
                        }
                        else
                        {
                            nTemp = middle;
                        }
                        break;
                    }
                    else if (nRt1 < DicTempTable[middle])
                    {
                        low = middle + 1;
                    }
                    else if (nRt1 > DicTempTable[middle])
                    {
                        high = middle - 1;
                    }
                    else if(nRt1 == DicTempTable[middle])
                    {
                        nTemp = middle;
                        break;
                    }
                }
            }

            return nTemp;
        }

        public static int Method(int[] nums, int low, int high, int target)
        {
            while (low <= high)
            {
                int middle = (low + high) / 2;
                if (target == nums[middle])
                {
                    return middle;
                }
                else if (target > nums[middle])
                {
                    low = middle + 1;
                }
                else if (target < nums[middle])
                {
                    high = middle - 1;
                }
            }
            return -1;
        }

        /// <summary>
        /// 二分法查找，非递归方法实现,二分查找的条件是原数组有序
        /// 没有找到，返回-1；找到了，则返回索引
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="low"></param>
        /// <param name="height"></param>
        /// <param name="value"></param>
        private static int BinarySearch(int[] arr, int low, int height, int value)
        {
            if (arr == null || arr.Length == 0 || low >= height)
            {
                return -1;
            }
            int hi = height - 1;
            int lowValue = arr[low];
            int heightValue = arr[hi];
            if (lowValue > value || value > heightValue)
            {
                return -1;
            }
            int mid;
            while (low <= hi)
            {
                mid = (low + hi) >> 1;
                int item = arr[mid];
                if (item == value)
                {
                    return mid;
                }
                else if (item > value)
                {
                    hi = mid - 1;
                }
                else
                {
                    low = mid + 1;
                }
            }
            return -1;
        }


    }
}
