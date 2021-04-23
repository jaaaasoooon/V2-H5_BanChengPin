using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using UDSEntity;

namespace UDSStudio
{


    public class StateVar : INotifyPropertyChanged
    {
        public StateVar() { }

        public StateVar(string name, int byte_index, int bit_index, string type = "Normal",
                        DigitVar linkDigit=null, CtrlVar linkCtrl=null )
        {
            StateName = name;
            StateType = type;
            ByteIndex = byte_index;
            BitIndex = bit_index;

            StateValue = false;

            LinkCtrlVar = linkCtrl;
            LinkDigitVar = linkDigit;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private string stateName;
        public string StateName
        {
            get { return stateName; }
            set
            {
                stateName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StateName"));
            }
        }

        private Boolean stateValue;
        public Boolean StateValue
        {
            get { return stateValue; }
            set
            {
                stateValue = value;                                
                OnPropertyChanged(new PropertyChangedEventArgs("StateValue"));


                if (StateType == "Warn")
                    StateText = stateValue ? "报警" : "无";
                else if (StateType == "Protect")
                    StateText = stateValue ? "保护" : "无";
                else if (StateType == "Normal")
                    StateText = stateValue ? "开启" : "关闭";
                else
                    StateText = stateValue ? "ON" : "OFF";
                VarPromptStatus = stateType + (stateValue ? "On" : "Off");
                OnPropertyChanged(new PropertyChangedEventArgs("VarPromptStatus"));
            }
        }

        private string stateText;
        public string StateText
        {
            get { return stateText; }
            set
            {
                stateText = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StateText"));
            }
        }

        private string stateType;
        public string StateType
        {
            get { return stateType; }
            set
            {
                stateType = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("StateType"));
            }
        }

        private int byteIndex;
        public int ByteIndex
        {
            get { return byteIndex; }
            set
            {
                byteIndex = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("ByteIndex"));
            }
        }

        private int bitIndex;
        public int BitIndex
        {
            get { return bitIndex; }
            set
            {
                bitIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BitIndex"));
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

        private DigitVar linkDigitVar;
        public DigitVar LinkDigitVar
        {
            get { return linkDigitVar; }
            set { linkDigitVar = value; }
        }

        private CtrlVar linkCtrlVar;
        public CtrlVar LinkCtrlVar
        {
            get { return linkCtrlVar; }
            set { linkCtrlVar = value; }
        }

    }


}