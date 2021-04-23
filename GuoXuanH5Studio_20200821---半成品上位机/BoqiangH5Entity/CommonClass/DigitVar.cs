using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using UDSEntity;

namespace UDSStudio
{   
    public class DigitVar : INotifyPropertyChanged
    {
        private bool isTemperatureVar = false;
        
        public static void CopyTo( DigitVar desVar, DigitVar srcVar )
        {
            desVar.VarName = srcVar.VarName;
            desVar.VarUnit = srcVar.VarUnit;
            desVar.VarScale = srcVar.VarScale;
            desVar.VarByteNum = srcVar.VarByteNum;
            desVar.IsBalance = srcVar.IsBalance;
            desVar.IsAFE = srcVar.IsAFE;
            desVar.isTemperatureVar = srcVar.isTemperatureVar;
            desVar.VarPromptStatus = srcVar.VarPromptStatus;
            desVar.VarValue = srcVar.VarValue;         
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }


        public float ConvertFill(int rawValue, string para =null  )
        {
            float result;            

            if (this.varUnit == "V"&&this.VarName.Contains("电芯"))
            {               
                //bool balance=((rawValue&(1<<13))==1);
                bool balance = (((rawValue >> 14) & 0x01) == 1);
                this.IsBalance = balance;
                bool afe = (((rawValue >> 13) & 0x01) == 1);
                this.IsAFE = afe;

                rawValue = getVolt(rawValue);
            }

            if (rawValue >= 0x8000)
            {
                if (this.VarUnit == "A" || (isTemperatureVar && Temperature.GetTempType(this.VarUnit) != Temperature.TemperatureType.K))
                    result = -((~rawValue + 1) & 0xFFFF);
                else
                    result = rawValue;
            }
            if (rawValue>=128)
            {
                if (this.varUnit == "℃" || isTemperatureVar)
                    result = -((~rawValue + 1) & 0xFF);
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

        private int getVolt(int voltValue)
        {
            int result = 0;

            result = voltValue & 0x1FFF;
            return result;
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

        private float varValue;
        public float VarValue
        {
            get { return varValue; }
            set
            {
                varValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarValue"));
            }
        }

        private string varUnit;
        public string VarUnit
        {
            get { return varUnit; }
            set
            {
                varUnit = value;
                if( Temperature.GetTempType( varUnit )== Temperature.TemperatureType.NonTemp )
                    isTemperatureVar = false;
                else
                    isTemperatureVar = true;                                    

                OnPropertyChanged(new PropertyChangedEventArgs("VarUnit"));
            }
        }

        private float varScale = 0.01f;
        public float VarScale
        {
            get { return varScale; }
            set
            {
                varScale = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarScale"));
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

        private string varPromptStatus;
        public string VarPromptStatus
        {
            get { return varPromptStatus; }
            set
            {
                varPromptStatus = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarPromptStatus"));
            }
        }

        private Boolean isBalance;
        public Boolean IsBalance
        {
            get { return isBalance; }
            set
            {
                isBalance = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsBalance"));
            }
        }

        public Boolean isAFE;
        public Boolean IsAFE
        {
            get { return isAFE; }
            set
            {
                isAFE = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsAFE"));
            }
        }
    }
}
