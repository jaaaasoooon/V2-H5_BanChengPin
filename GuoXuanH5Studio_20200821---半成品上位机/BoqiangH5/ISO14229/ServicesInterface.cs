using System;
using System.Collections.Generic;
using BoqiangH5.ISO15765;

namespace BoqiangH5.ISO14229
{
    public class ServicesInterface
    {

        #region 发送服务打包方法

        /// <summary>
        /// 诊断会话控制
        /// </summary>
        /// <param name="sessionType">会话类型</param>
        /// <returns></returns>
        public ApplicationLayerProtocol DiagnosticSessionControl(DiagnosticSessionType sessionType, bool priority)
        {
            ParaInfo para;
            para.ByteLen = 1;
            para.Value = (byte)sessionType;
            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.DiagnosticSessionControl,priority, para);
            return newFrame;
        }


        /// <summary>
        /// ECU重启指令
        /// </summary>
        /// <param name="resetType">重启类型</param>
        /// <returns></returns>
        public ApplicationLayerProtocol ECUReset(ResetType resetType, bool priority)
        {
            ParaInfo para;
            para.ByteLen = 1;
            para.Value = (byte)resetType;
            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.ECUReset,priority, para);
            return newFrame;
        }

        /// <summary>
        /// 清楚故障码
        /// </summary>
        /// <param name="groupOfDTC"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol ClearDiagnosticInformation(byte[] groupOfDTC, bool priority)
        {
            ParaInfo[] paras = new ParaInfo[groupOfDTC.Length];
            for (int i = 0; i < groupOfDTC.Length; i++)
            {
                paras[i].ByteLen = 1;
                paras[i].Value = groupOfDTC[i];
            }
            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.ClearDiagnosticInformation,priority, paras);
            return newFrame;
        }

        /// <summary>
        /// 读DTC信息
        /// </summary>
        /// <param name="reportType">子功能码</param>
        /// <param name="bts">内容</param>
        /// <returns></returns>
        public ApplicationLayerProtocol ReadDTCInformation(DTCReportType reportType,bool priority,params byte[] bytes)
        {
            ParaInfo[] paras = new ParaInfo[bytes.Length - 1];
     
            int index = 0;
            for (int n = 1; n < bytes.Length; n++)
            {
                //int index = 1;
                paras[index].ByteLen = 1;
                paras[index].Value = bytes[n];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.ReadDTCInformation,priority, paras);
            return newFrame;

        }

        /// <summary>
        /// 通过标识符读取数据
        /// </summary>
        /// <param name="identifier">标识符数组</param>
        /// <returns></returns>
        public ApplicationLayerProtocol ReadDataByIdentifier(byte[] identifier, bool priority)
        {
            ParaInfo[] paras = new ParaInfo[identifier.Length];
            for (int i = 0; i < identifier.Length; i++)
            {
                paras[i].ByteLen = 1;
                paras[i].Value = identifier[i];
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.ReadDataByIdentifier,priority, paras);
            return newFrame;
        }


        /// <summary>
        /// 通过内存地址读取数据
        /// </summary>
        /// <param name="addressFormat">地址格式</param>
        /// <param name="startAddress">开始地址</param>
        /// <param name="readByteCount">读取字节数量</param>
        /// <returns></returns>
        public ApplicationLayerProtocol ReadMemoryByAddress(byte addressFormat,byte[] startAddress,bool priority,byte[] readByteCount)
        {
            int index = 0;
            ParaInfo[] paras = new ParaInfo[startAddress.Length + readByteCount.Length + 1];

            paras[index].ByteLen = 1;
            paras[index].Value = addressFormat;
            index++;

            for (int i = 0; i < startAddress.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = startAddress[i];
                index++;
            }

            for (int i = 0; i < readByteCount.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = readByteCount[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.ReadMemoryByAddress,priority, paras);
            return newFrame;
        }


        /// <summary>
        /// 通过标识符读标定数据
        /// </summary>
        /// <param name="identifier">标识符</param>
        /// <returns></returns>
        public ApplicationLayerProtocol ReadScalingDataByIdentifier(byte[] identifier, bool priority)
        {
            ParaInfo[] paras = new ParaInfo[identifier.Length];
            for (int i = 0; i < identifier.Length; i++)
            {
                paras[i].ByteLen = 1;
                paras[i].Value = identifier[i];
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.ReadScalingDataByIdentifier,priority, paras);
            return newFrame;
        }


        /// <summary>
        /// 安全访问
        /// </summary>
        /// <param name="accessType"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol SecurityAccess(SecurityAccessType accessType,bool priority,params byte[] content)
        {
            ParaInfo[] paras = new ParaInfo[content.Length + 1];

            paras[0].ByteLen = 1;
            paras[0].Value = (byte)accessType;

            int index = 1;
            for (int i = 0; i < content.Length; i++)
            {                   
                paras[index].ByteLen = 1;
                paras[index].Value = content[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.SecurityAccess, priority,paras);
            return newFrame;
        }


        /// <summary>
        /// 通讯控制
        /// </summary>
        /// <param name="ctrlType"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol CommunicationControl(ControlType ctrlType,bool priority,params byte[] content)
        {
            ParaInfo[] paras = new ParaInfo[content.Length + 1];

            paras[0].ByteLen = 1;
            paras[0].Value = (byte)ctrlType;

            for (int i = 0; i < content.Length; i++)
            {
                int index = 1;
                paras[index].ByteLen = 1;
                paras[index].Value = content[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.CommunicationControl,priority, paras);
            return newFrame;            
        }

        /// <summary>
        /// 周期访问
        /// </summary>
        /// <param name="transmissionMode"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol ReadDataByPeriodicIdentifier(TransmissionMode transmissionMode,bool priority, params byte[] content)
        {
            ParaInfo[] paras = new ParaInfo[content.Length + 1];

            paras[0].ByteLen = 1;
            paras[0].Value = (byte)transmissionMode;

            for (int i = 0; i < content.Length; i++)
            {
                int index = 1;
                paras[index].ByteLen = 1;
                paras[index].Value = content[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.ReadDataByPeriodicIdentifier,priority, paras);
            return newFrame;      
        }

        /// <summary>
        /// 动态定义标识符
        /// </summary>
        /// <param name="definitionType"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol DynamicallyDefineDataIdentifier(DefinitionType definitionType,bool priority, params byte[] content)
        {
            ParaInfo[] paras = new ParaInfo[content.Length + 1];

            paras[0].ByteLen = 1;
            paras[0].Value = (byte)definitionType;

            for (int i = 0; i < content.Length; i++)
            {
                int index = 1;
                paras[index].ByteLen = 1;
                paras[index].Value = content[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.DynamicallyDefineDataIdentifier,priority, paras);
            return newFrame;  
        }


        /// <summary>
        /// 通过标识符写数据
        /// </summary>
        /// <param name="arrIdentfier"></param>
        /// <param name="arrData"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol WriteDataByIdentifier(byte[] arrIdentfier,bool priority,params byte[] arrData)
        {
            ParaInfo[] paras = new ParaInfo[arrIdentfier.Length + arrData.Length];

            int index = 0;

            for (int i = 0; i < arrIdentfier.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = arrIdentfier[i];
                index++;
            }

            for (int i = 0; i < arrData.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = arrData[i];
                index++;
            }
            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.WriteDataByIdentifier,priority, paras);

            return newFrame;
        }

        /// <summary>
        /// 输入输出控制
        /// </summary>
        /// <param name="identfier"></param>
        /// <param name="ctrlOptRecord"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol InputOutputControlByIdentifier(byte[] identfier,ControlOptionRecord ctrlOptRecord,bool priority, params byte[] content)
        {
            ParaInfo[] paras = new ParaInfo[identfier.Length + content.Length + 1];

            int index = 0;

            for (int i = 0; i < identfier.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = identfier[i];
                index++;
            }

            paras[index].ByteLen = 1;
            paras[index].Value = (byte)ctrlOptRecord;

            index++;

            for (int i = 0; i < content.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = content[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.InputOutputControlByIdentifier,priority, paras);
            return newFrame;  
        }


        /// <summary>
        /// 例程控制
        /// </summary>
        /// <param name="rouCtrlType"></param>
        /// <param name="rouIdentifier"></param>
        /// <param name="routineControlOptionRecord"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol RoutineControl(RoutineControlType rouCtrlType,RoutineIdentifier rouIdentifier,bool priority,params byte[] routineControlOptionRecord)
        {
            ParaInfo[] paras = new ParaInfo[routineControlOptionRecord.Length + 2];

            paras[0].ByteLen = 1;
            paras[0].Value = (byte)rouCtrlType;

            paras[1].ByteLen = 2;
            paras[1].Value = (Int16)rouIdentifier;

            int index = 2;
            for (int i = 0; i < routineControlOptionRecord.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = routineControlOptionRecord[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.RoutineControl,priority, paras);
            return newFrame;
        }


        /// <summary>
        /// 请求下载数据
        /// </summary>
        /// <param name="dataFormatIdentifier"></param>
        /// <param name="addressAndLengthFormatIdentifier"></param>
        /// <param name="startAddress"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol RequestDownload(byte dataFormatIdentifier,byte addressAndLengthFormatIdentifier,List<byte> startAddress,byte[] size,bool priority)
        {          

            ParaInfo[] paras = new ParaInfo[startAddress.Count + size.Length+2];

            paras[0].ByteLen = 1;
            paras[0].Value = dataFormatIdentifier;

            paras[1].ByteLen = 2;
            paras[1].Value = addressAndLengthFormatIdentifier;

            int index = 2;
            for (int i = 0; i < startAddress.Count; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = startAddress[i];
                index++;
            }

            for (int i = 0; i < size.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = size[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.RequestDownload,priority, paras);
            return newFrame;
        }


        /// <summary>
        /// 请求上传数据
        /// </summary>
        /// <param name="dataFormatIdentifier"></param>
        /// <param name="addressAndLengthFormatIdentifier"></param>
        /// <param name="startAddress"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol RequestUpload(byte dataFormatIdentifier, byte addressAndLengthFormatIdentifier, byte[] startAddress, byte[] size, bool priority)
        {

            ParaInfo[] paras = new ParaInfo[startAddress.Length + size.Length];

            paras[0].ByteLen = 1;
            paras[0].Value = dataFormatIdentifier;

            paras[1].ByteLen = 2;
            paras[1].Value = addressAndLengthFormatIdentifier;

            int index = 2;
            for (int i = 0; i < startAddress.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = startAddress[i];
                index++;
            }

            for (int i = 0; i < size.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = size[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.RequestUpload,priority, paras);
            return newFrame;
        }


        /// <summary>
        /// 传输数据
        /// </summary>
        /// <param name="blockSequenceCounter"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol TransferData(byte blockSequenceCounter, bool priority, List<byte> data)
        {
            ParaInfo[] paras = new ParaInfo[data.Count + 1];
            paras[0].ByteLen = 1;
            paras[0].Value = blockSequenceCounter;

            int index = 1;
            for (int i = 0; i < data.Count; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = data[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.TransferData,priority, paras);
            return newFrame;
        }

        /// <summary>
        /// 退出传输
        /// </summary>
        /// <param name="transferRequestParameterRecord"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol RequestTransferExit(bool priority, params byte[] transferRequestParameterRecord)
        {
            ParaInfo[] paras = new ParaInfo[transferRequestParameterRecord.Length];

            for (int i = 0; i < transferRequestParameterRecord.Length; i++)
            {
                paras[i].ByteLen = 1;
                paras[i].Value = transferRequestParameterRecord[i];
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.RequestTransferExit,priority, paras);
            return newFrame;
        }

        /// <summary>
        /// 通过内存地址写数据
        /// </summary>
        /// <param name="addressFormat"></param>
        /// <param name="startAddress"></param>
        /// <param name="readByteCount"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol WriteMemoryByAddress(byte addressFormat, byte[] startAddress, byte[] readByteCount, bool priority)
        {
            int index = 0;
            ParaInfo[] paras = new ParaInfo[startAddress.Length + readByteCount.Length + 1];

            paras[index].ByteLen = 1;
            paras[index].Value = addressFormat;
            index++;

            for (int i = 0; i < startAddress.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = startAddress[i];
                index++;
            }

            for (int i = 0; i < readByteCount.Length; i++)
            {
                paras[index].ByteLen = 1;
                paras[index].Value = readByteCount[i];
                index++;
            }

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.WriteMemoryByAddress,priority, paras);
            return newFrame;
        }


        /// <summary>
        /// 检测仪在线
        /// </summary>
        /// <param name="sub_function"></param>
        /// <returns></returns>
        public static ApplicationLayerProtocol TesterPresent(byte sub_function, bool priority)
        {
            ParaInfo para1;
            para1.ByteLen = 1;
            para1.Value = sub_function;

            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.FucRequestID, ServicesID.TesterPresent, priority, para1);
            return newFrame;
        }


        /// <summary>
        /// 定时参数管理
        /// </summary>
        /// <param name="tpat"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol AccessTimingParameter(TimingParameterAccessType tpat, bool priority, params byte[] content)
        {
            ParaInfo[] paras = new ParaInfo[content.Length + 1];
            paras[0].ByteLen = 1;
            paras[0].Value = (byte)tpat;

            int index = 1;
            for (int i = 0; i < content.Length; i++)
            {
                paras[i].ByteLen = 1;
                paras[i].Value = content[i];
                index++;
            }
            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.AccessTimingParameter,priority, paras);
            return newFrame;

        }


        /// <summary>
        /// DTC设定
        /// </summary>
        /// <param name="settingType"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol ControlDTCSetting(DTCSettingType settingType, bool priority, params byte[] content)
        {
            ParaInfo[] paras = new ParaInfo[content.Length + 1];
            paras[0].ByteLen = 1;
            paras[0].Value = (byte)settingType;

            int index = 1;
            for (int i = 0; i < content.Length; i++)
            {
                paras[i].ByteLen = 1;
                paras[i].Value = content[i];
                index++;
            }
            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.ControlDTCSetting,priority, paras);
            return newFrame;
        }


        /// <summary>
        /// 链路控制
        /// </summary>
        /// <param name="ctrlType"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ApplicationLayerProtocol LinkControl(LinkControlType ctrlType, bool priority, params byte[] content)
        {
            ParaInfo[] paras = new ParaInfo[content.Length + 1];
            paras[0].ByteLen = 1;
            paras[0].Value = (byte)ctrlType;

            int index = 1;
            for (int i = 0; i < content.Length; i++)
            {
                paras[i].ByteLen = 1;
                paras[i].Value = content[i];
                index++;
            }
            ApplicationLayerProtocol newFrame = new ApplicationLayerProtocol(ApplicationLayerProtocol.RequestID, ServicesID.LinkControl,priority, paras);
            return newFrame;
        }

        #endregion

        #region 响应服务解析方法

        public static byte[] getVaildData(ApplicationLayerProtocol app)
        {
            byte[] result = new byte[app.A_Data.Count];

            byte responseCode = (byte)(app.A_PCI - 0x40);

            switch (responseCode)
            {
                case ServicesID.DiagnosticSessionControl:

                    break;

                case ServicesID.ECUReset:

                    break;

                case ServicesID.ClearDiagnosticInformation:

                    break;

                case ServicesID.ReadDTCInformation:

                    break;

                case ServicesID.ReadDataByIdentifier:

                    break;

                case ServicesID.ReadMemoryByAddress:

                    break;

                case ServicesID.ReadScalingDataByIdentifier:

                    break;

                case ServicesID.SecurityAccess:

                    break;

                case ServicesID.CommunicationControl:

                    break;

                case ServicesID.ReadDataByPeriodicIdentifier:

                    break;

                case ServicesID.DynamicallyDefineDataIdentifier:

                    break;

                case ServicesID.WriteDataByIdentifier:

                    break;

                case ServicesID.InputOutputControlByIdentifier:

                    break;

                case ServicesID.RoutineControl:

                    break;

                case ServicesID.RequestDownload:

                    break;

                case ServicesID.RequestUpload:

                    break;

                case ServicesID.TransferData:

                    break;

                case ServicesID.RequestTransferExit:

                    break;

                case ServicesID.WriteMemoryByAddress:

                    break;

                case ServicesID.TesterPresent:

                    break;

                case ServicesID.AccessTimingParameter:

                    break;

                case ServicesID.SecuredDataTransmission:

                    break;

                case ServicesID.ControlDTCSetting:

                    break;

                case ServicesID.LinkControl:

                    break;
            }

            return result;
        }

        public static string getErrorInfo(byte NRC)
        {
            string result = string.Empty;
            NegativeResponseCode nrc=(NegativeResponseCode)NRC;

            switch (nrc)
            {
                case NegativeResponseCode.GeneralReject:
                    result = "请求被拒绝";
                    break;

                case NegativeResponseCode.ServiceNotSupported:
                    result = "服务不支持";
                    break;

                case NegativeResponseCode.Sub_functionNotSupported:
                    result = "子功能不支持";
                    break;

                case NegativeResponseCode.IncorrectMessageLengthOrInvalidFormat:
                    result = "消息长度错误或格式无效";
                    break;

                case NegativeResponseCode.ResponseTooLong:
                    result = "响应太长";
                    break;

                case NegativeResponseCode.BusyRepeatRequest:
                    result = "重复请求";
                    break;

                case NegativeResponseCode.ConditionsNotCorrect:
                    result = "条件不满足";
                    break;

                case NegativeResponseCode.RequestSequenceError:
                    result = "请求序列错误";
                    break;

                case NegativeResponseCode.NoResponseFromSubnetComponent:
                    result = "子网无响应";
                    break;

                case NegativeResponseCode.FailurePreventsExecutionOfRequestedAction:
                    result = "请求动作执行失败";
                    break;

                case NegativeResponseCode.RequestOutOfRange:
                    result = "请求超出范围";

                    break;

                case NegativeResponseCode.SecurityAccessDenied:
                    result = "安全访问被拒绝";
                    break;

                case NegativeResponseCode.InvalidKey:
                    result = "Key 无效";
                    break;

                case NegativeResponseCode.ExceedNumberOfAttempts:
                    result = "超过尝试次数";
                    break;

                case NegativeResponseCode.RequiredTimeDelayNotExpired:
                    result = "延时时间未到";
                    break;

                case NegativeResponseCode.UploadDownloadNotAccepted:
                    result = "上传下载未被接收";
                    break;

                case NegativeResponseCode.TransferDataSuspended:
                    result = "数据传输被暂停";
                    break;

                case NegativeResponseCode.GeneralProgrammingFailure:
                    result = "一般编程失败";
                    break;

                case NegativeResponseCode.WrongBlockSequenceCounter:

                    result = "块序列计数错误";
                    break;

                case NegativeResponseCode.RequestCorrectlyReceived_ResponsePending:
                    result = "请求正确等待接受响应";
                    break;

                case NegativeResponseCode.Sub_FunctionNotSupportedInActiveSession:
                    result = "当前会话不支持该子功能";
                    break;

                case NegativeResponseCode.ServiceNotSupportedInActiveSession:
                    result = "当前会话不支持该服务";
                    break;

                case NegativeResponseCode.RpmTooHigh:
                    result = "RPM 太高";
                    break;

                case NegativeResponseCode.RpmTooLow:
                    result = "RPM 太低";
                    break;

                case NegativeResponseCode.EngineIsRunning:
                    result = "引擎正在运行";
                    break;

                case NegativeResponseCode.EngineIsNotRunning:
                    result = "引擎没有运行";
                    break;

                case NegativeResponseCode.EngineRunTimeTooLow:
                    result = "引擎运转时间太短";
                    break;

                case NegativeResponseCode.TemperatureTooHigh:
                    result = "温度太高";
                    break;

                case NegativeResponseCode.TemperatureTooLow:
                    result = "温度太低";
                    break;

                case NegativeResponseCode.VehicleSpeedTooHigh:
                    result = "车速太快";
                    break;

                case NegativeResponseCode.VehicleSpeedTooLow:
                    result = "车速太慢";
                    break;

                case NegativeResponseCode.Throttle_PedalTooHigh:
                    result = "油门踏板太高";
                    break;

                case NegativeResponseCode.Throttle_PedalTooLow:
                    result = "油门踏板太低";
                    break;

                case NegativeResponseCode.TransmissionRangeNotInNeutral:
                    result = "变速器不在空档";
                    break;

                case NegativeResponseCode.TransmissionRangeNotInGear:
                    result = "变速器不在档位上";
                    break;

                case NegativeResponseCode.BrakeSwitchNotClosed:
                    result = "手刹未放开";
                    break;

                case NegativeResponseCode.ShifterLeverNotInPark:
                    result = "档位不在停车挡";
                    break;

                case NegativeResponseCode.TorqueConverterClutchLocked:
                    result = "离合器锁止";
                    break;

                case NegativeResponseCode.VoltageTooHigh:
                    result = "电压太高";
                    break;

                case NegativeResponseCode.VoltageTooLow:
                    result = "电压太低";
                    break;
            } 

            return result;
        }

        #endregion


    }

    

    
}
