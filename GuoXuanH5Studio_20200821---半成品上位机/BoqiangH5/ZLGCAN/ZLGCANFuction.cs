
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using BoqiangH5Entity;
using BoqiangH5Repository;



namespace BoqiangH5
{
    public class ZLGCANFuction:ZLGCAN_API //,INotifyPropertyChanged
    {
        List<UInt32> listRecvID = new List<UInt32> { 0x180f0120 };    // 要处理的接收数据ID hongsen

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RecvCANDataEventHandler(ZCAN_Receive_Data[] data, uint len);//CAN数据接收事件委托

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RecvFDDataEventHandler(ZCAN_ReceiveFD_Data[] data, uint len);//CANFD数据接收事件委托

        const int TYPE_CAN = 0;
        const int TYPE_CANFD = 1;

        bool m_bStart;
        IntPtr channel_handle;
        Thread recv_thread_;
        static object locker = new object();
        public static RecvCANDataEventHandler OnRecvCANDataEvent;
        public static RecvFDDataEventHandler OnRecvFDDataEvent;

        public ZLGInfo zlgInfo = new ZLGInfo();

        /// <summary>
        /// 接收总线数据的线程
        /// </summary>
        Thread listenThread = null;

        public static ZLGCANFuction m_zlgCANInstance;
        public static ZLGCANFuction ZlgCANFunInstance
        {
            get
            {
                if (m_zlgCANInstance == null)
                {
                    m_zlgCANInstance = new ZLGCANFuction();
                }
                return m_zlgCANInstance;
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
        #endregion


        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">接口卡的型号</param>
        public ZLGCANFuction()
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
            zlgInfo.CANFD = uint.Parse(XmlHelper.m_strCanFD);
            zlgInfo.ArbitrationBaudrate = SelectCANWnd.GetSelectArbitrationBaudRate(int.Parse(XmlHelper.m_strArbitration));
            zlgInfo.DataBaudRate = SelectCANWnd.GetSelectDataBaudRate(int.Parse(XmlHelper.m_strDataBaudRate));
            zlgInfo.TerminaiResistanceEnabled = int.Parse(XmlHelper.m_strTerminalResistance);
        }

        #endregion

        IntPtr device_handle;
        #region 私有方法

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

                device_handle = ZCAN_OpenDevice(ZLGInfo.DevType, zlgInfo.DevIndex, 0);
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
                UInt32 res = ZCAN_CloseDevice(device_handle);
                if ((Status)res == Status.SUCCESS)
                {
                    zlgInfo.IsRecFrame = false;
                }
                else
                {
                    zlgInfo.IsRecFrame = true;
                }
                return zlgInfo.IsRecFrame;
            }
            else
            {
                return true;
            }
        }

        //仲裁域波特率
        uint[] kAbitTiming =
{
            0x00018B2E,//1Mbps
	        0x00018E3A,//800kbps
	        0x0001975E,//500kbps
	        0x0001AFBE,//250kbps
	        0x0041AFBE,//125kbps
	        0x0041BBEE,//100kbps
	        0x00C1BBEE //50kbps
        };
        //数据域波特率
        uint[] kDbitTiming =
        {
            0x00010207,//5Mbps
	        0x0001020A,//4Mbps
	        0x0041020A,//2Mbps
	        0x0081830E //1Mbps
        };
        /// <summary>
        /// 初始化某一路CAN通道
        /// </summary>
        private bool InitDevice()
        {
            IntPtr ptr = GetIProperty(device_handle);
            if (0 == (int)ptr)
            {
                return false;//设置指定路径属性失败
            }

            IProperty _property = (IProperty)Marshal.PtrToStructure((IntPtr)((UInt32)ptr), typeof(IProperty));

            if(_property.SetValue(zlgInfo.DevChannel.ToString() + "/canfd_standard", zlgInfo.CANFD.ToString()) != 1)
            {
                return false;//设置CANFD标准失败
            }

            ZCAN_CHANNEL_INIT_CONFIG config = new ZCAN_CHANNEL_INIT_CONFIG();
            config.canfd.mode = zlgInfo.Mode;
            config.can_type = (uint)ZLGType.VCI_USBCANFD_100U;
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
            channel_handle = ZCAN_InitCAN(device_handle, (uint)zlgInfo.DevChannel, pConfig);
            Marshal.FreeHGlobal(pConfig);

            //Marshal.FreeHGlobal(ptr);

            if (0 == (int)channel_handle)
            {
                string strErr = "设备类型号:" + ZLGInfo.DevType.ToString() + "  设备索引号: " + zlgInfo.DevIndex.ToString() + "  CAN通道号: " + zlgInfo.DevChannel.ToString();
                return false;//初始化CAN失败
            }

            if(zlgInfo.TerminaiResistanceEnabled == 1)
            {
                if (_property.SetValue(zlgInfo.DevChannel.ToString() + "initenal_resistance", zlgInfo.TerminaiResistanceEnabled.ToString()) != 1)
                {
                    return false;//设置使能终端电阻失败
                }
            }
            return true;
        }

        /// <summary>
        /// 启动某一路CAN通道
        /// </summary>
        /// <param name="canIndex"></param>
        private bool StartDevice()
        {
            UInt32 res = ZCAN_StartCAN(channel_handle);
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
        ///// <summary>
        ///// 根据波特率设置存储参数有关数据缓冲区地址首指针2E-U等类型CAN
        ///// </summary>
        //private void SetBaudRate(BaudRate value, ref UInt32 pData)
        //{
        //    switch (value)
        //    {
        //        case BaudRate._5Kbps:
        //            pData = 0x1C01C1;
        //            break;
        //        case BaudRate._10Kbps:
        //            pData = 0x1C00E0;
        //            break;
        //        case BaudRate._20Kbps:
        //            pData = 0x1600B3;
        //            break;
        //        case BaudRate._50Kbps:
        //            pData = 0x1C002C;
        //            break;
        //        case BaudRate._100Kbps:
        //            pData = 0x160023;
        //            break;
        //        case BaudRate._125Kbps:
        //            pData = 0x1C0011;
        //            break;
        //        case BaudRate._250Kbps:
        //            pData = 0x1C0008;
        //            break;
        //        case BaudRate._500Kbps:
        //            pData = 0x060007;
        //            break;
        //        case BaudRate._800Kbps:
        //            pData = 0x060004;
        //            break;
        //        case BaudRate._1000Kbps:
        //            pData = 0x060003;
        //            break;
        //        default:
        //            pData = 0x060007;
        //            break;
        //    }
        //}

        #endregion


        #region 公有方法

        /// <summary>
        /// 运行设备的指定CAN通道，并开启线程接收CAN总线数据
        /// 如果需要多路CAN通道，此方法需要被多次调用
        /// </summary>
        public bool RunDevice()
        {
            if (!OpenDevice())
            {
                return false;
            }

            if (!InitDevice())
            {
                return false;
            }

            if (!StartDevice())
            {
                return false;
            }

            listenThread = new Thread(new ThreadStart(ReceiveDataFromCAN));

            listenThread.IsBackground = true;
            listenThread.Priority = ThreadPriority.AboveNormal;
            listenThread.Start();

            return true;
        }

        //public event RecvCANDataEventHandler RecvCANData
        //{
        //    add { OnRecvCANDataEvent += new RecvCANDataEventHandler(value); }
        //    remove { OnRecvCANDataEvent -= new RecvCANDataEventHandler(value); }
        //}

        public event RecvFDDataEventHandler RecvFDData
        {
            add { OnRecvFDDataEvent += new RecvFDDataEventHandler(value); }
            remove { OnRecvFDDataEvent -= new RecvFDDataEventHandler(value); }
        }


        //public void setChannelHandle(IntPtr channel_handle)
        //{
        //    lock (locker)
        //    {
        //        channel_handle = channel_handle;
        //    }
        //}
        /// <summary>
        /// 接收CAN总线上的数据
        /// </summary>
        private void ReceiveDataFromCAN()
        {
            //ZCAN_Receive_Data[] can_data = new ZCAN_Receive_Data[100];
            ZCAN_ReceiveFD_Data[] canfd_data = new ZCAN_ReceiveFD_Data[100];
            uint len;
            while (m_bStart)
            {
                lock (locker)
                {
                    //len = ZCAN_GetReceiveNum(channel_handle, TYPE_CAN);
                    //if (len > 0)
                    //{
                    //    int size = Marshal.SizeOf(typeof(ZCAN_Receive_Data));
                    //    IntPtr ptr = Marshal.AllocHGlobal((int)len * size);
                    //    len = ZCAN_Receive(channel_handle, ptr, len, 50);
                    //    for (int i = 0; i < len; ++i)
                    //    {
                    //        can_data[i] = (ZCAN_Receive_Data)Marshal.PtrToStructure(
                    //            (IntPtr)((UInt32)ptr + i * size), typeof(ZCAN_Receive_Data));
                    //    }
                    //    OnRecvCANDataEvent(can_data, len);
                    //    Marshal.FreeHGlobal(ptr);
                    //}

                    len = ZCAN_GetReceiveNum(channel_handle, TYPE_CANFD);
                    int nRecvIndex = 0;
                    byte[] byRecvData = new byte[len * 8];
                    if (len > 0)
                    {
                        int size = Marshal.SizeOf(typeof(ZCAN_ReceiveFD_Data));
                        IntPtr ptr = Marshal.AllocHGlobal((int)len * size);
                        len = ZCAN_ReceiveFD(channel_handle, ptr, len, 50);
                        for (int i = 0; i < len; ++i)
                        {
                            canfd_data[i] = (ZCAN_ReceiveFD_Data)Marshal.PtrToStructure(
                                (IntPtr)((UInt32)ptr + i * size), typeof(ZCAN_ReceiveFD_Data));
                            Buffer.BlockCopy(canfd_data[i].frame.data, 0, byRecvData, nRecvIndex, 8);
                            nRecvIndex += 8;
                        }
                        //OnRecvFDDataEvent(canfd_data, len);
                        if (RaiseZLGRecvDataEvent != null)
                        {
                            OnRaiseZLGRecvDataEvent(this, new CANEvent()
                            {
                                eventType = CANEventType.ReceEvent,
                                DataLen = byRecvData[2],
                                listData = new List<byte>(byRecvData)

                            });
                            Marshal.FreeHGlobal(ptr);
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 发送单帧信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="canIndex"></param>
        /// <param name="data"></param>
        /// <param name="dataLength"></param>
        public void SingleTransmit(ZLG_API.VCI_CAN_OBJ obj)
        {
            try
            {
                ZCAN_TransmitFD_Data canfd_data = new ZCAN_TransmitFD_Data();
                canfd_data.frame.can_id = obj.ID;
                canfd_data.frame.data = new byte[64];
                Buffer.BlockCopy(obj.Data, 0, canfd_data.frame.data, 0, obj.DataLen);
                canfd_data.frame.len = obj.DataLen;
                canfd_data.transmit_type = obj.SendType;
                canfd_data.frame.flags = 0x00;//CANFD不加速
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(canfd_data));
                Marshal.StructureToPtr(canfd_data, ptr, true);
                uint res = ZCAN_TransmitFD(channel_handle, ptr, 1);
                Marshal.FreeHGlobal(ptr);

                if (res == 0)
                {
                    _dispatcher.BeginInvoke(new Action(delegate()
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
                            ID = canfd_data.frame.can_id,
                            DataLen = canfd_data.frame.len,
                            listData = new List<byte>(canfd_data.frame.data)
                        });

                }

            }
            catch(Exception ex)
            {
               
            }
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
