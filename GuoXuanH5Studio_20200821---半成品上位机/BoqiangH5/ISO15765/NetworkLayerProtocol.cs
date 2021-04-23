using System;
using System.Collections.Generic;
using System.Linq;
using BoqiangH5.ISO14229;

namespace BoqiangH5.ISO15765
{
    public class MessageBuffer							        // 消息缓存，已退化为半双工。
													            // 是具有此功能的设计，目前取消了多 ECU 支持，只对单一 ECU 操作，但仍用此结构。
	{
        public MessageBuffer() { }
        //public bool IsBusy() { }								// [线程安全] 正忙检测。
       // public bool IsRequest() { }							// [线程安全] 是否发送（发送状态，或接收状态下发流控制帧）。


        public UInt32 nID;									    // ID
        public List<byte> ListByteData = new List<byte>();			// 将要发送，或是正在接收的数据。
        public UInt32 StartLocation;			                    // 将要发送或接收的位置。
        public ProtocolCtrlType ProCtrlType;						// 发送 / 验证 / 期望收到的 PCI 类型：SF / FF / CF / FC。
        public byte MinSeparationTime;				    	    // 连续帧发送的最小等待时间。
        public uint RemainderFrameCount;					    // 本次尚要发送的帧数，或是尚要接收的帧数；
													            // 发送状态下为 0 表示正在等待流控制帧；
													            // 接受状态下为 0 表示将要发送流控制帧。
        public byte ExpectedSequenceNumber;		    		    // 期望发送或收到的分段序列号。
        public int TimingStartTick;					            // 定时器开始的时间。

        public void ClearMessage()						        // [线程安全] 终止消息收发。
        {
            ListByteData.Clear();
	        StartLocation = 0;
	        ProCtrlType = ProtocolCtrlType.UnKnown;
	        MinSeparationTime = 0;
	        RemainderFrameCount = 0;
	        ExpectedSequenceNumber = 0;
	        TimingStartTick = 0;	        
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
                    DataLinkLayer.ActionDllSendFuc(_nF);
                };
        }

        public static MessageBuffer m_messageBuffer = new MessageBuffer();
        public static MessageBuffer m_recvMsgBuffer = new MessageBuffer();

        public DataLinkLayer datalink = new DataLinkLayer();

        public ApplicationLayerProtocol AppLayerPro = new ApplicationLayerProtocol();

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
	        m_messageBuffer.ListByteData = nData;
	        m_messageBuffer.StartLocation = 0;
	        m_messageBuffer.MinSeparationTime = 0;
	        m_messageBuffer.RemainderFrameCount = 0;
	        m_messageBuffer.ExpectedSequenceNumber = 0;
	        m_messageBuffer.TimingStartTick = 0;	        

	        if (nData.Count() < 7)
	        {
		        m_messageBuffer.ProCtrlType = ProtocolCtrlType.SingleFrame;
	        }
	        else
	        {
		        m_messageBuffer.ProCtrlType = ProtocolCtrlType.FirstFrame;
	        }

            return m_messageBuffer;
        }        

        public static void Response(byte[] nData)
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

                    m_recvMsgBuffer.ListByteData.Clear();
                    m_recvMsgBuffer.ListByteData = new List<byte>(new byte[nApplicationLayerDataLength]);//new List<byte>((int)nApplicationLayerDataLength);

                    for (int i = (int)nDataIndex; i != nApplicationLayerDataLength+1; ++i)
                    {
                        m_recvMsgBuffer.ListByteData[(int)(i - nDataIndex)] = nData[i];
                    }
                    //m_messageBuffer.ListByteData.AddRange(nData);

                    //applink.Response(m_RcvmessageBuffer.nID, m_RcvmessageBuffer.ListByteData);
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
                    m_recvMsgBuffer.ListByteData.Clear();
                    m_recvMsgBuffer.ListByteData = new List<byte>(new byte[nApplicationLayerDataLength]);
                    //m_messageBuffer.vbyData = new List<byte>((int)nApplicationLayerDataLength);

                    for (int i = (int)nDataIndex; i != 8; ++i)
                    {
                        m_recvMsgBuffer.ListByteData[(int)(i - nDataIndex)] = nData[i];
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

                    m_recvMsgBuffer.nID = 0x7AC;
                    m_recvMsgBuffer.StartLocation = 8 - nDataIndex;
                    m_recvMsgBuffer.ProCtrlType = ProtocolCtrlType.FlowControl;
                    m_recvMsgBuffer.RemainderFrameCount = frmCount;
                    m_recvMsgBuffer.ExpectedSequenceNumber = 1;
			        //m_signalFirstFrameIndication(nID, nApplicationLayerDataLength);

                    _Request(m_recvMsgBuffer);

                    break;

                case ProtocolCtrlType.ConsecutiveFrame:

                    // 15765-2: 6.5.4

			        // 错误处理。
                    if (m_recvMsgBuffer.ExpectedSequenceNumber != (byPCIFirst & 0x0F))
			        {				
				        return;
			        }
                    m_recvMsgBuffer.ExpectedSequenceNumber = (byte)((m_recvMsgBuffer.ExpectedSequenceNumber + 1) % 0x10);

			        nDataIndex = 1;
                    uint nReceiveLength = (uint)System.Math.Min(7, m_recvMsgBuffer.ListByteData.Count - m_recvMsgBuffer.StartLocation);
			        for (int i = 0; i != nReceiveLength; ++i)
			        {
                        m_recvMsgBuffer.ListByteData[(int)(m_recvMsgBuffer.StartLocation + i)] = nData[(int)(i + nDataIndex)];
			        }
                    m_recvMsgBuffer.StartLocation += nReceiveLength;
                    --m_recvMsgBuffer.RemainderFrameCount;
                    if (m_recvMsgBuffer.StartLocation > m_recvMsgBuffer.ListByteData.Count)
			        {
                        List<byte> vbyData = m_recvMsgBuffer.ListByteData;
                        m_recvMsgBuffer.ClearMessage();
				        //m_signalIndication(nID, m_messageBuffer.vbyData, Diagnostic::NetworkLayerResult::N_OK);
			        }
			        else
			        {
				        //m_messageBuffer.ResetTiming(TimingType::Cr, m_eventTiming);
                        if (m_recvMsgBuffer.RemainderFrameCount == 0)
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
                            m_recvMsgBuffer.ProCtrlType = ProtocolCtrlType.ConsecutiveFrame;
                            m_recvMsgBuffer.MinSeparationTime = nData[2];
                            if (m_recvMsgBuffer.MinSeparationTime >= 0x80 && m_recvMsgBuffer.MinSeparationTime <= 0xF0 || m_recvMsgBuffer.MinSeparationTime >= 0xFA)
					        {
                                m_recvMsgBuffer.MinSeparationTime = 0x7F;
					        }

                            m_recvMsgBuffer.RemainderFrameCount = nData[1];					        
					
					        // 15765-2: 6.5.5.4, Table 14
                            if (m_recvMsgBuffer.RemainderFrameCount == 0)
					        {
                                m_recvMsgBuffer.RemainderFrameCount = 1;	// 取最大值
					        }

                            //DWORD dwWaitResult = WaitForSingleObject(m_eventStopThread.m_hObject, m_messageBuffer.bySeparationTimeMin);
                            //if (dwWaitResult == WAIT_TIMEOUT)
                            //{
                            _Request(m_recvMsgBuffer);
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
                        m_recvMsgBuffer.ClearMessage();
				        //m_signalConfirm(Diagnostic::NetworkLayerResult::N_BUFFER_OVFLW);
				        return;
			        default:
                        //TRACE(_T("FlowControl: Invalid flowcontrol.\n"));
                        //_AddWatchEntry(EntryType::Receive, nID, IDS_NETWORKLAYERWATCH_FLOWCONTROL_INVALID, Color::Red);
                        //m_messageBuffer.ResetTiming(TimingType::Idle, m_eventTiming);
                        m_recvMsgBuffer.ClearMessage();
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
	        byPCIFirst = (byte)(((byte)m_messageBuffer.ProCtrlType) << 4);
	        int nDataSize = m_messageBuffer.ListByteData.Count;
	        int nPDUDataSize = 7;	// PDU 包含的数据长度，在首帧中需要减一，流控制帧中不包含数据帧。

	        switch (m_messageBuffer.ProCtrlType)
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
                        btArrRequest[n] = m_messageBuffer.ListByteData[n - 1];
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

                    m_messageBuffer.ListByteData.CopyTo(0, data, 0, data.Length);

                    for (int n = 1; n < data.Length; n++)
                    {
                        btArrRequest[n] = data[n - 1];
                    }
		
		            m_messageBuffer.StartLocation = (uint)nPDUDataSize;
		            m_messageBuffer.RemainderFrameCount = 0;
		            m_messageBuffer.ExpectedSequenceNumber = 1;
		            break;
	            case ProtocolCtrlType.ConsecutiveFrame:
		
		            if (m_messageBuffer.ListByteData.Count() <= m_messageBuffer.StartLocation)
		            {			
			            return;
		            }
		            byPCIFirst = (byte)(byPCIFirst | m_messageBuffer.ExpectedSequenceNumber);           
		            m_messageBuffer.ExpectedSequenceNumber = (byte)((m_messageBuffer.ExpectedSequenceNumber + 1) % 0x10);

                    btArrRequest[0] = byPCIFirst;
		            nPDUDataSize = (int)System.Math.Min(nPDUDataSize, m_messageBuffer.ListByteData.Count() - m_messageBuffer.StartLocation);

                    for (int n = 1; n < m_messageBuffer.ListByteData.Count; n++)
                    {
                        btArrRequest[n] = m_messageBuffer.ListByteData[n - 1];
                    }

		
		            m_messageBuffer.StartLocation += (uint)nPDUDataSize;
		            --m_messageBuffer.RemainderFrameCount;
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
            networkData.N_PCIType = (byte)(dataLinkData.ListDllData[0] & 0xF0);

            if (networkData.N_PCIType == 0x01)
            {
                networkData.SF_DL = (byte)(dataLinkData.ListDllData[0] & 0x0F);
                if ((networkData.SF_DL > 7) || (networkData.SF_DL == 0))
                    throw new Exception("单帧长度错误");
                networkData.N_PCI = networkData.GetSFN_PCI(networkData.N_PCIType, networkData.SF_DL);
                dataLinkData.ListDllData.RemoveRange(0, 1);
                networkData.N_Data = dataLinkData.ListDllData;
            }
            else if (networkData.N_PCIType == 0x03)
            {
                networkData.FC_FS = (byte)(dataLinkData.ListDllData[0] & 0xF0);
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
                    networkData.FF_DL = networkData.GetFF_DL(dataLinkData.ListDllData[0], dataLinkData.ListDllData[1]);
                    if ((networkData.FF_DL < 8) || (networkData.FF_DL > 0xFFF))
                        throw new Exception("首帧长度错误");

                    networkData.N_PCI = networkData.GetFFN_PCI(networkData.N_PCIType, networkData.FF_DL);
                    dataLinkData.ListDllData.RemoveRange(0, 2);
                    networkData.N_Data.AddRange(dataLinkData.ListDllData);
                }
                else
                {
                    networkData.CF_SN = (byte)(dataLinkData.ListDllData[0] & 0xF0);
                    if ((networkData.CF_SN > 15) || (networkData.CF_SN < 0))
                        throw new Exception("连续帧序号错误");

                    networkData.N_PCI = networkData.GetCFN_PCI(networkData.N_PCIType, dataLinkData.ListDllData[0]);
                    dataLinkData.ListDllData.RemoveRange(0, 1);
                    networkData.N_Data.AddRange(dataLinkData.ListDllData);
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
