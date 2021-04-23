using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace UDSStudio
{
    public class ParaBitVar : INotifyPropertyChanged
    {
        public ParaBitVar() { }

        public ParaBitVar(string name, int byte_index, int bit_index, ParaIntVar groupVar)
        {
            VarName = name;
            ByteIndex = byte_index;
            BitIndex = bit_index;
            GroupParaVar = groupVar;
            VarValue = false;//0;
        }
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

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

        //bool varValue = false;
        public bool VarValue
        {
            get
            {
                bool varValue;
                if (0 != (this.GroupParaVar.GroupData & (1 << this.bitIndex)))
                    varValue = true;
                else
                    varValue = false;
                return varValue;

            }
            set
            {
                bool varValue = value;
                int bitVal = (varValue == true) ? 1 : 0;
                int val = GroupParaVar.GroupData;
                val &= ~(1 << this.bitIndex);
                val |= (bitVal << this.bitIndex);
                GroupParaVar.GroupData = val;
                OnPropertyChanged(new PropertyChangedEventArgs("VarValue"));

                if (varValue == true)
                    VarPromptStatus = "NormalOn";
                else
                    VarPromptStatus = "NormalOff";
       
            }
        }

      
        private int byteIndex;
        public int ByteIndex
        {
            get { return byteIndex; }
            set
            {
                byteIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ByteIndex"));
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

        private ParaIntVar groupParaVar;
        public ParaIntVar GroupParaVar
        {
            get { return groupParaVar; }
            set
            {
                groupParaVar = value;
                OnPropertyChanged(new PropertyChangedEventArgs("GroupParaVar"));
            }
        }

    }

    public enum FunSwitchStatus { 开启, 关闭 };
}
