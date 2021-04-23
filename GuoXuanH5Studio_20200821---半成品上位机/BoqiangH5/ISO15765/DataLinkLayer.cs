using System;
using System.Collections.Generic;
using System.Windows;
using BoqiangH5Entity;
using System.Threading;

namespace BoqiangH5.ISO15765
{
    public class DataLinkLayer:FrameworkElement
    {
        public uint Address{ get; set; }

        public List<byte> ListDllData{ get; set; }

        /// <summary>
        /// 封装发送函数
        /// </summary>
        public static Action<ZLG_API.VCI_CAN_OBJ> ActionDllSendFuc;

        public static Mutex m_Mutex = new Mutex();
        /// <summary>
        /// CAN实体
        /// </summary>
        public static ZLGFuction DllZLGFun { get; set; }
        //public static ZLGCANFuction DllZLGCANFun { get; set; }

        public NetworkLayerProtocol NetWorkLayer; 

        public event EventHandler RaiseDataLinkLayerEvent;

        public void OnRaiseDataLinkLayerEvent(EventArgs e)
        {
            if (RaiseDataLinkLayerEvent != null)
            {
                RaiseDataLinkLayerEvent(this, e);
            }
        }        

        public DataLinkLayer()
        {
            DllZLGFun = (ZLGFuction)FindResource("ZLGCAN");
            DllZLGFun.RaiseZLGCommunicateEvent += HandleZLGCommunicateEvent;

            //DllZLGCANFun = (ZLGCANFuction)FindResource("ZLGCANFD");
            //DllZLGCANFun.RaiseZLGCommunicateEvent += HandleZLGCommunicateEvent;
            ActionDllSendFuc += _sf =>
                {
                    DllZLGFun.SingleTransmit(_sf);
                    ////DllZLGCANFun.SingleTransmit(_sf);
                };
        }

        private void HandleZLGCommunicateEvent(object sender, EventArgs e)
        {
            var canEvent = e as CANEvent;
            if (canEvent == null)
            {
                return;
            }
            switch (canEvent.eventType)
            {                    
                case CANEventType.SendEvent:
                    //DllZLGFun.SingleTransmit()
                    break;
                case CANEventType.ReceEvent:
                 
                    OnRaiseDataLinkLayerEvent(
                        new DataLayerEvent() 
                        {
                            eventType = DataLayerEventType.DataLayerReceiveEvent,
                            ID = canEvent.ID,
                            listData = canEvent.listData
                        });//把数据传到网络层
                    //BLLCommon.DisposeReceivedFlag(canEvent);
                    
                    break;
                case CANEventType.Other:
                    
                    break;
                default:
                   
                    break;
            }
            //throw new NotImplementedException();
        }


        private static Mutex mutex = new Mutex();

        public static void SendCanFrame(uint id, byte[] data)
        {
            try
            {
                ZLG_API.VCI_CAN_OBJ CanFrame = new ZLG_API.VCI_CAN_OBJ();
                CanFrame.ID = id;
                CanFrame.ExternFlag = 1;
                CanFrame.RemoteFlag = 0;
                CanFrame.SendType = 0;
                CanFrame.TimeFlag = 0;
                CanFrame.DataLen = (byte)data.Length;
                CanFrame.Data = data;

                mutex.WaitOne();
                //ZLGFuction zlgf = new ZLGFuction();
                Thread.Sleep(20);

                DllZLGFun.SingleTransmit(CanFrame);

             
            }
            catch (Exception ex)
            {}
            finally
            {
                mutex.ReleaseMutex();
            }
        }

    }
}
