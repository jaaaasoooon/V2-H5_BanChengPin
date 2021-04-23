using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5.ISO15765
{
    public class ApplicationLayer
    {
        public enum TimingType : byte
	    {
		    P2CANClient,
		    P2SCANClient,
		    P3CANClientPhys,
		    P3CANClientFunc,
	    }								// 对何种事件进行定时。

        UInt32 m_nTesterPhysicalAddress;
	    UInt32 m_nECUPhysicalAddress;
	    UInt32 m_nECUFunctionalAddress;

        uint[] m_anTimingParameters = new uint[4];				// 定时参数
	    uint m_nTimingS3Client;

        public UInt32 GetTesterPhysicalAddress()
        {
            return m_nTesterPhysicalAddress;
        }

        public void SetTesterPhysicalAddress(UInt32 nTesterPhysicalAddress)
        {
            m_nTesterPhysicalAddress = nTesterPhysicalAddress;
        }

        public UInt32 GetECUPhysicalAddress() 
        {
	        return m_nECUPhysicalAddress;
        }

        public void SetECUPhysicalAddress(UInt32 nECUPhysicalAddress)
        {
	        m_nECUPhysicalAddress = nECUPhysicalAddress;
        }

        public UInt32 GetECUFunctionalAddress() 
        {
	        return m_nECUPhysicalAddress;
        }

        public void SetECUFunctionalAddress(UInt32 nECUFunctionalAddress)
        {
	        m_nECUFunctionalAddress = nECUFunctionalAddress;
        }

        public uint GetP2CANClient() 
        {
	        return m_anTimingParameters[0];
        }

        public void SetP2CANClient(uint nP2CANClient)
        {
	        m_anTimingParameters[0] = nP2CANClient;
        }

        public uint GetP2SCANClient() 
        {
	        return m_anTimingParameters[1];
        }

        public void SetP2SCANClient(uint nP2SCANClient)
        {
	        m_anTimingParameters[1] = nP2SCANClient;
        }

        public uint GetP3CANClientPhys() 
        {
	        return m_anTimingParameters[2];
        }

        public void SetP3CANClientPhys(uint nP3CANClientPhys)
        {
	        m_anTimingParameters[2] = nP3CANClientPhys;
        }

        public uint GetP3CANClientFunc() 
        {
	        return m_anTimingParameters[3];
        }

        void SetP3CANClientFunc(uint nP3CANClientFunc)
        {
	        m_anTimingParameters[3] = nP3CANClientFunc;
        }

        public uint GetS3Client() 
        {
	        return m_nTimingS3Client;
        }

        public void SetS3Client(uint nS3Client)
        {
	        m_nTimingS3Client = nS3Client;
        }

        public void FirstFrameIndication(UInt32 nID, uint nLength)
        {

        }
        
    }
}
