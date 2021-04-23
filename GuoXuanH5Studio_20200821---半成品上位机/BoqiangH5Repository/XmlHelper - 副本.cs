using BoqiangH5Entity;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BoqiangH5Repository
{
    public class XmlHelper
    {
        //static Logger LogXmlHelp = LogManager.GetLogger("XmlHelp");

        static string strLoadCfgFile = "ProtocolFiles\\bq_config_info.xml";
                
        public static string m_strCanType = null;
        public static string m_strCanIndex = null;
        public static string m_strCanChannel = null;
        public static string m_strBaudrate = null;
        public static string m_strProtocol = null;

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
                //LogXmlHelp.Error(ex.Message + ex.StackTrace);
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
                //LogXmlHelp.Error(ex.Message + ex.StackTrace);
            }
            return xnSon;
        }



        public static void LoadXmlConfig<T>(string strFileName,string strSubNode1, string strSubNode2, IList<T> listNode) where T : H5BmsInfo, new()
        {
            //string strMasterAdjustAndCtrl = "ProtocolFiles\\master_adjust_control.xml";

            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlElement root = xmlDoc.DocumentElement;

                XmlNode subNode1 = root.SelectNodes(strSubNode1)[0];      // "master_adjust_config"

                int nCount = 0;

                foreach (XmlNode node in subNode1.SelectNodes(strSubNode2))  // "MasterAdjustNode"
                {
                    string strInnerText = null;
                    T nodeInfo = new T();

                    //strInnerText = node.SelectSingleNode("index").InnerText;
                    //if (!string.IsNullOrEmpty(strInnerText))
                    //{
                    //    nodeInfo.Index = int.Parse(strInnerText);
                    //}
                    nodeInfo.Index = ++ nCount;

                    if (node.SelectSingleNode("address") != null)
                    {
                        strInnerText = node.SelectSingleNode("address").InnerText;
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
                        //if (!string.IsNullOrEmpty(strInnerText))
                        //{
                        //    nodeInfo.Scale = double.Parse(strInnerText);
                        //}
                    }

                    if (node.SelectSingleNode("register_num") != null)
                    {
                        strInnerText = node.SelectSingleNode("register_num").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            nodeInfo.RegisterNum = byte.Parse(strInnerText);
                        }
                    }

                    if (node.SelectSingleNode("conversion") != null)
                    {
                        nodeInfo.Conversion = node.SelectSingleNode("conversion").InnerText;
                    }

                    listNode.Add(nodeInfo);

                }
                //LogXmlHelp.Info("节点数 ... listAdjust: " + listMaster.Count);

            }
            catch (Exception ex)
            {
                //LogXmlHelp.Error("加载 " + strFileName + " 错误！\n" + ex.Message + ex.StackTrace);
            }
        }

        public static void LoadCellVoltageConfig(string strFileName, string strSubNode1, string strSubNode2, IList<H5BmsInfo> listNode, string strAddr)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(strFileName);

                XmlElement root = xmlDoc.DocumentElement;

                //XmlNode subNode1 = root.SelectNodes(strSubNode1)[0];      // "master_adjust_config"

                int nCount = 0;
                //double minVal = 0;
                //double maxVal = 0;
                //string strUnit = "mV";
                //string scale = "1";
                //string strConversion = "y=x";


                XmlNodeList nodeList = xmlDoc.SelectNodes("/root/bms_info/register_node_info/address");

                foreach (XmlNode node in nodeList)  // "MasterAdjustNode"
                {
                    if (node.InnerText == strAddr)
                    {
                        string strCount = node.ParentNode.SelectSingleNode("register_num").InnerText; 
                        //if(!string.IsNullOrEmpty( nodelist2[3].InnerText))
                        //{
                        //    minVal = double.Parse(nodelist2[3].InnerText);
                        //}
                        //if (!string.IsNullOrEmpty(nodelist2[4].InnerText))
                        //{
                        //    minVal = double.Parse(nodelist2[4].InnerText);
                        //}
                        //strUnit = nodelist2[5].InnerText;
                        //scale = nodelist2[6].InnerText;
                        if (!string.IsNullOrEmpty(strCount))
                        {
                            nCount = int.Parse(strCount);
                            break;
                        }
                        //strConversion = nodelist2[8].InnerText;                     
                    }
                
                }
                             

                for (int n = 0; n < nCount; n++)
                {
                    H5BmsInfo nodeInfo = new H5BmsInfo();

                    nodeInfo.Index = listNode.Count + 1;

                    nodeInfo.Address = Convert.ToInt32(strAddr, 16);

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
                //LogXmlHelp.Info("节点数 ... listAdjust: " + listMaster.Count);

            }
            catch (Exception ex)
            {
                //LogXmlHelp.Error("加载 " + strFileName + " 错误！\n" + ex.Message + ex.StackTrace);
            }
        }

        public static void LoadBatStatConfig(string strFileName, string strSubNode1, string strSubNode2, string strSubNode3,ICollection<BitStatInfo> listSwitch, string strByteInfo)
        {
            try
            {
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

                        //strInnerText = node.SelectSingleNode("byte_index").InnerText;
                        //if (!string.IsNullOrEmpty(strInnerText))
                        //{
                        //    nodeInfo.ByteIndex = int.Parse(strInnerText);
                        //}

                        strInnerText = btNode.SelectSingleNode("bit_index").InnerText;
                        if (!string.IsNullOrEmpty(strInnerText))
                        {
                            bitNodeInfo.BitIndex = int.Parse(strInnerText);
                        }

                        listSwitch.Add(bitNodeInfo);
                    }
                }
                //LogXmlHelp.Info("节点数 ... listAdjust: " + listMaster.Count);

            }
            catch (Exception ex)
            {
                //LogXmlHelp.Error("加载 " + strFileName + " 错误！\n" + ex.Message + ex.StackTrace);
            }

            //return nByteCount;
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
                //LogXmlHelp.Error("另存为xml文件 " + fileName + " 错误！\n" + ex.Message + ex.StackTrace);
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
                //LogXmlHelp.Error("另存为xml文件 " + fileName + " 错误！\n" + ex.Message + ex.StackTrace);
            }
        }
        #endregion

        #region 读CSV文件 保存为CSV XmlNodeInfo

        public void CreateXmlFile(string strCsvFile1, string strXmlFile2)
        {
            // 从csv 生成xml 配置文件
            //string strCsvFile1 = "ProtocolFiles\\csv\\parameters.csv";
            //string strXmlFile2 = "ProtocolFiles\\master_parameters1.xml";
            DataTable dataTable = CSVFileHelper.OpenCSV(strCsvFile1);

            List<XmlNodeInfo> listNode = new List<XmlNodeInfo>();

            //DataTable dt = dataSet.Tables[0];
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
                //LogXmlHelp.Error("另存为xml文件 " + fileName + " 错误！\n" + ex.Message + ex.StackTrace);
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
                //LogXmlHelp.Error("另存为xml文件 " + fileName + " 错误！\n" + ex.Message + ex.StackTrace);
            }
        }
        #endregion
    }
}
