using BoqiangH5.BQProtocol;
using BoqiangH5Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace BoqiangH5
{
    public partial class UserCtrlRecord : UserControl
    {

        public H5RecordInfo ReadRecordInfo(List<byte> rdBuf)
        {
            try
            {
                if (rdBuf[0] != 0xA6 || rdBuf.Count < 0x4C)
                {
                    return null;
                }

                BqProtocol.bReadBqBmsResp = true;
                int offset = 1;
                H5RecordInfo recordInfo = new H5RecordInfo();
                recordInfo.RecordInfo = rdBuf[offset]; offset += 1;
                string second = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string minute = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string hour = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string week = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string day = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string month = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string year = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;

                recordInfo.RecordTime = string.Format("{0}年{1}月{2}日 {3}时{4}分{5}秒", "20" + year, month, day, hour, minute, second);
                string val = (rdBuf[offset] & 0xFF).ToString("X2");
                if (recordTypeDic.Keys.Contains(val))
                    recordInfo.RecordType = recordTypeDic[val];
                else
                    recordInfo.RecordType = val;
                offset += 1;
                recordInfo.PackStatus = "0x" + (rdBuf[offset] << 8 | rdBuf[offset + 1]).ToString("X4"); offset += 2;
                recordInfo.BatteryStatus = "0x" + (rdBuf[offset] << 8 | rdBuf[offset + 1]).ToString("X4"); offset += 2;
                recordInfo.FCC = (ulong)(rdBuf[offset + 3] | rdBuf[offset + 2] << 8 | rdBuf[offset + 1] << 16 | rdBuf[offset] << 24); offset += 4;
                recordInfo.RC = (ulong)(rdBuf[offset + 3] | rdBuf[offset + 2] << 8 | rdBuf[offset + 1] << 16 | rdBuf[offset] << 24); offset += 4;
                recordInfo.SOC = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell1Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell2Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell3Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell4Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell5Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell6Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell7Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell8Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell9Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell10Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell11Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell12Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell13Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell14Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell15Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                recordInfo.Cell16Voltage = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
   
                recordInfo.TotalVoltage =(double)(rdBuf[offset + 3] | rdBuf[offset + 2] << 8 | rdBuf[offset + 1] << 16 | rdBuf[offset] << 24)/ 1000.0; offset += 4;
                int current = 0;
                for (int m = 0; m < 4; m++)
                {
                    current = (current << 8 | rdBuf[offset + m]);
                }
                recordInfo.Current = current.ToString();
                offset += 4;
                recordInfo.AmbientTemp = ((rdBuf[offset + 1] | rdBuf[offset] << 8) - 2731) / 10.0; offset += 2;
                recordInfo.Cell1Temp = ((rdBuf[offset + 1] | rdBuf[offset] << 8) -2731) / 10.0; offset += 2;
                recordInfo.Cell2Temp = ((rdBuf[offset + 1] | rdBuf[offset] << 8) - 2731) / 10.0; offset += 2;
                int temp = rdBuf[offset] & 0xFF;
                recordInfo.Cell3Temp = (temp > 127) ? temp - 256 : temp; offset += 1;
                temp = rdBuf[offset] & 0xFF;
                recordInfo.Cell4Temp = (temp > 127) ? temp - 256 : temp; offset += 1;
                temp = rdBuf[offset] & 0xFF;
                recordInfo.Cell5Temp = (temp > 127) ? temp - 256 : temp; offset += 1;
                temp = rdBuf[offset] & 0xFF;
                recordInfo.Cell6Temp = (temp > 127) ? temp - 256 : temp; offset += 1;
                temp = rdBuf[offset] & 0xFF;
                recordInfo.Cell7Temp = (temp > 127) ? temp - 256 : temp; offset += 1;
                temp = rdBuf[offset] & 0xFF;
                recordInfo.PowerTemp = (temp > 127) ? temp - 256 : temp; offset += 1;

                return recordInfo;
            }
            catch (Exception ex) 
            { 
                return null; 
            }

        }
    }

}
