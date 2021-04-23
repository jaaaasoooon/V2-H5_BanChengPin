
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using BoqiangH5Entity;
using BoqiangH5Repository;



namespace BoqiangH5
{
    public class ZLGFuction:ZLG_API //,INotifyPropertyChanged
    {
        List<UInt32> listRecvID = new List<UInt32> { 0x180f0120 };    // 要处理的接收数据ID hongsen

        public ZLGInfo zlgInfo = new ZLGInfo();
        const int TYPE_CAN = 0;
        const int TYPE_CANFD = 1;

        /// <summary>
        /// 接收总线数据的线程
        /// </summary>
        Thread listenThread = null;

        public static ZLGFuction m_zlgInstance;
        public static ZLGFuction ZlgFunInstance
        {
            get
            {
                if (m_zlgInstance == null)
                {
                    m_zlgInstance = new ZLGFuction();
                }
                return m_zlgInstance;
            }
        }

        #region 定义事件
        /// <summary>
        /// 通讯事件
        /// </summary>
        public event EventHandler RaiseZLGCommunicateEvent;

        public void OnRaiseZLGCommunicateEvent(object sender, EventArgs e)
        {
            if (RaiseZLGCommunicateEvent != null)
            {
                RaiseZLGCommunicateEvent(this, e);
            }
        }


        public event EventHandler RaiseZLGRecvDataEvent;

        public void OnRaiseZLGRecvDataEvent(object sender, EventArgs e)
        {
            if (RaiseZLGRecvDataEvent != null)
            {
                RaiseZLGRecvDataEvent(this, e);
            }
        }

        public void OnRaisePcanRecvDataEvent(object sender, EventArgs e)
        {
            OnRaiseZLGRecvDataEvent(this, e);
        }
        #endregion


        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">接口卡的型号</param>
        public ZLGFuction()
        {
            XmlHelper.LoadConfigInfo();

            ZLGInfo.DevType = SelectCANWnd.GetCanType(int.Parse(XmlHelper.m_strCanType));
            zlgInfo.DevIndex = uint.Parse(XmlHelper.m_strCanIndex);
            zlgInfo.DevChannel = uint.Parse(XmlHelper.m_strCanChannel);
            ZLGInfo.Baudrate = SelectCANWnd.GetSelectBaudRate(int.Parse(XmlHelper.m_strBaudrate));
            zlgInfo.AccCode = 0x00000000;
            zlgInfo.AccMask = 0xFFFFFFFF;
            zlgInfo.Mode = 0;
            ZLGInfo.Timing0 = 0x00;
            ZLGInfo.Timing1 = 0x1C;

            //lipeng 2020.04.02       增加CANFD驱动     
            zlgInfo.CANFD = uint.Parse(XmlHelper.m_strCanFD);
            zlgInfo.ArbitrationBaudrate = SelectCANWnd.GetSelectArbitrationBaudRate(int.Parse(XmlHelper.m_strArbitration));
            zlgInfo.DataBaudRate = SelectCANWnd.GetSelectDataBaudRate(int.Parse(XmlHelper.m_strDataBaudRate));
            zlgInfo.TerminaiResistanceEnabled = int.Parse(XmlHelper.m_strTerminalResistance);
            if(ZLGInfo.DevType == (uint)ZLGType.PCAN)
                PCANInterface.PCANInstance.RaiseRecvDataEvent += OnRaisePcanRecvDataEvent;
        }

        #endregion

        #region 私有方法
        IntPtr device_handle;
        IntPtr channel_handle;
        /// <summary>
        /// 打开ZLG接口卡设备，此方法对于一个设备只能被调用一次
        /// </summary>
        private bool OpenDevice()
        {
            try
            {
                if (zlgInfo.IsRecFrame)
                {
                    return true;
                }
                
                if(ZLGInfo.DevType != (uint)ZLGType.VCI_USBCANFD_100U)
                {
                    uint ret = VCI_OpenDevice(ZLGInfo.DevType, zlgInfo.DevIndex, 0);
                    if ((Status)ret == Status.ERROR)
                    {
                        zlgInfo.IsRecFrame = false;
                        string strErr = "设备类型号:" + ZLGInfo.DevType.ToString() + "  设备索引号: " + zlgInfo.DevIndex.ToString();

                        return false;
                    }
                    else
                    {
                        zlgInfo.IsRecFrame = true;

                        return true;
                    }
                }
                else
                {
                    device_handle = ZLGCAN_API.ZCAN_OpenDevice(ZLGInfo.DevType, zlgInfo.DevIndex, 0);
                    if ((int)device_handle == 0)
                    {
                        zlgInfo.IsRecFrame = false;
                        string strErr = "设备类型号:" + ZLGInfo.DevType.ToString() + "  设备索引号: " + zlgInfo.DevIndex.ToString();

                        return false;
                    }
                    else
                    {
                        zlgInfo.IsRecFrame = true;

                        return true;
                    }
                }
            }
            catch(Exception ex)
            {                
            }

            return false;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        private bool CloseDevice()
        {
            if (zlgInfo.IsRecFrame)
            {
                if (ZLGInfo.DevType == (uint)ZLGType.VCI_USBCANFD_100U)
                {
                    UInt32 res = ZLGCAN_API.ZCAN_CloseDevice(device_handle);
                    if ((Status)res == Status.SUCCESS)
                    {
                        zlgInfo.IsRecFrame = false;
                    }
                    else
                    {
                        zlgInfo.IsRecFrame = true;
                    }
                }
                else if(ZLGInfo.DevType == (uint)ZLGType.PCAN)
                {
                    bool ret = PCANInterface.PCANInstance.ConnectRelease();
                    if(ret == true)
                    {
                        zlgInfo.IsRecFrame = false;
                    }
                    else
                    {
                        zlgInfo.IsRecFrame = true;
                    }
                }
                else
                {
                    UInt32 res = VCI_CloseDevice(ZLGInfo.DevType, zlgInfo.DevIndex);
                    if ((Status)res == Status.SUCCESS)
                    {
                        zlgInfo.IsRecFrame = false;
                    }
                    else
                    {
                        zlgInfo.IsRecFrame = true;
                    }
                }
                return zlgInfo.IsRecFrame;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 初始化某一路CAN通道
        /// </summary>
        private bool InitDevice()
        {
            if (ZLGInfo.DevType != (uint)ZLGType.VCI_USBCANFD_100U)
            {
                VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                UInt32 pData = 0x060007;
                if (ZLGInfo.DevType > 18 && ZLGInfo.DevType < 23)
                {
                    config.Mode = 0;
                    SetBaudRate(ZLGInfo.Baudrate, ref pData);
                    VCI_SetReference(ZLGInfo.DevType, zlgInfo.DevIndex, 0, 0, ref pData);
                }
                else
                {
                    config.AccCode = zlgInfo.AccCode;
                    config.AccMask = zlgInfo.AccMask;
                    config.Filter = 1;
                    config.Mode = zlgInfo.Mode;
                    config.Timing0 = ZLGInfo.Timing0;
                    config.Timing1 = ZLGInfo.Timing1;
                }
                UInt32 res = VCI_InitCAN(ZLGInfo.DevType, zlgInfo.DevIndex, zlgInfo.DevChannel, ref config);
                if ((Status)res == Status.ERROR)
                {
                    string strErr = "设备类型号:" + ZLGInfo.DevType.ToString() + "  设备索引号: " + zlgInfo.DevIndex.ToString() + "  CAN通道号: " + zlgInfo.DevChannel.ToString();

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                IntPtr ptr = ZLGCAN_API.GetIProperty(device_handle);
                if (0 == (int)ptr)
                {
                    return false;//设置指定路径属性失败
                }

                ZLGCAN_API.IProperty _property = (ZLGCAN_API.IProperty)Marshal.PtrToStructure((IntPtr)((UInt32)ptr), typeof(ZLGCAN_API.IProperty));

                if (_property.SetValue(zlgInfo.DevChannel.ToString() + "/canfd_standard", zlgInfo.CANFD.ToString()) != 1)
                {
                    return false;//设置CANFD标准失败
                }

                ZLGCAN_API.ZCAN_CHANNEL_INIT_CONFIG config = new ZLGCAN_API.ZCAN_CHANNEL_INIT_CONFIG();
                config.canfd.mode = zlgInfo.Mode;
                config.can_type = TYPE_CANFD;//设置CAN类型为CAN
                uint pData = 0x00018B2E;
                SetArbitrationBaudRate(zlgInfo.ArbitrationBaudrate, ref pData);
                config.canfd.abit_timing = pData;
                uint _pData = 0x00010207;
                SetDataBaudRate(zlgInfo.DataBaudRate, ref _pData);
                config.canfd.dbit_timing = _pData;

                IntPtr pConfig = Marshal.AllocHGlobal(Marshal.SizeOf(config));
                Marshal.StructureToPtr(config, pConfig, true);

                //int size = sizeof(ZCAN_CHANNEL_INIT_CONFIG);
                //IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(size);
                //System.Runtime.InteropServices.Marshal.StructureToPtr(config_, ptr, true);
                channel_handle = ZLGCAN_API.ZCAN_InitCAN(device_handle, (uint)zlgInfo.DevChannel, pConfig);
                Marshal.FreeHGlobal(pConfig);

                //Marshal.FreeHGlobal(ptr);

                if (0 == (int)channel_handle)
                {
                    string strErr = "设备类型号:" + ZLGInfo.DevType.ToString() + "  设备索引号: " + zlgInfo.DevIndex.ToString() + "  CAN通道号: " + zlgInfo.DevChannel.ToString();
                    return false;//初始化CAN失败
                }

                if (zlgInfo.TerminaiResistanceEnabled == 1)
                {
                    if (_property.SetValue(zlgInfo.DevChannel.ToString() + "/initenal_resistance", zlgInfo.TerminaiResistanceEnabled.ToString()) != 1)
                    {
                        return false;//设置使能终端电阻失败
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 启动某一路CAN通道
        /// </summary>
        /// <param name="canIndex"></param>
        private bool StartDevice()
        {
            if(ZLGInfo.DevType != (uint)ZLGType.VCI_USBCANFD_100U)
            {
                UInt32 res = VCI_StartCAN(ZLGInfo.DevType, zlgInfo.DevIndex, zlgInfo.DevChannel);
                if ((Status)res == Status.ERROR)
                {
                    string strErr = "设备类型号:" + ZLGInfo.DevType.ToString() + "  设备索引号: " + zlgInfo.DevIndex.ToString() + "  CAN通道号: " + zlgInfo.DevChannel.ToString();

                    return false;
                }
                else
                {

                    return true;
                }
            }
            else
            {
                UInt32 res = ZLGCAN_API.ZCAN_StartCAN(channel_handle);
                if ((Status)res == Status.ERROR)
                {
                    string strErr = "设备类型号:" + ZLGInfo.DevType.ToString() + "  设备索引号: " + zlgInfo.DevIndex.ToString() + "  CAN通道号: " + zlgInfo.DevChannel.ToString();

                    return false;
                }
                else
                {

                    return true;
                }
            }
        }


        /// <summary>
        /// 根据波特率设置存储参数有关数据缓冲区地址首指针2E-U等类型CAN
        /// </summary>
        private void SetBaudRate(BaudRate value, ref UInt32 pData)
        {
            switch (value)
            {
                case BaudRate._5Kbps:
                    pData = 0x1C01C1;
                    break;
                case BaudRate._10Kbps:
                    pData = 0x1C00E0;
                    break;
                case BaudRate._20Kbps:
                    pData = 0x1600B3;
                    break;
                case BaudRate._50Kbps:
                    pData = 0x1C002C;
                    break;
                case BaudRate._100Kbps:
                    pData = 0x160023;
                    break;
                case BaudRate._125Kbps:
                    pData = 0x1C0011;
                    break;
                case BaudRate._250Kbps:
                    pData = 0x1C0008;
                    break;
                case BaudRate._500Kbps:
                    pData = 0x060007;
                    break;
                case BaudRate._800Kbps:
                    pData = 0x060004;
                    break;
                case BaudRate._1000Kbps:
                    pData = 0x060003;
                    break;
                default:
                    pData = 0x060007;
                    break;
            }
        }


        /// <summary>
        /// 设置CANFD通讯的仲裁域波特率
        /// </summary>
        private void SetArbitrationBaudRate(ArbitrationBaudRate value, ref UInt32 pData)
        {
            switch (value)
            {
                case ArbitrationBaudRate._1Mbps:
                    pData = 0x00018B2E;//1Mbps
                    break;
                case ArbitrationBaudRate._800Kbps:
                    pData = 0x00018E3A;//800kbps
                    break;
                case ArbitrationBaudRate._500Kbps:
                    pData = 0x0001975E;//500kbps
                    break;
                case ArbitrationBaudRate._250Kbps:
                    pData = 0x0001AFBE;//250kbps
                    break;
                case ArbitrationBaudRate._125Kbps:
                    pData = 0x0041AFBE;//125kbps
                    break;
                case ArbitrationBaudRate._100Kbps:
                    pData = 0x0041BBEE;//100kbps
                    break;
                case ArbitrationBaudRate._50Kbps:
                    pData = 0x00C1BBEE;//50kbps
                    break;
                default:
                    pData = 0x00018B2E;//1Mbps
                    break;
            }
        }

        /// <summary>
        /// 设置CANFD通讯的数据域波特率
        /// </summary>
        private void SetDataBaudRate(DataBaudRate value, ref UInt32 pData)
        {
            switch (value)
            {
                case DataBaudRate._5Mbps:
                    pData = 0x00010207;//5Mbps
                    break;
                case DataBaudRate._4Mbps:
                    pData = 0x0001020A;//4Mbps
                    break;
                case DataBaudRate._2Mbps:
                    pData = 0x0041020A;//2Mbps
                    break;
                case DataBaudRate._1Mbps:
                    pData = 0x0081830E;//1Mbps
                    break;
                default:
                    pData = 0x00010207;//5Mbps
                    break;
            }
        }
        #endregion


        #region 公有方法

        /// <summary>
        /// 运行设备的指定CAN通道，并开启线程接收CAN总线数据
        /// 如果需要多路CAN通道，此方法需要被多次调用
        /// </summary>
        public int RunDevice()
        {
            if (ZLGInfo.DevType != (uint)ZLGType.PCAN)
            {
                if (!OpenDevice())
                {
                    return 1;
                }

                if (!InitDevice())
                {
                    return 2;
                }

                if (!StartDevice())
                {
                    return 3;
                }
            }
            else
            {
                bool ret = PCANInterface.PCANInstance.PCANInitSettings(ZLGInfo.Baudrate.ToString(), ZLGInfo.DevType);
                if (!ret)
                {
                    return 1;
                }
                zlgInfo.IsRecFrame = true;
            }

            listenThread = new Thread(new ThreadStart(ReceiveDataFromCAN));

            listenThread.IsBackground = true;
            listenThread.Priority = ThreadPriority.AboveNormal;
            listenThread.Start();

            return 0;
        }
        static object locker = new object();
        /// <summary>
        /// 接收CAN总线上的数据
        /// </summary>
        private void ReceiveDataFromCAN()
        {
            try
            {
                if (ZLGInfo.DevType == (uint)ZLGType.VCI_USBCANFD_100U)
                {
                    ZLGCAN_API.ZCAN_Receive_Data[] can_data = new ZLGCAN_API.ZCAN_Receive_Data[100];
                    //ZLGCAN_API.ZCAN_Receive_Data[] can_data = new ZLGCAN_API.ZCAN_Receive_Data[500];
                    uint len;
                    while (true)
                    {
                        lock (locker)
                        {
                            len = ZLGCAN_API.ZCAN_GetReceiveNum(channel_handle, TYPE_CAN);
                            int nRecvIndex = 0;
                            if (len > 0)
                            {
                                int size = Marshal.SizeOf(typeof(ZLGCAN_API.ZCAN_Receive_Data));
                                IntPtr ptr = Marshal.AllocHGlobal((int)len * size);
                                len = ZLGCAN_API.ZCAN_Receive(channel_handle, ptr, len, 50);
                                byte[] byRecvData = new byte[len * 8];
                                for (int i = 0; i < len; ++i)
                                {
                                    can_data[i] = (ZLGCAN_API.ZCAN_Receive_Data)Marshal.PtrToStructure(
                                        (IntPtr)((UInt32)ptr + i * size), typeof(ZLGCAN_API.ZCAN_Receive_Data));
                                    Buffer.BlockCopy(can_data[i].frame.data, 0, byRecvData, nRecvIndex, 8);
                                    nRecvIndex += 8;
                                }
                                Marshal.FreeHGlobal(ptr);
                                //OnRecvFDDataEvent(canfd_data, len);

                                //if (byRecvData[0] == 0xA2)
                                //{
                                //    string str = "MCU";
                                //}
                                //if (byRecvData[0] == 0xA3)
                                //{
                                //    string str = "Eeprom";
                                //}
                                if (RaiseZLGRecvDataEvent != null)
                                {
                                    OnRaiseZLGRecvDataEvent(this, new CANEvent()
                                    {
                                        eventType = CANEventType.ReceEvent,
                                        DataLen = byRecvData[1],
                                        listData = new List<byte>(byRecvData)

                                    });
                                    //Marshal.FreeHGlobal(ptr);
                                }
                            }
                        }

                        Thread.Sleep(100);
                    }
                }
                else if(ZLGInfo.DevType == (uint)ZLGType.PCAN)
                {
                    PCANInterface.PCANInstance.ReadMessages();
                }
                else
                {
                    VCI_ERR_INFO pErrInfo = new VCI_ERR_INFO();
                    while (true)
                    {
                        //为了不占用CPU
                        SpinWait.SpinUntil(() => false, 150);

                        //返回接收缓冲区中尚未被读取的帧数
                        UInt32 num = VCI_GetReceiveNum(ZLGInfo.DevType, zlgInfo.DevIndex, zlgInfo.DevChannel);

                        if (num == 0)
                        {
                            continue;
                        }

                        zlgInfo.IsSendFrame = false;

                        //分配一次最多接收VCI_GetReceiveNum函数返回值的帧数的数据存储内存空间
                        IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (int)num);

                        //返回实际读到的帧数，如返回的为0xFFFFFFFF，则读取数据失败，有错误发生。
                        UInt32 len = VCI_Receive(ZLGInfo.DevType, zlgInfo.DevIndex, zlgInfo.DevChannel, pt, num, -1);
                        if (len == 0xFFFFFFFF)
                        {
                            VCI_ReadErrInfo(ZLGInfo.DevType, zlgInfo.DevIndex, zlgInfo.DevChannel, ref pErrInfo);
                            //释放分配的内存空间
                            Marshal.FreeHGlobal(pt);
                            continue;
                        }
                        else
                        {

                        }

                        int nRecvIndex = 0;
                        byte[] byRecvData = new byte[len * 8];

                        //获取CAN总线上的数据并触发事件
                        for (int i = 0; i < len; i++)
                         {
                            VCI_CAN_OBJ recvData = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));

                            Buffer.BlockCopy(recvData.Data, 0, byRecvData, nRecvIndex, 8);
                            nRecvIndex += 8;

                            string strRecvData = String.Empty;

                            for (int j = 0; j < recvData.DataLen; j++)
                            {
                                strRecvData += String.Format("{0:X2}", recvData.Data[j]) + " ";
                            }

                        }

                        if (RaiseZLGRecvDataEvent != null)
                        {
                            //if (byRecvData[0] == 0x03 && byRecvData[1] == 0x06)
                            //{
                            //    BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "recv", "jieshouMCU");
                            //}
                            ////if (byRecvData[0] == 0xA6)
                            ////{
                            ////    BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "recv", "recv A6\r\n");
                            ////}
                            OnRaiseZLGRecvDataEvent(this, new CANEvent()
                            {
                                eventType = CANEventType.ReceEvent,
                                DataLen = byRecvData[2],
                                listData = new List<byte>(byRecvData)

                            });

                        }
                        VCI_ClearBuffer(ZLGInfo.DevType, zlgInfo.DevIndex, zlgInfo.DevChannel);
                        Marshal.FreeHGlobal(pt);
                    }
                }
            }
            catch(Exception ex)
            {
                string str = ex.Message;
            }

        }

        /// <summary>
        /// 发送单帧信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="canIndex"></param>
        /// <param name="data"></param>
        /// <param name="dataLength"></param>
        public void SingleTransmit(VCI_CAN_OBJ obj)
        {
            try
            {
                if (ZLGInfo.DevType == (uint)ZLGType.VCI_USBCANFD_100U)
                {
                    ZLGCAN_API.ZCAN_Transmit_Data can_data = new ZLGCAN_API.ZCAN_Transmit_Data();
                    obj.ExternFlag = 1;
                    can_data.frame.can_id = MakeCanId(obj.ID, obj.ExternFlag, 0, 0);
                    can_data.frame.data = new byte[8];
                    Buffer.BlockCopy(obj.Data, 0, can_data.frame.data, 0, obj.DataLen);
                    can_data.frame.can_dlc = (byte)obj.Data.Length;
                    can_data.transmit_type = obj.SendType;
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(can_data));
                    Marshal.StructureToPtr(can_data, ptr, true);

                    BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "发送", CSVFileHelper.ToHexStrFromByte(can_data.frame.data,false),false);
                    uint res = ZLGCAN_API.ZCAN_Transmit(channel_handle, ptr, 1);
                    Marshal.FreeHGlobal(ptr);


                    //ZLGCAN_API.ZCAN_TransmitFD_Data canfd_data = new ZLGCAN_API.ZCAN_TransmitFD_Data();
                    //canfd_data.frame.can_id = MakeCanId(obj.ID, obj.RemoteFlag, 0, 0);
                    //canfd_data.frame.data = new byte[64];
                    //Buffer.BlockCopy(obj.Data, 0, canfd_data.frame.data, 0, obj.DataLen);
                    //canfd_data.frame.len = obj.DataLen;
                    //canfd_data.transmit_type = obj.SendType;
                    //canfd_data.frame.flags = 0x00;//CANFD不加速
                    //IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(canfd_data));
                    //Marshal.StructureToPtr(canfd_data, ptr, true);
                    //uint res = ZLGCAN_API.ZCAN_TransmitFD(channel_handle, ptr, 1);
                    //Marshal.FreeHGlobal(ptr);

                    if (res == 0)
                    {
                        _dispatcher.BeginInvoke(new Action(delegate ()
                        {

                        }));
                        AbortThread();

                        return;
                    }
                    else
                    {
                        OnRaiseZLGCommunicateEvent(this,
                            new CANEvent()
                            {
                                eventType = CANEventType.SendEvent,
                                ID = can_data.frame.can_id,
                                DataLen = can_data.frame.data[2],
                                listData = new List<byte>(can_data.frame.data)
                            });

                    }
                }
                else if(ZLGInfo.DevType == (uint)ZLGType.PCAN)
                {
                    bool isRemote = false;
                    if (obj.RemoteFlag == 0x00) isRemote = false;
                    else isRemote = true;
                    BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "发送", CSVFileHelper.ToHexStrFromByte(obj.Data,false),false);
                    PCANInterface.PCANInstance.PCAN_WriteData(obj.ID,obj.DataLen,isRemote,obj.Data);
                }
                else
                {
                    //在非托管内存中分配一个VCI_CAN_OBJ结构体大小的内存空间 
                    IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * 1);
                    VCI_CAN_OBJ pSend = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt), typeof(VCI_CAN_OBJ));

                    //IsSendFrame = true;
                    pSend.ID = obj.ID;

                    pSend.SendType = obj.SendType;
                    pSend.RemoteFlag = obj.RemoteFlag;
                    pSend.ExternFlag = obj.ExternFlag;
                    pSend.DataLen = obj.DataLen;


                    string strSendDdata = String.Empty;
                    byte[] temp = obj.Data;


                    for (int j = 0; j < pSend.DataLen; j++)
                    {
                        strSendDdata += String.Format("{0:X2}", temp[j]) + " ";
                    }
                    for (int i = 0; i < obj.DataLen; i++)
                    {
                        pSend.Data[i] = obj.Data[i];
                    }

                    if (pSend.Data[0] == 0x30 && pSend.Data[1] == 0xff && pSend.Data[3] == 0x00)
                    {

                    }
                    BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "发送", CSVFileHelper.ToHexStrFromByte(pSend.Data,false),false);
                    //返回实际发送成功的帧数
                    UInt32 res = VCI_Transmit(ZLGInfo.DevType, zlgInfo.DevIndex, zlgInfo.DevChannel, ref pSend, 1);

                    if (res == 0)
                    {
                        _dispatcher.BeginInvoke(new Action(delegate ()
                        {

                        }));
                        AbortThread();

                        return;
                    }
                    else
                    {
                        OnRaiseZLGCommunicateEvent(this,
                            new CANEvent()
                            {
                                eventType = CANEventType.SendEvent,
                                ID = pSend.ID,
                                DataLen = pSend.DataLen,
                                listData = new List<byte>(pSend.Data)
                            });

                    }

                    Marshal.FreeHGlobal(pt);
                } 
            }
            catch(Exception ex)
            {
               
            }
        }
        public uint MakeCanId(uint id, int eff, int rtr, int err)//1:extend frame 0:standard frame
        {
            uint ueff = (uint)(!!(Convert.ToBoolean(eff)) ? 1 : 0);
            uint urtr = (uint)(!!(Convert.ToBoolean(rtr)) ? 1 : 0);
            uint uerr = (uint)(!!(Convert.ToBoolean(err)) ? 1 : 0);
            return id | ueff << 31 | urtr << 30 | uerr << 29;
        }

        #region 开启、关闭发送线程
        /// <summary>
        /// 获取当前dispatcher，用来异步操作
        /// </summary>
        System.Windows.Threading.Dispatcher _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        /// <summary>
        /// 发送CAN帧线程
        /// </summary>
        Thread threadSendInfo = null;
        /// <summary>
        /// 建立发送线程
        /// </summary>
        public void StartThread()
        {
            _sendNum = 0;
            zlgInfo.SendOutFlag = false;
            threadSendInfo = new Thread(new ThreadStart(SendUntilOut));
            threadSendInfo.IsBackground = true;
            threadSendInfo.Priority = ThreadPriority.AboveNormal;
            threadSendInfo.Start();
        }
        /// <summary>
        /// 用于判断是否发送完成的比对值
        /// </summary>
        int _sendNum = 0;
        /// <summary>
        /// 开始发送数据
        /// </summary>
        private void SendUntilOut()
        {
            while (true)
            {
                //SingleTransmit();
                _sendNum++;
                if (_sendNum == zlgInfo.SendTime)
                {
                    AbortThread();
                    break;
                }
                Thread.Sleep(zlgInfo.SendInterval);
            }
        }
        /// <summary>
        /// 关闭发送线程
        /// </summary>
        public void AbortThread()
        {
            zlgInfo.SendOutFlag = true;
            if (threadSendInfo != null)
            {
                threadSendInfo.Abort();
            }
        }

        #endregion

        /// <summary>
        /// 关闭设备，释放线程资源
        /// </summary>
        public void StopDevice()
        {
            CloseDevice();
            if (listenThread != null)
            {
                listenThread.Abort();
            }            
            AbortThread();            
        } 

        #endregion
        
    }
}
