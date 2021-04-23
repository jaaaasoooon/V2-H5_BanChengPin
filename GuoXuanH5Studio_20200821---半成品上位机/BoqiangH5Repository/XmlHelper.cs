using BoqiangH5Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Xml;

namespace BoqiangH5Repository
{
    public class XmlHelper
    {
        static string strLoadCfgFile = "ProtocolFiles\\bq_config_info.xml";       
                
        public static string m_strCanType = null;
        public static string m_strCanIndex = null;
        public static string m_strCanChannel = null;
        public static string m_strBaudrate = null;
        public static string m_strProtocol = null;
        public static string m_strCanFD = null;
        public static string m_strArbitration = null;
        public static string m_strDataBaudRate = null;
        public static string m_strTerminalResistance = null;
        public static string m_strDataRecordInterval = null;
        public static string m_strHandShakeInterval = null;
        //public static string m_strWakeInterval = null;
        public static string m_strIsClosePwd = null;
        //public static string m_strIsDebugMode = null;
        public static string m_strVoltageBase = null;
        public static string m_strVoltageError = null;
        public static string m_strTemperatureBase = null;
        public static string m_strTemperatureError = null;
        public static string m_strHardwareVersion = null;
        public static string m_strSoftwareVersion = null;

        #region 配置文件路径
        static string strBqProtocolFile = "ProtocolFiles\\bq_h5_bms_info.xml";
        static string strDdProtocolFile = "ProtocolFiles\\didi_h5_bms_info.xml";
        public static string m_strCfgFilePath
        {
            get
            {
                if (XmlHelper.m_strProtocol == "0")
                    return strBqProtocolFile;
                else
                    return strDdProtocolFile;
            }
        }
        #endregion

        public static void LoadConfigInfo()
        {
            if (!File.Exists(strLoadCfgFile))
            {
                return;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strLoadCfgFile);                   //加载文件
                XmlNode root = xmlDoc.DocumentElement;       //查找根节点

                XmlNode subNode = root.SelectNodes("config")[0];      // "config"

                XmlNodeList nodeList = subNode.ChildNodes;

                m_strCanType = nodeList[0].InnerText;
                m_strCanIndex = nodeList[1].InnerText;
                m_strCanChannel = nodeList[2].InnerText;
                m_strBaudrate = nodeList[3].InnerText;
                m_strProtocol = nodeList[4].InnerText;
                m_strCanFD = nodeList[5].InnerText;
                m_strArbitration = nodeList[6].InnerText;
                m_strDataBaudRate = nodeList[7].InnerText;
                m_strTerminalResistance = nodeList[8].InnerText;
                m_strHandShakeInterval = nodeList[9].InnerText;
                m_strDataRecordInterval = nodeList[10].InnerText;
                //m_strWakeInterval = nodeList[11].InnerText;
                m_strIsClosePwd = nodeList[12].InnerText;
                //m_strIsDebugMode = nodeList[13].InnerText;
                m_strVoltageBase = nodeList[14].InnerText;
                m_strVoltageError = nodeList[15].InnerText;
                m_strTemperatureBase = nodeList[16].InnerText;
                m_strTemperatureError = nodeList[17].InnerText;
                m_strHardwareVersion = nodeList[18].InnerText;
                m_strSoftwareVersion = nodeList[19].InnerText;
            }
            catch (Exception ex)
            {
            }
            return;

        }
        
        public static void SaveConfigInfo()
        {
            if (!File.Exists(strLoadCfgFile))
            {
                return;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strLoadCfgFile); //加载文件
                XmlElement root = xmlDoc.DocumentElement;

                XmlNode subNode = root.SelectNodes("config")[0];      // "config"

                XmlNodeList nodeList = subNode.ChildNodes;

                nodeList[0].InnerText = m_strCanType;
                nodeList[1].InnerText = m_strCanIndex;
                nodeList[2].InnerText = m_strCanChannel;
                nodeList[3].InnerText = m_strBaudrate;
                nodeList[4].InnerText = m_strProtocol;
                nodeList[5].InnerText = m_strCanFD;
                nodeList[6].InnerText = m_strArbitration;
                nodeList[7].InnerText = m_strDataBaudRate;
                nodeList[8].InnerText = m_strTerminalResistance;
                nodeList[9].InnerText = m_strHandShakeInterval;
                nodeList[10].InnerText = m_strDataRecordInterval;
                //nodeList[11].InnerText = m_strWakeInterval;
                nodeList[12].InnerText = m_strIsClosePwd;
                //nodeList[13].InnerText = m_strIsDebugMode;
                nodeList[14].InnerText = m_strVoltageBase;
                nodeList[15].InnerText = m_strVoltageError;
                nodeList[16].InnerText = m_strTemperatureBase;
                nodeList[17].InnerText = m_strTemperatureError;
                nodeList[18].InnerText = m_strHardwareVersion;
                nodeList[19].InnerText = m_strSoftwareVersion;
                xmlDoc.Save(strLoadCfgFile);

            }
            catch (Exception ex)
            {
            }

        }

        /// <summary>
        /// 检测文件是否存在
        /// </summary>
        /// <param name="strFileName">文件名</param>
        /// <returns></returns>
        public static bool ChechXmlFileExists(string strFileName)
        {
            try
            {          
                XmlDocument xmlDoc = new XmlDocument();

                if (!File.Exists(strFileName))
                {
                    XmlDeclaration Declaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
   
                    XmlElement rootNode = xmlDoc.CreateElement("root");

                    xmlDoc.AppendChild(rootNode);
                    xmlDoc.InsertBefore(Declaration, xmlDoc.DocumentElement);

                    xmlDoc.Save(@strFileName);

                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
               
            }
            return false;
        }


        public static XmlElement AddSonElement(XmlDocument xmlDoc, string strFileName, XmlElement xeFather, string sonName, string sonText)
        {
            XmlElement xnSon = null;
            try
            {
                xnSon = xmlDoc.CreateElement(sonName);
                xnSon.InnerText = sonText; //
                xeFather.AppendChild(xnSon);  //添加节点
                xmlDoc.Save(strFileName);
            }
            catch (Exception ex)
            {
                
            }
            return xnSon;
        }



        public static void LoadXmlConfig<T>(string strFileName,string strSubNode1, IList<T> listNode) where T : H5BmsInfo, new()
        {
            
            try
            {
                if (!File.Exists(strFileName))
                {
                    return;
                }

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlElement root = xmlDoc.DocumentElement;

                XmlNodeList subNodeList = root.SelectNodes(strSubNode1); 

                int nCount = 0;

                int nByteCont = 0;

                foreach (XmlNode node in subNodeList) 
                {
                    string strInnerText = null;
                    T nodeInfo = new T();

                    nodeInfo.Index = ++ nCount;

                    if (node.SelectSingleNode("address") != null)
                    {
                        strInnerText = node.SelectSingleNode("address").InnerText;
                        nodeInfo.AddressStr = "0x" + strInnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.Address = Convert.ToInt32(strInnerText, 16);
                        }
                    }


                    if (node.SelectSingleNode("description") != null)
                    {
                        nodeInfo.Description = node.SelectSingleNode("description").InnerText;

                        strInnerText = node.SelectSingleNode("value").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.StrValue = strInnerText; // double.Parse(strInnerText);
                        }
                    }

                    if (node.SelectSingleNode("min_value") != null)
                    {
                        strInnerText = node.SelectSingleNode("min_value").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.MinValue = double.Parse(strInnerText);
                        }
                    }

                    if (node.SelectSingleNode("max_value") != null)
                    {
                        strInnerText = node.SelectSingleNode("max_value").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.MaxValue = double.Parse(strInnerText);
                        }
                    }

                    if (node.SelectSingleNode("unit") != null)
                    {
                        nodeInfo.Unit = node.SelectSingleNode("unit").InnerText;
                    }

                    if (node.SelectSingleNode("scale") != null)
                    {
                        nodeInfo.Scale = node.SelectSingleNode("scale").InnerText;
                        
                    }

                    if (node.SelectSingleNode("register_num") != null)
                    {
                        strInnerText = node.SelectSingleNode("register_num").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.RegisterNum = byte.Parse(strInnerText);
                        }
                    }

                    if (node.SelectSingleNode("byte_count") != null)
                    {
                        strInnerText = node.SelectSingleNode("byte_count").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.ByteCount = byte.Parse(strInnerText);

                            nByteCont += nodeInfo.ByteCount;
                        }
                    }

                    if (node.SelectSingleNode("conversion") != null)
                    {
                        nodeInfo.Conversion = node.SelectSingleNode("conversion").InnerText;
                    }

                    if (node.SelectSingleNode("is_show") != null)
                    {
                        if ("true" == node.SelectSingleNode("is_show").InnerText)
                            nodeInfo.IsShow = true;
                        else
                            nodeInfo.IsShow = false;
                    }
                    else
                    {
                        nodeInfo.IsShow = true;
                    }

                    if (nodeInfo.IsShow)
                    {
                        listNode.Add(nodeInfo);
                        //nCount++;
                    }

                }
                
            }
            catch (Exception ex)
            {
                
            }
        }

        public static void LoadCellVoltageConfig(string strFileName, IList<H5BmsInfo> listNode, string strAddr)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlElement root = xmlDoc.DocumentElement;

                int nCount = 0;

                XmlNodeList nodeList = xmlDoc.SelectNodes("/root/bms_info/register_node_info/address");

                foreach (XmlNode node in nodeList)  // "MasterAdjustNode"
                {
                    if (node.InnerText == strAddr)
                    {
                        string strCount = node.ParentNode.SelectSingleNode("register_num").InnerText; 
          
                        if (!string.IsNullOrEmpty(strCount))
                        {
                            nCount = int.Parse(strCount);
                            break;
                        }
                                       
                    }
                
                }
                             

                for (int n = 0; n < nCount; n++)
                {
                    H5BmsInfo nodeInfo = new H5BmsInfo();

                    nodeInfo.Index = listNode.Count + 1;

                    nodeInfo.Address = Convert.ToInt32(strAddr, 16);
                    nodeInfo.AddressStr = "0x" + strAddr;

                    if (strAddr == "A210")
                        nodeInfo.Description = "高16组 电芯" + (n + 1).ToString() + "电压";
                    else
                        nodeInfo.Description = "低16组 电芯" + (n + 1).ToString() + "电压";

                    nodeInfo.StrValue = "0"; // 

                    nodeInfo.MinValue = 0;

                    nodeInfo.MaxValue = 0;

                    nodeInfo.Unit = "mV";

                    nodeInfo.Scale = "1";

                    nodeInfo.RegisterNum = 1;

                    nodeInfo.Conversion = "y=x";

                    listNode.Add(nodeInfo);
                }
               
            }
            catch (Exception ex)
            {
                
            }
        }

        public static void LoadBqBmsStatusConfig(string strFileName, string strSubNode, ICollection<BitStatInfo> listStatus)
        {
            try
            {
                if (!File.Exists(strFileName))
                {
                    return;
                }

                string[] arrNodes = strSubNode.Split('/');
                string strSubNode1 = arrNodes[0];
                string strSubNode2 = arrNodes[1];
                string strSubNode3 = arrNodes[2];

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlElement root = xmlDoc.DocumentElement;

                XmlNode subNode1 = root.SelectNodes(strSubNode1)[0];         // "master_function_switch"

                foreach (XmlNode byNode in subNode1.SelectNodes(strSubNode2))  // "master_function_switch_node"
                {
                    int nByteIndex = 0;

                    string byteIndex = byNode.SelectSingleNode("byte_index").InnerText;
                    if (!string.IsNullOrEmpty(byteIndex))
                    {
                        nByteIndex = int.Parse(byteIndex);
                    }

                    string strInnerText = null;

                    foreach (XmlNode btNode in byNode.SelectNodes(strSubNode3))
                    {
                        BitStatInfo bitNodeInfo = new BitStatInfo();

                        bitNodeInfo.ByteIndex = nByteIndex;
                        bitNodeInfo.BitInfo = btNode.SelectSingleNode("bit_info").InnerText;

                        strInnerText = btNode.SelectSingleNode("bit_index").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            bitNodeInfo.BitIndex = int.Parse(strInnerText);
                        }

                        strInnerText = btNode.SelectSingleNode("is_show").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            if ("true" == strInnerText)
                                bitNodeInfo.IsShow = true;
                            else
                                bitNodeInfo.IsShow = false;
                        }

                        strInnerText = btNode.SelectSingleNode("is_warning").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            if ("true" == strInnerText)
                                bitNodeInfo.IsWarning = true;
                            else
                                bitNodeInfo.IsWarning = false;
                        }
                        strInnerText = btNode.SelectSingleNode("is_protect").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            if ("true" == strInnerText)
                                bitNodeInfo.IsProtect = true;
                            else
                                bitNodeInfo.IsProtect = false;
                        }
                        if (bitNodeInfo.IsShow)
                            listStatus.Add(bitNodeInfo);
                    }
                }
               
            }
            catch (Exception ex)
            {
                
            }
                       
        }

        public static void LoadDdBatStatConfig(string strFileName, string strSubNode, ICollection<BitStatInfo> listSwitch, string strByteInfo)
        {
            try
            {
                if (!File.Exists(strFileName))
                {
                    return;
                }
                
                string[] arrNodes = strSubNode.Split('/');
                string strSubNode1 = arrNodes[0];
                string strSubNode2 = arrNodes[1];
                string strSubNode3 = arrNodes[2];

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlElement root = xmlDoc.DocumentElement;

                XmlNode subNode1 = root.SelectNodes(strSubNode1)[0];         // "master_function_switch"

                string strRegister = subNode1.SelectSingleNode("address").InnerText;
                string strDesc = subNode1.SelectSingleNode("description").InnerText;
                string strRegCount = subNode1.SelectSingleNode("register_num").InnerText;
                
                foreach (XmlNode byNode in subNode1.SelectNodes(strSubNode2))  // "master_function_switch_node"
                {
                    int nByteIndex = 0;
                    string byteInfo = byNode.SelectSingleNode("byte_info").InnerText;
                    if(byteInfo != strByteInfo)
                    {
                        continue;
                    }

                    string byteIndex = byNode.SelectSingleNode("byte_index").InnerText;
                    if (!string.IsNullOrEmpty(byteIndex))
                    {
                        nByteIndex = int.Parse(byteIndex);
                    }

                    string strInnerText = null;
                    foreach (XmlNode btNode in byNode.SelectNodes(strSubNode3))
                    {
                        BitStatInfo bitNodeInfo = new BitStatInfo();

                        bitNodeInfo.ByteIndex = nByteIndex;
                        bitNodeInfo.BitInfo = btNode.SelectSingleNode("bit_info").InnerText;

                        
                        strInnerText = btNode.SelectSingleNode("bit_index").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            bitNodeInfo.BitIndex = int.Parse(strInnerText);
                        }

                        strInnerText = btNode.SelectSingleNode("is_warning").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            if ("true" == strInnerText)
                                bitNodeInfo.IsWarning = true;
                            else
                                bitNodeInfo.IsWarning = false;
                        }

                        strInnerText = btNode.SelectSingleNode("is_protect").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            if ("true" == strInnerText)
                                bitNodeInfo.IsProtect = true;
                            else
                                bitNodeInfo.IsProtect = false;
                        }
                        listSwitch.Add(bitNodeInfo);
                    }
                }
                

            }
            catch (Exception ex)
            {
                
            }
                        
        }


        #region 参数另存为XML
        public static void SaveAsXmlFile<T>(string fileName, string strSubNode1, string strSubNode2, IList<T> listPara) where T : H5BmsInfo// XmlNodeInfo  csv->xml
        {
            
            try
            {

                XmlDocument xmlDoc = new XmlDocument();

                XmlHelper.ChechXmlFileExists(fileName);

                xmlDoc.Load(fileName);                   //加载文件
                XmlNode root = xmlDoc.DocumentElement;   //查找根节点

                XmlElement xeNode1 = xmlDoc.CreateElement(strSubNode1);

                root.AppendChild(xeNode1);

                foreach (T node in listPara)
                {
                    XmlElement xeNode2 = xmlDoc.CreateElement(strSubNode2);
                    xeNode1.AppendChild(xeNode2);

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "index", node.Index.ToString());
                    //XmlHelper.AddSonElement(xmlDoc, fileName, nodeAdjust, "did_num", node.DidNum);   // System.Convert.ToString(node.DidNum, 16)
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "did_num", System.Convert.ToString(node.Address, 16)); 
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "did_description", node.Description.ToString());

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "value", node.StrValue.ToString());
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "min_value", node.MinValue.ToString());
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "max_value", node.MaxValue.ToString());

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "unit", node.Unit.ToString());
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "scale", node.Scale.ToString());

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "byte_num", node.RegisterNum.ToString());
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "conversion", node.Conversion.ToString());

                }
            }
            catch(Exception ex)
            {
               
            }
        }
        
        public static void SwitchParaSaveAsXmlFile(string fileName, string strSubNode1, string strSubNode2, List<BitStatInfo> listSwitch)
        {
            try
            {

                XmlDocument xmlDoc = new XmlDocument();

                XmlHelper.ChechXmlFileExists(fileName);

                xmlDoc.Load(fileName);                   //加载文件
                XmlNode root = xmlDoc.DocumentElement;   //查找根节点

                XmlElement xeNode1 = xmlDoc.CreateElement(strSubNode1);

                root.AppendChild(xeNode1);

                XmlHelper.AddSonElement(xmlDoc, fileName, xeNode1, "did_num", "14AE");
                XmlHelper.AddSonElement(xmlDoc, fileName, xeNode1, "did_description", "主控位控参数");
                XmlHelper.AddSonElement(xmlDoc, fileName, xeNode1, "byte_num", "8");

                foreach (BitStatInfo node in listSwitch)
                {
                    XmlElement xeNode2 = xmlDoc.CreateElement(strSubNode2);
                    xeNode1.AppendChild(xeNode2);

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "name", node.BitInfo);
                    //XmlHelper.AddSonElement(xmlDoc, fileName, nodeAdjust, "did_num", node.DidNum);   // System.Convert.ToString(node.DidNum, 16)
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "byte_index", node.ByteIndex.ToString());
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "bit_index", node.BitIndex.ToString());

                }
            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion

        #region 读CSV文件 保存为CSV XmlNodeInfo

        public void CreateXmlFile(string strCsvFile1, string strXmlFile2)
        {
            DataTable dataTable = CSVFileHelper.OpenCSV(strCsvFile1);

            List<XmlNodeInfo> listNode = new List<XmlNodeInfo>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                XmlNodeInfo nodeInfo = new XmlNodeInfo();

                nodeInfo.Index = (i + 1).ToString();

                nodeInfo.DidNum = dataTable.Rows[i][0].ToString();

                nodeInfo.DidDescription = dataTable.Rows[i][1].ToString();

                nodeInfo.ByteNum = dataTable.Rows[i][2].ToString();

                nodeInfo.StrValue = "0";

                nodeInfo.MinValue = dataTable.Rows[i][6].ToString();

                nodeInfo.MaxValue = dataTable.Rows[i][7].ToString();

                nodeInfo.Unit = dataTable.Rows[i][8].ToString();

                nodeInfo.Scale = "0";

                nodeInfo.Conversion = dataTable.Rows[i][9].ToString();

                listNode.Add(nodeInfo);
            }


            XmlHelper.SaveAsXmlFileFromCsv_XmlNodeInfo(strXmlFile2, "master_parameters_config", "master_adjust_node", listNode);
        }

        public static void SaveAsXmlFileFromCsv_XmlNodeInfo<T>(string fileName, string strSubNode1, string strSubNode2, IList<T> listData) where T : XmlNodeInfo  // csv->xml
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlHelper.ChechXmlFileExists(fileName);

                xmlDoc.Load(fileName);                   //加载文件
                XmlNode root = xmlDoc.DocumentElement;   //查找根节点

                XmlElement xeNode1 = xmlDoc.CreateElement(strSubNode1);

                root.AppendChild(xeNode1);

                foreach (T node in listData)
                {
                    XmlElement xeNode2 = xmlDoc.CreateElement(strSubNode2);
                    xeNode1.AppendChild(xeNode2);

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "index", node.Index.ToString());
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "did_num", node.DidNum);   // System.Convert.ToString(node.DidNum, 16)
                    //XmlHelper.AddSonElement(xmlDoc, fileName, nodeAdjust, "did_num", System.Convert.ToString(node.DidNum, 16));
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "did_description", node.DidDescription.ToString());

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "value", node.StrValue);
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "min_value", node.MinValue.ToString());
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "max_value", node.MaxValue.ToString());

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "unit", node.Unit.ToString());
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "scale", node.Scale.ToString());

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "byte_num", node.ByteNum.ToString());
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "conversion", node.Conversion.ToString());

                }
            }
            catch (Exception ex)
            {
                
            }
        }
      
        /// <summary>
        /// 读CSV文件 保存为CSV DTCInfo
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="strSubNode1"></param>
        /// <param name="strSubNode2"></param>
        /// <param name="listData"></param>
        public static void SaveAsXmlFileFromCsv_XmlDTCInfo(string fileName, string strSubNode1, string strSubNode2, List<DTCInfo> listData) // csv->xml
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlHelper.ChechXmlFileExists(fileName);

                xmlDoc.Load(fileName);                   //加载文件
                XmlNode root = xmlDoc.DocumentElement;   //查找根节点

                XmlElement xeNode1 = xmlDoc.CreateElement(strSubNode1);

                root.AppendChild(xeNode1);

                foreach (DTCInfo node in listData)
                {
                    XmlElement xeNode2 = xmlDoc.CreateElement(strSubNode2);
                    xeNode1.AppendChild(xeNode2);

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "index", node.Index.ToString());
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "dtc_display", node.DTCDisplay);   // System.Convert.ToString(node.DidNum, 16)
           
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "dtc_bytes", node.DTCBytes);

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "dtc_meaning", node.DTCMeaning);
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "faults_attribute", node.FaultsAttribute);
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "mature_condition", node.MatureCondition);

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "system_action", node.SystemAction);
                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "demature_condition", node.DematureCondition);

                    XmlHelper.AddSonElement(xmlDoc, fileName, xeNode2, "possible_fault_causes", node.PossibleFaultCauses);
                   
                }
            }
            catch (Exception ex)
            {
               
            }
        }
        #endregion

        //lipeng  2020.03.27 读取备份数据配置文件
        public static void LoadBackupRecordConfig(string strFileName, Dictionary<string, string> recordInfoDic, Dictionary<string, string> recordTypeDic,
            List<Tuple<string, string, string>> packInfoList, List<Tuple<string, string, string>> batteryInfoList)
        {
            try
            {
                if (!File.Exists(strFileName))
                {
                    return;
                }

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlNode xn = xmlDoc.SelectSingleNode("root");
                XmlNodeList nodelist = xn.ChildNodes; 
                foreach(XmlNode item in nodelist)
                {
                    if(item.LocalName == "records")
                    {
                        XmlNodeList recordlist = item.ChildNodes;
                        foreach(XmlNode _item in recordlist)
                        {
                            if (_item.LocalName == "record_info")
                            {
                                XmlNodeList recordInfolist = _item.ChildNodes;
                                foreach(XmlNode _it in recordInfolist)
                                {
                                    if (_it.LocalName == "record_info_node")
                                    {
                                        string value = _it.SelectSingleNode("value").InnerText;
                                        string description = _it.SelectSingleNode("description").InnerText;
                                        recordInfoDic.Add(value, description);
                                    }
                                }
                            }
                            else if (_item.LocalName == "record_type")
                            {
                                XmlNodeList recordInfolist = _item.ChildNodes;
                                foreach (XmlNode _it in recordInfolist)
                                {
                                    if (_it.LocalName == "record_type_node")
                                    {
                                        string value = _it.SelectSingleNode("value").InnerText;
                                        string description = _it.SelectSingleNode("description").InnerText;
                                        recordTypeDic.Add(value, description);
                                    }
                                }
                            }
                            else if (_item.LocalName == "pack_info")
                            {
                                XmlNodeList recordInfolist = _item.ChildNodes;
                                foreach (XmlNode _it in recordInfolist)
                                {
                                    if (_it.LocalName == "pack_info_node")
                                    {
                                        XmlNodeList packlist = _it.ChildNodes;
                                        foreach(XmlNode pack in packlist)
                                        {
                                            if (pack.LocalName == "pack_info_bit")
                                            {
                                                string value = pack.SelectSingleNode("value").InnerText;
                                                string description = pack.SelectSingleNode("description").InnerText;
                                                string bit = pack.SelectSingleNode("bit").InnerText;
                                                packInfoList.Add(new Tuple<string, string, string>(bit, value, description));
                                            }
                                        }
                                    }
                                }
                            }
                            else if (_item.LocalName == "battery_info")
                            {
                                XmlNodeList recordInfolist = _item.ChildNodes;
                                foreach (XmlNode _it in recordInfolist)
                                {
                                    if (_it.LocalName == "battery_info_node")
                                    {
                                        XmlNodeList batterylist = _it.ChildNodes;
                                        foreach (XmlNode battery in batterylist)
                                        {
                                            if (battery.LocalName == "battery_info_bit")
                                            {
                                                string value = battery.SelectSingleNode("value").InnerText;
                                                string description = battery.SelectSingleNode("description").InnerText;
                                                string bit = battery.SelectSingleNode("bit").InnerText;
                                                batteryInfoList.Add(new Tuple<string, string, string>(bit, value, description));
                                            }
                                        }
                                    }
                                }
                            }
                            else
                                break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }

        public static void LoadDidiRecordConfig(string strFileName, Dictionary<string, string> recordTypeDic)
        {
            try
            {
                if (!File.Exists(strFileName))
                {
                    return;
                }

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlNode xn = xmlDoc.SelectSingleNode("root");
                XmlNodeList nodelist = xn.ChildNodes;
                foreach (XmlNode item in nodelist)
                {
                    if (item.LocalName == "didiRecords")
                    {
                        XmlNodeList recordlist = item.ChildNodes;
                        foreach (XmlNode _item in recordlist)
                        {
                            if (_item.LocalName == "record_type")
                            {
                                XmlNodeList recordInfolist = _item.ChildNodes;
                                foreach (XmlNode _it in recordInfolist)
                                {
                                    if (_it.LocalName == "record_type_node")
                                    {
                                        string value = _it.SelectSingleNode("value").InnerText;
                                        string description = _it.SelectSingleNode("description").InnerText;
                                        recordTypeDic.Add(value, description);
                                    }
                                }
                            }
                            else
                                break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }

        public static void LoadAFERegisterConfig(string strFileName, List<AFERegister> registerList,bool flag)
        {
            try
            {
                if (!File.Exists(strFileName))
                {
                    return;
                }

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlNode xn = xmlDoc.SelectSingleNode("root");
                XmlNodeList nodelist = xn.ChildNodes;
                foreach (XmlNode item in nodelist)
                {
                    if (item.LocalName == "afe_register")
                    {
                        XmlNodeList afelist = item.ChildNodes;
                        if(flag)
                        {
                            foreach (XmlNode _item in afelist)
                            {
                                if (_item.LocalName == "eeprom_register")
                                {
                                    XmlNodeList registerInfolist = _item.ChildNodes;
                                    foreach (XmlNode _it in registerInfolist)
                                    {
                                        AFERegister afe = new AFERegister();
                                        afe.Index = registerList.Count + 1;
                                        if (_it.LocalName == "register")
                                        {
                                            string name = _it.SelectSingleNode("name").InnerText;
                                            afe.Name = name;
                                            registerList.Add(afe);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (XmlNode _item in afelist)
                            {
                                if (_item.LocalName == "others_register")
                                {
                                    XmlNodeList registerInfolist = _item.ChildNodes;
                                    foreach (XmlNode _it in registerInfolist)
                                    {
                                        AFERegister afe = new AFERegister();
                                        afe.Index = registerList.Count + 1;
                                        if (_it.LocalName == "register")
                                        {
                                            string name = _it.SelectSingleNode("name").InnerText;
                                            afe.Name = name;
                                            registerList.Add(afe);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }

        public static void LoadAdjustParam(string strFileName, List<AdjustParam> adjustParamList)
        {
            try
            {
                if (!File.Exists(strFileName))
                {
                    return;
                }

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlNode xn = xmlDoc.SelectSingleNode("root");
                XmlNodeList nodelist = xn.ChildNodes;
                foreach (XmlNode item in nodelist)
                {
                    if (item.LocalName == "adjust_param")
                    {
                        XmlNodeList adjustlist = item.ChildNodes;
                        foreach (XmlNode _item in adjustlist)
                        {
                            if (_item.LocalName == "adjust_node_param")
                            {
                                XmlNodeList adjustParamlist = _item.ChildNodes;

                                AdjustParam adjust = new AdjustParam();
                                adjust.Index = adjustParamList.Count + 1;
                                adjust.Name = adjustParamlist[0].InnerText;
                                adjust.StrValue = adjustParamlist[1].InnerText;
                                adjust.Unit = adjustParamlist[2].InnerText;
                                adjust.ByteCount = adjustParamlist[3].InnerText;
                                adjustParamList.Add(adjust);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }

        public static void LoadManufactureCode(string strFileName, string strSubNode1, Dictionary<string,string> dic)
        {

            try
            {
                if (!File.Exists(strFileName))
                {
                    return;
                }

                string code = string.Empty;
                string description = string.Empty;
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlElement root = xmlDoc.DocumentElement;

                XmlNodeList subNodeList = root.SelectNodes(strSubNode1);

                foreach (XmlNode node in subNodeList)
                {
                    if (node.SelectSingleNode("code") != null)
                    {
                        code = node.SelectSingleNode("code").InnerText;
                    }

                    if (node.SelectSingleNode("description") != null)
                    {
                        description = node.SelectSingleNode("description").InnerText;
                    }

                    dic.Add(code, description);
                }
            }
            catch(Exception ex)
            {

            }
        }

        public static void LoadProtectParamXmlConfig<T>(string strFileName, string strSubNode1, IList<T> listNode) where T : H5ProtectParamInfo, new()
        {

            try
            {
                if (!File.Exists(strFileName))
                {
                    return;
                }

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlElement root = xmlDoc.DocumentElement;

                XmlNodeList subNodeList = root.SelectNodes(strSubNode1);

                int nCount = 0;

                int nByteCont = 0;

                foreach (XmlNode node in subNodeList)
                {
                    string strInnerText = null;
                    T nodeInfo = new T();

                    nodeInfo.Index = ++nCount;

                    if (node.SelectSingleNode("description") != null)
                    {
                        nodeInfo.Description = node.SelectSingleNode("description").InnerText;

                        strInnerText = node.SelectSingleNode("value").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.StrValue = strInnerText; // double.Parse(strInnerText);
                        }
                    }

                    if (node.SelectSingleNode("min_value") != null)
                    {
                        strInnerText = node.SelectSingleNode("min_value").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.MinValue = double.Parse(strInnerText);
                        }
                    }

                    if (node.SelectSingleNode("max_value") != null)
                    {
                        strInnerText = node.SelectSingleNode("max_value").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.MaxValue = double.Parse(strInnerText);
                        }
                    }

                    if (node.SelectSingleNode("unit") != null)
                    {
                        nodeInfo.Unit = node.SelectSingleNode("unit").InnerText;
                    }

                    if (node.SelectSingleNode("byte_count") != null)
                    {
                        strInnerText = node.SelectSingleNode("byte_count").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.ByteCount = byte.Parse(strInnerText);

                            nByteCont += nodeInfo.ByteCount;
                        }
                    }

                    if (node.SelectSingleNode("unsigned") != null)
                    {
                        nodeInfo.isUnsigned = node.SelectSingleNode("unsigned").InnerText == "true" ? true : false;
                    }
                    listNode.Add(nodeInfo);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
