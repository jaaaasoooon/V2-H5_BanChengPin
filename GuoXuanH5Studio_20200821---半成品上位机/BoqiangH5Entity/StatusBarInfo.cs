using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5Entity
{
    public class StatusBarInfo : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string strOnlineStatus = "断开";

        public bool IsOnline { get; set; }
        public string OnlineStatus
        {
            get
            {
                return strOnlineStatus;
            }
            set
            {
                strOnlineStatus = value;
                if (this.PropertyChanged != null)     
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OnlineStatus"));
                }
            }

        }


        
    }
}
