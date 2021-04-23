using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoqiangH5Entity;
using BoqiangH5.CommonClass;

namespace BoqiangH5.ISO15765
{
    public class NetworkLayerCommunication
    {
        public static SemaphoreSlim SemaphorePool = new SemaphoreSlim(1);

        /// <summary>
        /// 多帧接收缓存
        /// </summary>
        public static NetworkLayerDataBuffer BufferMultiFrameReceive = new NetworkLayerDataBuffer();

        /// <summary>
        /// 多帧发送缓存
        /// </summary>
        public static NetworkLayerDataBuffer BufferMultiFrameSend = new NetworkLayerDataBuffer();
        
        public static Action<NetworkLayerDataBuffer> ActionRecvFun;

        public static DataLinkLayer DataLinkLayerVar = new DataLinkLayer();

        public event EventHandler RaiseNetworkLayerCommEvent;

        public void OnRaiseNetworkLayerCommEvent(EventArgs e)
        {
            if (RaiseNetworkLayerCommEvent != null)
            {
                RaiseNetworkLayerCommEvent(this, e);
            }
        }

        public NetworkLayerCommunication()
        {
            DataLinkLayerVar.RaiseDataLinkLayerEvent += HandlerDataLinkLayerEvent;
            ActionRecvFun += _multi =>
            {
                OnRaiseNetworkLayerCommEvent(
                    new NetWorkLayerEvent()
                    {
                        eventType = NetWorkEventType.NetworkReceiveEvent,
                        ID = _multi.nID,
                        listData = _multi.ListNetLayerByteData
                    });
            };
        }


        public static void NetworkLayerSendMultiFrame(object obj)
        {
            string strSendBytes = DataFormatConvert.ListToStr(BufferMultiFrameSend.ListNetLayerByteData) ; 

            int nSendTimes = 0;

            try
            {
                while (true)
                {
                    if (BufferMultiFrameSend.Offset < BufferMultiFrameSend.ListNetLayerByteData.Count && BufferMultiFrameSend.RemainFrameCount > 0 &&
                        (MainWindow.m_OperateType == OperateType.WriteAllMasterPara || //ParaManageWnd.IsUpdateAllPara ||
                         MainWindow.m_OperateType == OperateType.WriteAllMasterAdjustPara ||
                         MainWindow.m_OperateType == OperateType.WriteMasterControlPara))  // (BufferMultiFrameSend.RemainFrameCount != 0)
                    {
                        if (BufferMultiFrameSend.Offset > 0)  // 非首帧
                        {
                            DataLinkLayer.ActionDllSendFuc(BufferMultiFrameSend.SendConsecutiveFrame());
                        }
                        else
                        {
                            DataLinkLayer.ActionDllSendFuc(BufferMultiFrameSend.SendFirstFrame());
                        }

                        BufferMultiFrameSend.RemainFrameCount--;
                    }
                    else
                    {
                        if (MainWindow.m_OperateType != OperateType.WriteAllMasterPara)  //(!ParaManageWnd.IsUpdateAllPara)  // 关闭参数管理窗口,停止设置参数 清空发送数据 
                        {
                            BufferMultiFrameSend.ClearMessage();
                        }

                        break;
                    }

                    Thread.Sleep(200);
                }
            }
            catch(Exception ex)
            {
                
            }

        }

        public void DisposeReceivedFrame(DataLayerEvent _event)
        {
            switch ((ProtocolCtrlType)NetworkLayerDataBuffer.GetHeadSign(_event.listData))
            {
                case ProtocolCtrlType.SingleFrame://如果接收到的是单帧则立刻把该单帧上传到应用层

                    ReceiveSingleFrame(_event);

                    break;
                case ProtocolCtrlType.FirstFrame://如果接收到的是首帧，则发送流控帧到数据链路层，然后准备接收该多帧信息
                    ReceiveFirstFrame(_event);
                    break;

                case ProtocolCtrlType.ConsecutiveFrame://接收首帧中指定服务的连续帧
                    ReceiveConsecutiveFrame(_event);
                    break;

                case ProtocolCtrlType.FlowControl:
                    ReceiveFlowControlFrame(_event);
                    break;
            }
        }

        private void ReceiveSingleFrame(DataLayerEvent _event)
        {
            BufferMultiFrameReceive.nID = _event.ID;
            BufferMultiFrameReceive.DataLen = NetworkLayerDataBuffer.GetSF_DLSign(_event.listData);
            if (BufferMultiFrameReceive.DataLen == 0 || BufferMultiFrameReceive.DataLen > 7)
                return;

            int nnDataIndex = 1;
            BufferMultiFrameReceive.ListNetLayerByteData.Clear();
            BufferMultiFrameReceive.ListNetLayerByteData = new List<byte>(new byte[BufferMultiFrameReceive.DataLen]);

            for (int i = (int)nnDataIndex; i != BufferMultiFrameReceive.DataLen + 1; ++i)
            {
                BufferMultiFrameReceive.ListNetLayerByteData[(int)(i - nnDataIndex)] = _event.listData[i];
            }

            ActionRecvFun(BufferMultiFrameReceive);
        }

        private void ReceiveFirstFrame(DataLayerEvent _event)
        {
            BufferMultiFrameReceive.nID = _event.ID;
            BufferMultiFrameReceive.DataLen = NetworkLayerDataBuffer.GetFF_DLSign(_event.listData);
            if (BufferMultiFrameReceive.DataLen < 8)
            {
                return;
            }

            int nDataIndex = 2;
            BufferMultiFrameReceive.ListNetLayerByteData.Clear();
            BufferMultiFrameReceive.ListNetLayerByteData = new List<byte>(new byte[BufferMultiFrameReceive.DataLen]);


            for (int i = nDataIndex; i != 8; ++i)
            {
                BufferMultiFrameReceive.ListNetLayerByteData[(int)(i - nDataIndex)] = _event.listData[i];
            }

            int nCount = 0;

            if ((BufferMultiFrameReceive.DataLen - 6) / 7 == 0)
            {
                nCount = (BufferMultiFrameReceive.DataLen - 6) / 7;
            }
            else
            {
                nCount = (BufferMultiFrameReceive.DataLen - 6) / 7 + 1;
            }

            BufferMultiFrameReceive.Offset = 6;
            BufferMultiFrameReceive.ProCtrlType = ProtocolCtrlType.FlowControl;
            BufferMultiFrameReceive.RemainFrameCount = nCount;
            BufferMultiFrameReceive.ExpectedSN = 1;

            DataLinkLayer.ActionDllSendFuc(BufferMultiFrameReceive.SendFlowControl());
        }

        private void ReceiveConsecutiveFrame(DataLayerEvent _event)
        {
            if (BufferMultiFrameReceive.ExpectedSN != NetworkLayerDataBuffer.GetSN_DLSign(_event.listData))
            {
                return;
            }

            if (BufferMultiFrameReceive.ListNetLayerByteData.Count > BufferMultiFrameReceive.Offset)
            {
                BufferMultiFrameReceive.ExpectedSN = (byte)((BufferMultiFrameReceive.ExpectedSN + 1) % 0x10);
                int nReceiveLength = System.Math.Min(7, BufferMultiFrameReceive.ListNetLayerByteData.Count - BufferMultiFrameReceive.Offset);
                for (int i = 0; i != nReceiveLength; ++i)
                {
                    if (BufferMultiFrameReceive.Offset + i <= BufferMultiFrameReceive.ListNetLayerByteData.Count)
                        BufferMultiFrameReceive.ListNetLayerByteData[(int)(BufferMultiFrameReceive.Offset + i)] = _event.listData[(int)(i + 1)];
                }

                BufferMultiFrameReceive.Offset += nReceiveLength;
                --BufferMultiFrameReceive.RemainFrameCount;

                if (BufferMultiFrameReceive.Offset > BufferMultiFrameReceive.ListNetLayerByteData.Count)
                {
                    BufferMultiFrameReceive.ClearMessage();

                }
                else
                {
                    if (BufferMultiFrameReceive.RemainFrameCount <= 0)
                    {
                        ActionRecvFun(BufferMultiFrameReceive);
                    }
                }
            }
        }

        private void ReceiveFlowControlFrame(DataLayerEvent _event)
        {
            PCT_FlowControl(_event);
        }

        private void PCT_FlowControl(DataLayerEvent _event)
        {
            switch (NetworkLayerDataBuffer.GetFS_DLSign(_event.listData))
            {
                case FlowControlType.ContinueToSend:
                    BufferMultiFrameReceive.MinInterval = _event.listData[2];
                    if (BufferMultiFrameReceive.MinInterval >= 0x80 && BufferMultiFrameReceive.MinInterval <= 0xF0 || BufferMultiFrameReceive.MinInterval >= 0xFA)
                    {
                        BufferMultiFrameReceive.MinInterval = 0x7F;                       
                    }
                    BufferMultiFrameSend.RemainFrameCount = _event.listData[1];
                    ThreadPool.QueueUserWorkItem(NetworkLayerSendMultiFrame);
                    
                    break;
                case FlowControlType.Wait:
                    //StopTimer();
                    //停止发送定时器
                    break;
                case FlowControlType.Overflow:
                    BufferMultiFrameSend.ClearMessage();
                    return;
                default:
                    BufferMultiFrameSend.ClearMessage();
                    break;
            }
        }

        private static void SynchroSend(object sfc_object)
        {
            var sfc = sfc_object as ApplicationLayerProtocol;
            //字节小于7个
            if (sfc.A_Data.Count <= 7)
            {
                SemaphorePool.Wait();
                List<byte> sendData = new List<byte> { };
                sendData.AddRange(NetworkLayerDataBuffer.SetSingleFrame(sfc.A_Data.Count));
                sendData.AddRange(sfc.A_Data);

                DataLinkLayer.ActionDllSendFuc(ConvertToCANFrame(sfc.Address, sendData));
                SemaphorePool.Release();
            }
            else
            {
                BufferMultiFrameSend.ListNetLayerByteData = sfc.A_Data;
                BufferMultiFrameSend.nID = sfc.Address;

                #region 设置所有参数 重置参数
                BufferMultiFrameSend.RemainFrameCount = 1;  // 发送首帧
                BufferMultiFrameSend.Offset = 0;
                BufferMultiFrameSend.ExpectedSN = 0;
                #endregion

                ThreadPool.QueueUserWorkItem(NetworkLayerSendMultiFrame);
            }
        }
        public static void DisposeSendFlag(ApplicationLayerProtocol AppFrame)
        {
            //启用线程池
            ThreadPool.QueueUserWorkItem(new WaitCallback(SynchroSend), AppFrame);
        }

        public static ZLG_API.VCI_CAN_OBJ ConvertToCANFrame(UInt32 id, List<byte> listData)
        {
            ZLG_API.VCI_CAN_OBJ CanFrame = new ZLG_API.VCI_CAN_OBJ();
            CanFrame.ID = id;
            CanFrame.ExternFlag = 0;
            CanFrame.RemoteFlag = 0;
            CanFrame.SendType = 1;
            CanFrame.TimeFlag = 1;
            CanFrame.DataLen = (byte)listData.Count;
            CanFrame.Data = listData.ToArray();

            return CanFrame;
        }

        private void HandlerDataLinkLayerEvent(object sender, EventArgs e)
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
                    DisposeReceivedFrame(dataLayerEvent);
                    break;
                case DataLayerEventType.Other:
                    break;
                default:
                    break;
            }
        }
    }
}
