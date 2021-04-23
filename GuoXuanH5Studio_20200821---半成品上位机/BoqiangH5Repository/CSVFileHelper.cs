using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BoqiangH5Entity;

namespace BoqiangH5Repository
{
    public class CSVFileHelper
    {

        /// <summary>
        /// 将数据写入到CSV文件中
        /// </summary>
        /// <param name="listData">提供保存数据的list</param>
        /// <param name="path">CSV的文件路径</param>
        public static void SaveCSV(List<string> listData, string path)
        {
            bool isCreate = false;
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                FileInfo fi = new FileInfo(path);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();  
                }

                if (!File.Exists(path))
                {
                    fs = File.Create(path);//创建该文件
                    isCreate = true;
                }
                else
                {
                    fs = new FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                }
      
                sw = new StreamWriter(fs, System.Text.Encoding.Default);

                //写出列名称
                if (isCreate)
                {
                    string strColumnsName = "序号,时间,收发,ID,CAN数据类型,协议数据类型,服务类型，数据";
                    sw.WriteLine(strColumnsName);
                }

                //写出各行数据
                if (null != listData)
                {
                    for (int i = 0; i < listData.Count; i++)
                    {
                        sw.WriteLine(listData[i], false);
                    }
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                if (null != sw)
                    sw.Close();
                if (null != fs)
                    fs.Close();
            }

        }

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string filePath)
        {
            DataTable dt = new DataTable();
            FileStream fs = null;
            StreamReader sr = null;

            try
            {
                Encoding encoding = System.Text.Encoding.Default; // System.Text.Encoding.GetEncoding(936); // System.Data.Common.GetType(filePath); //Encoding.ASCII;//
                
                fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                sr = new StreamReader(fs, encoding);
            
                string strLine = "";
          
                string[] aryLine = null;
                string[] tableHead = null;
    
                int columnCount = 10;
          
                bool IsFirst = true;
     
                while ((strLine = sr.ReadLine()) != null)
                {

                    if (IsFirst == true)
                    {
                        tableHead = strLine.Split(',');
                        IsFirst = false;
                        columnCount = tableHead.Length;
                        //创建列
                        for (int i = 0; i < columnCount; i++)
                        {
                            DataColumn dc = new DataColumn(tableHead[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        aryLine = strLine.Split(',');
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                        {
                            dr[j] = aryLine[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                if (aryLine != null && aryLine.Length > 0)
                {
                    dt.DefaultView.Sort = tableHead[0] + " " + "asc";
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (null != sr)
                    sr.Close();
                if (null != fs)
                    fs.Close();
            }
            return dt;
        }


        /// <summary>
        /// 读取Excel文件到DataSet中
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static DataSet LoadExcel(string filePath)
        {
            string connStr = "";
            string fileType = System.IO.Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(fileType)) return null;

            if (fileType == ".xls")
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            else
                connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
            string sql_F = "Select * FROM [{0}]";

            OleDbConnection conn = null;
            OleDbDataAdapter da = null;
            System.Data.DataTable dtSheetName = null;

            DataSet ds = new DataSet();
            try
            {
                conn = new OleDbConnection(connStr);
                conn.Open();
                    
                string SheetName = "";
                dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                // 初始化适配器
                da = new OleDbDataAdapter();
                //for (int i = 0; i < dtSheetName.Rows.Count; i++)
                {
                    SheetName = (string)dtSheetName.Rows[0]["TABLE_NAME"];
                    
                    if (SheetName.Contains("$") && !SheetName.Replace("'", "").EndsWith("$"))
                    {
                        //continue;
                    }

                    da.SelectCommand = new OleDbCommand(String.Format(sql_F, SheetName), conn);
                    DataSet dsItem = new DataSet();
                   
                    da.Fill(dsItem);

                    ds.Tables.Add(dsItem.Tables[0].Copy());
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                // 关闭连接
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    da.Dispose();
                    conn.Dispose();
                }
            }
            return ds;
        }


        /// <summary>
        /// 将条码保存在CSV文件中   
        /// </summary>
        public static void SaveSNInfomation(string path, string sn ,string software,string hardware,List<H5BmsInfo> bmsInfo,List<H5BmsInfo> cellInfo)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            bool isCreate = false;
            try
            {
                if (!File.Exists(path))
                {
                    fs = File.Create(path);//创建该文件
                    isCreate = true;
                }
                else
                {
                    fs = new FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                }
                sw = new StreamWriter(fs, System.Text.Encoding.Default);
                if(isCreate)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("测试时间,");
                    sb.Append("条码,");
                    sb.Append("软件版本号,");
                    sb.Append("硬件版本号,");
                    foreach (var item in bmsInfo)
                    {
                        sb.Append(string.Format("{0}({1})", item.Description, item.Unit));
                        sb.Append(",");
                        if (item.Description == "SOC")
                        {
                            sb.Append("总电压(mV)");
                            sb.Append(",");
                            sb.Append("实时电流(mA)");
                            sb.Append(",");
                        }
                    }

                    foreach (var item in cellInfo)
                    {
                        if (item.Description == "总电压" || item.Description == "实时电流")
                        {
                            continue;
                        }
                        sb.Append(string.Format("{0}({1})", item.Description, item.Unit));
                        sb.Append(",");
                    }
                    sw.WriteLine(sb.ToString());
                }

                if(!string.IsNullOrEmpty(sn))
                {
                    string str = System.DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
                    string strLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50}",
                            System.DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"), sn, software, hardware, bmsInfo[0].StrValue, bmsInfo[1].StrValue, bmsInfo[2].StrValue, bmsInfo[3].StrValue, bmsInfo[4].StrValue,
                            bmsInfo[5].StrValue, cellInfo[16].StrValue, cellInfo[17].StrValue, bmsInfo[6].StrValue, bmsInfo[7].StrValue, bmsInfo[8].StrValue, bmsInfo[9].StrValue, bmsInfo[10].StrValue, bmsInfo[11].StrValue,
                            bmsInfo[12].StrValue, bmsInfo[13].StrValue, bmsInfo[14].StrValue, bmsInfo[15].StrValue, bmsInfo[16].StrValue, bmsInfo[17].StrValue, bmsInfo[18].StrValue,
                            bmsInfo[19].StrValue, bmsInfo[20].StrValue, bmsInfo[21].StrValue, bmsInfo[22].StrValue, bmsInfo[23].StrValue, bmsInfo[24].StrValue,
                            bmsInfo[25].StrValue, bmsInfo[26].StrValue, bmsInfo[27].StrValue, bmsInfo[28].StrValue, cellInfo[0].StrValue,
                            cellInfo[1].StrValue, cellInfo[2].StrValue, cellInfo[3].StrValue, cellInfo[4].StrValue, cellInfo[5].StrValue, cellInfo[6].StrValue, cellInfo[7].StrValue,
                            cellInfo[8].StrValue, cellInfo[9].StrValue, cellInfo[10].StrValue, cellInfo[11].StrValue, cellInfo[12].StrValue, cellInfo[13].StrValue, cellInfo[14].StrValue, cellInfo[15].StrValue);
                    sw.WriteLine(strLine);
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                if (null != sw)
                    sw.Close();
                if (null != fs)
                    fs.Close();
            }

        }

        /// <summary>
        /// 读条码 
        /// </summary>
        public static List<string> ReadSN(string path)
        {
            FileStream fs = null;
            StreamReader sr = null;
            List<string> list = new List<string>();
            try
            {
                if (!File.Exists(path))
                {
                    return list;
                }
                else
                {
                    fs = new FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                }
                sr = new StreamReader(fs, System.Text.Encoding.Default);
                bool isfirst = true;
                while(!sr.EndOfStream)
                {
                    string valLine = sr.ReadLine();
                    string[] str = valLine.Split(',');
                    if (!isfirst)
                        list.Add(str[1]);
                    isfirst = false;
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                if (null != sr)
                    sr.Close();
                if (null != fs)
                    fs.Close();

            }
            return list;
        }

        /// <summary>
        /// 写BMS数据或单体数据CSV文件标题             lipeng   2020.3.26,增加实时信息记录
        /// </summary>
        /// <param name="listData">提供保存数据的list</param>
        /// <param name="path">CSV的文件路径</param>
        public static void SaveBmsORCellCSVTitle(string path,bool isBqProtocol,List<H5BmsInfo> listBMS, List<H5BmsInfo> listCell, List<H5BmsInfo> listDevice)
        {
            StreamWriter sw = null;
            FileStream fs = null;
            try
            {
                if (File.Exists(path))
                {
                    fs = new FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    sw = new StreamWriter(fs, System.Text.Encoding.Default);

                    if(isBqProtocol == true)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("测试时间,");
                        foreach (var item in listBMS)
                        {
                            sb.Append(string.Format("{0}({1})",item.Description,item.Unit));
                            sb.Append(",");
                            if(item.Description == "SOC")
                            {
                                sb.Append("总电压(mV)");
                                sb.Append(",");
                                sb.Append("实时电流(mA)");
                                sb.Append(",");
                            }
                        }

                        foreach (var item in listCell)
                        {
                            if(item.Description == "总电压" || item.Description == "实时电流")
                            {
                                continue;
                            }
                            sb.Append(string.Format("{0}({1})", item.Description, item.Unit));
                            sb.Append(",");
                        }
                        /*sb.Append("环境温度,");
                        sb.Append("电芯温度2,");
                        sb.Append("电芯温度3,");
                        sb.Append("满充容量,");
                        sb.Append("剩余电量,");
                        sb.Append("SOC,");
                        sb.Append("循环放电次数,");
                        sb.Append("Pack状态,");
                        sb.Append("电池状态,");
                        sb.Append("Pack配置,");
                        sb.Append("制造信息,");
                        sb.Append("电芯温度4,");
                        sb.Append("电芯温度5,");
                        sb.Append("电芯温度6,");
                        sb.Append("电芯温度7,");
                        sb.Append("湿度,");
                        sb.Append("功率温度,");
                        sb.Append("AFE状态,");
                        sb.Append("最高电压,");
                        sb.Append("最高电压单体号,");
                        sb.Append("最低电压,");
                        sb.Append("最低电压单体号,");
                        sb.Append("单体最大温度,");
                        sb.Append("单体最小温度,");
                        sb.Append("均衡状态,");
                        sb.Append("RTC通讯状态,");
                        sb.Append("EEPROM通讯状态,");
                        sb.Append("最大压差,");
                        sb.Append("电芯01电压,");
                        sb.Append("电芯02电压,");
                        sb.Append("电芯03电压,");
                        sb.Append("电芯04电压,");
                        sb.Append("电芯05电压,");
                        sb.Append("电芯06电压,");
                        sb.Append("电芯07电压,");
                        sb.Append("电芯08电压,");
                        sb.Append("电芯09电压,");
                        sb.Append("电芯10电压,");
                        sb.Append("电芯11电压,");
                        sb.Append("电芯12电压,");
                        sb.Append("电芯13电压,");
                        sb.Append("电芯14电压,");
                        sb.Append("电芯15电压,");
                        sb.Append("电芯16电压,");
                        sb.Append("总电压,");
                        sb.Append("实时电流,");*/
                        sw.WriteLine(sb.ToString());
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("测试时间,");
                        //描述中有逗号，需做特殊处理
                        //sb.Append("电池组健康百分比SOH,");
                        //sb.Append("BMS是否持续输出电压,");
                        //sb.Append("BMS输出延时");
                        //sb.Append("电池组内部温度/电池表面温度,");
                        //sb.Append("电池组总电压,");
                        //sb.Append("电池实时电流,");
                        //sb.Append("电池相对容量百分比RSOC,");
                        //sb.Append("电池绝对容量百分比ASOC,");
                        //sb.Append("电池剩余容量RSCAP,");
                        //sb.Append("电池满电容量,");
                        //sb.Append("电池循环次数,");
                        //sb.Append("保留");
                        //sb.Append("电池高16组每节电池电压,");
                        //sb.Append("电池低16组每节电池电压");
                        //sb.Append("电池当前充电间隔时间,");
                        //sb.Append("电池最大充电间隔时间,");
                        //sb.Append("电池允许最大放电电流,");
                        //sb.Append("电池组允许的最大充电电压,");
                        //sb.Append("电池允许最大充电电流,");
                        //sb.Append("一级过放保护电压阈值,");
                        //sb.Append("保留,");
                        //sb.Append("电池状态信息,");
                        //sb.Append("电芯型号,");
                        //sb.Append("电池组内部实时时钟RTC,");
                        //sb.Append("电池设计容量 (毫安时),");
                        //sb.Append("电池累计放电安时数 (毫安时),");
                        //sb.Append("电池组内部温度，第二组电芯温度探测点温度,");
                        //sb.Append("电池组内部温度，第三组电芯温度探测点温度,");
                        //sb.Append("电池组内部温度，第四组电芯温度探测点温度,");
                        //sb.Append("电池组内部温度，MOS管温度,");
                        //sb.Append("电池内部湿度百分比,");
                        //sb.Append("SOP，高16位电压值，低16位电流值,");
                        //sb.Append("FCC,");
                        //sb.Append("累计充电能量,");
                        //sb.Append("累计放电能量,");
                        //sb.Append("绝缘电阻,");
                        foreach (var item in listBMS)
                        {
                            sb.Append(string.Format("{0}({1})", item.Description, item.Unit));
                            sb.Append(",");
                        }
                        foreach (var item in listCell)
                        {
                            sb.Append(string.Format("{0}({1})", item.Description, item.Unit));
                            sb.Append(",");
                        }
                        //foreach (var item in listDevice)
                        //{
                        //    sb.Append(item.Description);
                        //    sb.Append(",");
                        //}
                        /*sb.Append("电池组健康百分比SOH,");
                        sb.Append("BMS是否持续输出电压,");
                        sb.Append("BMS输出延时");
                        sb.Append("电池组内部温度/电池表面温度,");
                        sb.Append("电池组总电压,");
                        sb.Append("电池实时电流,");
                        sb.Append("电池相对容量百分比RSOC,");
                        sb.Append("电池绝对容量百分比ASOC,");
                        sb.Append("电池剩余容量RSCAP,");
                        sb.Append("电池满电容量,");
                        sb.Append("电池循环次数,");
                        sb.Append("保留");
                        sb.Append("电池高16组每节电池电压,");
                        sb.Append("电池低16组每节电池电压");
                        sb.Append("电池当前充电间隔时间,");
                        sb.Append("电池最大充电间隔时间,");
                        sb.Append("电池允许最大放电电流,");
                        sb.Append("电池组允许的最大充电电压,");
                        sb.Append("电池允许最大充电电流,");
                        sb.Append("一级过放保护电压阈值,");
                        sb.Append("保留,");
                        sb.Append("电池状态信息,");
                        sb.Append("电芯型号,");
                        sb.Append("电池组内部实时时钟RTC,");
                        sb.Append("电池设计容量 (毫安时),");
                        sb.Append("电池累计放电安时数 (毫安时),");
                        sb.Append("电池组内部温度，第二组电芯温度探测点温度,");
                        sb.Append("电池组内部温度，第三组电芯温度探测点温度,");
                        sb.Append("电池组内部温度,第四组电芯温度探测点温度,");
                        sb.Append("电池组内部温度，MOS管温度,");
                        sb.Append("电池内部湿度百分比,");
                        sb.Append("SOP，高16位电压值，低16位电流值,");
                        sb.Append("FCC,");
                        sb.Append("累计充电能量,");
                        sb.Append("累计放电能量,");
                        sb.Append("绝缘电阻,");
                        sb.Append("高16组 电芯1电压,");
                        sb.Append("高16组 电芯2电压,");
                        sb.Append("高16组 电芯3电压,");
                        sb.Append("高16组 电芯4电压,");
                        sb.Append("高16组 电芯5电压,");
                        sb.Append("高16组 电芯6电压,");
                        sb.Append("高16组 电芯7电压,");
                        sb.Append("高16组 电芯8电压,");
                        sb.Append("高16组 电芯9电压,");
                        sb.Append("高16组 电芯10电压,");
                        sb.Append("高16组 电芯11电压,");
                        sb.Append("高16组 电芯12电压,");
                        sb.Append("高16组 电芯13电压,");
                        sb.Append("高16组 电芯14电压,");
                        sb.Append("高16组 电芯15电压,");
                        sb.Append("高16组 电芯16电压,");
                        sb.Append("设备类型,");
                        sb.Append("固件版本号,");
                        sb.Append("硬件版本号,");
                        sb.Append("制造厂信息,");
                        sb.Append("设备SN号,");
                        sb.Append("硬件型号编号.客户型号编号,");
                        sb.Append("固件版本号,");*/
                        sw.WriteLine(sb.ToString());
                    }
                }

            }
            catch (Exception ex)
            { }
            finally
            {
                if (null != sw)
                    sw.Close();
                if (null != fs)
                    fs.Close();
            }

        }

        public static void SaveRecordDataCSV(System.Collections.ObjectModel.ObservableCollection<H5RecordInfo> listData, string path)
        {
            bool isCreate = false;
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                FileInfo fi = new FileInfo(path);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }

                if (!File.Exists(path))
                {
                    fs = File.Create(path);//创建该文件
                    isCreate = true;
                }
                else
                {
                    fs = new FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                }

                sw = new StreamWriter(fs, System.Text.Encoding.Default);

                //写出列名称
                if (isCreate)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("序号,");
                    sb.Append("时间,");
                    sb.Append("记录类型,");
                    sb.Append("Pack状态,");
                    sb.Append("电池状态,");
                    sb.Append("满充容量(mAh),");
                    sb.Append("剩余容量(mAh),");
                    sb.Append("SOC(%),");
                    sb.Append("Cell1(mV),");
                    sb.Append("Cell2(mV),");
                    sb.Append("Cell3(mV),");
                    sb.Append("Cell4(mV),");
                    sb.Append("Cell5(mV),");
                    sb.Append("Cell6(mV),");
                    sb.Append("Cell7(mV),");
                    sb.Append("Cell8(mV),");
                    sb.Append("Cell9(mV),");
                    sb.Append("Cell10(mV),");
                    sb.Append("Cell11(mV),");
                    sb.Append("Cell12(mV),");
                    sb.Append("Cell13(mV),");
                    sb.Append("Cell14(mV),");
                    sb.Append("Cell15(mV),");
                    sb.Append("Cell16(mV),");
                    sb.Append("总压(V),");
                    sb.Append("电流(mA),");
                    sb.Append("环境温度(℃),");
                    sb.Append("温度保留1(℃),");
                    sb.Append("温度保留2(℃),");
                    sb.Append("电芯温度1(℃),");
                    sb.Append("电芯温度2(℃),");
                    sb.Append("电芯温度3(℃),");
                    sb.Append("电芯温度4(℃),");
                    sb.Append("湿度(%),");
                    sb.Append("功率温度(℃)");
                    sw.WriteLine(sb.ToString());
                }

                //写出各行数据
                if (null != listData)
                {
                    foreach(var item in listData)
                    {
                        string strLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34}",
                            item.Index, item.RecordTime, item.RecordType, item.PackStatus, item.BatteryStatus, item.FCC, item.RC, item.SOC, item.Cell1Voltage, item.Cell2Voltage,
                            item.Cell3Voltage, item.Cell4Voltage, item.Cell5Voltage, item.Cell6Voltage, item.Cell7Voltage, item.Cell8Voltage, item.Cell9Voltage, item.Cell10Voltage, 
                            item.Cell11Voltage, item.Cell12Voltage, item.Cell13Voltage, item.Cell14Voltage, item.Cell15Voltage, item.Cell16Voltage, item.TotalVoltage, item.Current, 
                            item.AmbientTemp, item.Cell1Temp, item.Cell2Temp,item.Cell3Temp, item.Cell4Temp,item.Cell5Temp,item.Cell6Temp,item.Cell7Temp,item.PowerTemp);
                        sw.WriteLine(strLine);
                    }
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                if (null != sw)
                    sw.Close();
                if (null != fs)
                    fs.Close();
            }

        }

        public static void SaveDdRecordDataCSV(System.Collections.ObjectModel.ObservableCollection<H5DidiRecordInfo> listData, string path,bool isReadAll,string uid,List<string> list,string mcuMsg)
        {
            bool isCreate = false;
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                FileInfo fi = new FileInfo(path);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }

                if (!File.Exists(path))
                {
                    fs = File.Create(path);//创建该文件
                    isCreate = true;
                }
                else
                {
                    fs = new FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                }

                sw = new StreamWriter(fs, System.Text.Encoding.Default);

                sw.WriteLine(string.Format("UID：{0}", uid));
                sw.WriteLine(string.Format("设备类型：{0}", list[0]));
                sw.WriteLine(string.Format("制造厂信息：{0}", list[3]));
                sw.WriteLine(string.Format("设备SN号：{0}", list[4]));
                sw.WriteLine(string.Format("硬件型号编号.客户型号编号：{0}", list[5]));
                sw.WriteLine(string.Format("固件版本号：{0}", list[6]));
                sw.WriteLine(string.Format("硬件版本号：{0}", list[7]));
                sw.WriteLine(string.Format("当前程序状态：{0}", list[8]));
                string[] msgs = mcuMsg.Split('$');
                if(msgs.Count() < 3)
                {
                    sw.WriteLine(string.Format("软件版本号：{0}", mcuMsg));
                    sw.WriteLine(string.Format("硬件版本号：{0}", mcuMsg));
                    sw.WriteLine(string.Format("生产日期：{0}", mcuMsg));
                }
                else
                {
                    sw.WriteLine(string.Format("软件版本号：{0}", msgs[0]));
                    sw.WriteLine(string.Format("硬件版本号：{0}", msgs[1]));
                    sw.WriteLine(string.Format("生产日期：{0}", msgs[2]));
                }
                sw.WriteLine(string.Empty);
                //写出列名称
                if (isCreate)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("序号,");
                    sb.Append("时间,");
                    if(isReadAll)
                    {
                        sb.Append("记录类型(BQ),");
                        sb.Append("Pack状态(BQ),");
                        sb.Append("电池状态(BQ),");
                        sb.Append("FCC(mAh),");
                        sb.Append("循环次数,");
                    }
                    sb.Append("总压(mV),");
                    sb.Append("电流(mA),");
                    sb.Append("SOC(%),");
                    sb.Append("电池组状态,");
                    sb.Append("充电MOS状态,");
                    sb.Append("放电MOS状态,");
                    sb.Append("DET状态,");
                    //sb.Append("剩余容量(mAh),");
                    //sb.Append("满充容量(mAh),");
                    //sb.Append("循环次数,");
                    sb.Append("均衡,");
                    sb.Append("电芯温度1(℃),");
                    sb.Append("电芯温度2(℃),");
                    sb.Append("MOS温度(℃),");
                    sb.Append("电芯温度3(℃),");
                    sb.Append("电芯温度4(℃),");
                    sb.Append("Cell1(mV),");
                    sb.Append("Cell2(mV),");
                    sb.Append("Cell3(mV),");
                    sb.Append("Cell4(mV),");
                    sb.Append("Cell5(mV),");
                    sb.Append("Cell6(mV),");
                    sb.Append("Cell7(mV),");
                    sb.Append("Cell8(mV),");
                    sb.Append("Cell9(mV),");
                    sb.Append("Cell10(mV),");
                    sb.Append("Cell11(mV),");
                    sb.Append("Cell12(mV),");
                    sb.Append("Cell13(mV),");
                    sb.Append("Cell14(mV),");
                    sb.Append("Cell15(mV),");
                    sb.Append("Cell16(mV),");
                    sb.Append("湿度(%)");
                    sw.WriteLine(sb.ToString());
                }

                //写出各行数据
                if (null != listData)
                {
                    foreach (var item in listData)
                    {
                        string strLine = string.Empty;
                        if(isReadAll)
                        {
                            strLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36}",
                                        item.Index, item.RecordTime,item.RecordType,item.PackStatus,item.BatStatus, item.FCC,item.LoopNumber,item.TotalVoltage, item.Current, item.SOC, item.BatteryStatus, item.ChargeMOSStatus, item.DischargeMOSStatus, item.DetStatus, 
                                        item.Balance, item.Cell1Temp, item.Cell2Temp, item.Cell3Temp, item.Cell4Temp, item.Cell5Temp, item.Cell1Voltage, item.Cell2Voltage, item.Cell3Voltage, item.Cell4Voltage, item.Cell5Voltage, item.Cell6Voltage, item.Cell7Voltage, item.Cell8Voltage, item.Cell9Voltage,
                                        item.Cell10Voltage, item.Cell11Voltage, item.Cell12Voltage, item.Cell13Voltage, item.Cell14Voltage, item.Cell15Voltage, item.Cell16Voltage,
                                        item.Humidity);
                        }
                        else
                        {
                            strLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32}",
                                        item.Index, item.RecordTime, item.TotalVoltage, item.Current, item.SOC, item.BatteryStatus, item.ChargeMOSStatus, item.DischargeMOSStatus, item.DetStatus, item.LoopNumber, item.Balance,
                                        item.Cell1Temp, item.Cell2Temp, item.Cell3Temp, item.Cell4Temp, item.Cell5Temp, item.Cell1Voltage, item.Cell2Voltage, item.Cell3Voltage, item.Cell4Voltage, item.Cell5Voltage, 
                                        item.Cell6Voltage, item.Cell7Voltage, item.Cell8Voltage, item.Cell9Voltage,
                                        item.Cell10Voltage, item.Cell11Voltage, item.Cell12Voltage, item.Cell13Voltage, item.Cell14Voltage, item.Cell15Voltage, item.Cell16Voltage,
                                        item.Humidity);
                        }
                        sw.WriteLine(strLine);
                    }
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                if (null != sw)
                    sw.Close();
                if (null != fs)
                    fs.Close();
            }

        }
        public static void WriteLogs(string fileName, string type, string content,bool isSpacing)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!string.IsNullOrEmpty(path))
                {
                    path = AppDomain.CurrentDomain.BaseDirectory + fileName;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    if (!File.Exists(path))
                    {
                        FileStream fs = File.Create(path);
                        fs.Close();
                    }
                    if (File.Exists(path))
                    {
                        StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + type + "-->" + content);
                        if (isSpacing)
                            sw.WriteLine("\r\n");
                        sw.Close();
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public static string ToHexStrFromByte(byte[] byteDatas,bool isSpacing)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                if(isSpacing)
                {
                    builder.Append(string.Format("{0:X2}", byteDatas[i]));
                }
                else
                {
                    builder.Append(string.Format("{0:X2} ", byteDatas[i]));
                }
            }
            return builder.ToString().Trim();
        }

        public static string ToStrFromInt(int[] Datas)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < Datas.Length; i++)
            {
                builder.Append(string.Format("{0} ", Datas[i]));
            }
            return builder.ToString().Trim();
        }

        /// <summary>
        /// 写Eeprom数据CSV文件标题
        /// </summary>
        /// <param name="listData">提供保存数据的list</param>
        /// <param name="path">CSV的文件路径</param>
        public static void SaveEepromData(string path, bool isDataOK,  List<int> datas)
        {
            bool isCreate = false;
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                FileInfo fi = new FileInfo(path);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }

                if (!File.Exists(path))
                {
                    fs = File.Create(path);//创建该文件
                    isCreate = true;
                }
                else
                {
                    fs = new FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                }

                sw = new StreamWriter(fs, System.Text.Encoding.Default);
                if (isCreate)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("测试时间,");
                    sb.Append("数据类型,");
                    sb.Append("预充控制,");
                    sb.Append("充电MOS恢复控制,");
                    sb.Append("过流MOS控制,");
                    sb.Append("平衡功能,");
                    sb.Append("禁止低压充电,");
                    sb.Append("欠压关闭CHG,");
                    sb.Append("二次过充保护使能,");
                    sb.Append("过流保护定时恢复,");
                    sb.Append("负载锁定,");
                    sb.Append("CTL管脚控制,");
                    sb.Append("负载释放延迟,");
                    sb.Append("过压保护延时,");
                    sb.Append("欠压保护延时,");
                    sb.Append("放电电流1保护电压,");
                    sb.Append("放电电流1保护延时,");
                    sb.Append("放电电流2保护电压,");
                    sb.Append("放电电流2保护延时,");
                    sb.Append("短路保护电压,");
                    sb.Append("短路保护延时,");
                    sb.Append("充电过流保护电压,");
                    sb.Append("充电过流保护延时,");
                    sb.Append("充放电状态检测电压,");
                    sb.Append("充电MOS开启延时,");
                    sb.Append("过流自恢复延时,");
                    sb.Append("过压保护电压,");
                    sb.Append("过压保护释放电压,");
                    sb.Append("欠压保护电压,");
                    sb.Append("欠压保护释放电压,");
                    sb.Append("平衡开启电压,");
                    sb.Append("预充开启电压,");
                    sb.Append("低压禁充电压,");
                    sb.Append("二次过充保护电压,");
                    sb.Append("充电高温保护,");
                    sb.Append("充电高温保护释放,");
                    sb.Append("充电低温保护,");
                    sb.Append("充电低温保护释放,");
                    sb.Append("放电高温保护,");
                    sb.Append("放电高温保护释放,");
                    sb.Append("放电低温保护,");
                    sb.Append("放电低温保护,");
                    sw.WriteLine(sb.ToString());
                }

                if(datas != null)
                {
                    string strLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41}",
                        string.Format("{0}年{1}月{2}日 {3}时{4}分{5}秒",DateTime.Now.Year, DateTime.Now.Month,DateTime.Now.Day,DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second),isDataOK ? "正常":"异常",
                        datas[0],datas[1],datas[2],datas[3],datas[4],datas[5], datas[6], datas[7], datas[8], datas[9], datas[10], datas[11], datas[12], datas[13], datas[14], datas[15], datas[16], datas[17], datas[18], datas[19],
                        datas[20], datas[21], datas[22], datas[23], datas[24], datas[25], datas[26], datas[27], datas[28], datas[29], datas[30], datas[31], datas[32], datas[33], datas[34], datas[35], datas[36], datas[37], datas[38], datas[39]);
                    sw.WriteLine(strLine);
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                if (null != sw)
                    sw.Close();
                if (null != fs)
                    fs.Close();
            }
        }
    }
}
