using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5
{
    public class UpdateSystemInfo : INotifyPropertyChanged
    {

        public string[] SourceData { get; set; }        // 源文件数据 

        public event PropertyChangedEventHandler PropertyChanged;
        
        private int _max, _min, _currVal;

        public int Max
        {
            get { return _max; }
            set
            {
                _max = value;
                if (this.PropertyChanged != null)//激发事件，参数为Max属性  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Max"));
                }
            }
        }

        public int Min
        {
            get { return _min; }
            set
            {
                _min = value;
                if (this.PropertyChanged != null)//激发事件，参数为Min属性  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Min"));
                }
            }
        }

        public int CurrentValue
        {
            get { return _currVal; }
            set
            {
                _currVal = value;
                if (this.PropertyChanged != null)//激发事件，参数为CurrentValue属性  
                {            
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentValue"));
                }
            }
        }

        string updateProgress = null;
        public string UpdateProgress
        {
            get { return updateProgress; }
            set
            {
                updateProgress = value;
                if (this.PropertyChanged != null)//激发事件，参数为UpdateProgress属性  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("UpdateProgress"));
                }
            }
        }


        int nRandNum = 0;
        public int RandNum
        {
            get { return nRandNum; }
            set
            {
                nRandNum = value;
                if (this.PropertyChanged != null)//激发事件，参数为RandNum属性  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("RandNum"));
                }
            }
        }

        #region 下载按钮是否可用
        bool bBtnIsEnabled = true;
        public bool BtnIsEnabled
        {
            get { return bBtnIsEnabled; }
            set
            {
                bBtnIsEnabled = value;
                if (this.PropertyChanged != null)  //激发事件，参数为 BtnIsEnabled 属性  
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("BtnIsEnabled"));
                }
            }
        }
        #endregion
    }
}
