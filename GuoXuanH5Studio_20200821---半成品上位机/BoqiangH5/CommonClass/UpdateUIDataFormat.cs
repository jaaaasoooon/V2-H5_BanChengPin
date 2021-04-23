using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BoqiangH5Entity;

namespace BoqiangH5.CommonClass
{
    public class UpdateUIDataFormat
    {
        static SolidColorBrush brushRed = new SolidColorBrush(Color.FromArgb(255, 255, 150, 150));   

        static SolidColorBrush brushGreen = new SolidColorBrush(Color.FromArgb(255, 150, 255, 150)); 

        static SolidColorBrush brushWhite = new SolidColorBrush(Color.FromArgb(255, 250, 250, 250));

        static SolidColorBrush brushGray = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));

        public static string UpdateParaStringFormat(List<byte> listRecvData, H5BmsInfo nodeInfo)
        {
            string strRet = null;
            int nOffset = 2;   // 前两个字节为DID

            if (nodeInfo.Scale != "FF")
            {
                double dRecv = DataFormatConvert.GetIntData(listRecvData, nOffset, nodeInfo.RegisterNum);
                double dConver = DataFormatConvert.BytesToUIDataConverter(nodeInfo.Conversion, dRecv);

                if (nodeInfo.Scale != "1")
                    strRet = dConver.ToString();
                else if (nodeInfo.Scale != "0.1")
                    strRet = dConver.ToString("0.0");
                else if (nodeInfo.Scale != "0.01")
                    strRet = dConver.ToString("0.00");

            }
            else
            {
                byte[] btRecv = new byte[nodeInfo.RegisterNum];
                for (int k = 0; k < nodeInfo.RegisterNum; k++)
                {
                    btRecv[k] = listRecvData[nOffset + k];
                }

                strRet = DataFormatConvert.BytesToHexStr(btRecv);
            }

            return strRet;
        }

        public static void NoResponseUpdateBitStatus(IList<BitStatInfo> listBitStatus)
        { 
            foreach(var statusInfo in listBitStatus)
            {
                statusInfo.BackColor = brushGray;
            }        
        }
    }
}
