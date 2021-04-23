using System;
using System.Collections.Generic;
using BoqiangH5.ISO14229;
using BoqiangH5Entity;

namespace BoqiangH5.ISO15765
{

    public struct ParaInfo
    {
        public int Value;
        public int ByteLen;
    }
    public enum FrameDirection { Request, Response };

    /// <summary>
    /// AppLayer请求事件类。
    /// </summary>
    public class RequestSendEventArgs : EventArgs
    {
        public RequestSendEventArgs(ApplicationLayerProtocol frame )
        {
            FrameToSend = frame;            
        }

        public ApplicationLayerProtocol  FrameToSend;       
    }

    /// <summary>
    /// DataLayer请求事件类
    /// </summary>
    public class DataLayerRequestSendEventArgs : EventArgs
    {
        public DataLayerRequestSendEventArgs(ZLG_API.VCI_CAN_OBJ frame)
        {
            FrameToSend = frame;
        }
        public ZLG_API.VCI_CAN_OBJ FrameToSend;       
    }
    

    public class ApplicationLayerProtocol
    {
        public delegate void requestInfoevent(object sender, RequestSendEventArgs e);     

        public const uint RequestID = 0x7AC;      //物理寻址请求11BitCANID
        public const uint ResponseID = 0x7CC;     //物理寻址响应11BitCANID  
        public const uint FucRequestID = 0x7DF;   //功能寻址响应11BitCANID  



        public event EventHandler RaiseAppLayerProtocolEvent;

        public void OnRaiseAppLayerProtocolEvent(EventArgs e)
        {
            if (RaiseAppLayerProtocolEvent != null)
            {
                RaiseAppLayerProtocolEvent(this, e);
            }
        }

        public FrameDirection Direction;
        /// <summary>
        /// 地址
        /// </summary>
        //private uint address;
        public uint Address{ get; set; }

        /// <summary>
        /// 应用层协议控制信息--服务类型
        /// </summary>
        public byte A_PCI{ get; set; }

        public string ServicesType{ get; set; }

        public bool IsPriority{ get; set; }

        public static bool isValidAppFrame = false;

        public List<byte> A_Data = new List<byte>();

        public static ApplicationLayerProtocol receiveFrame;// = new ApplicationLayerProtocol();
        public static NetworkLayerCommunication netWorkCommClass = new NetworkLayerCommunication();
        public static Action<ApplicationLayerProtocol> AppLayerSenFun; //发送函数封装

        public ApplicationLayerProtocol()
        { }

        public void Initialize()
        {
            AppLayerSenFun += _apf =>
            {                   
                NetworkLayerCommunication.DisposeSendFlag(_apf);
            };
            netWorkCommClass.RaiseNetworkLayerCommEvent += HandlerNetworkLayerCommEvent;
        }

        public ApplicationLayerProtocol(uint canId, byte servicesId,bool priority, params ParaInfo[] paraArray)
        {
            Direction = FrameDirection.Request;

            Address = canId;
            A_PCI = servicesId;
            IsPriority = priority;

            A_Data = new List<byte>();

            if (paraArray.Length == 0)
                A_Data.Add(A_PCI);
            else
            {   
                foreach (ParaInfo para in paraArray)
                {
                    if (para.ByteLen == 4)
                    {
                        A_Data.Add((byte)(para.Value >> 24));
                        A_Data.Add((byte)(para.Value >> 16));
                    }

                    if (para.ByteLen == 2)
                    {
                        A_Data.Add((byte)((para.Value >> 8) & 0x0FF));
                    }

                    A_Data.Add((byte)(para.Value & 0x0FF));
                }
                A_Data.Insert(0, A_PCI);
            }            
            //Request();
        }

        private void HandlerNetworkLayerCommEvent(object sender, EventArgs e)
        {
            var networkLayerEvent = e as NetWorkLayerEvent;
            if (networkLayerEvent == null)
            {
                return;
            }
            switch (networkLayerEvent.eventType)
            {
                case NetWorkEventType.NetworkSendEvent:                    
                    break;
                case NetWorkEventType.NetworkReceiveEvent:

                    ApplicationLayerProtocol app = networkDataDeal(networkLayerEvent);    
                    OnRaiseAppLayerProtocolEvent(
                        new AppLayerEvent()
                        {
                            eventType = AppEventType.AppReceiveEvent,
                            ID = app.Address,
                            A_PCI = app.A_PCI,
                            listData = app.A_Data

                        });//传输数据到上层
                    break;
                case NetWorkEventType.Other:
                    
                    break;
                default:
                   
                    break;
            }
            
        }


        public ApplicationLayerProtocol networkDataDeal(NetWorkLayerEvent _event)
        {
            ApplicationLayerProtocol app = new ApplicationLayerProtocol();
            app.Address = _event.ID;

            int validDataIndex = 1;

            app.A_PCI = _event.listData[0];

            for (int i = validDataIndex; i != _event.listData.Count; ++i)
            {
                app.A_Data.Add(_event.listData[i]);
            }
            return app;
        }
      
        public static ApplicationLayerProtocol GetValidAppFrame()
        {
            return receiveFrame;
        }        

        public static string GetAppServiceID(byte id)
        {
            string result = string.Empty;
 
            switch(id)
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
            }

            return result;
        }

        public int GetByteData(List<byte> data, int loc)
        {
            return data[loc];
        }

        public int GetIntData(List<byte> listByteData,int start, int byteNum)
        {
            int tmpVal = 0;
            try
            {
                if (byteNum > 4)
                    throw new Exception("Not Supported");

                if (listByteData.Count == 0)
                    return 0;
                
                for (int i = 0; i < byteNum; i++)
                {
                    tmpVal <<= 8;
                    tmpVal |= listByteData[start + i];
                }
            }
            catch (Exception e)
            {
            }
            return tmpVal;
        }

        //-bigendian
        public float GetFloatData(List<byte> data,int start, int byteNum)
        {
            Int64 tmpVal = 0;
            for (int i = 0; i < byteNum; i++)
            {
                tmpVal <<= 8;
                tmpVal |= data[start + i];
            }
            return (float)tmpVal;
        }


        public List<byte> getDataFromNetworkLayer(NetworkLayerProtocol networkLayerFrame)
        {
            A_Data = networkLayerFrame.N_Data;
            return A_Data;
        }       
        
    }
}
