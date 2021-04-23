using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UDSEntity;

namespace UDSStudio
{
    public class StudioConfig:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        //public string SoftName
        //{
        //    get { return "UDS Studio V1.0"; }            
        //}

        private Master masterVar;
        public Master MasterVar
        {
            get { return masterVar; }
            set 
            {
                masterVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MasterVar"));
            }
        }
        /// <summary>
        /// 告警和保护模式类型
        /// </summary>
        private bool compactWarn = true;
        public bool CompactWarn
        {
            get { return compactWarn; }
            set
            {
                compactWarn = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CompactWarn"));
            }
        }

        /// <summary>
        /// 连接与否
        /// </summary>
        private bool islink = false;
        public bool IsLink
        {
            get { return islink; }
            set
            {
                islink = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsLink"));
            }
        }

        private byte[] keyCode = new byte[2];
        public byte[] KeyCode
        {
            get { return keyCode; }
            set
            {
                keyCode = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("KeyCode"));
            }
        }

        private string keyCodeStr = string.Empty;
        public string KeyCodeStr
        {
            get { return keyCodeStr; }
            set
            {
                keyCodeStr = value;
                OnPropertyChanged(new PropertyChangedEventArgs("KeyCodeStr"));
            }
        }

        private int currentPack = 0;
        public int CurrentPack
        {
            get { return currentPack; }
            set
            {
                currentPack = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("CurrentPack"));
            }
 
        }

        private int currentPackDid = 0;
        public int CurrentPackDid
        {
            get { return currentPackDid; }
            set
            {
                currentPackDid = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("CurrentPackDid"));
            }
        }

        private int currentDid = 0;
        public int CurrentDid
        {
            get { return currentDid; }
            set
            {
                currentDid = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("CurrentDid"));
            }
        }

        private string boardDateTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        public string BoardDateTimeStr
        {
            get { return boardDateTimeStr; }
            set
            {
                boardDateTimeStr = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BoardDateTimeStr"));
            }
        }

        private Pack packVar;
        public Pack PackVar
        {
            get { return packVar; }
            set
            {
                packVar = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("PackVar"));
            }
        }

        #region Custom Protocol Config

        public int CP_BitParaStartIndex;
        public int CP_BitParaByteNum;
        public string CP_ParaTmprType;
        public bool IsMaster = true;
        public Boolean UploadingAllParas = false;
        public Boolean UploadingAllAdjustParas = false;    
        
        #endregion
        /// <summary>
        /// 协议成功导入标识
        /// </summary>
        private bool protocolLoaded = false;
        public bool ProtocolLoaded
        {
            get { return protocolLoaded; }
            set
            {
                protocolLoaded = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ProtocolLoaded"));
            }
        }

        private bool isReocrding = false;
        public bool IsReocrding
        {
            get { return isReocrding; }
            set
            {
                isReocrding = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsReocrding"));

            }
        }

        private int digitRecordCount = 0;
        public int DigitRecordCount
        {
            get { return digitRecordCount; }
            set
            {
                digitRecordCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("digitRecordCount"));

            }
        }

        /// <summary>
        /// 参数成功导入标识
        /// </summary>
        private bool paramLoaded = false;
        public bool ParamLoaded
        {
            get { return paramLoaded; }
            set
            {
                paramLoaded = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("ParamLoaded"));
            }
        }

        #region DTC

        /// <summary>
        /// DTCStatusMask
        /// </summary>
        private byte dtcStatusMask;
        public byte DtcStatusMask
        {
            get { return dtcStatusMask; }
            set
            {
                dtcStatusMask = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DtcStatusMask"));
            }
        }

        /// <summary>
        /// DTC报文格式码
        /// </summary>
        private byte formatCode;
        public byte FormatCode
        {
            get { return formatCode; }
            set
            {
                formatCode = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FormatCode"));
            }
        }

        /// <summary>
        /// DTC可用记录条数
        /// </summary>
        private int dtcCount;
        public int DtcCount
        {
            get { return dtcCount; }
            set
            {
                dtcCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DtcCount"));
            }
        }

        private List<string> dtcTypeList;// = new List<string>();
        public List<string> DtcTypeList
        {
            get { return dtcTypeList; }
            set
            {
                dtcTypeList = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DtcTypeList"));
            }
        }

        #endregion
        
        
        #region 仪表配置

        //电压表盘配置

        //量程开始值
        private int voltRangeStartValue = 0;
        public int VoltRangeStartValue
        {
            get { return voltRangeStartValue; }
            set
            {
                voltRangeStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltRangeStartValue"));
            }
        }

        //量程结束值
        private int voltRangeEndValue = 100;
        public int VoltRangeEndValue
        {
            get { return voltRangeEndValue; }
            set
            {
                voltRangeEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltRangeEndValue"));
            }
        }

        //刻度个数
        private uint voltMajorIntervalCount = 10;
        public uint VoltMajorIntervalCount
        {
            get { return voltMajorIntervalCount; }
            set
            {
                voltMajorIntervalCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltMajorIntervalCount"));
            }
        }

        //低压保护范围

        //低压保护开始值
        private DevExpress.Xpf.Gauges.RangeValue voltLowProtStartValue = new DevExpress.Xpf.Gauges.RangeValue(0);
        public DevExpress.Xpf.Gauges.RangeValue VoltLowProtStartValue
        {
            get { return voltLowProtStartValue; }
            set
            {
                voltLowProtStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltLowProtStartValue"));
            }
        }

        //低压保护结束值
        private DevExpress.Xpf.Gauges.RangeValue voltLowProtEndValue = new DevExpress.Xpf.Gauges.RangeValue(10);
        public DevExpress.Xpf.Gauges.RangeValue VoltLowProtEndValue
        {
            get { return voltLowProtEndValue; }
            set
            {
                voltLowProtEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltLowProtEndValue"));
            }
        }

        //低压告警范围

        //低压告警开始值
        private DevExpress.Xpf.Gauges.RangeValue voltLowWarnStartValue = new DevExpress.Xpf.Gauges.RangeValue(10);
        public DevExpress.Xpf.Gauges.RangeValue VoltLowWarnStartValue
        {
            get { return voltLowWarnStartValue; }
            set
            {
                voltLowWarnStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltLowWarnStartValue"));
            }
        }

        //低压告警结束值
        private DevExpress.Xpf.Gauges.RangeValue voltLowWarnEndValue = new DevExpress.Xpf.Gauges.RangeValue(30);
        public DevExpress.Xpf.Gauges.RangeValue VoltLowWarnEndValue
        {
            get { return voltLowWarnEndValue; }
            set
            {
                voltLowWarnEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltLowWarnEndValue"));
            }
        }

        //电压正常范围

        //电压正常开始值
        private DevExpress.Xpf.Gauges.RangeValue voltNormalStartValue = new DevExpress.Xpf.Gauges.RangeValue(30);
        public DevExpress.Xpf.Gauges.RangeValue VoltNormalStartValue
        {
            get { return voltNormalStartValue; }
            set
            {
                voltNormalStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltNormalStartValue"));
            }
        }

        //低压保护结束值
        private DevExpress.Xpf.Gauges.RangeValue voltNormalEndValue = new DevExpress.Xpf.Gauges.RangeValue(53);
        public DevExpress.Xpf.Gauges.RangeValue VoltNormalEndValue
        {
            get { return voltNormalEndValue; }
            set
            {
                voltNormalEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltNormalEndValue"));
            }
        }

        //高压告警范围

        //高压告警开始值
        private DevExpress.Xpf.Gauges.RangeValue voltHighWarnStartValue = new DevExpress.Xpf.Gauges.RangeValue(53);
        public DevExpress.Xpf.Gauges.RangeValue VoltHighWarnStartValue
        {
            get { return voltHighWarnStartValue; }
            set
            {
                voltHighWarnStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltHighWarnStartValue"));
            }
        }

        //高压告警结束值
        private DevExpress.Xpf.Gauges.RangeValue voltHighWarnEndValue = new DevExpress.Xpf.Gauges.RangeValue(80);
        public DevExpress.Xpf.Gauges.RangeValue VoltHighWarnEndValue
        {
            get { return voltHighWarnEndValue; }
            set
            {
                voltHighWarnEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltHighWarnEndValue"));
            }
        }

        //高压保护范围

        //高压保护开始值
        private DevExpress.Xpf.Gauges.RangeValue voltHighProtStartValue = new DevExpress.Xpf.Gauges.RangeValue(80);
        public DevExpress.Xpf.Gauges.RangeValue VoltHighProtStartValue
        {
            get { return voltHighProtStartValue; }
            set
            {
                voltHighProtStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltHighProtStartValue"));
            }
        }

        //高压保护结束值
        private DevExpress.Xpf.Gauges.RangeValue voltHighProtEndValue = new DevExpress.Xpf.Gauges.RangeValue(100);
        public DevExpress.Xpf.Gauges.RangeValue VoltHighProtEndValue
        {
            get { return voltHighProtEndValue; }
            set
            {
                voltHighProtEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltHighProtEndValue"));
            }
        }


        //电流表盘配置

        //量程开始值
        private int curRangeStartValue = -200;
        public int CurRangeStartValue
        {
            get { return curRangeStartValue; }
            set
            {
                curRangeStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurRangeStartValue"));
            }
        }

        //量程结束值
        private int curRangeEndValue = 200;
        public int CurRangeEndValue
        {
            get { return curRangeEndValue; }
            set
            {
                curRangeEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurRangeEndValue"));
            }
        }

        //刻度个数
        private uint curMajorIntervalCount = 10;
        public uint CurMajorIntervalCount
        {
            get { return curMajorIntervalCount; }
            set
            {
                curMajorIntervalCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurMajorIntervalCount"));
            }
        }

        //放电电流保护范围

        //放电电流保护开始值
        private DevExpress.Xpf.Gauges.RangeValue curLowProtStartValue = new DevExpress.Xpf.Gauges.RangeValue(-200);
        public DevExpress.Xpf.Gauges.RangeValue CurLowProtStartValue
        {
            get { return curLowProtStartValue; }
            set
            {
                curLowProtStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurLowProtStartValue"));
            }
        }

        //放电电流保护结束值
        private DevExpress.Xpf.Gauges.RangeValue curLowProtEndValue = new DevExpress.Xpf.Gauges.RangeValue(-160);
        public DevExpress.Xpf.Gauges.RangeValue CurLowProtEndValue
        {
            get { return curLowProtEndValue; }
            set
            {
                curLowProtEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurLowProtEndValue"));
            }
        }

        //低压告警范围

        //放电电流告警开始值
        private DevExpress.Xpf.Gauges.RangeValue curLowWarnStartValue = new DevExpress.Xpf.Gauges.RangeValue(-160);
        public DevExpress.Xpf.Gauges.RangeValue CurLowWarnStartValue
        {
            get { return curLowWarnStartValue; }
            set
            {
                curLowWarnStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurLowWarnStartValue"));
            }
        }

        //放电电流告警结束值
        private DevExpress.Xpf.Gauges.RangeValue curLowWarnEndValue = new DevExpress.Xpf.Gauges.RangeValue(-100);
        public DevExpress.Xpf.Gauges.RangeValue CurLowWarnEndValue
        {
            get { return curLowWarnEndValue; }
            set
            {
                curLowWarnEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurLowWarnEndValue"));
            }
        }

        //电压正常范围

        //正常电流开始值
        private DevExpress.Xpf.Gauges.RangeValue curNormalStartValue = new DevExpress.Xpf.Gauges.RangeValue(-100);
        public DevExpress.Xpf.Gauges.RangeValue CurNormalStartValue
        {
            get { return curNormalStartValue; }
            set
            {
                curNormalStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurNormalStartValue"));
            }
        }

        //正常电流结束值
        private DevExpress.Xpf.Gauges.RangeValue curNormalEndValue = new DevExpress.Xpf.Gauges.RangeValue(100);
        public DevExpress.Xpf.Gauges.RangeValue CurNormalEndValue
        {
            get { return curNormalEndValue; }
            set
            {
                curNormalEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurNormalEndValue"));
            }
        }

        //充电电流告警范围

        //充电电流告警开始值
        private DevExpress.Xpf.Gauges.RangeValue curHighWarnStartValue = new DevExpress.Xpf.Gauges.RangeValue(100);
        public DevExpress.Xpf.Gauges.RangeValue CurHighWarnStartValue
        {
            get { return curHighWarnStartValue; }
            set
            {
                curHighWarnStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurHighWarnStartValue"));
            }
        }

        //充电电流告警结束值
        private DevExpress.Xpf.Gauges.RangeValue curHighWarnEndValue = new DevExpress.Xpf.Gauges.RangeValue(160);
        public DevExpress.Xpf.Gauges.RangeValue CurHighWarnEndValue
        {
            get { return curHighWarnEndValue; }
            set
            {
                curHighWarnEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurHighWarnEndValue"));
            }
        }

        //充电电流保护范围

        //充电电流保护开始值
        private DevExpress.Xpf.Gauges.RangeValue curHighProtStartValue = new DevExpress.Xpf.Gauges.RangeValue(160);
        public DevExpress.Xpf.Gauges.RangeValue CurHighProtStartValue
        {
            get { return curHighProtStartValue; }
            set
            {
                curHighProtStartValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurHighProtStartValue"));
            }
        }

        //充电电流保护结束值
        private DevExpress.Xpf.Gauges.RangeValue curHighProtEndValue = new DevExpress.Xpf.Gauges.RangeValue(200);
        public DevExpress.Xpf.Gauges.RangeValue CurHighProtEndValue
        {
            get { return curHighProtEndValue; }
            set
            {
                curHighProtEndValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurHighProtEndValue"));
            }
        }

        #endregion

        #region  HostProtocolName, HostProtocolVerStr

        public string hostProtocolFileName = "请加载";
        public string HostProtocolFileName
        {
            get { return hostProtocolFileName; }
            set
            {
                hostProtocolFileName = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("HostProtocolFileName"));
            }
        }

        public string hostProtocolName = "请加载";
        public string HostProtocolName
        {
            get { return hostProtocolName; }
            set
            {
                hostProtocolName = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("HostProtocolName"));
            }
        }

        public string hostProtocolVerStr = "请加载";
        public string HostProtocolVerStr
        {
            get { return hostProtocolVerStr; }
            set
            {
                hostProtocolVerStr = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("HostProtocolVerStr"));
            }
        }
        #endregion
    }


    public class ProtoclBlock
    {
        public string Name;
        public bool NumFieldEnable;
        public int MeterNum;
        public string BlockType;
        public int MeterStart;
        public int MeterSignalByteOrWordNum;
    }
}
