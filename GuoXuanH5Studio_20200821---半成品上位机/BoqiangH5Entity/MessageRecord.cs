using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5Entity
{
    public class MessageRecord
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int index;           // 索引编号
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Index"));
                }
            }
        }

        private string frameTime;
        public string FrameTime
        {
            get
            {
                return frameTime;
            }
            set
            {
                frameTime = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FrameTime"));
                }
            }
        }

        private string direction;      
        public string Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Direction"));
                }
            }
        }

        private string frameID;
        public string FrameID
        {
            get
            {
                return frameID;
            }
            set
            {
                frameID = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FrameID"));
                }
            }
        }

        string canFrameType;
        public string CANFrameType
        {
            get
            {
                return canFrameType;
            }
            set
            {
                canFrameType = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CANFrameType"));
                }
            }
        }

        string protocolFrameType;
        public string ProtocolFrameType
        {
            get
            {
                return protocolFrameType;
            }
            set
            {
                protocolFrameType = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ProtocolFrameType"));
                }
            }
        }

        string serviceType;
        public string ServiceType
        {
            get
            {
                return serviceType;
            }
            set
            {
                serviceType = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ServiceType"));
                }
            }
        }

        string frameData;
        public string FrameData
        {
            get
            {
                return frameData;
            }
            set
            {
                frameData = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FrameData"));
                }
            }
        }

    }
}
