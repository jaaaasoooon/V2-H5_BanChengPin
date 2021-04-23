using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5
{
    public class CRC_Check
    {

        public static byte[] CRC16(byte[] data,int nStart,int len ) //(string sInputString)
           {
               if (len > 0)
               {
                   ushort crc = 0xFFFF;

                   for (int i = nStart; i < len; i++)
                  {
                      crc = (ushort)(crc ^ (data[i]));
                      for (int j = 0; j < 8; j++)
                      {
                          crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                      }
                 }
                  byte hi = (byte)((crc & 0xFF00) >> 8); 
                  byte lo = (byte)(crc & 0x00FF);         
 
                  return new byte[] { hi, lo };
              }
              return new byte[] { 0, 0 };
          }
 


    }
}
