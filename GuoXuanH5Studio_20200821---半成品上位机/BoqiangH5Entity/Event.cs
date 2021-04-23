using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BoqiangH5Entity;

namespace BoqiangH5Entity
{
    /// <summary>
    /// CAN事件类
    /// </summary>
    public class CANEvent :EventArgs
    {
        public CANEventType eventType;        
        public UInt32 ID { get; set; } 
        public byte DataLen { get; set; }

        public List<byte> listData ;

    }

    public class DataLayerEvent : EventArgs
    {
        public DataLayerEventType eventType;

        public UInt32 ID{ get; set; } 
        
        public List<byte> listData;        

    }

    public class NetWorkLayerEvent : EventArgs
    {
        public NetWorkEventType eventType;

        public UInt32 ID{ get; set; }
        
        public List<byte> listData ;
        
    }

    public class AppLayerEvent : EventArgs
    {
        public AppEventType eventType;

        public UInt32 ID{ get; set; }

        public byte A_PCI{ get; set; }
       
        public List<byte> listData = new List<byte> { };        
    }
}