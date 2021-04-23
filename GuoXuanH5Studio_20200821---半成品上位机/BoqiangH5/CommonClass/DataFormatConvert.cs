using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoqiangH5Entity;



namespace BoqiangH5.CommonClass
{
    public class DataFormatConvert
    {

        public static string BytesToHexStr(byte[] bytes)
        {
            string strHex = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    strHex += bytes[i].ToString("X2") + " ";
                }
            }
            return strHex;
        }

        public static string ListToStr(List<byte> listByte)
        {
            if (listByte == null)
                return null;
            string strData = String.Empty;                    
            for (int n = 0; n < listByte.Count; n++)
            {
                strData += String.Format("{0:X2}", listByte[n]) + " ";
            }

            return strData;
        }


        public static int GetIntData(List<byte> listByteData, int start, int byteNum)
        {
            int tmpVal = 0;
            try
            {
                if (listByteData.Count == 0)
                    return 0;

                if (byteNum > 4)
                {
                    //throw new Exception("Not Supported");
                }

                for (int i = 0; i < byteNum; i++)
                {
                    tmpVal <<= 8;
                    tmpVal |= listByteData[start + i];
                }
            }
            catch (Exception e)
            {

            }
            return tmpVal;
        }


        public static double BytesToUIDataConverter(string strConvert, double dValue)
        {
            double dResult = 0;

            switch (strConvert)
            {
                case "y=x":
                case "y=±x":
                    dResult = dValue;
                    break;

                case "y=x+1":                     
                    dResult = dValue + 1;
                    break;

                case "y=x/10":
                case "y=±x/10":
                    dResult = dValue / 10;
                    break;

                case "y=x/100":
                    dResult = dValue / 100;
                    break;

                case "y=x/10.0*100":               
                    dResult = dValue / 10.0 * 100;
                    break;

                case "y=x*100":                   
                    dResult = dValue * 100;
                    break;

                default:
                    dResult = dValue;
                    break;

            }

            return dResult;
        }
            


    }
}
