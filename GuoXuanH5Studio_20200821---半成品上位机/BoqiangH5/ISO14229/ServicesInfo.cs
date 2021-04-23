using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDSStudio.ISO14229
{

    public class DefinitionID
    {
        #region DID
        public const int MasterTelemDID = 0x1000;      //主控遥测
        public const int MasterParaDID = 0x1200;       //主控参数
        public const int MasterAdjustDID = 0x1400;     //主控校准
        public const int MasterCtrlDID = 0x1600;       //主控控制
        public const int PackTelemDID = 0x2000;        //Pack遥测
        public const int PackParaDID = 0x2500;         //Pack参数
        public const int PackAdjustDID = 0x2180;       //Pack校准
        public const int PackCtrlDID = 0x2200;         //Pack控制
        public const int ReadMasterTimeDID = 0x1602;    //BMS时间
        #endregion
    }

    public class ServicesID
    {
        #region 服务ID
        public const byte DiagnosticSessionControl = 0x10;           //会话控制
        public const byte ECUReset = 0x11;                           //ECU复位
        public const byte ClearDiagnosticInformation = 0x14;         //清除故障码
        public const byte ReadDTCInformation = 0x19;                 //读取故障码
        public const byte ReadDataByIdentifier = 0x22;               //读取数据
        public const byte ReadMemoryByAddress = 0x23;                //由地址读取内存
        public const byte ReadScalingDataByIdentifier = 0x24;        //读标定信息        
        public const byte SecurityAccess = 0x27;                     //安全访问
        public const byte CommunicationControl = 0x28;               //通讯控制
        public const byte ReadDataByPeriodicIdentifier = 0x2A;       //周期性读数据
        public const byte DynamicallyDefineDataIdentifier = 0x2C;    //动态定义数据
        public const byte WriteDataByIdentifier = 0x2E;              //写数据
        public const byte InputOutputControlByIdentifier = 0x2F;     //输入输出控制
        public const byte RoutineControl = 0x31;                     //例程控制
        public const byte RequestDownload = 0x34;                    //请求下载
        public const byte RequestUpload = 0x35;                      //请求上传
        public const byte TransferData = 0x36;                       //传输数据
        public const byte RequestTransferExit = 0x37;                //请求退出传输
        public const byte WriteMemoryByAddress = 0x3D;               //写入内存
        public const byte TesterPresent = 0x3E;                      //诊断仪在线
        public const byte AccessTimingParameter = 0x83;              //访问定时参数
        public const byte SecuredDataTransmission = 0x84;            //安全数据传输
        public const byte ControlDTCSetting = 0x85;                  //控制DTC设置
        public const byte LinkControl = 0x87;                        //链路控制  
        #endregion
    }

    /// <summary>
    /// 会话类型
    /// </summary>
    public enum DiagnosticSessionType
    {
        DefaultSession = 0x01,
        ProgrammingSession = 0x02,
        ExtendedDiagnosticSession = 0x03
    }


    /// <summary>
    /// ECU重启类型
    /// </summary>
    public enum ResetType
    {
        HardReset=0x01,
        KeyOffOnReset=0x02,
        SoftReset=0x03,
        EnableRapidPowerShutDown=0x04,
        DisableRapidPowerShutDown=0x05
    }

    /// <summary>
    /// DTC
    /// </summary>
    public enum ReportType
    {
        ReportNumberOfDTCByStatusMask=0x01,                     //This parameter specifies that the server shall transmit to the client the
                                                                //number of DTCs matching a client defined status mask.

        ReportDTCByStatusMask=0x02,                             //This parameter specifies that the server shall transmit to the client a list of
                                                                //DTCs and corresponding statuses matching a client defined status mask.

        ReportDTCSnapshotIdentification=0x03,                   //This parameter specifies that the server shall transmit to the client all
                                                                //DTCSnapshot data record identifications (DTC number(s) and DTCSnapshot record number(s)).

        ReportDTCSnapshotRecordByDTCNumber=0x04,                //This parameter specifies that the server shall transmit to the client the
                                                                //DTCSnapshot record(s) associated with a client defined DTC number and
                                                                //DTCSnapshot record number (0xFF for all records).

        ReportDTCStoredDataByRecordNumber=0x05,                 //This parameter specifies that the server shall transmit to the client the
                                                                //DTCStoredDatarecord(s) associated with a client defined DTCStoredData
                                                                //record number (0xFF for all records).

        ReportDTCExtDataRecordByDTCNumber=0x06,                 //This parameter specifies that the server shall transmit to the client the
                                                                //DTCExtendedData record(s) associated with a client defined DTC number
                                                                //and DTCExtendedData record number (0xFF for all records, 0xFE for all
                                                                //OBD records).

        ReportNumberOfDTCBySeverityMaskRecord=0x07,             //This parameter specifies that the server shall transmit to the client the
                                                                //number of DTCs matching a client defined severity mask record.

        ReportDTCBySeverityMaskRecord=0x08,                     //This parameter specifies that the server shall transmit to the client a list of
                                                                //DTCs and corresponding statuses matching a client defined severity mask
                                                                //record.

        ReportSeverityInformationOfDTC=0x09,                    //This parameter specifies that the server shall transmit to the client the
                                                                //severity information of a specific DTC specified in the client request message.

        ReportSupportedDTC=0x0A,                                //This parameter specifies that the server shall transmit to the client a list of
                                                                //all DTCs and corresponding statuses supported within the server.

        ReportFirstTestFailedDTC=0x0B,                          //This parameter specifies that the server shall transmit to the client the first
                                                                //failed DTC to be detected by the server since the last clear of diagnostic
                                                                //information. Note that the information reported via this sub-function
                                                                //parameter shall be independent of whether or not the DTC was confirmed
                                                                //or aged.

        ReportFirstConfirmedDTC=0x0C,                           //This parameter specifies that the server shall transmit to the client the first
                                                                //confirmed DTC to be detected by the server since the last clear of
                                                                //diagnostic information.
                                                                //The information reported via this sub-function parameter shall be
                                                                //independent of the aging process of confirmed DTCs (e.g. if a DTC ages
                                                                //such that its status is allowed to be reset, the first confirmed DTC record
                                                                //shall continue to be preserved by the server, regardless of any other DTCs
                                                                //that become confirmed afterwards).

        ReportMostRecentTestFailedDTC=0x0D,                     //This parameter specifies that the server shall transmit to the client the most
                                                                //recent failed DTC to be detected by the server since the last clear of
                                                                //diagnostic information. Note that the information reported via this subfunction
                                                                //parameter shall be independent of whether or not the DTC was
                                                                //confirmed or aged.

        ReportMostRecentConfirmedDTC=0x0E,                      //This parameter specifies that the server shall transmit to the client the most
                                                                //recent confirmed DTC to be detected by the server since the last clear of
                                                                //diagnostic information.
                                                                //Note that the information reported via this sub-function parameter shall be
                                                                //independent of the aging process of confirmed DTCs (e.g. if a DTC ages
                                                                //such that its status is allowed to be reset, the first confirmed DTC record
                                                                //shall continue to be preserved by the server assuming no other DTCs
                                                                //become confirmed afterwards).

        ReportMirrorMemoryDTCByStatusMask=0x0F,                 //This parameter specifies that the server shall transmit to the client a list of
                                                                //DTCs out of the DTC mirror memory and corresponding statuses matching
                                                                //a client defined status mask.

        RMMDEDRBDN = 0x10,                                      //report Mirror Memory DTC ExtDataRecord By DTCNumber
                                                                //This parameter specifies that the server shall transmit to the client the
                                                                //DTCExtendedData record(s) - out of the DTC mirror memory - associated
                                                                //with a client defined DTC number and DTCExtendedData record number
                                                                //(0xFF for all records, 0xFE for all OBD records) DTCs.

        RNOMMDTCBSM = 0x11,                                     //report Number Of Mirror Memory DTC By StatusMask
                                                                //This parameter specifies that the server shall transmit to the client the
                                                                //number of DTCs out of mirror memory matching a client defined status
                                                                //mask.

        RNOOEOBDDTCBSM = 0x12,                                  //report Number Of Emissions OBD DTC By StatusMask
                                                                //This parameter specifies that the server shall transmit to the client the
                                                                //number of emissions-related OBD DTCs matching a client defined status
                                                                //mask. The number of OBD DTCs reported shall only be those which are
                                                                //required to be compatible with emissions-related legal requirements.

        ROBDDTCBSM = 0x13,                                      //report Emissions OBD DTC By StatusMask
                                                                //This parameter specifies that the server shall transmit to the client a list of
                                                                //emissions-related OBD DTCs and corresponding statuses matching a
                                                                //client defined status mask. The list of OBD DTCs reported shall only be
                                                                //those which are required to be compatible with emissions-related legal
                                                                //requirements.
        ReportDTCFaultDetectionCounter=0x14,                    //

        ReportDTCWithPermanentStatus=0x15,                      //This parameter specifies that the server shall transmit to the client a list of
                                                                //DTCs with "permanent DTC" status as described in 3.1.

        RDTCEDBR = 0x16,                                        //report DTC Ext DataRecord By RecordNumber
                                                                //This parameter specifies that the server shall transmit to the client the
                                                                //DTCExtendedData records associated with a client defined
                                                                //DTCExtendedData record number less than 0xF0.

        RUDMDTCBSM = 0x17,                                      //report User Def Memory DTC By StatusMask
                                                                //This parameter specifies that the server shall transmit to the client a list of
                                                                //DTCs out of the user defined DTC memory and corresponding statuses
                                                                //matching a client defined status mask.

        RUDMDTCSSBDTC = 0x18,                                   //report User Def Memory DTC Snapshot Record By DTCNumber
                                                                //This parameter specifies that the server shall transmit to the client the
                                                                //DTCSnapshot record(s) – out of the user defined DTC memory -
                                                                //associated with a client defined DTC number and DTCSnapshot record
                                                                //number (0xFF for all records).

        RUDMDTCEDRBDN = 0x19,                                   //report User Def Memory DTC Ext DataRecord By DTCNumber
                                                                //This parameter specifies that the server shall transmit to the client the
                                                                //DTCExtendedData record(s) – out of the user defined DTC memory -
                                                                //associated with a client defined DTC number and DTCExtendedData
                                                                //record number (0xFF for all records).

        RWWHOBDDTCBMR = 0x42,                                   //reportWWHOBDDTCByMaskRecord
                                                                //This parameter specifies that the server shall transmit to the client a list of
                                                                //WWH OBD DTCs and corresponding status and severity information
                                                                //matching a client defined status mask and severity mask record.

        RWWHOBDDTCWPS = 0x55,                                   //reportWWHOBDDTCWithPermanentStatus
                                                                //This parameter specifies that the server shall transmit to the client a list of
                                                                //WWH OBD DTCs with "permanent DTC" status as described in 3.1.

    }


    /// <summary>
    /// 安全访问类型
    /// </summary>
    public enum SecurityAccessType
    {
        RequestSeed = 0x01,             //RequestSeed with the level of security defined by the vehicle manufacturer.
        SendKey = 0x02,                 //SendKey with the level of security defined by the vehicle manufacturer.
    }


    /// <summary>
    /// 通讯控制类型
    /// </summary>
    public enum ControlType
    {
        EnableRxAndTx=0x00,             //This value indicates that the reception and transmission of messages shall be
                                        //enabled for the specified communicationType.

        EnableRxAndDisableTx=0x01,      //This value indicates that the reception of messages shall be enabled and the
                                        //transmission shall be disabled for the specified communicationType.

        DisableRxAndEnableTx=0x02,      //This value indicates that the reception of messages shall be disabled and the
                                        //transmission shall be enabled for the specified communicationType.

        DisableRxAndTx=0x03,            //This value indicates that the reception and transmission of messages shall be
                                        //disabled for the specified communicationType.

        ERXDTXWEAI = 0x04,              //enable Rx And Disable Tx With Enhanced Address Information
                                        //This value indicates that the addressed bus master shall switch the related
                                        //sub-bus segment to the diagnostic-only scheduling mode.

        ERXTXWEAI = 0x05,               //enable Rx And Tx With Enhanced Address Information
                                        //This value indicates that the addressed bus master shall switch the related
                                        //sub-bus segment to the application scheduling mode.
    }


    /// <summary>
    /// 周期性传输数据的传输模式
    /// </summary>
    public enum TransmissionMode
    {
        SendAtSlowRate=0x01,
        SendAtMediumRate=0x02,
        SendAtFastRate=0x03,
        StopSending=0x04
    }

    /// <summary>
    /// 动态定义数据定义类型
    /// </summary>
    public enum DefinitionType
    {
        DefineByIdentifier=0x01,
        DefineByMemoryAddress=0x02,
        ClearDynamicallyDefinedDataIdentifier=0x03
    }

    /// <summary>
    /// 输入输出控制
    /// </summary>
    public enum ControlOptionRecord
    {
        ReturnControlToECU=0x00,
        ResetToDefault=0x01,
        FreezeCurrentState=0x02,
        ShortTermAdjustment=0x03
    }


    /// <summary>
    /// 例程控制类型
    /// </summary>
    public enum RoutineControlType
    {
        StartRoutine=0x01,
        StopRoutine=0x02,
        RequestRoutineResults=0x03
    }

    /// <summary>
    /// 例程控制可用标识符
    /// </summary>
    public enum RoutineIdentifier
    {
        DeployLoopRoutineID=0xE200,
        EraseMemory=0xFF00,
        CheckProgrammingDependencies=0xFF01,
        EraseMirrorMemoryDTCs=0xFF02
    }

    /// <summary>
    /// 定时参数子功能
    /// </summary>
    public enum TimingParameterAccessType
    {
        RETPS=0x01,
        STPTDV=0x02,
        RCATP=0x03,
        STPTGV=0x04
    }


    /// <summary>
    /// DTC控制类型
    /// </summary>
    public enum DTCSettingType
    {
        ON=0x01,
        OFF=0x02
    }

    /// <summary>
    /// 链路控制类型
    /// </summary>
    public enum LinkControlType
    {
        VMTWFP=0x01,                //verify Mode Transition With Fixed Parameter
                                    //This parameter is used to verify if a transition with a pre-defined parameter,
                                    //which is specified by the linkControlModeIdentifier data-parameter can be
                                    //performed.

        VMTWSP=0x02,                //verify Mode Transition With Specific Parameter 
                                    //This parameter is used to verify if a transition to a specifically defined
                                    //parameter (e.g., specific baudrate), which is specified by the linkRecord
                                    //data-parameter can be performed.

        TransitionMode=0x03,        //This sub-function parameter requests the server(s) to transition the data link
                                    //into the mode which was requested in the preceding verification message.
    }

    /// <summary>
    /// 消极响应码
    /// </summary>
    public enum NegativeResponseCode
    {
        GeneralReject = 0x10,                               //This NRC indicates that the requested action has been rejected by the server.
                                                            //The generalReject response code shall only be implemented in the server if none
                                                            //of the negative response codes defined in this document meet the needs of the
                                                            //implementation. At no means shall this NRC be a general replacement for the
                                                            //response codes defined in this document.

        ServiceNotSupported = 0x11,                         //This NRC indicates that the requested action will not be taken because the server
                                                            //does not support the requested service.The server shall send this NRC in case the client has sent a request message with
                                                            //a service identifier which is unknown, not supported by the server, or is specified as
                                                            //a response service identifier. Therefore this negative response code is not shown in
                                                            //the list of negative response codes to be supported for a diagnostic service,
                                                            //because this negative response code is not applicable for supported services.

        Sub_functionNotSupported = 0x12,                    //This NRC indicates that the requested action will not be taken because the server
                                                            //does not support the service specific parameters of the request message.
                                                            //The server shall send this NRC in case the client has sent a request message with
                                                            //a known and supported service identifier but with "sub-function“ which is either
                                                            //unknown or not supported.

        IncorrectMessageLengthOrInvalidFormat = 0x13,       //This NRC indicates that the requested action will not be taken because the length
                                                            //of the received request message does not match the prescribed length for the
                                                            //specified service or the format of the paramters do not match the prescribed format
                                                            //for the specified service.

        ResponseTooLong = 0x14,                             //This NRC shall be reported by the server if the response to be generated exceeds
                                                            //the maximum number of bytes available by the underlying network layer. This
                                                            //could occur if the response message exceeds the maximum size allowed by the
                                                            //underlying transport protocol or if the response message exceeds the server buffer
                                                            //size allocated for that purpose.
                                                            //EXAMPLE This problem may occur when several DIDs at a time are requested
                                                            //and the combination of all DIDs in the response exceeds the limit of the underlying
                                                            //transport protocol.

        BusyRepeatRequest = 0x21,                           //This NRC indicates that the server is temporarily too busy to perform the requested
                                                            //operation. In this circumstance the client shall perform repetition of the "identical
                                                            //request message" or "another request message". The repetition of the request shall
                                                            //be delayed by a time specified in the respective implementation documents.
                                                            //EXAMPLE In a multi-client environment the diagnostic request of one client
                                                            //might be blocked temporarily by a NRC 0x21 while a different client finishes a
                                                            //diagnostic task.
                                                            //If the server is able to perform the diagnostic task but needs additional time to finish
                                                            //the task and prepare the response, the NRC 0x78 shall be used instead of NRC
                                                            //0x21.
                                                            //This NRC is in general supported by each diagnostic service, as not otherwise
                                                            //stated in the data link specific implementation document, therefore it is not listed in
                                                            //the list of applicable response codes of the diagnostic services.

        ConditionsNotCorrect = 0x22,                        //This NRC indicates that the requested action will not be taken because the server
                                                            //prerequisite conditions are not met.

        RequestSequenceError = 0x24,                        //This NRC indicates that the requested action will not be taken because the server
                                                            //expects a different sequence of request messages or message as sent by the
                                                            //client. This may occur when sequence sensitive requests are issued in the wrong
                                                            //order.
                                                            //EXAMPLE A successful SecurityAccess service specifies a sequence of
                                                            //requestSeed and sendKey as sub-fuctions in the request messages. If the
                                                            //sequence is sent different by the client the server shall send a negative response
                                                            //message with the negative response code 0x24 requestSequenceError.

        NoResponseFromSubnetComponent = 0x25,               //This NRC indicates that the server has received the request but the requested
                                                            //action could not be performed by the server as a subnet component which is
                                                            //necessary to supply the requested information did not respond within the specified
                                                            //time.
                                                            //The noResponseFromSubnetComponent negative response shall be implemented
                                                            //by gateways in electronic systems which contain electronic subnet components
                                                            //and which do not directly respond to the client's request. The gateway may receive
                                                            //the request for the subnet component and then request the necessary information
                                                            //from the subnet component. If the subnet component fails to respond, the server
                                                            //shall use this negative response to inform the client about the failure of the subnet
                                                            //component.
                                                            //This NRC is in general supported by each diagnostic service, as not otherwise
                                                            //stated in the data link specific implementation document, therefore it is not listed in
                                                            //the list of applicable response codes of the diagnostic services.

        FailurePreventsExecutionOfRequestedAction = 0x26,   //This NRC indicates that the requested action will not be taken because a failure
                                                            //condition, identified by a DTC (with at least one DTC status bit for TestFailed,
                                                            //Pending, Confirmed or TestFailedSinceLastClear set to 1), has occurred and that
                                                            //this failure condition prevents the server from performing the requested action.
                                                            //This NRC can, for example, direct the technician to read DTCs in order to identify
                                                            //and fix the problem.
                                                            //NOTE This implies that diagnostic services used to access DTCs shall not
                                                            //implement this NRC as an external test tool may check for the above NRC and
                                                            //automatically request DTCs whenever the above NRC has been received.
                                                            //This NRC is in general supported by each diagnostic service (except the services
                                                            //mentioned above), as not otherwise stated in the data link specific implementation
                                                            //document, therefore it is not listed in the list of applicable response codes of the
                                                            //diagnostic services.

        RequestOutOfRange = 0x31,                           //This NRC indicates that the requested action will not be taken because the server
                                                            //has detected that the request message contains a parameter which attempts to
                                                            //substitute a value beyond its range of authority (e.g. attempting to substitute a data
                                                            //byte of 111 when the data is only defined to 100), or which attempts to access a
                                                            //dataIdentifier/routineIdentifer that is not supported or not supported in active
                                                            //session.
                                                            //This NRC shall be implemented for all services, which allow the client to read data,
                                                            //write data or adjust functions by data in the server.

        SecurityAccessDenied = 0x33,                        //This NRC indicates that the requested action will not be taken because the
                                                            //server's security strategy has not been satisfied by the client.
                                                            //The server shall send this NRC if one of the following cases occur:
                                                            //⎯ the test conditions of the server are not met,
                                                            //⎯ the required message sequence e.g. DiagnosticSessionControl,
                                                            //securityAccess is not met,
                                                            //⎯ the client has sent a request message which requires an unlocked server.
                                                            //Beside the mandatory use of this negative response code as specified in the
                                                            //applicable services within this standard, this negative response code can also be
                                                            //used for any case where security is required and is not yet granted to perform the
                                                            //required service.

        InvalidKey = 0x35,                                  //This NRC indicates that the server has not given security access because the key
                                                            //sent by the client did not match with the key in the server's memory. This counts
                                                            //as an attempt to gain security. The server shall remain locked and increment ist
                                                            //internal securityAccessFailed counter.

        ExceedNumberOfAttempts = 0x36,                      //This NRC indicates that the requested action will not be taken because the client
                                                            //has unsuccessfully attempted to gain security access more times than the server's
                                                            //security strategy will allow.

        RequiredTimeDelayNotExpired = 0x37,                 //This NRC indicates that the requested action will not be taken because the client's
                                                            //latest attempt to gain security access was initiated before the server's required
                                                            //timeout period had elapsed.

        UploadDownloadNotAccepted = 0x70,                   //This NRC indicates that an attempt to upload/download to a server's memory
                                                            //cannot be accomplished due to some fault conditions.

        TransferDataSuspended = 0x71,                       //This NRC indicates that a data transfer operation was halted due to some fault.
                                                            //The active transferData sequence shall be aborted.

        GeneralProgrammingFailure = 0x72,                   //This NRC indicates that the server detected an error when erasing or programming
                                                            //a memory location in the permanent memory device (e.g. Flash Memory).

        WrongBlockSequenceCounter = 0x73,                   //This NRC indicates that the server detected an error in the sequence of
                                                            //blockSequenceCounter values. Note that the repetition of a TransferData request
                                                            //message with a blockSequenceCounter equal to the one included in the previous
                                                            //TransferData request message shall be accepted by the server.

        RequestCorrectlyReceived_ResponsePending = 0x78,    //This NRC indicates that the request message was received correctly, and that all
                                                            //parameters in the request message were valid, but the action to be performed is
                                                            //not yet completed and the server is not yet ready to receive another request. As
                                                            //soon as the requested service has been completed, the server shall send a
                                                            //positive response message or negative response message with a response code
                                                            //different from this.

        Sub_FunctionNotSupportedInActiveSession = 0x7E,     //This NRC indicates that the requested action will not be taken because the server
                                                            //does not support the requested sub-function in the session currently active. This
                                                            //NRC shall only be used when the requested sub-function is known to be supported
                                                            //in another session, otherwise response code SFNS (sub-functionNotSupported)
                                                            //shall be used (e.g., servers executing the boot software generally do not know
                                                            //which subfunctions are supported in the application (and vice versa) and therefore
                                                            //may need to respond with NRC 0x12 instead).
                                                            //This NRC shall be supported by each diagnostic service with a sub-function
                                                            //parameter, if not otherwise stated in the data link specific implementation
                                                            //document, therefore it is not listed in the list of applicable response codes of the
                                                            //diagnostic services.

        ServiceNotSupportedInActiveSession = 0x7F,          //This NRC indicates that the requested action will not be taken because the server
                                                            //does not support the requested service in the session currently active. This NRC
                                                            //shall only be used when the requested service is known to be supported in another
                                                            //session, otherwise response code SNS (serviceNotSupported) shall be used (e.g.,
                                                            //servers executing the boot software generally do not know which services are
                                                            //supported in the application (and vice versa) and therefore may need to respond
                                                            //with NRC 0x11 instead).
                                                            //This NRC is in general supported by each diagnostic service, as not otherwise
                                                            //stated in the data link specific implementation document, therefore it is not listed in
                                                            //the list of applicable response codes of the diagnostic services.

        RpmTooHigh = 0x81,                                //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for RPM is not met (current RPM is above a preprogrammed
        //maximum threshold).

        RpmTooLow = 0x82,                                 //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for RPM is not met (current RPM is below a preprogrammed
        //minimum threshold).

        EngineIsRunning = 0x83,                           //This NRC is required for those actuator tests which cannot be actuated while the
        //Engine is running. This is different from RPM too high negative response, and
        //needs to be allowed.

        EngineIsNotRunning = 0x84,                        //This NRC is required for those actuator tests which cannot be actuated unless the
        //Engine is running. This is different from RPM too low negative response, and
        //needs to be allowed.

        EngineRunTimeTooLow = 0x85,                       //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for engine run time is not met (current engine run time is
        //below a pre-programmed limit).

        TemperatureTooHigh = 0x86,                        //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for temperature is not met (current temperature is above a
        //pre-programmed maximum threshold).

        TemperatureTooLow = 0x87,                         //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for temperature is not met (current temperature is below a
        //pre-programmed minimum threshold).

        VehicleSpeedTooHigh = 0x88,                       //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for vehicle speed is not met (current VS is above a preprogrammed
        //maximum threshold).

        VehicleSpeedTooLow = 0x89,                        //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for vehicle speed is not met (current VS is below a preprogrammed
        //minimum threshold).

        Throttle_PedalTooHigh = 0x8A,                     //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for throttle/pedal position is not met (current TP/APP is
        //above a pre-programmed maximum threshold).

        Throttle_PedalTooLow = 0x8B,                      //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for throttle/pedal position is not met (current TP/APP is
        //below a pre-programmed minimum threshold).

        TransmissionRangeNotInNeutral = 0x8C,             //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for being in neutral is not met (current transmission range is
        //not in neutral).

        TransmissionRangeNotInGear = 0x8D,                //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for being in gear is not met (current transmission range is
        //not in gear).

        BrakeSwitchNotClosed = 0x8F,                      //This NRC indicates that for safety reasons, this is required for certain tests before
        //it begins, and must be maintained for the entire duration of the test.

        ShifterLeverNotInPark = 0x90,                     //This NRC indicates that for safety reasons, this is required for certain tests before
        //it begins, and must be maintained for the entire duration of the test.

        TorqueConverterClutchLocked = 0x91,               //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for torque converter clutch is not met (current TCC status
        //above a pre-programmed limit or locked).

        VoltageTooHigh = 0x92,                            //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for voltage at the primary pin of the server (ECU) is not met
        //(current voltage is above a pre-programmed maximum threshold).

        VoltageTooLow = 0x93,                             //This NRC indicates that the requested action will not be taken because the server
        //prerequisite condition for voltage at the primary pin of the server (ECU) is not met
        //(current voltage is below a pre-programmed maximum threshold).

    }
}
