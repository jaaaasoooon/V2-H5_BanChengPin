using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace UDSStudio
{
    public class Pack:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public Pack() { }
        public Pack(string name,List<DigitVar> cellList,List<DigitVar> tmprList)
        {
            PackName = name;
            CellsListDigitVar = cellList;
            TmprListDigitVar = tmprList;
        }

        /// <summary>
        /// 单体电池列表
        /// </summary>
        private List<DigitVar> cellsListDigitVar;
        public List<DigitVar> CellsListDigitVar
        {
            get { return cellsListDigitVar; }
            set
            {
                cellsListDigitVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CellsListDigitVar"));
            }
        }

        /// <summary>
        /// 单体温度
        /// </summary>
        private List<DigitVar> tmprsListDigitVar;
        public List<DigitVar> TmprListDigitVar
        {
            get { return tmprsListDigitVar; }
            set
            {
                tmprsListDigitVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TmprListDigitVar"));
            }
        }

#if false
        /// <summary>
        /// 软件版本
        /// </summary>
        private string packSoftVer;
        public string PackSoftVer
        {
            get { return packSoftVer; }
            set
            {
                packSoftVer = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PackSoftVer"));
            }
        }

        /// <summary>
        /// 硬件版本
        /// </summary>        
        private string packHardVer;
        public string PackHardVer
        {
            get { return packHardVer; }
            set
            {
                packHardVer = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PackHardVer"));
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        private byte packAddress;
        public byte PackAddress
        {
            get { return packAddress; }
            set
            {
                packAddress = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PackAddress"));
            }
        }
#endif

        /// <summary>
        /// 在线标识
        /// </summary>
        private Boolean isOnLine = false;
        public Boolean IsOnLine
        {
            get { return isOnLine; }
            set
            {
                isOnLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsOnLine"));
            }
        }

        private Boolean hasAFE = false;
        public Boolean HasAFE
        {
            get { return hasAFE; }
            set
            {
                hasAFE = value;
                OnPropertyChanged(new PropertyChangedEventArgs("HasAFE"));
            }
        }

        private string packName;
        public string PackName
        {
            get { return packName; }
            set
            {
                packName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PackName"));
            }
        }

    }
}
