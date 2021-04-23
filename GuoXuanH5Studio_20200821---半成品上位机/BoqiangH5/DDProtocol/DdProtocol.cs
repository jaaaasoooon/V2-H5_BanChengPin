using BoqiangH5.ISO15765;
using System;
using System.Threading;
using System.Timers;

namespace BoqiangH5.DDProtocol
{
    public class DdProtocol
    {
        static DdProtocol m_ddInstance;

        public event EventHandler RaiseMenuBreakEvent;

        public System.Timers.Timer timerBmsStatus;

        static readonly uint DidiProtocolID = 0x1CEB0300;  // 0x14050320
        
        public static bool bReadDdBmsResp = true;

        public int nReadSOHFailure = 0;

        int nReadBmsTimes = 0;

        //public bool m_bIsUpdateDdBmsInfo = true;
        public bool m_bIsSaveBmsInfo = true;
        public int m_bUpateBmsInterval = 500;

        public bool m_bIsStopCommunication = false;//增加此参数，当点击读寄存器时，将该值置为ture,停掉UTC读取、握手和数据刷新，用于判断读寄存器的返回消息是否成功

        public bool m_bIsFlag = false;
        public bool m_bIsTest = false;

        private DdProtocol()
        {
        }

        public static DdProtocol DdInstance
        {
            get
            {
                if (m_ddInstance == null)
                {
                    m_ddInstance = new DdProtocol();
                }
                return m_ddInstance;
            }
        }

        public void SetTimerReadSOH()
        {
            timerBmsStatus = new System.Timers.Timer(SelectCANWnd.m_HandShakeTime * Math.Pow(10,3));            
            timerBmsStatus.Elapsed += OnTimedReadSOHEvent;
            timerBmsStatus.AutoReset = true; 
            timerBmsStatus.Enabled = true;   
        }

        private void OnTimedReadSOHEvent(Object source, ElapsedEventArgs e)
        {
            if (m_bIsStopCommunication)
            {
                return;
            }
            if (nReadSOHFailure > 10)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    //StopTimerReadSOH();
                     OnRaiseMenuBreakEvent(null, null);
                    nReadSOHFailure = 0;
                }), null);
            }
            else
            {
                nReadSOHFailure++;
                CheckConnectReadSOH();

            }
        }

        public void StopTimerReadSOH()
        {
            if (timerBmsStatus != null)
            {
                timerBmsStatus.Stop();
                timerBmsStatus.Close();
            }
            nReadSOHFailure = 0;
        }

        private void OnRaiseMenuBreakEvent(Object source, EventArgs e)
        {
            EventHandler handler = RaiseMenuBreakEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void CheckConnectReadSOH()
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0x03, 0xA2, 0x00, 0x01, 0x00, 0x00 };

            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, 6);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }

        public void ThreadReadMasterTeleData(object o)
        {
            while (!MainWindow.bIsBreak)
            {
                //if(MainWindow.m_statusBarInfo.IsOnline)
                {
                    if (m_bIsTest)
                    {
                        continue;
                    }
                    if (m_bIsStopCommunication)
                    {
                        continue;
                    }
                    if (m_bIsFlag)
                    {
                        continue;
                    }

                    //if (!bReadDdBmsResp)
                    //{
                    //    Thread.Sleep(1200);
                    //    nReadBmsTimes++;
                    //    if (nReadBmsTimes > 5)
                    //    {
                    //        bReadDdBmsResp = true;
                    //    }
                    //    continue;
                    //}
                    if (nReadSOHFailure >= 3)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            //StopTimerReadSOH();
                            OnRaiseMenuBreakEvent(null, null);
                            nReadSOHFailure = 0;
                        }), null);
                    }
                    else
                    {
                        nReadSOHFailure++;

                        ReadDdBmsInfo();
                        bReadDdBmsResp = false;
                        Thread.Sleep(m_bUpateBmsInterval);
                    }
                }
            }
        }

        public void ReadDdBmsInfo()
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0x03, 0xA2, 0x00, 0x69, 0x65, 0x85 };

            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }
        public void Didi_ReadRTC()
        {
            if (m_bIsStopCommunication)
            {
                return;
            }
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0x03, 0xA2, 0x48,0x02,0x00,0x00 };
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }

        public static DateTime systemStartTime = new DateTime(1970, 1, 1, 8, 0, 0);
        public void AdjustDidiRTC(DateTime dt)
        {
            TimeSpan ts = dt - systemStartTime;
            byte[] data = BitConverter.GetBytes(((uint)(ts.Ticks / Math.Pow(10,7))));
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0x10, 0xA2, 0x48, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            rdBuf[rdBuf.Length - 3] = data[0];
            rdBuf[rdBuf.Length - 4] = data[1];
            rdBuf[rdBuf.Length - 5] = data[2];
            rdBuf[rdBuf.Length - 6] = data[3];
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];


            byte[] cmdBuf = new byte[10];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            SendMultiFrame(cmdBuf, cmdBuf.Length);
        }

        public void AdjustDidiRTC(uint dt)
        {
            byte[] data = BitConverter.GetBytes(dt);
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0x10, 0xA2, 0x48, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            rdBuf[rdBuf.Length - 3] = data[0];
            rdBuf[rdBuf.Length - 4] = data[1];
            rdBuf[rdBuf.Length - 5] = data[2];
            rdBuf[rdBuf.Length - 6] = data[3];
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];


            byte[] cmdBuf = new byte[10];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            SendMultiFrame(cmdBuf, cmdBuf.Length);
        }
        public void ReadDeviceInfo()
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0x03, 0xA0, 0x00, 0x2B, 0x65, 0x85 };

            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }

        public void DD_PowerOn()
        {
            byte[] poBuf = new byte[] { 0x3A, 0x03, 0x10, 0xA2, 0x00, 0x01, 0x00, 0x01, 0x9B, 0x29 };

            byte[] crc16 = CRC_Check.CRC16(poBuf, 0, poBuf.Length - 2);

            poBuf[poBuf.Length - 2] = crc16[1];
            poBuf[poBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[8];
            Buffer.BlockCopy(poBuf, 2, cmdBuf, 0, 8);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }

        public void DD_PowerOff()
        {
            byte[] poBuf = new byte[] { 0x3A, 0x03, 0x10, 0xA2, 0x00, 0x01, 0x00, 0x00, 0x5A, 0xE9 };

            byte[] crc16 = CRC_Check.CRC16(poBuf, 0, poBuf.Length - 2);

            poBuf[poBuf.Length - 2] = crc16[1];
            poBuf[poBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[8];
            Buffer.BlockCopy(poBuf, 2, cmdBuf, 0, 8);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);

        }

        public void DD_WriteRegister(byte[] addr, byte num, byte[] data)
        {
            byte[] poBuf = new byte[addr.Length + data.Length + 1 + 3 + 2];
            poBuf[0] = 0x3A;
            poBuf[1] = 0x03;
            poBuf[2] = 0x10;
            int offset = 3;
            Buffer.BlockCopy(addr,0,poBuf,offset,addr.Length);
            offset += addr.Length;
            poBuf[offset] = num;
            offset += 1;
            Buffer.BlockCopy(data, 0, poBuf, offset, data.Length);
            offset += data.Length;
            poBuf[offset] = 0x00;
            offset += 1;
            poBuf[offset] = 0x00;
            offset += 1;

            byte[] crc16 = CRC_Check.CRC16(poBuf, 0, poBuf.Length - 2);

            poBuf[poBuf.Length - 2] = crc16[1];
            poBuf[poBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[poBuf.Length - 2];
            Buffer.BlockCopy(poBuf, 2, cmdBuf, 0, poBuf.Length - 2);

            SendMultiFrame(cmdBuf, cmdBuf.Length);

        }

        public void DD_ReadRegister(byte[] addr,byte num)
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Buffer.BlockCopy(addr, 0, rdBuf, 3, addr.Length);
            rdBuf[addr.Length + 3] = num;
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }

        public void SendMultiFrame(byte[] dataBuf, int len)
        {

            int nFrameLen = 0;
            if ((dataBuf.Length) % 8 == 0)
            {
                nFrameLen = (dataBuf.Length ) / 8;
            }
            else
            {
                nFrameLen = (dataBuf.Length ) / 8 + 1;
            }

            int nSendIndex = 0;
            for (int n = nFrameLen; n > 0; n--)
            {
                byte[] byteCmdBuf;
                if (nSendIndex + 8 > dataBuf.Length)
                {
                    byteCmdBuf = new byte[dataBuf.Length - nSendIndex];
                    Buffer.BlockCopy(dataBuf, nSendIndex, byteCmdBuf, 0, (dataBuf.Length - nSendIndex));
                }
                else
                {
                    byteCmdBuf = new byte[8];
                    Buffer.BlockCopy(dataBuf, nSendIndex, byteCmdBuf, 0, 8);
                }

                uint ProtID = (uint)(0x1CEB0300 | (n - 1));
                DataLinkLayer.SendCanFrame(ProtID, byteCmdBuf);

                nSendIndex += 8;
                Thread.Sleep(5);
            }

        }

        public void ReadDidiRecordCount()
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0x03, 0xA3, 0x00, 0x01, 0x00, 0x00 };
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }
        public void ReadDidiRecord()
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0x03, 0xA3, 0x01, 0x24, 0x00, 0x00 };
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }

        public void ReadDidiFirstRecordData()
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0xA7,0x00, 0x00, 0x00 };
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }
        public void ReadDidiNextRecordData()
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0xA7, 0x01, 0x00, 0x00 };
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }
        public void ReadDidiCurrentRecordData()
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0xA7, 0x02, 0x00, 0x00 };
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }
        public void EraseDidiRecord()
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0xD7, 0x00, 0x00 };
            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[3];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }

        public void DD_SettingBatteryStatus(byte status)
        {
            byte[] Buf = new byte[] { 0x3A, 0x03, 0x10, 0xA2, 0x64, 0x01, 0x00, status, 0x00, 0x00 };

            byte[] crc16 = CRC_Check.CRC16(Buf, 0, Buf.Length - 2);

            Buf[Buf.Length - 2] = crc16[1];
            Buf[Buf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[8];
            Buffer.BlockCopy(Buf, 2, cmdBuf, 0, 8);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }
        public void DD_ReadFeedbackInfo()
        {
            byte[] rdBuf = new byte[] { 0x3A, 0x03, 0x03, 0xA2, 0xA0, 0x03, 0x00, 0x00 };

            byte[] crc16 = CRC_Check.CRC16(rdBuf, 0, rdBuf.Length - 2);

            rdBuf[rdBuf.Length - 2] = crc16[1];
            rdBuf[rdBuf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[6];
            Buffer.BlockCopy(rdBuf, 2, cmdBuf, 0, rdBuf.Length - 2);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }

        public void DD_SettingBrakeStatus(byte status)
        {
            byte[] Buf = new byte[] { 0x3A, 0x03, 0x10, 0xA2, 0x68, 0x01, status, 0x00, 0x00, 0x00 };

            byte[] crc16 = CRC_Check.CRC16(Buf, 0, Buf.Length - 2);

            Buf[Buf.Length - 2] = crc16[1];
            Buf[Buf.Length - 1] = crc16[0];

            byte[] cmdBuf = new byte[8];
            Buffer.BlockCopy(Buf, 2, cmdBuf, 0, 8);

            DataLinkLayer.SendCanFrame(DidiProtocolID, cmdBuf);
        }
    }
}

