using BoqiangH5.BQProtocol;
using BoqiangH5Entity;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BoqiangH5
{
    /// <summary>
    /// UserCtrlEEPROM.xaml 的交互逻辑
    /// </summary>
    public partial class UserCtrlEEPROM : UserControl
    {
        List<byte> listReadRom = null;

        static Dictionary<int, int> DicTempTable = new Dictionary<int, int>();

        public UserCtrlEEPROM()
        {            
            InitializeComponent();

            InitEepromWnd();

            InitTempTable();
        }

        private void InitEepromWnd()
        {
            // 0-3
            for (int n = 5; n < 25; n++ )
            {
                cbCN30.Items.Add(n.ToString());
            }

            cbCN30.SelectedIndex = 11;
            
            UpdateEepromWnd();
        }

        private void InitTempTable()
        {
            FileStream fs = null;
            StreamReader sr = null;
            string strFilePath = System.Windows.Forms.Application.StartupPath + "\\ProtocolFiles\\Temp_103AT.txt";

            try
            {
                Encoding encoding = System.Text.Encoding.ASCII; 

                fs = new FileStream(strFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                                
                sr = new StreamReader(fs, encoding);

                //记录每次读取的一行记录
                string strLine = "";

                int nIndex = 0;
                //逐行读取数据
                while ((strLine = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(strLine))
                    {
                        continue;
                    }

                    string[] arrVal = strLine.Split(',');
                    DicTempTable.Add(int.Parse(arrVal[0]), int.Parse(arrVal[1]));
                }
            }
            catch (Exception ex)
            {

            }
        }

        public bool isSetPassword = false;
        public void UpdateEepromWnd()
        {
            if (MainWindow.m_statusBarInfo.IsOnline)
            {
                if(isSetPassword)
                {
                    if(SelectCANWnd.m_IsClosePwd)
                    {
                        btnLoadPara.IsEnabled = true;
                        btnSavePara.IsEnabled = true;
                        btnReadPara.IsEnabled = true;
                    }
                    else
                    {
                        PasswordWnd wnd = new PasswordWnd("666");
                        wnd.ShowDialog();
                        if (wnd.isOK)
                        {
                            btnLoadPara.IsEnabled = true;
                            btnSavePara.IsEnabled = true;
                            btnReadPara.IsEnabled = true;
                        }
                        else
                        {
                            btnLoadPara.IsEnabled = false;
                            btnSavePara.IsEnabled = false;
                            btnReadPara.IsEnabled = false;
                            btnWritePara.IsEnabled = false;
                        }
                    }
                }
                else
                {
                    btnLoadPara.IsEnabled = true;
                    btnSavePara.IsEnabled = true;
                    btnReadPara.IsEnabled = true;
                }
            }
            else
            {
                btnLoadPara.IsEnabled = false;
                btnSavePara.IsEnabled = false;
                btnReadPara.IsEnabled = false;
                btnWritePara.IsEnabled = false;
            }
        }
        
        private void btnLoadPara_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "程序文件(*.dat)|*.dat|所有文件(*.*)|*.*";
            ofd.FileName = System.Windows.Forms.Application.StartupPath + "\\ProtocolFiles\\H5_EEPROM_config.dat";
            bool? result = ofd.ShowDialog();
            if (result != true)
                return;

            FileStream fs = null;
            StreamReader sr = null;

            try
            { 
                Encoding encoding = System.Text.Encoding.UTF8; // System.Text.Encoding.GetEncoding(936); // System.Data.Common.GetType(filePath); //Encoding.ASCII;//

                fs = new FileStream(ofd.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                sr = new StreamReader(fs, encoding);
   
                //记录每次读取的一行记录
                string strLine = "";

                int nIndex = 0;
                //逐行读取数据
                while ((strLine = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(strLine))
                    {
                        continue;
                    }

                    string[] arrVal = strLine.Split(':');
                    switch (arrVal[0])
                    {
                        case "芯片功能":
                            break;
                        case "电池节数": // 16
                            cbCN30.Text = arrVal[1].Trim();
                            break;
                        case "预充控制": // Off
                            cbEnpch.Text = arrVal[1].Trim(); 
                            break;
                        case "充电MOS恢复控制": // On
                            cbEnmos.Text = arrVal[1].Trim(); 
                            break;
                        case "过流MOS控制": // Off
                            cbOcpm.Text = arrVal[1].Trim(); 
                            break;
                        case "平衡功能": // On
                            cbBal.Text = arrVal[1].Trim(); 
                            break;
                        case "二次过充保护使能": // On
                            cbDIS_PF.Text = arrVal[1].Trim(); 
                            break;
                        case "禁止低压充电": // On
                            cbE0VB.Text = arrVal[1].Trim(); 
                            break;
                        case "负载释放延迟": // 500mS
                            cbLDRT.Text = arrVal[1].Trim(); 
                            break;
                        case "CTL管脚控制": // Chg/Dsg-MOS
                            cbCTL.Text = arrVal[1].Trim(); 
                            break;
                        case "过流保护定时恢复": // Off
                            cbOCRA.Text = arrVal[1].Trim(); 
                            break;
                        case "负载锁定": // On
                            cbEUVR.Text = arrVal[1].Trim(); 
                            break;
                        case "欠压关闭CHG": // Off
                            cbUV_OP.Text = arrVal[1].Trim(); 
                            break;

                        case "二次过充保护电压": // 3900
                            tbPFV_B.Text = arrVal[1].Trim(); 
                            break;
                        case "二次过充保护延时": // 8
                            cbPFT_10_10.Text = arrVal[1].Trim(); 
                            break;
                        case "过压保护电压": // 3700
                            tbOV_23.Text = arrVal[1].Trim(); 
                            break;
                        case "过压保护延时": // 2s
                            cbOVT_2_74.Text = arrVal[1].Trim(); 
                            break;
                        case "过压保护释放电压": // 3450
                            tbOVR_45.Text = arrVal[1].Trim(); 
                            break;
                        case "平衡开启电压": // 3400
                            tbBALV_8.Text = arrVal[1].Trim(); 
                            break;
                        case "欠压保护电压": // 2500
                            tbUV_6.Text = arrVal[1].Trim(); 
                            break;
                        case "欠压保护延时": // 2s
                            cbUVT_4_74.Text = arrVal[1].Trim(); 
                            break;
                        case "欠压保护释放电压": // 3000
                            tbUVR_7.Text = arrVal[1].Trim(); 
                            break;
                        case "预充开启电压": // 2000
                            tbPREV_9.Text = arrVal[1].Trim(); 
                            break;
                        case "低压禁充电压": // 1500
                            tbL0V_A.Text = arrVal[1].Trim(); 
                            break;
                        case "充放电状态检测电压": // 200
                            cbCHS76.Text = arrVal[1].Trim(); 
                            break;

                        case "放电电流1保护电压": // 70
                            cbOCD1V_C_74.Text = arrVal[1].Trim(); 
                            break;
                        case "放电电流1保护延时": // 4s
                            cbOCD1T_C_30.Text = arrVal[1].Trim(); 
                            break;
                        case "放电电流2保护电压": // 100
                            cbOCD2V_D_74.Text = arrVal[1].Trim(); 
                            break;
                        case "放电电流2保护延时": // 200ms
                            cbOCD2T_D_30.Text = arrVal[1].Trim(); 
                            break;
                        case "短路保护电压": // 400
                            cbSCV74.Text = arrVal[1].Trim(); 
                            break;
                        case "短路保护延时": // 256
                            cbSCT30.Text = arrVal[1].Trim(); 
                            break;
                        case "充电过流保护电压": // 30 
                            cbOCCV74.Text = arrVal[1].Trim(); 
                            break;
                        case "充电过流保护延时": // 4s
                            cbOCCT30.Text = arrVal[1].Trim(); 
                            break;
                        case "充电MOS开启延时": // 128
                            cbMOST54.Text = arrVal[1].Trim(); 
                            break;
                        case "过流自恢复延时": // 64
                            cbOCRT32.Text = arrVal[1].Trim(); 
                            break;

                        case "充电高温保护": // 175
                            tbOTC_11.Text = arrVal[1].Trim(); 
                            break;
                        case "充电高温保护释放": // 194
                            tbOTCR_12.Text = arrVal[1].Trim(); 
                            break;
                        case "充电低温保护": // 153
                            tbUTC_13.Text = arrVal[1].Trim(); 
                            break;
                        case "充电低温保护释放": // 135
                            tbUTCR_14.Text = arrVal[1].Trim(); 
                            break;
                        case "放电高温保护": // 175
                            tbOTD_15.Text = arrVal[1].Trim(); 
                            break;
                        case "放电高温保护释放": // 194
                            tbOTDR_16.Text = arrVal[1].Trim(); 
                            break;
                        case "放电低温保护": // 185
                            tbUTD_17.Text = arrVal[1].Trim(); 
                            break;
                        case "放电低温保护释放": // 153 
                            tbUTDR_18.Text = arrVal[1].Trim(); 
                            break;
                        default:
                            break;
                    }

                }

                MessageBox.Show("参数加载成功！","加载参数提示",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSavePara_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "EEPROM 参数";
                sfd.Filter = "EEPROM 参数文件(*.dat)|*.dat|所有文件(*.*)|*.*";
                sfd.FileName = string.Format("EEPROM参数_{0:yyyyMMdd_HHmm}", DateTime.Now);

                bool? result = sfd.ShowDialog();

                if (result == true)
                {
                    FileStream fs = new FileStream(sfd.FileName, FileMode.OpenOrCreate);
                    StreamWriter sw = new StreamWriter(fs);

                    sw.WriteLine("芯片功能:");
                    sw.WriteLine("电池节数: " + cbCN30.Text.Trim());

                    sw.WriteLine("预充控制: " + cbEnpch.Text);           // Off
                    sw.WriteLine("充电MOS恢复控制: " + cbEnmos.Text);        // On
                    sw.WriteLine("过流MOS控制: " + cbOcpm.Text);         // Off
                    sw.WriteLine("平衡功能: " + cbBal.Text);          // On
                    sw.WriteLine("二次过充保护使能: " + cbDIS_PF.Text);      // On
                    sw.WriteLine("禁止低压充电: " + cbE0VB.Text);        // On
                    sw.WriteLine("负载释放延迟: " + cbLDRT.Text);        // 500mS
                    sw.WriteLine("CTL管脚控制: " + cbCTL.Text);          // Chg/Dsg-MOS
                    sw.WriteLine("过流保护定时恢复: " + cbOCRA.Text);    // Off
                    sw.WriteLine("负载锁定: " + cbEUVR.Text);            // On
                    sw.WriteLine("欠压关闭CHG: " + cbUV_OP.Text);        // Off

                    sw.WriteLine("二次过充保护电压: " + tbPFV_B.Text);        // 3900
                    sw.WriteLine("二次过充保护延时: " + cbPFT_10_10.Text);    // 8
                    sw.WriteLine("过压保护电压: " + tbOV_23.Text);            // 3700
                    sw.WriteLine("过压保护延时: " + cbOVT_2_74.Text);         // 2s
                    sw.WriteLine("过压保护释放电压: " + tbOVR_45.Text);       // 3450
                    sw.WriteLine("平衡开启电压: " + tbBALV_8.Text);           // 3400
                    sw.WriteLine("欠压保护电压: " + tbUV_6.Text);             // 2500
                    sw.WriteLine("欠压保护延时: " + cbUVT_4_74.Text);         // 2s
                    sw.WriteLine("欠压保护释放电压: " + tbUVR_7.Text);        // 3000
                    sw.WriteLine("预充开启电压: " + tbPREV_9.Text);           // 2000
                    sw.WriteLine("低压禁充电压: " + tbL0V_A.Text);            // 1500
                    sw.WriteLine("充放电状态检测电压: " + cbCHS76.Text);      // 200

                    sw.WriteLine("放电电流1保护电压: " + cbOCD1V_C_74.Text);    // 70
                    sw.WriteLine("放电电流1保护延时: " + cbOCD1T_C_30.Text);    // 4s
                    sw.WriteLine("放电电流2保护电压: " + cbOCD2V_D_74.Text);    // 100
                    sw.WriteLine("放电电流2保护延时: " + cbOCD2T_D_30.Text);    // 200ms
                    sw.WriteLine("短路保护电压: " + cbSCV74.Text);              // 400
                    sw.WriteLine("短路保护延时: " + cbSCT30.Text);              // 256
                    sw.WriteLine("充电过流保护电压: " + cbOCCV74.Text);         // 30
                    sw.WriteLine("充电过流保护延时: " + cbOCCT30.Text);         // 4s
                    sw.WriteLine("充电MOS开启延时: " + cbMOST54.Text);          // 128
                    sw.WriteLine("过流自恢复延时: " + cbOCRT32.Text);           // 64

                    sw.WriteLine("充电高温保护: " + tbOTC_11.Text);             // 175
                    sw.WriteLine("充电高温保护释放: " + tbOTCR_12.Text);        // 194
                    sw.WriteLine("充电低温保护: " + tbUTC_13.Text);             // 153
                    sw.WriteLine("充电低温保护释放: " + tbUTCR_14.Text);        // 135
                    sw.WriteLine("放电高温保护: " + tbOTD_15.Text);             // 175
                    sw.WriteLine("放电高温保护释放: " + tbOTDR_16.Text);        // 194
                    sw.WriteLine("放电低温保护: " + tbUTD_17.Text);             // 185
                    sw.WriteLine("放电低温保护释放: " + tbUTDR_18.Text);        // 153

                    sw.Close();
                    fs.Close();
                    MessageBox.Show("Eeprom 参数保存成功! ", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {

            }

        }

        bool isReadEeprom = false;
        private void btnReadPara_Click(object sender, RoutedEventArgs e)
        {
            isReadEeprom = false;
            BqProtocol.bReadBqBmsResp = true;
            BqProtocol.BqInstance.m_bIsStopCommunication = true;
            BqProtocol.BqInstance.ReadEepromData();
            isReadEeprom = true;

            btnLoadPara.IsEnabled = true;
            btnWritePara.IsEnabled = true;
        }

        bool isWriteEeprom = false;
        private void btnWritePara_Click(object sender, RoutedEventArgs e)
        {
            isWriteEeprom = false;
            //if (listReadRom == null)
            //{
            //    return;
            //}

            byte[] romData = new byte[32];
            if(!GetEEPROMDataBuf(romData))
            {
                MessageBox.Show("写入 EEPROM 参数失败，请检查输入数据！", "写入 EEPROM 提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            BqProtocol.BqInstance.m_bIsStopCommunication = true;
            BqProtocol.BqInstance.SendMultiFrame(romData, 26, 0xB3);
            isWriteEeprom = true;
        }


        public void HandleEepromWndUpdateEvent(object sender, EventArgs e)
        {
            if (SelectCANWnd.m_H5Protocol == H5Protocol.DI_DI)
            {
                return;
            }

            UpdateEepromWnd();
        }

        public void HandleRecvEepromDataEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(isReadEeprom)
            {
                BqUpdateRomInfo(e.RecvMsg);
                BqProtocol.BqInstance.m_bIsStopCommunication = false;
                isReadEeprom = false;
            }
        }

        public void HandleWriteEepromDataEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(isWriteEeprom)
            {
                BqProtocol.bReadBqBmsResp = true;

                if (e.RecvMsg[0] == 0xB3 || e.RecvMsg.Count == 0x03)
                {
                    MessageBox.Show("写入 EEPROM 参数成功！", "写入 EEPROM 提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("写入 EEPROM 参数失败！", "写入 EEPROM 提示", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                isWriteEeprom = false;
                BqProtocol.BqInstance.m_bIsStopCommunication = false;
            }
        }

        private bool GetEEPROMDataBuf(byte[] romBuf)
        {
            bool bRet = false;
            //byte[] romBuf = new byte[32];
            //int nRomIndex = 0;

            try
            {
                romBuf[0] = GetByte1_00Hex();
                romBuf[1] = GetByte2_01Hex();
                romBuf[2] = GetByte3_02Hex();
                romBuf[3] = (byte)(uint.Parse(tbOV_23.Text) / 5);
                romBuf[4] = GetByte5_04Hex();
                romBuf[5] = (byte)(ushort.Parse(tbOVR_45.Text) / 5);
                romBuf[6] = (byte)(uint.Parse(tbUV_6.Text) / 20);
                romBuf[7] = (byte)(uint.Parse(tbUVR_7.Text) / 20);
                romBuf[8] = (byte)(uint.Parse(tbBALV_8.Text) / 20);
                romBuf[9] = (byte)(uint.Parse(tbPREV_9.Text) / 20);
                romBuf[10] = (byte)(uint.Parse(tbL0V_A.Text) / 20);
                romBuf[11] = (byte)(uint.Parse(tbPFV_B.Text) / 20);
                romBuf[12] = GetByte12_0CHex();
                romBuf[13] = GetByte13_0DHex();
                romBuf[14] = GetByte14_0EHex();
                romBuf[15] = GetByte15_0FHex();
                romBuf[16] = GetByte16_10Hex();

                double nRref = GetRrefVal(listReadRom[26]);

                int nRt1 = GetRt1ValFromTable(int.Parse(tbOTC_11.Text));
                romBuf[17] = GetHighTempByteVal(nRref, nRt1);  // (byte.Parse(tbOTC_11.Text)); 

                nRt1 = GetRt1ValFromTable(int.Parse(tbOTCR_12.Text));
                romBuf[18] = GetHighTempByteVal(nRref, nRt1);  // (byte.Parse(tbOTCR_12.Text));

                nRt1 = GetRt1ValFromTable(int.Parse(tbUTC_13.Text));
                romBuf[19] = GetLowTempByteVal(nRref, nRt1);  // (byte.Parse(tbUTC_13.Text));

                nRt1 = GetRt1ValFromTable(int.Parse(tbUTCR_14.Text));
                romBuf[20] = GetLowTempByteVal(nRref, nRt1);  // (byte.Parse(tbUTCR_14.Text));

                nRt1 = GetRt1ValFromTable(int.Parse(tbOTD_15.Text));
                romBuf[21] = GetHighTempByteVal(nRref, nRt1);  // (byte.Parse(tbOTD_15.Text));

                nRt1 = GetRt1ValFromTable(int.Parse(tbOTDR_16.Text));
                romBuf[22] = GetHighTempByteVal(nRref, nRt1);  // (byte.Parse(tbOTDR_16.Text));

                nRt1 = GetRt1ValFromTable(int.Parse(tbUTD_17.Text));
                romBuf[23] = GetLowTempByteVal(nRref, nRt1);  // (byte.Parse(tbUTD_17.Text));

                nRt1 = GetRt1ValFromTable(int.Parse(tbUTDR_18.Text));
                romBuf[24] = GetLowTempByteVal(nRref, nRt1);  // (byte.Parse(tbUTDR_18.Text));

                romBuf[25] = listReadRom[26];


                bRet = true;
            }
            catch (Exception ex)
            {
                bRet = false;
            }
            return bRet;
        }

        private byte GetByte1_00Hex()
        {
            byte byteVal = 0;

            byteVal = (byte)(byteVal | (GetBitOnOffVal(cbEnpch) << 7));
            byteVal = (byte)(byteVal | (GetBitOnOffVal(cbEnmos) << 6));

            byteVal = (byte)(byteVal | (GetBitOnOffVal(cbOcpm) << 5));
            byteVal = (byte)(byteVal | (GetBitOnOffVal(cbBal) << 4));

            byteVal = (byte)(byteVal | GetCellNum());
            return byteVal;
        }
        
        private byte GetByte2_01Hex()
        {
            byte byteVal = 0;

            byteVal = (byte)(byteVal | (GetBitOnOffVal(cbE0VB) << 7));
            byteVal = (byte)(byteVal | 0x40);  // Reserved

            byteVal = (byte)(byteVal | (GetBitOnOffVal(cbUV_OP) << 5));
            byteVal = (byte)(byteVal | (GetBitOnOffVal(cbDIS_PF) << 4));

            byteVal = (byte)(byteVal | (GetBitOnOffVal(cbOCRA) << 1));
            byteVal = (byte)(byteVal | (GetBitOnOffVal(cbEUVR) << 0));

            byteVal = (byte)(byteVal | (GetCTLCVal() << 2));
            return byteVal;
        }

        private byte GetByte3_02Hex()
        {
            byte byteVal = 0;

            //cbOVT_2_74
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbOVT_2_74) << 4));
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbLDRT) << 2));
            byteVal = (byte)(byteVal | ((int.Parse(tbOV_23.Text) / 5)>> 8));

            return byteVal;
        }

        private byte GetByte5_04Hex()
        {
            byte byteVal = 0;

            byteVal = (byte)(byteVal | (GetComboBoxVal(cbUVT_4_74) << 4));
            byteVal = (byte)(byteVal | ((int.Parse(tbOVR_45.Text) / 5) >> 8));

            return byteVal;
        }

        private byte GetByte12_0CHex()
        {
            byte byteVal = 0;
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbOCD1V_C_74) << 4));
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbOCD1T_C_30) << 0));
            return byteVal;
        }

        private byte GetByte13_0DHex()
        {
            byte byteVal = 0;
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbOCD2V_D_74) << 4));
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbOCD2T_D_30) << 0));
            return byteVal;
        }

        private byte GetByte14_0EHex()
        {
            byte byteVal = 0;
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbSCV74) << 4));
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbSCT30) << 0));
            return byteVal;
        }

        private byte GetByte15_0FHex()
        {
            byte byteVal = 0;
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbOCCV74) << 4));
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbOCCT30) << 0));
            return byteVal;
        }

        private byte GetByte16_10Hex()
        {
            byte byteVal = 0;
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbCHS76) << 6));
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbMOST54) << 4));
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbOCRT32) << 2));
            byteVal = (byte)(byteVal | (GetComboBoxVal(cbPFT_10_10) << 0));
            return byteVal;
        }

        private byte GetCellNum()
        {
            byte cellNum = 0;

            if (cbCN30.SelectedIndex <= 10)  // (cbCN30.SelectedIndex < 7 && cbCN30.SelectedIndex > 0)
            {
                cellNum = byte.Parse(cbCN30.Text);
            }
            else
            {
                cellNum = 0;  // 16 串
            }

            return cellNum;
        }

        private byte GetBitOnOffVal(ComboBox cb)
        {
            byte val = 0;

            if(cb.SelectedIndex == 0)
            {
                val = 0;
            }
            else
            {
                val = 1;
            }

            return val;
        }

        private byte GetCTLCVal()
        {
            byte ctlVal = 0x00;
            switch (cbCTL.SelectedIndex)
            {
                case 0:
                    ctlVal = 0x00;
                    break;
                case 1:
                    ctlVal = 0x01;
                    break;
                case 2:
                    ctlVal = 0x02;
                    break;
                case 3:
                    ctlVal = 0x03;
                    break;
                default:
                    ctlVal = 0x00;
                    break;
            }

            return ctlVal;
        }

        private byte GetComboBoxVal(ComboBox cb)
        {
            byte byVal = 0x00;
            
            if (cb.SelectedIndex != -1)
            {
                byVal = (byte)cb.SelectedIndex;
            }
            else
            {
                byVal = 0x00;
            }
            return byVal;
        }

        private int GetRt1ValFromTable(int nTemp)
        {
            int nRt1 = 0;

            int low = -40;
            int high = 85;
            while (low <= high)
            {
                int middle = (low + high) >> 1;

                if (middle == nTemp)
                {
                    nRt1 = DicTempTable[middle];
                    break;
                }
                else if (nTemp > middle)
                {
                    low = middle + 1;
                }
                else if (nTemp < middle)
                {
                    high = middle - 1;
                }

            }


            return nRt1;
        }

        private byte GetHighTempByteVal(double nRrefVal, int nRt1Val)
        {
            double byteVal = 0;

            double nRt1 = (double)nRt1Val / 1000;

            byteVal = (byte)(int)(512 * nRt1 / (nRrefVal + nRt1));

            return (byte)(int)byteVal;
        }

        private byte GetLowTempByteVal(double nRrefVal, int nRt1Val)
        {
            byte byteVal = 0;

            double nRt1 = (double)nRt1Val / 1000;

            byteVal = (byte)(int)(512 * (nRt1 / (nRrefVal + nRt1)) - 256);

            return byteVal;
        }
    }
}
