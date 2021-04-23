using BoqiangH5Entity;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls;
using BoqiangH5Repository;
using BoqiangH5.DDProtocol;
using System.Linq;
using System.Text;

namespace BoqiangH5
{
    public partial class UserCtrlDidiRecord :UserControl
    {
        public H5DidiRecordInfo UpdateDidiRecord(List<byte> rdBuf)
        {
            try
            {
                if (rdBuf[0] != 0x03)
                {
                    if (rdBuf[0] != 0xA7)
                    {
                        return null;
                    }
                    else
                    {
                        if (rdBuf.Count < 0x53)
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    if (rdBuf.Count < 0x48)
                    {
                        return null;
                    }
                }
                int offset = 2;
                H5DidiRecordInfo recordInfo = new H5DidiRecordInfo();
                if(rdBuf[0] == 0xA7)
                {
                    if(rdBuf[1] == 0x05 || rdBuf[1] == 0x06)
                    {
                        recordInfo.RecordType = "错误历史故障";
                    }
                    else
                    {
                        string value = (rdBuf[offset] & 0xFF).ToString("X2");
                        if (recordEventTypeDic.Keys.Contains(value))
                            recordInfo.RecordType = recordEventTypeDic[value];
                        else
                            recordInfo.RecordType = value;
                    }

                    offset += 1;
                    recordInfo.PackStatus = "0x" + (rdBuf[offset] << 8 | rdBuf[offset + 1]).ToString("X4"); offset += 2;
                    recordInfo.BatStatus = "0x" + (rdBuf[offset] << 8 | rdBuf[offset + 1]).ToString("X4"); offset += 2;
                    recordInfo.FCC = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                    recordInfo.LoopNumber = (uint)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                    offset += 2;
                }
                string year = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string month = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string day = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string hour = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string minute = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                string second = (rdBuf[offset] & 0xFF).ToString("X2"); offset += 1;
                recordInfo.RecordTime = string.Format("{0}年{1}月{2}日 {3}时{4}分{5}秒", "20" + year, month, day, hour, minute, second);
                recordInfo.TotalVoltage = (double)(rdBuf[offset + 3] | rdBuf[offset + 2] << 8 | rdBuf[offset + 1] << 16 | rdBuf[offset] << 24); offset += 4;
                recordInfo.Current = (rdBuf[offset + 3] | rdBuf[offset + 2] << 8 | rdBuf[offset + 1] << 16 | rdBuf[offset] << 24).ToString(); offset += 4;
                recordInfo.Cell1Temp = ((rdBuf[offset + 1] | rdBuf[offset] << 8) - 2731) / 10.0; offset += 2;
                recordInfo.Cell2Temp = ((rdBuf[offset + 1] | rdBuf[offset] << 8) - 2731) / 10.0; offset += 2;
                recordInfo.Cell3Temp = ((rdBuf[offset + 1] | rdBuf[offset] << 8) - 2731) / 10.0; offset += 2;
                recordInfo.Cell4Temp = ((rdBuf[offset + 1] | rdBuf[offset] << 8) - 2731) / 10.0; offset += 2;
                recordInfo.Cell5Temp = ((rdBuf[offset + 1] | rdBuf[offset] << 8) - 2731) / 10.0; offset += 2;
                recordInfo.Humidity = (rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
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
                //recordInfo.RC = (ulong)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                //recordInfo.FCC = (ulong)(rdBuf[offset + 1] | rdBuf[offset] << 8); offset += 2;
                offset += 4;
                offset += 1;
                recordInfo.DetStatus = (rdBuf[offset] & 0x04) == 0x04 ? "闭合" : "断开";
                recordInfo.DischargeMOSStatus = (rdBuf[offset] & 0x02) == 0x02 ? "闭合" : "断开";
                recordInfo.ChargeMOSStatus = (rdBuf[offset] & 0x01) == 0x01 ? "闭合" : "断开";
                offset += 1;
                recordInfo.SOC = (uint)rdBuf[offset]; offset += 2;
                //string val = (rdBuf[offset] << 24 | rdBuf[offset + 1] << 16 | rdBuf[offset + 2] << 8 | rdBuf[offset + 3]).ToString("X8"); offset += 4;
                //if (recordTypeDic.Keys.Contains(val))
                //    recordInfo.BatteryStatus = recordTypeDic[val];
                //else
                //    recordInfo.BatteryStatus = "0x" +  val;
                byte[] bytes = new byte[4] { rdBuf[offset], rdBuf[offset + 1], rdBuf[offset + 2], rdBuf[offset + 3] };
                offset += 4;
                recordInfo.BatteryStatus = GetDidiRecordType(bytes);
                recordInfo.Balance = "0x" + (rdBuf[offset] << 8 | rdBuf[offset + 1]).ToString("X4"); offset += 2;
                return recordInfo;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public int GetRecordCount(List<byte> rdBuf)
        {
            try
            {
                if (rdBuf[0] != 0x03 || rdBuf.Count != 0x08)
                {
                    return -1;
                }
                //int count = (rdBuf[2]<< 8 | rdBuf[3]);
                int count = rdBuf[2];
                return count;
            }
            catch(Exception ex)
            {
                return -1;
            }
        }

        string GetDidiRecordType(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            int offset = 0;
            if ((bytes[offset] & 0x20) == 0x20) sb.Append("系统复位；");
            if ((bytes[offset] & 0x10) == 0x10) sb.Append("系统关机；");
            if ((bytes[offset] & 0x08) == 0x08) sb.Append("系统开机；");
            if ((bytes[offset] & 0x04) == 0x04) sb.Append("非法充电；");
            if ((bytes[offset] & 0x02) == 0x02) sb.Append("总压过低保护；");
            if ((bytes[offset] & 0x01) == 0x01) sb.Append("总压过高保护；");
            offset++;
            if ((bytes[offset] & 0x80) == 0x80) sb.Append("MOS过温；");
            if ((bytes[offset] & 0x40) == 0x40) sb.Append("温差过大故障；");
            if ((bytes[offset] & 0x20) == 0x20) sb.Append("充电压差过大故障；");
            if ((bytes[offset] & 0x10) == 0x10) sb.Append("电芯掉线；");
            if ((bytes[offset] & 0x08) == 0x08) sb.Append("放电保护；");
            if ((bytes[offset] & 0x04) == 0x04) sb.Append("充电保护；");
            if ((bytes[offset] & 0x02) == 0x02) sb.Append("压差过大故障；");
            if ((bytes[offset] & 0x01) == 0x01) sb.Append("压差过大告警；");
            offset++;
            if ((bytes[offset] & 0x80) == 0x80) sb.Append("电芯故障；");
            if ((bytes[offset] & 0x40) == 0x40) sb.Append("放电MOS故障；");
            if ((bytes[offset] & 0x20) == 0x20) sb.Append("充电MOS故障；");
            if ((bytes[offset] & 0x10) == 0x10) sb.Append("电芯严重过压保护；");
            if ((bytes[offset] & 0x08) == 0x08) sb.Append("电芯过压二级保护；");
            if ((bytes[offset] & 0x04) == 0x04) sb.Append("电芯过压一级保护；");
            if ((bytes[offset] & 0x02) == 0x02) sb.Append("充电低温；");
            if ((bytes[offset] & 0x01) == 0x01) sb.Append("充电过温；");
            offset++;
            if ((bytes[offset] & 0x80) == 0x80) sb.Append("充电过流；");
            if ((bytes[offset] & 0x40) == 0x40) sb.Append("二级欠压保护；");
            if ((bytes[offset] & 0x20) == 0x20) sb.Append("欠压保护；");
            if ((bytes[offset] & 0x10) == 0x10) sb.Append("短路保护；");
            if ((bytes[offset] & 0x08) == 0x08) sb.Append("放电低温；");
            if ((bytes[offset] & 0x04) == 0x04) sb.Append("放电过温；");
            if ((bytes[offset] & 0x02) == 0x02) sb.Append("放电二级过流；");
            if ((bytes[offset] & 0x01) == 0x01) sb.Append("放电一级过流；");

            if (sb.Length == 0)
                sb.Append("0x" + (bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3]).ToString("X8"));
            return sb.ToString().Trim('；');
        }
    }
}
