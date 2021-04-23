using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDSStudio.ISO14229;
using System.Threading;
using System.Collections.Concurrent;
using UDSEntity;
using NLog;

namespace UDSStudio.ISO15765
{
    public class MessageBuffer							// 消息缓存，已退化为半双工。
													// 是具有此功能的设计，目前取消了多 ECU 支持，只对单一 ECU 操作，但仍用此结构。
	{
        public MessageBuffer() { }
        //public bool IsBusy() { }								// [线程安全] 正忙检测。
       // public bool IsRequest() { }							// [线程安全] 是否发送（发送状态，或接收状态下发流控制帧）。


        public UInt32 nID;									// ID
        public List<byte> vbyData = new List<byte>();							// 将要发送，或是正在接收的数据。
        public UInt32 stLocation;			                // 将要发送或接收的位置。
        public ProtocolCtrlType pciType;							// 发送 / 验证 / 期望收到的 PCI 类型：SF / FF / CF / FC。
        public byte bySeparationTimeMin;					// 连续帧发送的最小等待时间。
        public uint nRemainderFrameCount;					// 本次尚要发送的帧数，或是尚要接收的帧数；
													// 发送状态下为 0 表示正在等待流控制帧；
													// 接受状态下为 0 表示将要发送流控制帧。
        public byte byExpectedSequenceNumber;				// 期望发送或收到的分段序列号。
        public int dwTimingStartTick;					// 定时器开始的时间。

        public void ClearMessage()						// [线程安全] 终止消息收发。
        {
            vbyData.Clear();
	        stLocation = 0;
	        pciType = ProtocolCtrlType.UnKnown;
	        bySeparationTimeMin = 0;
	        nRemainderFrameCount = 0;
	        byExpectedSequenceNumber = 0;
	        dwTimingStartTick = 0;	        
        }
	}

    public class NetworkLayerDataBuffer:IDisposable
    {
        private static Logger LogNetLayer = LogManager.GetLogger("NetworkLayerDataBuffer");

        //是否回收完毕
        public bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //这里的参数表示示是否需要释放那些实现IDisposable接口的托管对象
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return; //如果已经被回收，就中断执行
            if (disposing)
            {
                //TODO:释放那些实现IDisposable接口的托管对象
            }
            //TODO:释放非托管资源，设置对象为null
            _disposed = true;
        }

        /// <summary>
        /// ID
        /// </summary>
        public UInt32 nID;									
        /// <summary>
        /// 将要发送，或是正在接收的数据
        /// </summary>
        public List<byte> nDataList = new List<byte>();							

        /// <summary>
        /// 将要发送或接收的位置
        /// </summary>
        public int currentDataOffset;			                
        /// <summary>
        /// 协议类型 PCI 类型：SF / FF / CF / FC。
        /// </summary>
        public ProtocolCtrlType pciType;							
        /// <summary>
        ///  连续帧发送的最小等待时间
        /// </summary>
        public byte bySeparationTimeMin;					
        /// <summary>
        ///  本次尚要发送的帧数，或是尚要接收的帧数
        /// </summary>
        public int nRemainderFrameCount;					
        /// <summary>
        /// 期望发送或收到的分段序列号
        /// </summary>
        public byte byExpectedSequenceNumber;
        public int TotalNum { get; set; }

        public int FS_Sign { get; set; }
        public byte BS_Num { get; set; }
        public byte STmin_ms { get; set; }
        
        public void ClearMessage()						
        {
            nDataList.Clear();
            currentDataOffset = 0;
            pciType = ProtocolCtrlType.UnKnown;
            bySeparationTimeMin = 0;
            nRemainderFrameCount = 0;
            byExpectedSequenceNumber = 0;            
        }
        
        public NetworkLayerDataBuffer()
        {
            BS_Num = 0xFF;
            byExpectedSequenceNumber = 0; 
            FS_Sign = (int)FlowControlType.ContinueToSend;
            STmin_ms = 0x00;
            //multiTimer.Elapsed += new System.Timers.ElapsedEventHandler(multiTimer_Tick);
        }

        private void multiTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
        }        

        public static byte[] SetSingleFrame(int _num)
        {
            return new byte[] { (byte)(((int)ProtocolCtrlType.SingleFrame) << 4 | _num) };
        }
        public static byte[] SetFirstFrame(int _num)
        {
            return new byte[] { (byte)(((int)ProtocolCtrlType.FirstFrame) << 4 | _num >> 8), (byte)(_num & 0x000000FF) };
        }
        public static byte[] SetConsecutiveFrame(int _num)
        {
            return new byte[] { (byte)(((int)ProtocolCtrlType.ConsecutiveFrame) << 4 | _num) };
        }
        public static byte[] SetFlowControlFrame(int _status, byte _bsNum, byte _stmin)
        {
            return new byte[] { (byte)(((int)ProtocolCtrlType.FlowControl) << 4 | _status), _bsNum, _stmin };
        }

        public static byte GetSISign(List<byte> canData)
        {
            if ((canData[0] >> 4) == (byte)ProtocolCtrlType.FirstFrame)
            {
                return canData[2];
            }
            else
            {
                return 0;
            }
        }

        public static int GetDIDSign(List<byte> canData)
        {
            //int result = 0;
            return (canData[3] << 8) | canData[4];
        }

        /// <summary>
        /// 根据CAN帧数据获取头标识
        /// </summary>
        /// <param name="canData">CAN帧数据</param>
        /// <returns></returns>
        public static int GetHeadSign(List<byte> canData)
        {
            if (canData.Count == 0)
                return -1;
            return canData[0] >> 4;
        }
        /// <summary>
        /// 获取SingleFrame_DL——单帧字节数[1-7]
        /// </summary>
        /// <param name="canData">CAN帧数据</param>
        /// <returns></returns>
        public static int GetSF_DLSign(List<byte> canData)
        {
            return canData[0] & 0x0F;
        }
        /// <summary>
        /// 获取FirstFrame_DL——多帧总字节数[8-0xFFF]
        /// </summary>
        /// <param name="canData">CAN帧数据</param>
        /// <returns></returns>
        public static int GetFF_DLSign(List<byte> canData)
        {
            return ((canData[0] & 0x0F) << 8) | canData[1];
        }
        /// <summary>
        /// 获取ConsecutiveFrame多帧包的当前帧索引，连续帧分帧[0-15]循环
        /// </summary>
        /// <param name="canData">CAN帧数据</param>
        /// <returns></returns>
        public static int GetSN_DLSign(List<byte> canData)
        {
            return canData[0] & 0x0F;
        }
        /// <summary>
        /// 获取FlowControl_DL流状态
        /// </summary>
        /// <param name="canData">CAN帧数据</param>
        /// <returns></returns>
        public static FlowControlType GetFS_DLSign(List<byte> canData)
        {
            switch (canData[0] & 0x0F)
            {
                case 1:
                    return FlowControlType.Wait;

                case 2:
                    return FlowControlType.Overflow;

                default:
                    return FlowControlType.ContinueToSend;
            }
        }
        /// <summary>
        /// 获取BlockSize_DL——块容量：流控间要发送的CF数目
        /// </summary>
        /// <param name="canData">CAN帧数据</param>
        /// <returns></returns>
        public static int GetBS_Sign(List<byte> canData)
        {
            return canData[1];
        }
        /// <summary>
        /// 获取SingleFrame_DL
        /// </summary>
        /// <param name="canData">CAN帧数据</param>
        /// <returns></returns>
        public static int GetSTmin_DLSign(List<byte> canData)
        {
            return canData[2];
        }

        /// <summary>
        /// 发送单帧
        /// </summary>
        /// <returns></returns>
        public ZLG_API.VCI_CAN_OBJ SendSingleFrame()
        {
            List<byte> sendData = new List<byte> { };
            sendData.AddRange(SetSingleFrame(TotalNum));
            sendData.AddRange(SetFlowControlFrame(FS_Sign, BS_Num, STmin_ms));

            return NetWorkCommClass.ConvertToCANFrame(this.nID, sendData);
        }

        /// <summary>
        /// 发送流控
        /// </summary>
        /// <returns></returns>
        public ZLG_API.VCI_CAN_OBJ SendFlowControl()
        {
            List<byte> sendData = new List<byte> { };
            sendData.AddRange(SetFlowControlFrame(FS_Sign, BS_Num, STmin_ms));

            return NetWorkCommClass.ConvertToCANFrame(ApplicationLayerProtocol.RequestID, sendData);
        }


        /// <summary>
        /// 发送首帧
        /// </summary>
        /// <returns></returns>
        public ZLG_API.VCI_CAN_OBJ SendFirstFrame()
        {
            List<byte> sendData = new List<byte> { };
            sendData.AddRange(SetFirstFrame(TotalNum));
            sendData.AddRange(nDataList.Take(6));
            return NetWorkCommClass.ConvertToCANFrame(this.nID, sendData);
        }
        /// <summary>
        /// 发送连续帧
        /// </summary>
        /// <param name="_runout"></param>
        /// <returns></returns>
        public ZLG_API.VCI_CAN_OBJ SendConsecutiveFrame()
        {
            List<byte> sendData = new List<byte> { };
            sendData.AddRange(SetConsecutiveFrame(byExpectedSequenceNumber));
            for (int i = 0; i < 7; i++)
            {
                if (nDataList.Count > 0)
                {
                    sendData.Add(nDataList[currentDataOffset]);
                    currentDataOffset++;
                }
            }
            return NetWorkCommClass.ConvertToCANFrame(this.nID, sendData);//new SendSingleFrame(_strID, sendData, "连续帧");
        }         
        
    }    


    public class NetWorkCommClass
    {
        private static Logger LogNetLayer = LogManager.GetLogger("NetworkLayerDataBuffer");

        public static SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        
        /// <summary>
        /// 多帧接收缓存
        /// </summary>
        public static NetworkLayerDataBuffer multiFrameReceive = new NetworkLayerDataBuffer();

        /// <summary>
        /// 多帧发送缓存
        /// </summary>
        public static NetworkLayerDataBuffer multiFrameSend = new NetworkLayerDataBuffer();



        public static Action<NetworkLayerDataBuffer> recFuc; 

        public static DataLinkLayer DataLinkLayerVar = new DataLinkLayer();

        public static System.Timers.Timer multiTimer = new System.Timers.Timer(100);

        public event EventHandler NetWorkLayerCommEventHandler;

        public void NetWorkLayerCommEvent(EventArgs e)
        {
            if (NetWorkLayerCommEventHandler != null)
            {
                NetWorkLayerCommEventHandler(this, e);
            }
        }

        public NetWorkCommClass()
        {
            DataLinkLayerVar.DataLayerCommEventHandler += DataLinkLayer_DataLayerCommEventHandler;
            recFuc += _multi =>
            {
                NetWorkLayerCommEvent(
                    new NetWorkLayerEvent() 
                    {
                        eventType = NetWorkEventType.NetworkReceiveEvent,
                        ID = _multi.nID,
                        listData = _multi.nDataList
                    });
            };
            multiTimer.Elapsed+=multiTimer_Elapsed;
        }

        private void multiTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {               
            if (multiFrameSend.nRemainderFrameCount != 0)
            {
                DataLinkLayer.sendFuc(multiFrameSend.SendConsecutiveFrame());
            }            
        }        
        

        public void DisposeReceivedFlag(DataLayerEvent _event)
        {
            switch ((ProtocolCtrlType)NetworkLayerDataBuffer.GetHeadSign(_event.listData))
            {
                case ProtocolCtrlType.SingleFrame://如果接收到的是单帧则立刻把该单帧上传到应用层

                    LogNetLayer.Info("接收数据...4...0 ... ");
                    multiFrameReceive.nID = _event.ID;
                    multiFrameReceive.TotalNum = NetworkLayerDataBuffer.GetSF_DLSign(_event.listData);
                    if (multiFrameReceive.TotalNum == 0 || multiFrameReceive.TotalNum > 7)
                        return;

                    int nnDataIndex = 1;
                    multiFrameReceive.nDataList.Clear();
                    multiFrameReceive.nDataList = new List<byte>(new byte[multiFrameReceive.TotalNum]);

                    for (int i = (int)nnDataIndex; i != multiFrameReceive.TotalNum + 1; ++i)
                    {
                        multiFrameReceive.nDataList[(int)(i - nnDataIndex)] = _event.listData[i];
                    }
                   
                    recFuc(multiFrameReceive);
                    
                    break;
                case ProtocolCtrlType.FirstFrame://如果接收到的是首帧，则发送流控帧到数据链路层，然后准备接收该多帧信息
                    multiFrameReceive.nID = _event.ID;
                    multiFrameReceive.TotalNum = NetworkLayerDataBuffer.GetFF_DLSign(_event.listData);
                    if (multiFrameReceive.TotalNum < 8)
                        return;

                    int nDataIndex = 2;
                    multiFrameReceive.nDataList.Clear();
                    multiFrameReceive.nDataList = new List<byte>(new byte[multiFrameReceive.TotalNum]);                    
                                       

                    for (int i = nDataIndex; i != 8; ++i)
                    {
                        multiFrameReceive.nDataList[(int)(i - nDataIndex)] = _event.listData[i];
                    }

                    int frmCount = 0;

                    if ((multiFrameReceive.TotalNum - 6) / 7 == 0)
                    {
                        frmCount = (multiFrameReceive.TotalNum - 6) / 7;
                    }
                    else
                    {
                        frmCount = (multiFrameReceive.TotalNum - 6) / 7 + 1;
                    }

                    multiFrameReceive.currentDataOffset = 6;
                    multiFrameReceive.pciType = ProtocolCtrlType.FlowControl;
                    multiFrameReceive.nRemainderFrameCount = frmCount;
                    multiFrameReceive.byExpectedSequenceNumber = 1;                    

                    //发送流控
                    DataLinkLayer.sendFuc(multiFrameReceive.SendFlowControl());
                    //multiFrameReceive.SetMultiReceivedFrame(_event);             
                    break;
                case ProtocolCtrlType.ConsecutiveFrame://接收首帧中指定服务的连续帧
                    if (multiFrameReceive.byExpectedSequenceNumber != NetworkLayerDataBuffer.GetSN_DLSign(_event.listData))
                        return;

                    if (multiFrameReceive.nDataList.Count > multiFrameReceive.currentDataOffset)
                    {
                        multiFrameReceive.byExpectedSequenceNumber = (byte)((multiFrameReceive.byExpectedSequenceNumber + 1) % 0x10);
                        int nReceiveLength = System.Math.Min(7, multiFrameReceive.nDataList.Count - multiFrameReceive.currentDataOffset);
                        for (int i = 0; i != nReceiveLength; ++i)
                        {
                            if (multiFrameReceive.currentDataOffset + i <= multiFrameReceive.nDataList.Count)
                                multiFrameReceive.nDataList[(int)(multiFrameReceive.currentDataOffset + i)] = _event.listData[(int)(i + 1)];
                        }
                        multiFrameReceive.currentDataOffset += nReceiveLength;
                        --multiFrameReceive.nRemainderFrameCount;
                        if (multiFrameReceive.currentDataOffset > multiFrameReceive.nDataList.Count)
                        {
                            //List<byte> vbyData = multiFrameReceive.nDataList;
                            multiFrameReceive.ClearMessage();

                        }
                        else
                        {
                            if (multiFrameReceive.nRemainderFrameCount == 0)
                            {
                                //ApplicationLayerProtocol.Response(multiFrameReceive.nID, multiFrameReceive.nDataList);
                                recFuc(multiFrameReceive);
                            }
                        }
                    }
                    break;

                case ProtocolCtrlType.FlowControl:

                    switch (NetworkLayerDataBuffer.GetFS_DLSign(_event.listData))
                    {
                        case FlowControlType.ContinueToSend:
                            multiFrameReceive.bySeparationTimeMin = _event.listData[2];                            
                            if (multiFrameReceive.bySeparationTimeMin >= 0x80 && multiFrameReceive.bySeparationTimeMin <= 0xF0 || multiFrameReceive.bySeparationTimeMin >= 0xFA)
                            {
                                multiFrameReceive.bySeparationTimeMin = 0x7F;
                            }
                            multiTimer.Interval = multiFrameReceive.bySeparationTimeMin;
                            multiFrameSend.nRemainderFrameCount = _event.listData[1];
                            StartTimer();
                            //开启发送定时器
                            break;
                        case FlowControlType.Wait:
                            StopTimer();
                            //停止发送定时器
                            break;
                        case FlowControlType.Overflow:
                            multiFrameSend.ClearMessage();
                            return;
                        default:
                            multiFrameSend.ClearMessage();
                            break;
                    }
                    break;                
            }
        }

        /// <summary>
        /// 开始发送多帧数据
        /// </summary>
        public static void StartSend()
        {
            DataLinkLayer.sendFuc(multiFrameSend.SendFirstFrame());
            StartTimer();
        }

        //开启发送定时器
        public static void StartTimer()
        {
            if (multiTimer != null)
                multiTimer.Start();
        }

        /// <summary>
        /// 关闭定时器
        /// </summary>
        public static void StopTimer()
        {
            if (multiTimer != null)
                multiTimer.Stop();
        }        

        private static void SynchroSend(object sfc_object)
        {
            var sfc = sfc_object as ApplicationLayerProtocol;
            //字节小于7个
            if (sfc.A_Data.Count <= 7)
            {
                _semaphore.Wait();
                List<byte> sendData = new List<byte> { };
                sendData.AddRange(NetworkLayerDataBuffer.SetSingleFrame(sfc.A_Data.Count));
                sendData.AddRange(sfc.A_Data);
                DataLinkLayer.sendFuc(ConvertToCANFrame(sfc.Address, sendData));                
                _semaphore.Release();
            }
            else
            {
                multiFrameSend.nDataList = sfc.A_Data;
                multiFrameSend.nID = sfc.Address;
                StartSend();

              
            }
        }
        public static void DisposeSendFlag(ApplicationLayerProtocol AppFrame)
        { 
            //启用线程池
            ThreadPool.QueueUserWorkItem(new WaitCallback(SynchroSend), AppFrame);  
        }
        
        public static ZLG_API.VCI_CAN_OBJ ConvertToCANFrame(UInt32 id,List<byte> data)
        {   
            ZLG_API.VCI_CAN_OBJ CanFrame = new ZLG_API.VCI_CAN_OBJ();
            CanFrame.ID = id;
            CanFrame.ExternFlag = 0;
            CanFrame.RemoteFlag = 0;
            CanFrame.SendType = 1;
            CanFrame.TimeFlag = 1;
            CanFrame.DataLen = (byte)data.Count;
            CanFrame.Data = data.ToArray();

            return CanFrame;
        }

        private void DataLinkLayer_DataLayerCommEventHandler(object sender, EventArgs e)
        {
            var dataLayerEvent = e as DataLayerEvent;
            if (dataLayerEvent == null)
            {
                return;
            }
            switch (dataLayerEvent.eventType)
            {
                case DataLayerEventType.DataLayerSendEvent:                    
                    break;
                case DataLayerEventType.DataLayerReceiveEvent:
                    LogNetLayer.Info("接收数据...3...0 ... ");
                    DisposeReceivedFlag(dataLayerEvent);                    
                    break;
                case DataLayerEventType.Other:
                    break;
                default:
                    break;
            }
        }
    }

    public class NetworkLayerProtocol
    {
        /// <summary>
        /// 地址信息
        /// </summary>
        //private uint n_AI;
        public uint N_AI{ get; set; }

        /// <summary>
        /// 网络层协议控制信息
        /// </summary>
        //private List<byte> n_PCI;
        public List<byte> N_PCI{ get; set; }

        /// <summary>
        /// 帧数据
        /// </summary>
        //private List<byte> n_Data;
        public List<byte> N_Data{ get; set; }


        /// <summary>
        /// 帧类型
        /// </summary>
        //private byte n_PCIType;
        public byte N_PCIType{ get; set; }

        /// <summary>
        /// 单帧数据长度
        /// </summary>
        //private byte sF_DL;
        public byte SF_DL{ get; set; }

        /// <summary>
        /// 首帧数据长度
        /// </summary>
        //private Int16 fF_DL;
        public Int16 FF_DL{ get; set; }

        /// <summary>
        /// 连续帧编号
        /// </summary>
        //private byte cF_SN;
        public byte CF_SN{ get; set; }

        /// <summary>
        /// 流控帧状态
        /// </summary>
        //private byte fC_FS;
        public byte FC_FS{ get; set; }

        /// <summary>
        /// 允许的帧数量
        /// </summary>
        //private byte fC_BS;
        public byte FC_BS{ get; set; }

        /// <summary>
        /// 时间间隔
        /// </summary>
        //private byte fC_STmin;
        public byte FC_STmin{ get; set; }


        //private static bool isBusy = false;
        public static bool IsBusy { get; set; }

        //private static bool isPriority = false;
        public static bool IsPriority{ get; set; }


        //private ProtocolCtrlType pCIType;
        public ProtocolCtrlType PCIType{ get; set; }


        //public event EventHandler NetworkCommEventHandler;

        //public void NetworkCommEvent(EventArgs e)
        //{
        //    if (NetworkCommEventHandler != null)
        //    {
        //        NetworkCommEventHandler(this, e);
        //    }
        //}

        public static Action<ZLG_API.VCI_CAN_OBJ> sendFuc;

        public NetworkLayerProtocol()
        {
            sendFuc += _nF =>
                {
                    DataLinkLayer.sendFuc(_nF);
                };
        }

        public static MessageBuffer m_messageBuffer = new MessageBuffer();
        public static MessageBuffer m_RcvmessageBuffer = new MessageBuffer();

        public DataLinkLayer datalink = new DataLinkLayer();

        public ApplicationLayerProtocol applink = new ApplicationLayerProtocol();

        public static void Request(UInt32 nID, List<byte> nData, bool isPriority)
        {
            IsPriority = isPriority;
            if (!IsBusy && !isPriority)
            {
                _Request(FillBuffer(nID, nData));
            }
            else if (isPriority)
            {
                _Request(FillBuffer(nID, nData));
            }

        }

        public static MessageBuffer FillBuffer(UInt32 nID, List<byte> nData)
        {            
	        m_messageBuffer.nID = nID;
	        m_messageBuffer.vbyData = nData;
	        m_messageBuffer.stLocation = 0;
	        m_messageBuffer.bySeparationTimeMin = 0;
	        m_messageBuffer.nRemainderFrameCount = 0;
	        m_messageBuffer.byExpectedSequenceNumber = 0;
	        m_messageBuffer.dwTimingStartTick = 0;	        

	        if (nData.Count() < 7)
	        {
		        m_messageBuffer.pciType = ProtocolCtrlType.SingleFrame;
	        }
	        else
	        {
		        m_messageBuffer.pciType = ProtocolCtrlType.FirstFrame;
	        }

            return m_messageBuffer;
        }        

        public static void Response(UInt32 nID,List<byte> nData)
        {               
            IsBusy = true;
            //MessageBuffer messageBuffer = FillBuffer(nID, nData);
            byte byPCIFirst = nData[0];
            byte byPCIType = (byte)((byPCIFirst & 0xF0) >> 4);
            uint nDataIndex;
            uint nApplicationLayerDataLength;
            // 对于收到非期望 N_PDU 的处理。
            // 15765-2: 6.7.3
            switch ((ProtocolCtrlType)byPCIType)
            {
                case ProtocolCtrlType.SingleFrame:
                    // 15675-2: 6.5.2
                    nApplicationLayerDataLength = (uint)(byPCIFirst & 0x0F);
                    if (nApplicationLayerDataLength == 0 || nApplicationLayerDataLength > 7)
                        return;

                    nDataIndex = 1;

                    m_RcvmessageBuffer.vbyData.Clear();
                    m_RcvmessageBuffer.vbyData = new List<byte>(new byte[nApplicationLayerDataLength]);//new List<byte>((int)nApplicationLayerDataLength);

                    for (int i = (int)nDataIndex; i != nApplicationLayerDataLength+1; ++i)
                    {
                        m_RcvmessageBuffer.vbyData[(int)(i - nDataIndex)] = nData[i];
                    }
                    //m_messageBuffer.vbyData.AddRange(nData);

                    //applink.Response(m_RcvmessageBuffer.nID, m_RcvmessageBuffer.vbyData);
                    IsBusy = false;
                    break;

                case ProtocolCtrlType.FirstFrame:
                    nApplicationLayerDataLength = (uint)(((byPCIFirst & 0x0F) << 8) | nData[1]);
                    if (nApplicationLayerDataLength < 8)
			        {				
				        return;
			        }

                    nDataIndex = 2;
			        // 假设不会溢出。
                    m_RcvmessageBuffer.vbyData.Clear();
                    m_RcvmessageBuffer.vbyData = new List<byte>(new byte[nApplicationLayerDataLength]);
                    //m_messageBuffer.vbyData = new List<byte>((int)nApplicationLayerDataLength);

                    for (int i = (int)nDataIndex; i != 8; ++i)
                    {
                        m_RcvmessageBuffer.vbyData[(int)(i - nDataIndex)] = nData[i];
                    }


                    uint frmCount = 0;

                    if ((nApplicationLayerDataLength - 6) / 7 == 0)
                    {
                        frmCount = (nApplicationLayerDataLength - 6) / 7;
                    }
                    else
                    {
                        frmCount = (nApplicationLayerDataLength - 6) / 7 + 1;
                    }

                    m_RcvmessageBuffer.nID = 0x7AC;
                    m_RcvmessageBuffer.stLocation = 8 - nDataIndex;
                    m_RcvmessageBuffer.pciType = ProtocolCtrlType.FlowControl;
                    m_RcvmessageBuffer.nRemainderFrameCount = frmCount;
                    m_RcvmessageBuffer.byExpectedSequenceNumber = 1;
			        //m_signalFirstFrameIndication(nID, nApplicationLayerDataLength);

                    _Request(m_RcvmessageBuffer);

                    break;

                case ProtocolCtrlType.ConsecutiveFrame:

                    // 15765-2: 6.5.4

			        // 错误处理。
                    if (m_RcvmessageBuffer.byExpectedSequenceNumber != (byPCIFirst & 0x0F))
			        {				
				        return;
			        }
                    m_RcvmessageBuffer.byExpectedSequenceNumber = (byte)((m_RcvmessageBuffer.byExpectedSequenceNumber + 1) % 0x10);

			        nDataIndex = 1;
                    uint nReceiveLength = (uint)System.Math.Min(7, m_RcvmessageBuffer.vbyData.Count - m_RcvmessageBuffer.stLocation);
			        for (int i = 0; i != nReceiveLength; ++i)
			        {
                        m_RcvmessageBuffer.vbyData[(int)(m_RcvmessageBuffer.stLocation + i)] = nData[(int)(i + nDataIndex)];
			        }
                    m_RcvmessageBuffer.stLocation += nReceiveLength;
                    --m_RcvmessageBuffer.nRemainderFrameCount;
                    if (m_RcvmessageBuffer.stLocation > m_RcvmessageBuffer.vbyData.Count)
			        {
                        List<byte> vbyData = m_RcvmessageBuffer.vbyData;
                        m_RcvmessageBuffer.ClearMessage();
				        //m_signalIndication(nID, m_messageBuffer.vbyData, Diagnostic::NetworkLayerResult::N_OK);
			        }
			        else
			        {
				        //m_messageBuffer.ResetTiming(TimingType::Cr, m_eventTiming);
                        if (m_RcvmessageBuffer.nRemainderFrameCount == 0)
				        {
                            //m_messageBuffer.pciType = ProtocolCtrlType.FlowControl;
                            //_Request(m_messageBuffer);
                            //applink.Response(m_RcvmessageBuffer.nID, m_RcvmessageBuffer.vbyData);
                            IsBusy = false;
				        }
			        }

                    break;

                case ProtocolCtrlType.FlowControl:

                    // 15765-2: 6.5.5
                    byte byFlowControlType = (byte)(byPCIFirst & 0x0F);
                    // 错误处理与流控制类型判断。
                    switch((FlowControlType)byFlowControlType)
			        {
			        case FlowControlType.ContinueToSend:
				        {
                            m_RcvmessageBuffer.pciType = ProtocolCtrlType.ConsecutiveFrame;
                            m_RcvmessageBuffer.bySeparationTimeMin = nData[2];
                            if (m_RcvmessageBuffer.bySeparationTimeMin >= 0x80 && m_RcvmessageBuffer.bySeparationTimeMin <= 0xF0 || m_RcvmessageBuffer.bySeparationTimeMin >= 0xFA)
					        {
                                m_RcvmessageBuffer.bySeparationTimeMin = 0x7F;
					        }

                            m_RcvmessageBuffer.nRemainderFrameCount = nData[1];					        
					
					        // 15765-2: 6.5.5.4, Table 14
                            if (m_RcvmessageBuffer.nRemainderFrameCount == 0)
					        {
                                m_RcvmessageBuffer.nRemainderFrameCount = 1;	// 取最大值
					        }

                            //DWORD dwWaitResult = WaitForSingleObject(m_eventStopThread.m_hObject, m_messageBuffer.bySeparationTimeMin);
                            //if (dwWaitResult == WAIT_TIMEOUT)
                            //{
                            _Request(m_RcvmessageBuffer);
                            //}
					        break;
				        }
			        case FlowControlType.Wait:
                        //TRACE(_T("FlowControl: Wait.\n"));
                        //_AddWatchEntry(EntryType::Receive, nID, IDS_NETWORKLAYERWATCH_FLOWCONTROL_WAIT);
                        //m_messageBuffer.ResetTiming(TimingType::Bs, m_eventTiming);
				        return;
				        
			        case FlowControlType.Overflow:
                        //TRACE(_T("FlowControl: Overflow.\n"));
                        //_AddWatchEntry(EntryType::Receive, nID, IDS_NETWORKLAYERWATCH_FLOWCONTROL_OVERFLOW, Color::Red);
                        m_RcvmessageBuffer.ClearMessage();
				        //m_signalConfirm(Diagnostic::NetworkLayerResult::N_BUFFER_OVFLW);
				        return;
			        default:
                        //TRACE(_T("FlowControl: Invalid flowcontrol.\n"));
                        //_AddWatchEntry(EntryType::Receive, nID, IDS_NETWORKLAYERWATCH_FLOWCONTROL_INVALID, Color::Red);
                        //m_messageBuffer.ResetTiming(TimingType::Idle, m_eventTiming);
                        m_RcvmessageBuffer.ClearMessage();
				        //m_signalConfirm(Diagnostic::NetworkLayerResult::N_INVALID_FS);
				        return;
			        }
                    break;
            }
            
        }

        public static void _Request(MessageBuffer m_messageBuffer)
        {

            byte[] btArrRequest = new byte[8];

            //vbyPDU.Capacity=8;	

	        byte byPCIFirst;
	        byPCIFirst = (byte)(((byte)m_messageBuffer.pciType) << 4);
	        int nDataSize = m_messageBuffer.vbyData.Count;
	        int nPDUDataSize = 7;	// PDU 包含的数据长度，在首帧中需要减一，流控制帧中不包含数据帧。

	        switch (m_messageBuffer.pciType)
	        {
	            case ProtocolCtrlType.SingleFrame:		

		            if (nDataSize > nPDUDataSize)
		            {
			            nDataSize = nPDUDataSize;
		            }
		            byPCIFirst = (byte)(byPCIFirst | nDataSize);
		            btArrRequest[0] = byPCIFirst;

                    for (int n = 1; n < btArrRequest.Length;n++ )
                    {
                        btArrRequest[n] = m_messageBuffer.vbyData[n - 1];
                    }
                        //vbyPDU.AddRange(m_messageBuffer.vbyData);
		
		            break;
	            case ProtocolCtrlType.FirstFrame:
		
		            if (nDataSize > 0xFFF)
		            {
			            nDataSize = 0xFFF;
		            }	

		            --nPDUDataSize;
		            byPCIFirst = (byte)(byPCIFirst | nDataSize >> 8);

                    btArrRequest[0] = byPCIFirst;
                    btArrRequest[0] = (byte)(nDataSize & 0xFF);

                    byte[] data = new byte[nPDUDataSize];

                    m_messageBuffer.vbyData.CopyTo(0, data, 0, data.Length);

                    for (int n = 1; n < data.Length; n++)
                    {
                        btArrRequest[n] = data[n - 1];
                    }
		
		            m_messageBuffer.stLocation = (uint)nPDUDataSize;
		            m_messageBuffer.nRemainderFrameCount = 0;
		            m_messageBuffer.byExpectedSequenceNumber = 1;
		            break;
	            case ProtocolCtrlType.ConsecutiveFrame:
		
		            if (m_messageBuffer.vbyData.Count() <= m_messageBuffer.stLocation)
		            {			
			            return;
		            }
		            byPCIFirst = (byte)(byPCIFirst | m_messageBuffer.byExpectedSequenceNumber);           
		            m_messageBuffer.byExpectedSequenceNumber = (byte)((m_messageBuffer.byExpectedSequenceNumber + 1) % 0x10);

                    btArrRequest[0] = byPCIFirst;
		            nPDUDataSize = (int)System.Math.Min(nPDUDataSize, m_messageBuffer.vbyData.Count() - m_messageBuffer.stLocation);

                    for (int n = 1; n < m_messageBuffer.vbyData.Count; n++)
                    {
                        btArrRequest[n] = m_messageBuffer.vbyData[n - 1];
                    }

		
		            m_messageBuffer.stLocation += (uint)nPDUDataSize;
		            --m_messageBuffer.nRemainderFrameCount;
		            break;

	            case ProtocolCtrlType.FlowControl:	
                        //m_byBlockSize=0xFF;
		            // 暂时仅发送 ContinueToSend。
		            byPCIFirst = (byte)(byPCIFirst | 0x0);

                    btArrRequest[0] = byPCIFirst;
                    btArrRequest[1] = 0xFF;
                    btArrRequest[2] = 0x64;
		
		            break;
	        }
            DataLinkLayer.SendCanFrame(m_messageBuffer.nID, btArrRequest);	
        }

        //public NetworkLayerProtocol() 
        //{            
        //    //m_messageBuffer = new MessageBuffer();
        //    //datalink.responseSendEvent += responseSendEvent_Trigged;
        //} 
        
        
        private List<byte> getN_PCI(byte type)
        {
            List<byte> result = new List<byte>();

            switch (type)
            {
                case 0:
                    N_PCIType = type;
                    result = GetSFN_PCI(type, SF_DL);
                    break;
                case 1:
                    result = GetFFN_PCI(type, FF_DL);
                    break;
                case 2:
                    result = GetCFN_PCI(type, CF_SN);
                    break;
                case 3:
                    result = GetFCN_PCI();
                    break;
            }

            return result;
        }

        /// <summary>
        /// 获得单帧N_PCI
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sf_DL"></param>
        /// <returns></returns>
        private List<byte> GetSFN_PCI(byte type,byte sf_DL)
        {
            List<byte> result = new List<byte>();
            byte pci = (byte)((type << 4) | (sf_DL));
            result.Add(pci);
            return result;
        }

        /// <summary>
        /// 获得首帧N_PCI
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ff_DL"></param>
        /// <returns></returns>
        private List<byte> GetFFN_PCI(byte type,Int16 ff_DL)
        {
            List<byte> result = new List<byte>();

            byte[] pci = new byte[2];
            byte msbByte = (byte)((ff_DL & 0xFF00) | (type << 4));
            byte lsbByte = (byte)(ff_DL & 0x0FF);

            pci[0] = msbByte;
            pci[1] = lsbByte;

            result = pci.ToList();
            return result;
        }

        /// <summary>
        /// 获得连续帧N_PCI
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cf_SN"></param>
        /// <returns></returns>
        private List<byte> GetCFN_PCI(byte type, byte cf_SN)
        {
            List<byte> result = new List<byte>();
            byte pci = (byte)((type << 4) | (cf_SN));
            
            result.Add(pci);
            return result;
        }

        /// <summary>
        /// 获得流控帧N_PCI
        /// </summary>
        /// <returns></returns>
        private List<byte> GetFCN_PCI()
        {
            List<byte> result = new List<byte>();
            byte[] pci = new byte[3];
            pci[0] = FC_FS;
            pci[1] = FC_BS;
            pci[2] = FC_STmin;
            result = pci.ToList();

            return result;            
        }

        public static NetworkLayerProtocol Parse(DataLinkLayer dataLinkData)
        {
            NetworkLayerProtocol networkData = new NetworkLayerProtocol();
            networkData.N_AI = dataLinkData.Address;
            networkData.N_PCIType = (byte)(dataLinkData.L_Data[0] & 0xF0);

            if (networkData.N_PCIType == 0x01)
            {
                networkData.SF_DL = (byte)(dataLinkData.L_Data[0] & 0x0F);
                if ((networkData.SF_DL > 7) || (networkData.SF_DL == 0))
                    throw new Exception("单帧长度错误");
                networkData.N_PCI = networkData.GetSFN_PCI(networkData.N_PCIType, networkData.SF_DL);
                dataLinkData.L_Data.RemoveRange(0, 1);
                networkData.N_Data = dataLinkData.L_Data;
            }
            else if (networkData.N_PCIType == 0x03)
            {
                networkData.FC_FS = (byte)(dataLinkData.L_Data[0] & 0xF0);
                if (networkData.FC_FS == 3)
                    throw new Exception("Buffer 溢出");
                //networkData.N_PCI = networkData.GetFCN_PCI();
                //dataLinkData.L_Data.RemoveRange(0, 3);
                //networkData.N_Data = dataLinkData.L_Data;
            }
            else
            {
                if (networkData.N_PCIType == 0x01)
                {
                    networkData.FF_DL = networkData.GetFF_DL(dataLinkData.L_Data[0], dataLinkData.L_Data[1]);
                    if ((networkData.FF_DL < 8) || (networkData.FF_DL > 0xFFF))
                        throw new Exception("首帧长度错误");

                    networkData.N_PCI = networkData.GetFFN_PCI(networkData.N_PCIType, networkData.FF_DL);
                    dataLinkData.L_Data.RemoveRange(0, 2);
                    networkData.N_Data.AddRange(dataLinkData.L_Data);
                }
                else
                {
                    networkData.CF_SN = (byte)(dataLinkData.L_Data[0] & 0xF0);
                    if ((networkData.CF_SN > 15) || (networkData.CF_SN < 0))
                        throw new Exception("连续帧序号错误");

                    networkData.N_PCI = networkData.GetCFN_PCI(networkData.N_PCIType, dataLinkData.L_Data[0]);
                    dataLinkData.L_Data.RemoveRange(0, 1);
                    networkData.N_Data.AddRange(dataLinkData.L_Data);
                }
            }           

            return networkData;
        }

        public static string GetProtocolFrameType(byte typeId)
        {
            string result = string.Empty;

            switch ((typeId&0xF0)>>4)
            {
                case 0:
                    result = "单帧";
                    break;

                case 1:
                    result = "首帧";
                    break;

                case 2:
                    result = "连续帧";
                    break;

                case 3:
                    result = "流控帧";
                    break;

                default:
                    result = "UnKnown";
                    break;
            }

            return result;
        }

        public static string GetServiceType(byte[] data,bool isSend)
        {
            string result = string.Empty;

            byte sId=0x00;
            byte frameType = (byte)(((data[0] & 0xF0) >> 4));
            if (frameType == 0x00 || frameType == 0x01)
            {
                if (isSend)
                {
                    if (frameType == 0x01) //判断是否为首帧
                    {
                        sId = data[2];
                    }
                    else if (frameType == 0x00)
                    {
                        sId = data[1];
                    }
                }
                else
                {
                    if (frameType == 0x01) //判断是否为首帧
                    {
                        sId = (byte)(data[2] - 0x40);
                    }
                    else if (frameType == 0x00)
                    {
                        sId = (byte)(data[1] - 0x40);
                    }
                }
                switch (sId)
                {
                    case ServicesID.DiagnosticSessionControl:
                        result = "会话控制";
                        break;
                    case ServicesID.ECUReset:
                        result = "ECU复位";
                        break;
                    case ServicesID.ClearDiagnosticInformation:
                        result = "清除故障码";
                        break;
                    case ServicesID.ReadDTCInformation:
                        result = "读取故障码";
                        break;
                    case ServicesID.ReadDataByIdentifier:
                        result = "读取数据";
                        break;
                    case ServicesID.ReadMemoryByAddress:
                        result = "由地址读取内存";
                        break;
                    case ServicesID.ReadScalingDataByIdentifier:
                        result = "读标定信息";
                        break;
                    case ServicesID.SecurityAccess:
                        result = "安全访问";
                        break;
                    case ServicesID.CommunicationControl:
                        result = "通讯控制";
                        break;
                    case ServicesID.ReadDataByPeriodicIdentifier:
                        result = "周期性读数据";
                        break;
                    case ServicesID.DynamicallyDefineDataIdentifier:
                        result = "动态定义数据";
                        break;
                    case ServicesID.WriteDataByIdentifier:
                        result = "写数据";
                        break;
                    case ServicesID.InputOutputControlByIdentifier:
                        result = "输入输出控制";
                        break;
                    case ServicesID.RoutineControl:
                        result = "例程控制";
                        break;
                    case ServicesID.RequestDownload:
                        result = "请求下载";
                        break;
                    case ServicesID.RequestUpload:
                        result = "请求上传";
                        break;
                    case ServicesID.TransferData:
                        result = "传输数据";
                        break;
                    case ServicesID.RequestTransferExit:
                        result = "请求退出传输";
                        break;
                    case ServicesID.WriteMemoryByAddress:
                        result = "写入内存";
                        break;
                    case ServicesID.TesterPresent:
                        result = "诊断仪在线";
                        break;
                    case ServicesID.AccessTimingParameter:
                        result = "访问定时参数";
                        break;
                    case ServicesID.SecuredDataTransmission:
                        result = "安全数据传输";
                        break;
                    case ServicesID.ControlDTCSetting:
                        result = "控制DTC设置";
                        break;
                    case ServicesID.LinkControl:
                        result = "链路控制";
                        break;
                    default:
                        result = "";
                        break;
                }
            }                        

            return result;
        }

        public void NetworkLayerToApplicationLayer(NetworkLayerProtocol networkLayer)
        {
            //ApplicationLayerProtocol app = ApplicationLayerProtocol.Parse(networkLayer);            
        }

        private Int16 GetFF_DL(byte bt1, byte bt2)
        {
            Int16 result = 0;
            result <<= 8;
            result |= bt1;
            result <<= 8;
            result |= bt2;

            result &= 0x0FFF;
            return result;
        }
        

        public void canFrameAnalysis(ZLG_API.VCI_CAN_OBJ canFrame)
        {
            N_AI = canFrame.ID;

        }

        public void sendFlowCtrlFrameToDataLinkLayer()
        {
            byte[] pci = new byte[3];
            pci[0] = 0x30;
            pci[1] = 0x00;
            pci[2] = 0x00;
            N_PCI = pci.ToList();
            N_Data.InsertRange(0, N_PCI);
            //N_PCI=
        }

        private byte[] Datatransform(byte bt, int integer)
        {
            byte[] result = new  byte[2];
            byte msbByte = (byte)((integer & 0xFF00) | (bt<<4));
            byte lsbByte = (byte)(integer & 0x0FF);

            result[0] = msbByte;
            result[1] = lsbByte;
            return result;
        }
        
    }
    

    public enum ProtocolCtrlType
    {
        SingleFrame,            //For unsegmented messages, the network layer protocol provides an optimized implementation of the
                                //network protocol with the message length embedded in the PCI byte only. SingleFrame (SF) shall be
                                //used to support the transmission of messages that can fit in a single CAN frame.

        FirstFrame,             //A FirstFrame (FF) shall only be used to support the transmission of messages that cannot fit in a single
                                //CAN frame, i.e. segmented messages. The first frame of a segmented message is encoded as an FF.
                                //On receipt of an FF, the receiving network layer entity shall start assembling the segmented message.

        ConsecutiveFrame,       //When sending segmented data, all consecutive frames following the FF are encoded as
                                //ConsecutiveFrame (CF). On receipt of a CF, the receiving network layer entity shall assemble the
                                //received data bytes until the whole message is received. The receiving entity shall pass the assembled
                                //message to the adjacent upper protocol layer after the last frame of the message has been received
                                //without error.

        FlowControl,            //The purpose of FlowControl (FC) is to regulate the rate at which CF N_PDUs are sent to the receiver.
                                //Three distinct types of Fe protocol control information are specified to support this function. The type is
                                //indicated by a field of the protocol control information called FlowStatus (FS), as defined hereafter.

        UnKnown
    }

    public enum  FlowControlType : byte	// 流控制帧类型。
	{
		ContinueToSend = 0,
		Wait,
		Overflow
	}
    
}
