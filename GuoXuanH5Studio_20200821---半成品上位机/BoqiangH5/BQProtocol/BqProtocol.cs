using BoqiangH5.ISO15765;
using System;
using System.Threading;
using System.Timers;

namespace BoqiangH5.BQProtocol
{
    class BqProtocol
    {
        static BqProtocol m_bqInstance;

        public event EventHandler RaiseMenuBreakEvent;

        public System.Timers.Timer timer;

        static readonly uint BqProtocolID = 0x1CEB0300;

        public static bool bReadBqBmsResp = true;

        public int nHandshakeFailure = 0;

        int nReadBmsTimes = 0;

        //public bool m_bIsUpdateBmsInfo = true;

        public bool m_bIsSaveBmsInfo = true;

        public bool m_bIsSaveCellInfo = true;

        public int m_bUpateBmsInterval = 500;
        //public DateTime m_bShallowSleepTime = new DateTime(1970,1,1,8,0,0);
        //public bool m_bIsShallowSleep = false;

        public bool m_bIsStopCommunication = false;//用于下发消息时停止心跳、数据刷新和RTC的读取
        public bool m_bIsSendMultiFrame = false;//用于下发消息时停止心跳、数据刷新和RTC的读取

        public bool m_bIsTest = false;


        private BqProtocol()
        {
        }

        public static BqProtocol BqInstance
        {
            get
            {
                if (m_bqInstance == null)
                {
                    m_bqInstance = new BqProtocol();
                }
                return m_bqInstance;
            }
        }

        #region  
        public void SetTimer()
        {
            timer = new System.Timers.Timer(1500);
            timer.Elapsed += OnTimerEvent;
            timer.AutoReset = true;
            //timer.Enabled = true;
        }

        private void OnTimerEvent(Object source, ElapsedEventArgs e)
        {
            if (isReturn == false)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    nHandshakeFailure = 0;
                    OnRaiseMenuBreakEvent(null, null);
                    BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "error", "断开完成",true);
                }), null);
            }

        }

        public void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Close();
            }
        }
        bool isReturn = false;
        #endregion
        private void OnRaiseMenuBreakEvent(Object source, EventArgs e)
        {
            EventHandler handler = RaiseMenuBreakEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        public void ThreadReadMasterTeleData(object o)
        {
            while (!MainWindow.bIsBreak)
            {
                //if(MainWindow.m_statusBarInfo.IsOnline)
                {
                    if (m_bIsStopCommunication)
                    {
                        continue;
                    }
                    if (m_bIsTest)
                    {
                        continue;
                    }
                    if (m_bIsSendMultiFrame)
                    {
                        continue;
                    }

                    if (nHandshakeFailure >= 3)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            //StopTimerHandshake();
                            OnRaiseMenuBreakEvent(null, null);
                            nHandshakeFailure = 0;
                            BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "error", "断线",true);
                        }), null);
                    }
                    else
                    {
                        nHandshakeFailure++;
                        byte[] rdBmsBuf = new byte[] { 0x3A, 0x03, 0xA1, 0x90, 0x85 };
                        isReturn = false;
                        timer.Start();
                        SendSingleFrameData(rdBmsBuf);
                        timer.Stop();
                        isReturn = true;
                        Thread.Sleep(m_bUpateBmsInterval);//lipeng  2020.03.27修改BMS信息刷新时间
                    }
                }
            }
        }

        public void ReadMcuData()
        {
            while (MainWindow.m_statusBarInfo.IsOnline)
            {
                byte[] rdMcuBuf = new byte[] { 0x3A, 0x03, 0xA2, 0xD0, 0x84 };

                if (bReadBqBmsResp)
                {
                    SendSingleFrameData(rdMcuBuf);
                    break;
                }
                else
                {
                    ReadDataNoResponse();
                }     
            }
        }

        public void ReadEepromData()
        {
            while (MainWindow.m_statusBarInfo.IsOnline)
            {    
                byte[] rdEepromBuf = new byte[] { 0x3A, 0x03, 0xA3, 0x11, 0x44 };

                if (bReadBqBmsResp)
                {
                    SendSingleFrameData(rdEepromBuf);
                    break;
                }
                else
                {
                    ReadDataNoResponse();
                }
                
            }
        }

        //lipeng 2020.03.26增加读取备份数据的命令
        public void ReadRecordData(int readtype)
        {
            while (MainWindow.m_statusBarInfo.IsOnline)
            {
                byte[] rdRecordmBuf;
                if(readtype == 0)
                    rdRecordmBuf = new byte[] { 0x3A, 0x03, 0xA6, 0x00, 0x00, 0x00 };
                else if(readtype == 1)
                    rdRecordmBuf = new byte[] { 0x3A, 0x03, 0xA6, 0x01, 0x00, 0x00 };
                else
                    rdRecordmBuf = new byte[] { 0x3A, 0x03, 0xA6, 0x02, 0x00, 0x00 };

                if (bReadBqBmsResp)
                {
                    SendSingleFrameData(rdRecordmBuf);
                    //BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "send", "fasongshuju");
                    break;
                }
                else
                {
                    ReadDataNoResponse();
                }
            }
        }

        //lipeng 2020.03.31增加擦除数据的命令
        public void EraseRecord()
        {
            while (MainWindow.m_statusBarInfo.IsOnline)
            {
                byte[] rdRecordmBuf = new byte[] { 0x3A, 0x03, 0xD6,  0x00, 0x00 };

                if (bReadBqBmsResp)
                {
                    SendSingleFrameData(rdRecordmBuf);
                    break;
                }
                else
                {
                    ReadDataNoResponse();
                }
            }
        }

        private void SendSingleFrameData(byte[] rdBuf)
        {
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[rdBuf.Length - 2];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(BqProtocolID, cmdBuf);

            bReadBqBmsResp = false;
        }

        private void ReadDataNoResponse()
        {
            Thread.Sleep(200);
            nReadBmsTimes++;
            if (nReadBmsTimes > 3)
            {
                bReadBqBmsResp = true;
            }
        }


        public void BQ_JumpToBoot()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xD0, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_Reset()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xD1, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_AlterSOC(byte byteSOC)
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xD2, 0x00, 0x00, 0x00 };
            buf[3] = byteSOC;

            SendSingleFrameData(buf);
        }

        public void BQ_FactoryReset()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xD3, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_Shutdown()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xD4, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_Sleep()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xD5, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_ReadBootInfo()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xDA, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_ReadUID()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xDC, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_WriteManufacturingInformation (string dataStr)// lipeng  2020.4.2 制造信息写入
        {
            int d = int.Parse(dataStr);
            byte data = (byte)((d / 10) << 4 | (d % 10));
            byte[] buf = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            buf[1] = data;
            //SendSingleFrameData(buf);
            SendMultiFrame(buf, buf.Length, 0xDE);
        }

        public void BQ_ReadRTC()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xA5, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void SendMultiFrame(byte[] dataBuf, int len, byte nCmd)
        {
            byte[] cmdBuf = new byte[len + 5];
            cmdBuf[0] = 0x3A;
            cmdBuf[1] = 0x03;
            cmdBuf[2] = nCmd; 
            Buffer.BlockCopy(dataBuf, 0, cmdBuf, 3, len);

            byte[] crc16 = CRC_Check.CRC16(cmdBuf, 0, cmdBuf.Length - 2);

            cmdBuf[cmdBuf.Length - 2] = crc16[1];
            cmdBuf[cmdBuf.Length - 1] = crc16[0];

            int nFrameLen = 0;
            if ((cmdBuf.Length - 2) % 8 == 0)
            {
                nFrameLen = (cmdBuf.Length - 2) / 8;
            }
            else
            {
                nFrameLen = (cmdBuf.Length - 2) / 8 + 1;
            }
            m_bIsSendMultiFrame = true;
            int nSendIndex = 2;
            for (int n = nFrameLen; n > 0; n--)
            {
                byte[] byteCmdBuf = new byte[8];
                if (nSendIndex + 8 > cmdBuf.Length)
                {
                    Buffer.BlockCopy(cmdBuf, nSendIndex, byteCmdBuf, 0, (cmdBuf.Length - nSendIndex));
                }
                else
                {
                    Buffer.BlockCopy(cmdBuf, nSendIndex, byteCmdBuf, 0, 8);
                }

                uint BqProtID = (uint)(0x1CEB0300 | (n - 1));
                DataLinkLayer.SendCanFrame(BqProtID, byteCmdBuf);
              
                nSendIndex += 8;
                Thread.Sleep(5);
            }
            m_bIsSendMultiFrame = false;
        }


        public void AdjustRtCurrent(int nRTCurrent)
        {
            byte[] arrRtCur = BitConverter.GetBytes(nRTCurrent);

            byte[] adCmdBuf = new byte[] { 0x3A, 0x03, 0xC2, 0x00, 0x00, 0x00, 0x00 };

            adCmdBuf[3] = arrRtCur[1];
            adCmdBuf[4] = arrRtCur[0];

            SendSingleFrameData(adCmdBuf);
        }

        //lipeng 2020.03.31增加校准RTC的命令
        public void AdjustRTC(DateTime dt)
        {
            int year_high = dt.Year / 100;
            int year_low = dt.Year % 100;
            byte[] array = new byte[7];
            array[0] = (byte)((year_high / 10) << 4 | (year_high % 10));
            array[1] = (byte)((year_low / 10) << 4 | (year_low % 10));
            array[2] = (byte)((dt.Month / 10) << 4 | (dt.Month % 10));
            array[3] = (byte)((dt.Day / 10) << 4 | (dt.Day % 10));
            array[4] = (byte)((dt.Hour / 10) << 4 | (dt.Hour % 10));
            array[5] = (byte)((dt.Minute / 10) << 4 | (dt.Minute % 10));
            array[6] = (byte)((dt.Second / 10) << 4 | (dt.Second % 10));;

            SendMultiFrame(array, array.Length, 0xB5);
        }
        public void AdjustZeroCurrent(int nCurrent)
        {
            byte[] arrRtCur = BitConverter.GetBytes(nCurrent);

            byte[] adCmdBuf = new byte[] { 0x3A, 0x03, 0xC1, 0x00, 0x00, 0x00, 0x00 };

            adCmdBuf[3] = arrRtCur[1];
            adCmdBuf[4] = arrRtCur[0];

            SendSingleFrameData(adCmdBuf);
        }

        public void BQ_EnterTestMode()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xDB, 0x01, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_ExitTestMode()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xDB, 0x00, 0x00, 0x00};

            SendSingleFrameData(buf);
        }

        public void BQ_ReadVoltageProtectParam()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xAA, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }
        public void BQ_ReadCurrentProtectParam()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xAB, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }
        public void BQ_ReadTemperatureProtectParam()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xAC, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }
        public void BQ_ReadWarningProtectParam()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xAD, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_RequireReadEepromRegister()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xD8, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_RequireReadOthersRegister()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xD9, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_ReadOthersRegister()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xA4, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }

        public void BQ_ReadAdjustParam()
        {
            byte[] buf = new byte[] { 0x3A, 0x03, 0xA8, 0x00, 0x00 };

            SendSingleFrameData(buf);
        }
    }
}
