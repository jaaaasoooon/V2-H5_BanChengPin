using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using BoqiangH5Entity;

namespace BoqiangH5
{
    public partial class UserCtrlProtectParam : UserControl
    {
        public void UpdateVoltageParam(List<byte> rdBuf,List<H5ProtectParamInfo> list,byte count,byte frameTitle)
        {
            if (rdBuf.Count < count || rdBuf[0] != frameTitle)
            {
                return;
            }

            int offset = 1;
            byte[] array = rdBuf.ToArray();
            bool isLittleEndian = BitConverter.IsLittleEndian;
            
            for (int n = 0; n < list.Count; n++)
            {
                //int paramVal = 0;
                //for (int m = 0; m < list[n].ByteCount; m++)
                //{
                //    paramVal = ((paramVal) << 8 | (rdBuf[offset + m] & 0xFF));
                //}
                //list[n].StrValue = paramVal.ToString();
                byte[] bytes = new byte[list[n].ByteCount];
                Buffer.BlockCopy(array, offset, bytes, 0, bytes.Length);
                if(BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }
                if (bytes.Length == 2)
                {
                    if(list[n].isUnsigned)
                    {
                        ushort paramVal = BitConverter.ToUInt16(bytes, 0);
                        list[n].StrValue = paramVal.ToString();
                    }
                    else
                    {
                        short paramVal = BitConverter.ToInt16(bytes, 0);
                        list[n].StrValue = paramVal.ToString();
                    }

                }
                else
                {
                    if(list[n].isUnsigned)
                    {
                        uint paramVal = BitConverter.ToUInt32(bytes, 0);
                        list[n].StrValue = paramVal.ToString();
                    }
                    else
                    {
                        int paramVal = BitConverter.ToInt32(bytes, 0);
                        list[n].StrValue = paramVal.ToString();
                    }
                }
                offset += list[n].ByteCount;
            }
        }

        public void UpdateCurrentParam(List<byte> rdBuf)
        {
            if (rdBuf.Count < 0x18 || rdBuf[0] != 0xAB)
            {
                return;
            }

            int byteIndex = 1;
            for (int n = 0; n < m_CurrentParamList.Count; n++)
            {
                int paramVal = 0;
                for (int m = 0; m < m_CurrentParamList[n].ByteCount; m++)
                {
                    paramVal = (paramVal << 8 | rdBuf[byteIndex + m]);
                }

                m_CurrentParamList[n].StrValue = paramVal.ToString();

                byteIndex += m_CurrentParamList[n].ByteCount;
            }
        }
    }
}
