using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UDSEntity;

namespace UDSStudio
{
    public class ParaIntVar : INotifyPropertyChanged, IDataErrorInfo
    {
        private bool isTemperatureVar = false;
        public int GroupData = 0;  //-for BitParaVar

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region IDataErrorInfo Members
        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (propertyError != string.Empty)
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.ToString();
            }
        }


        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "VarValue")
                {
                    if (VarValue < VarMin || VarValue > VarMax)
                        return string.Format("数值{0}必须介于{1:F3}和{2:F3}之间！",  VarValue, VarMin, VarMax);
                }

                return string.Empty;
            }

        }
        #endregion

        public ParaIntVar() { }

        public ParaIntVar(string name, string unit, int byteNum, decimal scale, int index,
            decimal min = decimal.MinValue, decimal max = decimal.MaxValue, decimal value = decimal.One)
        {
            VarName = name;
            VarValue = value;
            VarUnit = unit;
            VarByteNum = byteNum;
            VarScale = scale;
            VarMax = max;
            VarMin = min;
            ParaIndex = index;
        }

        public decimal ConvertFill(int rawValue, string para = null)
        {
            decimal result;

            if (rawValue >= 0x8000)
            {
                if (this.VarUnit == "A" || (isTemperatureVar && Temperature.GetTempType(this.VarUnit) != Temperature.TemperatureType.K))
                    result = -((~rawValue + 1) & 0xFFFF);
                else
                    result = rawValue;
            }
            else
            {
                result = rawValue;
            }

            result *= this.VarScale;

            if (isTemperatureVar)
            {
                result = Temperature.Get(this.VarUnit, result, para);
            }

            this.VarValue = result;

            return result;
        }

        public int ConvertBack(string para = null)
        {
            decimal result;

            if (isTemperatureVar)
            {
                result = Temperature.Get(para, this.VarValue, this.VarUnit);
                result /= this.VarScale;
            }
            else
            {
                result = this.VarValue / this.VarScale;
            }

            return (int)result;

        }

        private string varName;
        public string VarName
        {
            get { return varName; }
            set
            {
                varName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarName"));
            }
        }

        private decimal varValue;
        public decimal VarValue
        {
            get { return varValue; }
            set
            {
                varValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarValue"));
            }
        }

        private int varByteNum;
        public int VarByteNum
        {
            get { return varByteNum; }
            set
            {
                varByteNum = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarByteNum"));
            }
        }

        private string varUnit;
        public string VarUnit
        {
            get { return varUnit; }
            set
            {
                varUnit = value;

                if (Temperature.GetTempType(varUnit) == Temperature.TemperatureType.NonTemp)
                    isTemperatureVar = false;
                else
                    isTemperatureVar = true;

                OnPropertyChanged(new PropertyChangedEventArgs("VarUnit"));
            }
        }

        private decimal varScale=decimal.MaxValue;
        public decimal VarScale
        {
            get { return varScale; }
            set
            {
                varScale = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarScale"));
            }
        }

        private decimal varMax;
        public decimal VarMax
        {
            get { return varMax; }
            set
            {
                varMax = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarMax"));
            }
        }

        private decimal varMin;
        public decimal VarMin
        {
            get { return varMin; }
            set
            {
                varMin = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarMin"));
            }
        }

        private int paraIndex;
        public int ParaIndex
        {
            get { return paraIndex; }
            set
            {
                paraIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ParaIndex"));
            }
        }

        private int viewIndex;
        public int ViewIndex
        {
            get { return viewIndex; }
            set
            {
                viewIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ViewIndex"));
            }
        }
    }
}
