using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BoqiangH5
{
    public class StudioCommands
    {
        /// <summary>
        /// 操作设备命令
        /// </summary>
        private static RoutedUICommand operateDevice = new RoutedUICommand("OperateDevice", "OperateDevice", typeof(StudioCommands));
        public static RoutedUICommand OperateDevice
        {
            get { return operateDevice; }
        }

        /// <summary>
        /// 处理数据命令
        /// </summary>
        private static RoutedUICommand dealData = new RoutedUICommand("DealData", "DealData", typeof(StudioCommands));
        public static RoutedUICommand DealData
        {
            get { return dealData; }
        }

        /// <summary>
        /// 加载命令
        /// </summary>
        private static RoutedUICommand loadCommand;
        public static RoutedUICommand LoadCommand
        {
            get { return loadCommand; }
        }

        /// <summary>
        /// 上传命令
        /// </summary>
        private static RoutedUICommand upLoadCommand;
        public static RoutedUICommand UpLoadCommand
        {
            get { return upLoadCommand; }
        }

        /// <summary>
        /// 查看记录命令
        /// </summary>
        private static RoutedUICommand viewRecordsCommand;
        public static RoutedUICommand ViewRecordsCommand
        {
            get { return viewRecordsCommand; }
        }

        /// <summary>
        /// 校准管理
        /// </summary>
        private static RoutedUICommand adjustManageCommand;
        public static RoutedUICommand AdjustManageCommand
        {
            get { return adjustManageCommand; }
        }

        private static RoutedUICommand writeAdjustManageCommand;
        public static RoutedUICommand WriteAdjustManageCommand
        {
            get { return writeAdjustManageCommand; }
        }

        /// <summary>
        /// 调试管理
        /// </summary>
        private static RoutedUICommand debugManageCommand;
        public static RoutedUICommand DebugManageCommand
        {
            get { return debugManageCommand; }
        }

        /// <summary>
        /// 辅助信息查看
        /// </summary>
        private static RoutedUICommand aidInfoManageCommand;
        public static RoutedUICommand AidInfoManageCommand
        {
            get { return aidInfoManageCommand; }
        }

        /// <summary>
        /// pack信息查看
        /// </summary>
        private static RoutedUICommand viewPackInfoCommand;
        public static RoutedUICommand ViewPackInfoCommand
        {
            get { return viewPackInfoCommand; }
        }

        /// <summary>
        /// 设置命令
        /// </summary>
        private static RoutedUICommand settingCommand;
        public static RoutedUICommand SettingCommand
        {
            get { return settingCommand; }
        }

        /// <summary>
        /// 语言选择命令
        /// </summary>
        private static RoutedUICommand languageSelectCommand;
        public static RoutedUICommand LanguageSelectCommand
        {
            get { return languageSelectCommand; }
        }

        /// <summary>
        /// 保存文件命令
        /// </summary>
        private static RoutedUICommand saveFileCommand;
        public static RoutedUICommand SaveFileCommand
        {
            get { return saveFileCommand; }
        }

        /// <summary>
        /// 设置所有参数
        /// </summary>
        private static RoutedUICommand setAllParams;
        public static RoutedUICommand SetAllParams
        {
            get { return setAllParams; }
        }

        /// <summary>
        /// 设置单个参数
        /// </summary>
        private static RoutedUICommand setOnePara;
        public static RoutedUICommand SetOnePara
        {
            get { return setOnePara; }
        }

        /// <summary>
        /// 保存参数设置指令
        /// </summary>
        private static RoutedUICommand saveParaSetCommand;
        public static RoutedUICommand SaveParaSetCommand
        {
            get { return saveParaSetCommand; }
        }

        /// <summary>
        /// 重置参数设置
        /// </summary>
        private static RoutedUICommand resetParaSetCommand;
        public static RoutedUICommand ResetParaSetCommand
        {
            get { return resetParaSetCommand; }
        }

        /// <summary>
        /// 设置所有校准
        /// </summary>
        private static RoutedUICommand setAllAdjust;
        public static RoutedUICommand SetAllAdjust
        {
            get { return setAllAdjust; }
        }

        /// <summary>
        /// 设置单个校准
        /// </summary>
        private static RoutedUICommand setOneAdjust;
        public static RoutedUICommand SetOneAdjust
        {
            get { return setOneAdjust; }
        }

        /// <summary>
        /// 保存参数设置指令
        /// </summary>
        private static RoutedUICommand saveAdjustSetCommand = new RoutedUICommand("SaveAdjustSetCommand", "SaveAdjustSetCommand", typeof(StudioCommands));
        public static RoutedUICommand SaveAdjustSetCommand
        {
            get { return saveAdjustSetCommand; }
        }

        /// <summary>
        /// 重置参数设置
        /// </summary>
        private static RoutedUICommand resetAdjustSetCommand = new RoutedUICommand("ResetAdjustSetCommand", "ResetAdjustSetCommand", typeof(StudioCommands));
        public static RoutedUICommand ResetAdjustSetCommand
        {
            get { return resetAdjustSetCommand; }
        }


        /// <summary>
        /// 读取BMS时间
        /// </summary>
        private static RoutedUICommand readBoardTime;
        public static RoutedUICommand ReadBoardTime
        {
            get { return readBoardTime; }
        }

        /// <summary>
        /// 设置BMS时间
        /// </summary>
        private static RoutedUICommand setBoardTime;
        public static RoutedUICommand SetBoardTime
        {
            get { return setBoardTime; }
        }

        /// <summary>
        /// 发送控制信息
        /// </summary>
        private static RoutedUICommand sendCtrlCommand;
        public static RoutedUICommand SendCtrlCommand
        {
            get { return sendCtrlCommand; }
        }

        /// <summary>
        /// 会话切换
        /// </summary>
        private static RoutedUICommand changeSession;
        public static RoutedUICommand ChangeSession
        {
            get { return changeSession; }
        }

        /// <summary>
        /// 请求seed
        /// </summary>
        private static RoutedUICommand reqSeedCommand;
        public static RoutedUICommand ReqSeedCommand
        {
            get { return reqSeedCommand; }
        }

        /// <summary>
        /// 发送Key
        /// </summary>
        private static RoutedUICommand sendKeyCommand;
        public static RoutedUICommand SendKeyCommand
        {
            get { return sendKeyCommand; }
        }

        /// <summary>
        /// 打开DTC信息窗口
        /// </summary>
        private static RoutedUICommand openDTCWindowCommand;
        public static RoutedUICommand OpenDTCWindowCommand
        {
            get { return openDTCWindowCommand; }
        }

        /// <summary>
        /// 读DTC信息命令
        /// </summary>
        private static RoutedUICommand readDTCInformationCommand;
        public static RoutedUICommand ReadDTCInformationCommand
        {
            get { return readDTCInformationCommand; }
        }

        static StudioCommands()
        {
            loadCommand = new RoutedUICommand("loadCommand", "loadCommand", typeof(StudioCommands), null);

            upLoadCommand = new RoutedUICommand("upLoadCommand", "upLoadCommand", typeof(StudioCommands), null);

            viewRecordsCommand = new RoutedUICommand("viewRecordsCommand", "viewRecordsCommand", typeof(StudioCommands), null);

            adjustManageCommand = new RoutedUICommand("adjustManageCommand", "adjustManageCommand", typeof(StudioCommands), null);

            writeAdjustManageCommand = new RoutedUICommand("writeAdjustManageCommand", "writeAdjustManageCommand", typeof(StudioCommands), null);

            debugManageCommand = new RoutedUICommand("debugManageCommand", "debugManageCommand", typeof(StudioCommands), null);

            aidInfoManageCommand = new RoutedUICommand("aidInfoManageCommand", "aidInfoManageCommand", typeof(StudioCommands), null);

            viewPackInfoCommand = new RoutedUICommand("viewPackInfoCommand", "viewPackInfoCommand", typeof(StudioCommands), null);

            settingCommand = new RoutedUICommand("settingCommand", "settingCommand", typeof(StudioCommands), null);

            languageSelectCommand = new RoutedUICommand("languageSelectCommand", "languageSelectCommand", typeof(StudioCommands), null);

            saveFileCommand = new RoutedUICommand("saveFileCommand", "saveFileCommand", typeof(StudioCommands), null);

            setAllParams = new RoutedUICommand("setAllParams", "setAllParams", typeof(StudioCommands), null);

            setOnePara = new RoutedUICommand("setOnePara", "setOnePara", typeof(StudioCommands), null);

            saveParaSetCommand = new RoutedUICommand("saveParaSetCommand", "saveParaSetCommand", typeof(StudioCommands), null);

            resetParaSetCommand = new RoutedUICommand("resetParaSetCommand", "resetParaSetCommand", typeof(StudioCommands), null);

            setAllAdjust = new RoutedUICommand("setAllAdjust", "setAllAdjust",  typeof(StudioCommands), null);

            setOneAdjust = new RoutedUICommand("setOneAdjust", "setOneAdjust", typeof(StudioCommands), null);

            readBoardTime = new RoutedUICommand("readBoardTime", "readBoardTime", typeof(StudioCommands), null);

            setBoardTime = new RoutedUICommand("setBoardTime", "setBoardTime",  typeof(StudioCommands), null);

            sendCtrlCommand = new RoutedUICommand("sendCtrlCommand", "sendCtrlCommand", typeof(StudioCommands), null);

            reqSeedCommand = new RoutedUICommand("reqSeedCommand", "reqSeedCommand", typeof(StudioCommands), null);

            sendKeyCommand = new RoutedUICommand("sendKeyCommand", "sendKeyCommand", typeof(StudioCommands), null);

            changeSession = new RoutedUICommand("changeSession", "changeSession", typeof(StudioCommands), null);

            openDTCWindowCommand = new RoutedUICommand("openDTCWindowCommand", "openDTCWindowCommand", typeof(StudioCommands), null);

            readDTCInformationCommand = new RoutedUICommand("readDTCInformationCommand", "readDTCInformationCommand", typeof(StudioCommands), null);
        }
    }
}
