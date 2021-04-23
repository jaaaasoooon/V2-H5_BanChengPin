using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5
{
    public class CustomRecvDataEventArgs
    {
        public CustomRecvDataEventArgs(List<byte> lsByte)
        {
            recvMsg = lsByte;
        }
        private List<byte> recvMsg;

        public List<byte> RecvMsg
        {
            get { return recvMsg; }
            set { recvMsg = value; }
        }
    }
}
