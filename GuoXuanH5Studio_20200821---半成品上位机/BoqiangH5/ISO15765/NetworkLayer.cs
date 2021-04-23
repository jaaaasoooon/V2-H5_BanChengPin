using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BoqiangH5.ISO15765
{
#if true
    public class NetworkLayer
    {
        public enum PCIType : byte	// 单帧 / 首帧 / 连续帧 / 流控制帧。
	    {
		    Unknown = 4,
		    SingleFrame = 0,
		    FirstFrame,
		    ConsecutiveFrame,
		    FlowControl,
	    }

        public enum Status : byte   // 空闲 / 收 / 发。
        {
            Idle = 0,
            ReceiveInProgress = 1,
            TransmitInProgress = 2
        }

        public enum FlowControlType : byte  // 流控制帧类型。
        {
            ContinueToSend = 0,
            Wait,
            Overflow
        }

        public enum TimingType : byte   // 对何种事件进行定时。
        {
            As,
            Ar,
            Bs,
            Br,
            Cs,
            Cr,
            Idle
        }

        
        public uint CANFRAMEDATALENGTHMAX	            = 8;			// CAN 帧最大数据长度。
		public uint DIAGNOSTICSFRAMEDATALENGTH			= 7;			// 诊断帧数据长度。
		public uint REMOTEDIAGNOSTICSFRAMEDATALENGTH	= 6;			// 远程诊断帧数据长度。
		public uint TESETRPRESENTSERVICEID				= 0x3E;			// TP Service ID。
        public uint TIMINGCYCLE                         = 100;			// 检查定时的周期。
       

        public enum  J1939ParameterGroupNumber : byte
	    {
		    MixedAddressingFunctional		= 205,
		    MixedAddressingPhysical			= 206,
		    NormalFixedAddressingPhysical	= 218,
		    NormalFixedAddressingFunctional	= 219
	    }

        public struct MessageBuffer							// 消息缓存，已退化为半双工。
        // 是具有此功能的设计，目前取消了多 ECU 支持，只对单一 ECU 操作，但仍用此结构。
        {
            //MessageBuffer();
            //bool IsBusy();								// [线程安全] 正忙检测。
            //bool IsRequest();							// [线程安全] 是否发送（发送状态，或接收状态下发流控制帧）。

            // CCriticalSection csectionProcess;		// 消息处理临界对象。
            //std::recursive_mutex rmutexMessageBuffer;	// 访问互斥量。
            public Status status;
            UInt32 nID;									// ID
            List<byte> vbyData;							// 将要发送，或是正在接收的数据。
            int stLocation;			// 将要发送或接收的位置。
            PCIType pciType;							// 发送 / 验证 / 期望收到的 PCI 类型：SF / FF / CF / FC。
            byte bySeparationTimeMin;					// 连续帧发送的最小等待时间。
            uint nRemainderFrameCount;					// 本次尚要发送的帧数，或是尚要接收的帧数；
            // 发送状态下为 0 表示正在等待流控制帧；
            // 接受状态下为 0 表示将要发送流控制帧。
            byte byExpectedSequenceNumber;				// 期望发送或收到的分段序列号。
            UInt16 dwTimingStartTick;					// 定时器开始的时间。
            TimingType timingType;						// 对何种事件进行定时。

            //void ResetTiming(TimingType timingType, CEvent &eventTiming);	// [线程安全] 重置定时器。
            //void ClearMessage();						// [线程安全] 终止消息收发。
        }

        MessageBuffer m_messageBuffer;

        byte m_bySeparationTimeMin;					// 流控制帧包含的发送间隔时间。
	    byte m_byBlockSize;							// 流控制帧包含的发送块大小。

	    uint m_nWaitFrameTransimissionMax;			// 15765-2: 6.6, （由发送方保证的）流控制等待帧发送的最大次数。

        uint[] m_anTimingParameters = new uint[6];			// 定时参数 As, Ar, Bs, Br, Cs, Cr

        Thread m_pTimingThread;

        public uint GetAs() 
        {
	        return m_anTimingParameters[0];
        }

        public void SetAs(uint nAs)
        {
	        m_anTimingParameters[0] = nAs;
        }

        public uint GetAr() 
        {
	        return m_anTimingParameters[1];
        }

        public void SetAr(uint nAr)
        {
	        m_anTimingParameters[1] = nAr;
        }

        public uint GetBs() 
        {
	        return m_anTimingParameters[2];
        }

        public void SetBs(uint nBs)
        {
	        m_anTimingParameters[2] = nBs;
        }

        public uint GetBr() 
        {
	        return m_anTimingParameters[3];
        }

        public void SetBr(uint nBr)
        {
	        m_anTimingParameters[3] = nBr;
        }

        uint GetCs() 
        {
	        return m_anTimingParameters[4];
        }

        public void SetCs(uint nCs)
        {
	        m_anTimingParameters[4] = nCs;
        }

        public uint GetCr()
        {
	        return m_anTimingParameters[5];
        }

        public void SetCr(uint nCr)
        {
	        m_anTimingParameters[5] = nCr;
        }

        public byte GetSeparationTimeMin() 
        {
	        return m_bySeparationTimeMin;
        }

        public void SetSeparationTimeMin(byte bySeparationTimeMin)
        {
	        m_bySeparationTimeMin = bySeparationTimeMin;
        }

        public byte GetBlockSize() 
        {
	        return m_byBlockSize;
        }

        public void SetBlockSize(byte byBlockSize)
        {
	        m_byBlockSize = byBlockSize;
        }

        public uint GetWaitFrameTransimissionMax() 
        {
	        return m_nWaitFrameTransimissionMax;
        }

        public void SetWaitFrameTransimissionMax(uint nWaitFrameTransimissionMax)
        {
	        m_nWaitFrameTransimissionMax = nWaitFrameTransimissionMax;
        }

    }
#endif
}
