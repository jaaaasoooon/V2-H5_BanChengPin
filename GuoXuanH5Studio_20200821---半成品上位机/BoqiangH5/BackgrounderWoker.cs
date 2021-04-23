
using System;
using System.Collections.Generic;
using System.Windows;
using BoqiangH5.ISO14229;
using BoqiangH5.ISO15765;
using BoqiangH5Entity;
using BoqiangH5.CommonClass;

namespace BoqiangH5
{
    public partial class MainWindow : Window
    {                      

        ApplicationLayerProtocol AppLayProtocol = new ApplicationLayerProtocol();         

        private bool teleMeterFresh = false;       
             

        public void treatRecvFrame(AppLayerEvent appLayEvent)
        {
            if (appLayEvent.A_PCI == 0x7F)
            {
                BlinkCommStatus(CommunicationStatus.RcvError);        
                return;
            }

            string strRecv = DataFormatConvert.ListToStr(appLayEvent.listData);

            BlinkCommStatus(CommunicationStatus.RcvOk);
            byte sID = (byte)(appLayEvent.A_PCI - 0x40);
            int offset = 0;

            switch (sID)
            {
                case ServicesID.ReadDataByIdentifier:                    
                    ReadDataByID(appLayEvent);
                    break;
    
                case ServicesID.WriteDataByIdentifier:
                    
                    int responseDid1 = AppLayProtocol.GetIntData(appLayEvent.listData, offset, 2);
                                        
                    if (responseDid1 >= DIDNumber.MasterParaDID && responseDid1 < 0x14AE)
                    {
         
                    }
                    else if (responseDid1 >= DIDNumber.MasterAdjustDID && responseDid1 < DIDNumber.MasterCtrlDID)
                    {
                        
                    }

                    else
                    {
                        
                    }
                    break;
       
                case  ServicesID.InputOutputControlByIdentifier:
                          
                    int responseDid2 = AppLayProtocol.GetIntData(appLayEvent.listData, offset, 2);
                    if (responseDid2 == DIDNumber.BMSSysTimeDID && MainWindow.m_OperateType == OperateType.ReadBmsTime)//if (responseDid2 == studioConfig.CurrentDid)
                    {
                    }
                    else if (responseDid2 >= DIDNumber.MasterCtrlDID && responseDid2 <= 0x18FF)
                    {

                    }

                    break;
       
                case ServicesID.DiagnosticSessionControl: //会话切换

                    if ((DiagnosticSessionType)appLayEvent.listData[0] == DiagnosticSessionType.ExtendedDiagnosticSession ||
                        (DiagnosticSessionType)appLayEvent.listData[0] == DiagnosticSessionType.DefaultSession  ||
                        (DiagnosticSessionType)appLayEvent.listData[0] == DiagnosticSessionType.ProgrammingSession)
                    {
                        switch (MainWindow.m_OperateType)
                        {
                            case OperateType.WriteMasterPara:
                            case OperateType.WriteAllMasterPara:
                            case OperateType.WriteMasterAdjustPara:
                            case OperateType.WriteAllMasterAdjustPara:
                            case OperateType.WriteMasterControlPara:
                            case OperateType.ReadBmsTime:               
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            
                case ServicesID.SecurityAccess://安全访问服务            
                    SecurityAccess(appLayEvent);
                    break;

                case ServicesID.ClearDiagnosticInformation: //清楚DTC信息
                
                    break;

                case ServicesID.ReadDTCInformation: //读取DTC信息
                              
                    ReadDTCInfo(appLayEvent);
                    break;

                case ServicesID.TesterPresent: //诊断仪在线
                    
                    break;

                case ServicesID.RoutineControl:   // 请求下载
                    
                    break;
                case ServicesID.RequestDownload :
                    
                    break;
                case ServicesID.TransferData:
                    
                    break;
                default:
                    
                    break;

                    
            }

            
        }

        private void ReadDataByID(AppLayerEvent appLayEvent)
        {
      
            int offset = 0;

            int nDid = DataFormatConvert.GetIntData(appLayEvent.listData, offset, 2);
            offset += 2;

            if (nDid == DIDNumber.MasterTelemDID)
            {
                MasterRemoteTest(appLayEvent, offset);
            }
            else if (((nDid - DIDNumber.PackTelemDID) / 0x400 >= 0) && (nDid - DIDNumber.PackTelemDID) / 0x400 <= 0x0F && (nDid - DIDNumber.PackTelemDID) % 0x400 == 0)//pack DID范围
            {
            }


            else if (nDid >= DIDNumber.MasterParaDID && nDid < DIDNumber.MasterAdjustDID)  //(nDid == DefinitionID.MasterParaDID)//主控参数
            {

            }
            else if (nDid == DIDNumber.PackParaDID)//pack参数
            {

            }
            else if (nDid >= DIDNumber.MasterAdjustDID && nDid < DIDNumber.MasterCtrlDID)  // 主控校准DID起止(1600, 17FF)
            {
                if (nDid == DIDNumber.MasterAdjustDID)  // 更新全部
                {

                }
                else
                {                  
    
                }
       
            }
            else if (nDid == DIDNumber.PackAdjustDID)//Pack校准
            {
            }
            else if (nDid == DIDNumber.ReadMasterTimeDID)
            {
                ReadMasterTime(appLayEvent, offset);
            }

        }

        #region ReadDataByID
        private void MasterRemoteTest(AppLayerEvent appLayEvent, int offset)
        {
            MainWindow.m_OperateType = OperateType.NoAction;

            if (appLayEvent.listData.Count < 2 + DIDNumber.MasterTeleBytesCount) // 1400 2个字节
            {                
                throw new Exception("读取所有主控参数 字节数错误");
            }

            int nOffset = 2; // 0,1 为 Did 0x1400

            //UpdateUIDataFormat.UpdateAllParaStringFormat(appLayEvent.listData, ListBmsInfo, ref nOffset);

        }



        private void ReadMasterTime(AppLayerEvent appLayEvent, int offset)
        {
            int year = AppLayProtocol.GetIntData(appLayEvent.listData, offset, 2);
            offset += 2;
            int month = AppLayProtocol.A_Data[offset++];
            int day = AppLayProtocol.A_Data[offset++];
            int hour = AppLayProtocol.A_Data[offset++];
            int miniute = AppLayProtocol.A_Data[offset++];
            int second = AppLayProtocol.A_Data[offset++];

        }
        #endregion

        private void SecurityAccess(AppLayerEvent appLayEvent)//安全访问服务
        {
                                                                           
            if ((SecurityAccessType)appLayEvent.listData[0] == SecurityAccessType.RequestSeed)
            {
                string strData = null;
                foreach (var v in appLayEvent.listData)
                {
                    strData +=","+ String.Format("{0:X2}", v);
                }
 
                if (appLayEvent.listData.Count == 3 && appLayEvent.listData[1] == 0x00 && appLayEvent.listData[2] == 0x00)
                {
                    //this.prompt.ResultText = (string)Application.Current.Resources["unLockSucessPromt"];//"解锁成功";
                }
                else
                {
                    byte[] seed = new byte[appLayEvent.listData.Count - 1];
                    for (int i = 1; i < appLayEvent.listData.Count; i++)
                    {
                        seed[i - 1] = appLayEvent.listData[i];
                    }
                    byte[] arrKey = getKey(seed);
                    //studioConfig.KeyCode = arrKey;
                    string strKey = string.Empty;
                    for (int i = 0; i < arrKey.Length; i++)
                    {
                        strKey += string.Format("{0:X2}", arrKey[i]) + " ";
                    }

                    switch (MainWindow.m_OperateType)
                    {
                        case OperateType.WriteMasterPara:
                        case OperateType.WriteAllMasterPara:
                        case OperateType.WriteMasterAdjustPara:
                        case OperateType.WriteAllMasterAdjustPara:
                        case OperateType.WriteMasterControlPara:
                        case OperateType.ReadBmsTime:
                            break;
                        default:
                            break;
                    }
                }
            }
            else if ((SecurityAccessType)appLayEvent.listData[0] == SecurityAccessType.SendKey)
            {
                if (appLayEvent.listData[0] == 0x02)//解锁成功
                {
                    switch (MainWindow.m_OperateType)
                    {
                        case OperateType.WriteMasterPara:                            
                            break;
                        case OperateType.WriteAllMasterPara:
                          
                            break;
                        case OperateType.WriteMasterAdjustPara:
                   
                            break;
                        case OperateType.WriteAllMasterAdjustPara:
             
                            break;
                        case OperateType.WriteMasterControlPara:
                                
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void ReadDTCInfo(AppLayerEvent appLayEvent)//读取DTC信息
        {
            //int offset = 0;
            byte reportType = appLayEvent.listData[0];
            if ((DTCReportType)reportType == DTCReportType.ReportNumberOfDTCByStatusMask)//报告DTC数目
            {

            }
            else if ((DTCReportType)reportType == DTCReportType.ReportDTCByStatusMask)//报告故障码
            {

            }
            else if ((DTCReportType)reportType == DTCReportType.ReportDTCSnapshotRecordByDTCNumber) //DTC快照
            {

            }

        }
        
        private byte[] getKey(byte[] seed)
        {
            byte[] result = new byte[seed.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(seed[i] << 1);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int getIntValue(byte msb, byte lsb)
        {
            int result = 0;
            result = msb << 8;
            result |= lsb;
            return result;
        }

        private uint getDtcCodeValue(int start, byte byteCount, List<byte> data)
        {
            uint result = 0;
            for (int i = 0; i < byteCount; i++)
            {
                result <<= 8;
                result |= data[start++];                
            }
            return result;
        }

        public DateTime GetDateTime(int start, int byteNum, List<byte> data)
        {
            if (byteNum != 6)
                throw new Exception("Illeagle byte Num for DateTime");

            int year = data[start + 1] + 2000;
            int month = data[start + 2];
            int day = data[start + 3];
            int hour = data[start + 4];
            int minute = data[start + 5];
            int second = data[start + 6];

            return new DateTime(year, month, day, hour, minute, second);
        } 

    }
}