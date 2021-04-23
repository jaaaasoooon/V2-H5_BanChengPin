using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5Entity
{
    public class ZLGInfo
    {                   

        #region 定义ZLGCAN变量

        private static UInt32 devType;
        /// <summary>
        /// 设备类型号
        /// </summary>
        public static UInt32 DevType
        {
            get { return devType; }
            set
            {
                devType = value;
            }
        }


        private UInt32 devIndex;
        /// <summary>
        /// 设备索引号
        /// </summary>
        public UInt32 DevIndex
        {
            get { return devIndex; }
            set
            {
                devIndex = value;
            }
        }

        private UInt32 devChannel;
        /// <summary>
        /// 设备通道号
        /// </summary>
        public UInt32 DevChannel
        {
            get { return devChannel; }
            set
            {
                devChannel = value;
            }
        }
        private static BaudRate baudRate;
        /// <summary>
        /// 设备波特率
        /// </summary>
        public static BaudRate Baudrate
        {
            get { return baudRate; }
            set
            {
                baudRate = value;
            }
        }

        private UInt32 accCode;
        /// <summary>
        /// 验收码
        /// </summary>
        public UInt32 AccCode
        {
            get { return accCode; }
            set
            {
                accCode = value;
            }
        }

        private UInt32 accMask;
        /// <summary>
        /// 屏蔽码
        /// </summary>
        public UInt32 AccMask
        {
            get { return accMask; }
            set
            {
                accMask = value;
            }
        }


        private static byte timing0;
        /// <summary>
        /// 波特率定时器0
        /// </summary>
        public static byte Timing0
        {
            get { return timing0; }
            set
            {
                timing0 = value;
            }
        }


        private static byte timing1;
        /// <summary>
        /// 波特率定时器1
        /// </summary>
        public static byte Timing1
        {
            get { return timing1; }
            set
            {
                timing1 = value;
            }
        }


        private byte mode;
        /// <summary>
        /// 模式。=0表示正常模式（相当于正常节点），=1表示只听模式（只接收，不影响总线）。
        /// </summary>
        public byte Mode
        {
            get { return mode; }
            set
            {
                mode = value;                     
            }
        }

        private ArbitrationBaudRate arbitrationBaudRate;
        /// <summary>
        /// 仲裁域波特率
        /// </summary>
        public ArbitrationBaudRate ArbitrationBaudrate
        {
            get { return arbitrationBaudRate; }
            set
            {
                arbitrationBaudRate = value;
            }
        }

        private DataBaudRate dataBaudrate;
        /// <summary>
        /// 数据域波特率
        /// </summary>
        public DataBaudRate DataBaudRate
        {
            get { return dataBaudrate; }
            set
            {
                dataBaudrate = value;
            }
        }

        private uint canFD;
        /// <summary>
        /// CANFD标准   0表示CANFD ISO，1表示CANFD BOSCH
        /// </summary>
        public uint CANFD
        {
            get { return canFD; }
            set
            {
                canFD = value;
            }
        }

        private int terminaiResistanceEnabled;
        /// <summary>
        /// 终端电阻使能 0表示使能，1表示不使能
        /// </summary>
        public int TerminaiResistanceEnabled
        {
            get { return terminaiResistanceEnabled; }
            set
            {
                terminaiResistanceEnabled = value;
            }
        }
        #endregion


        #region 界面功能必需参数定义

        private int sendNum;
        /// <summary>
        /// 发送帧数
        /// </summary>
        public int SendNum
        {
            get { return sendNum; }
            set
            {
                sendNum = value;
                TotalNum = sendNum + receiveNum;                   
            }
        }

        private bool isSendFrame = false;
        public bool IsSendFrame
        {
            get { return isSendFrame; }
            set
            {
                isSendFrame = value;                  
            }
        }

        private int receiveNum;
        /// <summary>
        /// 接收帧数
        /// </summary>
        public int ReceiveNum
        {
            get { return receiveNum; }
            set
            {
                receiveNum = value;
                TotalNum = sendNum + receiveNum;                     
            }
        }

        private int totalNum;
        /// <summary>
        /// 帧数总数
        /// </summary>
        public int TotalNum
        {
            get { return totalNum; }
            set
            {
                totalNum = value;                 
            }
        }

        private String connObject;
        /// <summary>
        /// TabControl Head
        /// </summary>
        public String ConnObject
        {
            get { return connObject; }
            set
            {
                connObject = value;                    
            }
        }

        private string sendID;
        /// <summary>
        /// 要发送的数据ID
        /// </summary>
        public string SendID
        {
            get { return sendID; }
            set
            {
                sendID = value;                      
            }
        }

        private int sendTime;
        /// <summary>
        /// 发送次数
        /// </summary>
        public int SendTime
        {
            get { return sendTime; }
            set
            {
                sendTime = value;                     
            }
        }

        private bool sendOutFlag = true;
        /// <summary>
        /// 发送数据完成标识
        /// </summary>
        public bool SendOutFlag
        {
            get { return sendOutFlag; }
            set
            {
                sendOutFlag = value;                  
            }
        }

        private int sendInterval;
        /// <summary>
        /// 发送次数
        /// </summary>
        public int SendInterval
        {
            get { return sendInterval; }
            set
            {
                sendInterval = value;                
            }
        }


        private bool isRecFrame = false;
        /// <summary>
        /// 标识位，记录接口卡设备是否被打开，false=关闭，true=打开
        /// </summary>
        public bool IsRecFrame
        {
            get { return isRecFrame; }
            set
            {
                isRecFrame = value;                 
            }
        }


        #endregion
    }
}
