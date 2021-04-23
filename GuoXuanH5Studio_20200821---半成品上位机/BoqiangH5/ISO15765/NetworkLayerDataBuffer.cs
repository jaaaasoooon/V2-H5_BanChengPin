using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5.ISO15765
{
    public class NetworkLayerDataBuffer : IDisposable
    {
        //是否回收完毕
        bool _disposed;
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
        public List<byte> ListNetLayerByteData = new List<byte>();

        /// <summary>
        /// 将要发送或接收的位置
        /// </summary>
        public int Offset;
        /// <summary>
        /// 协议类型 PCI 类型：SF / FF / CF / FC。
        /// </summary>
        public ProtocolCtrlType ProCtrlType;
        /// <summary>
        ///  连续帧发送的最小等待时间
        /// </summary>
        public byte MinInterval;
        /// <summary>
        ///  未接收的帧数
        /// </summary>
        public int RemainFrameCount;
        /// <summary>
        /// 期望发送或收到的分段序列号
        /// </summary>
        public byte ExpectedSN;
        public int DataLen { get; set; }

        public int FS_Sign { get; set; }
        public byte BS_Num { get; set; }
        public byte STmin_ms { get; set; }

        public void ClearMessage()
        {
            ListNetLayerByteData.Clear();
            Offset = 0;
            ProCtrlType = ProtocolCtrlType.UnKnown;
            MinInterval = 0;
            RemainFrameCount = 0;
            ExpectedSN = 0;
        }

        public NetworkLayerDataBuffer()
        {
            BS_Num = 0xFF;
            ExpectedSN = 0;
            FS_Sign = (int)FlowControlType.ContinueToSend;
            STmin_ms = 0x00;
            //multiTimer.Elapsed += new System.Timers.ElapsedEventHandler(multiTimer_Tick);
        }

        private void MultiTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
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
            sendData.AddRange(SetSingleFrame(DataLen));
            sendData.AddRange(SetFlowControlFrame(FS_Sign, BS_Num, STmin_ms));

            return NetworkLayerCommunication.ConvertToCANFrame(this.nID, sendData);
        }

        /// <summary>
        /// 发送流控
        /// </summary>
        /// <returns></returns>
        public ZLG_API.VCI_CAN_OBJ SendFlowControl()
        {
            List<byte> sendData = new List<byte> { };
            sendData.AddRange(SetFlowControlFrame(FS_Sign, BS_Num, STmin_ms));

            return NetworkLayerCommunication.ConvertToCANFrame(ApplicationLayerProtocol.RequestID, sendData);
        }


        /// <summary>
        /// 发送首帧
        /// </summary>
        /// <returns></returns>
        public ZLG_API.VCI_CAN_OBJ SendFirstFrame()
        {
            List<byte> listSendData = new List<byte> { };

            DataLen = ListNetLayerByteData.Count; // hongs
            listSendData.AddRange(SetFirstFrame(DataLen));            
            listSendData.AddRange(ListNetLayerByteData.Take(6));

            Offset += 6; // hongs
            ExpectedSN++;

            return NetworkLayerCommunication.ConvertToCANFrame(this.nID, listSendData);
        }
        /// <summary>
        /// 发送连续帧
        /// </summary>
        /// <param name="_runout"></param>
        /// <returns></returns>
        public ZLG_API.VCI_CAN_OBJ SendConsecutiveFrame()
        {
            
                List<byte> listSendFrame = new List<byte> { };
                try
                {
                byte[] arrTimes = BitConverter.GetBytes(ExpectedSN++);  // hong
                byte nConsecutiveTimes = (byte)(2 << 4 | (arrTimes[0] & 0x0F));

                listSendFrame.AddRange(SetConsecutiveFrame(nConsecutiveTimes));
                for (int i = 0; i < 7; i++)
                {
                    if (ListNetLayerByteData.Count - Offset > 0)  // hong  if (ListNetLayerByteData.Count > 0)
                    {
                        listSendFrame.Add(ListNetLayerByteData[Offset++]);
                        //Offset++;
                    }
                }

            }
            catch(Exception ex)
            {
            }
            return NetworkLayerCommunication.ConvertToCANFrame(this.nID, listSendFrame);//new SendSingleFrame(_strID, sendData, "连续帧");
        }

    }    
}
