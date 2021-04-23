using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using UDSEntity;

namespace UDSStudio
{
    public class Master:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        //private bool protocolLoaded = true;
        //public bool ProtocolLoaded
        //{
        //    get { return protocolLoaded; }
        //    set
        //    {
        //        protocolLoaded = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("ProtocolLoaded"));
        //    }
        //}

        /// <summary>
        /// 电池电压
        /// </summary>
        private DigitVar batteryVoltage;
        public DigitVar BatteryVoltage
        {
            get { return batteryVoltage; }
            set
            {
                batteryVoltage = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BatteryVoltage"));
            }
        }

        /// <summary>
        /// 电流
        /// </summary>
        private DigitVar electricCurrent;
        public DigitVar ElectricCurrent
        {
            get { return electricCurrent; }
            set
            {
                electricCurrent = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ElectricCurrent"));
            }
        }

        /// <summary>
        /// 最高单体电压
        /// </summary>
        private DigitVar maxCellVoltage;
        public DigitVar MaxCellVoltage
        {
            get { return maxCellVoltage; }
            set
            {
                maxCellVoltage = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MaxCellVoltage"));
            }
        }

        /// <summary>
        /// 最高单体电池序号
        /// </summary>
        private int maxCellIndex;
        public int MaxCellIndex
        {
            get { return maxCellIndex; }
            set
            {
                maxCellIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MaxCellIndex"));
            }
        }

        /// <summary>
        /// 最高单体序号
        /// </summary>
        private DigitVar maxCellVoltIndex;
        public DigitVar MaxCellVoltIndex
        {
            get { return maxCellVoltIndex; }
            set
            {
                maxCellVoltIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MaxCellVoltIndex"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string maxCellHeader;
        public string MaxCellHeader
        {
            get { return maxCellHeader; }
            set
            {
                maxCellHeader = (string)Application.Current.Resources["gpbHighestVolt"] +string.Format("C {0}", MaxCellIndex);
                OnPropertyChanged(new PropertyChangedEventArgs("MaxCellHeader"));
            }
        }


        /// <summary>
        /// 最低单体电压
        /// </summary>
        private DigitVar minCellVoltDigitVar;
        public DigitVar MinCellVoltDigitVar
        {
            get { return minCellVoltDigitVar; }
            set
            {
                minCellVoltDigitVar = value;
                VoltageDifference = maxCellVoltage.VarValue - minCellVoltDigitVar.VarValue;
                OnPropertyChanged(new PropertyChangedEventArgs("MinCellVoltDigitVar"));
            }
        }

        /// <summary>
        /// 最低单体电池序号
        /// </summary>
        private DigitVar minCellVoltIndex;
        public DigitVar MinCellVoltIndex
        {
            get { return minCellVoltIndex; }
            set
            {
                minCellVoltIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MinCellVoltIndex"));
            }
        }


        /// <summary>
        /// 最低单体电池序号
        /// </summary>
        private int minCellIndex;
        public int MinCellIndex
        {
            get { return minCellIndex; }
            set
            {
                minCellIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MinCellIndex"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string minCellHeader;
        public string MinCellHeader
        {
            get { return minCellHeader; }
            set 
            {
                minCellHeader = (string)Application.Current.Resources["gpbLowestVolt"] + string.Format("C {0}", MinCellIndex);
                OnPropertyChanged(new PropertyChangedEventArgs("MinCellHeader"));
            }
        }

        /// <summary>
        /// 平均单体电压
        /// </summary>
        private DigitVar avgCellVoltDigitVar;
        public DigitVar AvgCellVoltDigitVar
        {
            get { return avgCellVoltDigitVar; }
            set
            {
                avgCellVoltDigitVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AvgCellVoltDigitVar"));
            }
        }

        /// <summary>
        /// 压差
        /// </summary>
        private float voltageDifference = 0.0f;
        public float VoltageDifference
        {
            get { return voltageDifference; }
            set
            {
                voltageDifference = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VoltageDifference"));
            }
        }

        /// <summary>
        /// 温差
        /// </summary>
        private float temperatureDifference = 0.0f;
        public float TemperatureDifference
        {
            get { return temperatureDifference; }
            set
            {
                temperatureDifference = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TemperatureDifference"));
            }
        }

        private string avgCellHeader;
        public string AvgCellHeader
        {
            get { return avgCellHeader; }
            set
            {
                avgCellHeader = (string)Application.Current.Resources["gpbAverageVolt"];
                OnPropertyChanged(new PropertyChangedEventArgs("AvgCellHeader"));
            }
        }

        /// <summary>
        /// 温度采集点最高温度
        /// </summary>
        private DigitVar maxTmprDigitVar;
        public DigitVar MaxTmprDigitVar
        {
            get { return maxTmprDigitVar; }
            set
            {
                maxTmprDigitVar = value;                
                OnPropertyChanged(new PropertyChangedEventArgs("MaxTmprDigitVar"));
            }
        }

        /// <summary>
        /// 最高温度采集点序号
        /// </summary>
        private DigitVar maxTmprDigitVarIndex;
        public DigitVar MaxTmprDigitVarIndex
        {
            get { return maxTmprDigitVarIndex; }
            set
            {
                maxTmprDigitVarIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MaxTmprDigitVarIndex"));
            }
        }

        /// <summary>
        /// 温度采集点低高温度
        /// </summary>
        private DigitVar minTmprDigitVar;
        public DigitVar MinTmprDigitVar
        {
            get { return minTmprDigitVar; }
            set
            {
                minTmprDigitVar = value;
                TemperatureDifference = maxTmprDigitVar.VarValue - minTmprDigitVar.VarValue;
                OnPropertyChanged(new PropertyChangedEventArgs("MinTmprDigitVar"));
            }
        }

        /// <summary>
        /// 最低温度采集点序号
        /// </summary>
        private DigitVar minTmprDigitVarIndex;
        public DigitVar MinTmprDigitVarIndex
        {
            get { return minTmprDigitVarIndex; }
            set
            {
                minTmprDigitVarIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MinTmprDigitVarIndex"));
            }
        }

        /// <summary>
        /// 最高温度采集点序号
        /// </summary>
        private int maxTmprIndex;
        public int MaxTmprIndex
        {
            get { return maxTmprIndex; }
            set
            {
                maxTmprIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MaxTmprIndex"));
            }
        }

        private string maxTmprHeader;
        public string MaxTmprHeader
        {
            get { return maxTmprHeader; }
            set
            {
                maxTmprHeader = (string)Application.Current.Resources["gpbHighestTmpr"] + string.Format("T {0}", MaxTmprIndex);
                OnPropertyChanged(new PropertyChangedEventArgs("MaxTmprHeader"));
            }
        }

        /// <summary>
        /// 电池SOC
        /// </summary>
        private DigitVar batSocDigitVar;
        public DigitVar BatSocDigitVar
        {
            get { return batSocDigitVar; }
            set
            {
                batSocDigitVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BatSocDigitVar"));
            }
        }

        /// <summary>
        /// 电池Soh
        /// </summary>
        private DigitVar batSohDigitVar;
        public DigitVar BatSohDigitVar
        {
            get { return batSohDigitVar; }
            set
            {
                batSohDigitVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BatSohDigitVar"));
            }
        }

        /// <summary>
        /// 系统绝缘阻值
        /// </summary>
        private DigitVar sysIslDigitVar;
        public DigitVar SysIslDigitVar
        {
            get { return sysIslDigitVar; }
            set
            {
                sysIslDigitVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SysIslDigitVar"));
            }
        }

        /// <summary>
        /// 正极绝缘阻值
        /// </summary>
        private DigitVar batPositiveInsulationDigit;
        public DigitVar BatPositiveInsulationDigit
        {
            get { return batPositiveInsulationDigit; }
            set
            {
                batPositiveInsulationDigit = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BatPositiveInsulationDigit"));
            }
        }

        /// <summary>
        /// 负极绝缘阻值
        /// </summary>
        private DigitVar batNegativeInsulationDigit;
        public DigitVar BatNegativeInsulationDigit
        {
            get { return batNegativeInsulationDigit; }
            set
            {
                batNegativeInsulationDigit = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BatNegativeInsulationDigit"));
            }
        }

        /// <summary>
        /// 系统运行模式
        /// </summary>
        private Dictionary<int, string> mode;
        public Dictionary<int, string> Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Mode"));
            }
        }


        /// <summary>
        /// 显示模式
        /// </summary>
        private string rcvModeText;
        public string RcvModeText
        {
            get { return rcvModeText; }
            set
            {
                rcvModeText = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RcvModeText"));
            }
        }

        /// <summary>
        /// 其它主控数据列表
        /// </summary>
        private List<DigitVar> otherMasterDataListDigitVar;
        public List<DigitVar> OtherMasterDataListDigitVar
        {
            get { return otherMasterDataListDigitVar; }
            set 
            {
                otherMasterDataListDigitVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("OtherMasterDataListDigitVar"));
            }
        }

        /// <summary>
        /// 继电器状态
        /// </summary>
        private List<StateVar> relayStateVar;
        public List<StateVar> RelayStateVar
        {
            get { return relayStateVar; }
            set
            {
                relayStateVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RelayStateVar"));
            }
        }

        /// <summary>
        /// 输入输出信号列表
        /// </summary>
        private List<StateVar> auxInputSignalStateVar;
        public List<StateVar> AuxInputSignalStateVar
        {
            get { return auxInputSignalStateVar; }
            set
            {
                auxInputSignalStateVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AuxInputSignalStateVar"));
            }
        }

        /// <summary>
        /// 告警和保护
        /// </summary>
        private List<StateVar> warnAndProtect;
        public List<StateVar> WarnAndProtect
        {
            get { return warnAndProtect; }
            set
            {
                warnAndProtect = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WarnAndProtect"));
            }
        }

        /// <summary>
        /// 电池类型
        /// </summary>
        private string batType;
        public string BatType
        {
            get { return batType; }
            set
            {
                batType = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("BatType"));
            }
        }
        
        /// <summary>
        /// 生产厂商
        /// </summary>
        private string manufacturers;
        public string Manufacturers
        {
            get { return manufacturers; }
            set
            {
                manufacturers = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("Manufacturers"));
            }
        }

        /// <summary>
        /// 软件版本
        /// </summary>
        private string softVer;
        public string SoftVer
        {
            get { return softVer; }
            set
            {
                softVer = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("SoftVer"));
            }
        }

        /// <summary>
        /// 硬件版本
        /// </summary>
        private string hardVer;
        public string HardVer
        {
            get { return hardVer; }
            set
            {
                hardVer = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("HardVer"));
            }
        }

        /// <summary>
        /// 电芯串数
        /// </summary>
        private int cellCnt;
        public int CellCnt
        {
            get { return cellCnt; }
            set
            {
                cellCnt = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("CellCnt"));
            }
        }


        /// <summary>
        /// 主控整形参数
        /// </summary>
        //private List<ParaIntVar> masterIntParaList;
        //public List<ParaIntVar> MasterIntParaList
        //{
        //    get { return masterIntParaList; }
        //    set
        //    {
        //        masterIntParaList = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("MasterIntParaList"));
        //    }
        //}

        /// <summary>
        /// 主控位参数
        /// </summary>
        //private List<ParaBitVar> masterBitParaList;
        //public List<ParaBitVar> MasterBitParaList
        //{
        //    get { return masterBitParaList; }
        //    set
        //    {
        //        masterBitParaList = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("MasterBitParaList"));
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        //private List<ParaIntVar> masterBitParaGroupList;
        //public List<ParaIntVar> MasterBitParaGroupList
        //{
        //    get { return masterBitParaGroupList; }
        //    set
        //    {
        //        masterBitParaGroupList = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("MasterBitParaGroupList"));
        //    }
        //}


        /// <summary>
        /// Pack列表
        /// </summary>
        private List<Pack> packList;
        public List<Pack> PackList
        {
            get { return packList; }
            set
            {
                packList = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PackList"));
            }
        }

        private RelayControl relayCtrl;
        public RelayControl RelayCtrl
        {
            get { return relayCtrl; }
            set
            {
                relayCtrl = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RelayCtrl"));
            }
        }

    }

    public class RelayControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private CtrlVar debugSwitch;
        public CtrlVar DebugSwitch
        {
            get { return debugSwitch; }
            set
            {
                debugSwitch = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DebugSwitch"));
            }
        }

        private List<CtrlVar> childCtrl = new List<CtrlVar>();
        public List<CtrlVar> ChildCtrl
        {
            get { return childCtrl; }
            set
            {
                childCtrl = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ChildCtrl"));
            }
        }
    }

}
