using BoqiangH5.BQProtocol;
using BoqiangH5.DDProtocol;
using BoqiangH5Entity;
using BoqiangH5Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BoqiangH5
{
    public interface IHandleRecvDataEvent
    {
        void HandleRecvBmsInfoDataEvent(object sender, CustomRecvDataEventArgs e);

    }

    public partial class MainWindow : Window
    {
        public delegate void DelegateBoqiangH5(object sender, CustomRecvDataEventArgs e);
        public event DelegateBoqiangH5 RaiseReadBqBmsInfoEvent;
        public event DelegateBoqiangH5 RaiseReadDdBmsInfoEvent;
        public event DelegateBoqiangH5 RequireRaiseReadDeviceInfoEvent;// lipeng 2020.04.05读取设备信息
        public event DelegateBoqiangH5 RaiseReadDeviceInfoEvent;// lipeng 2020.04.05读取设备信息
        public event DelegateBoqiangH5 RaiseReadEepromEvent;
        public event DelegateBoqiangH5 RaiseReadMcuEvent;
        public event DelegateBoqiangH5 RaiseReadRecordInfoEvent;// lipeng 2020.03.26处理备份数据的读取
        public event DelegateBoqiangH5 RaiseEraseInfoEvent;// lipeng 2020.03.31擦除备份数据的返回读取
        public event DelegateBoqiangH5 RaiseAdjustEvent;
        public event DelegateBoqiangH5 RaiseReadBqRTCEvent;// lipeng 2020.04.03读取RTC时间
        public event DelegateBoqiangH5 RaiseReadBqBootEvent;// lipeng 2020.04.03读取RTC时间
        public event DelegateBoqiangH5 RaiseReadRegisterEvent;//lipeng  2020.04.22 读寄存器事件
        public event DelegateBoqiangH5 RaiseReadProtectParamEvent;//lipeng 2020.05.14 读保护参数事件
        public event DelegateBoqiangH5 RaiseReadDdRecordCountEvent;//lipeng 2020.06.22 读滴滴故障记录数
        public event DelegateBoqiangH5 RaiseReadDdRecordEvent;//lipeng 2020.06.22 读滴滴故障记录
        public event DelegateBoqiangH5 RaiseEraseDdRecordEvent;//lipeng 2020.06.22 擦除滴滴故障记录
        public event DelegateBoqiangH5 RaiseReadUIDEvent;
        public event DelegateBoqiangH5 RaiseRequireReadEepromEvent;
        public event DelegateBoqiangH5 RaiseRequireReadOthersEvent;
        public event DelegateBoqiangH5 RaiseReadOthersEvent;
        public event DelegateBoqiangH5 RaiseReadAdjustParamEvent;

        public event DelegateBoqiangH5 RaiseWriteEepromEvent;
        public event DelegateBoqiangH5 RaiseWriteMcuEvent;
        public event DelegateBoqiangH5 RaiseWriteManufacturingInformationEvent;// lipeng 2020.04.02 写入制造信息事件
        public event DelegateBoqiangH5 RaiseWriteRegisterEvent;//lipeng  2020.04.16 写寄存器事件

        public event DelegateBoqiangH5 RaiseAdjustRTCurrenEvent;
        public event DelegateBoqiangH5 RaiseAdjustZeroCurrenEvent;
        public event DelegateBoqiangH5 RaiseAdjustRTCEvent; // lipeng 2020.03.31校准RTC数据的返回读取
        public event DelegateBoqiangH5 RaiseAdjustDdRTCEvent; // lipeng 2020.04.6校准滴滴RTC数据的返回读取
        public event DelegateBoqiangH5 RaiseDebugEvent;
        public event DelegateBoqiangH5 RaiseSettingBatteryStatusEvent;
        public event DelegateBoqiangH5 RaiseSettingBrakeStatusEvent;
        public event DelegateBoqiangH5 RaiseReadDdFeedbackEvent;//读取滴滴反馈信息

        public event EventHandler RaiseEepromWndUpdateEvent;
        public event EventHandler RaiseMcuWndUpdateEvent;
        public event EventHandler RaiseAdjustWndUpdateEvent;
        public event EventHandler RaiseDebugWndUpdateEvent;
        public event EventHandler RaiseBootTestWndUpdateEvent;
        //public event EventHandler RaiseRecordWndUpdateEvent;

        public event EventHandler RaisePowerOnOffEvent;

        private static readonly object m_lock = new object();

        private void HandlerZLGRecvDataEvent(object sender, EventArgs e)
        {
            var canEvent = e as CANEvent;
            if (canEvent == null)
            {
                return;
            }

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
            {
                if (canEvent.listData[0] == 0x46)
                {
                    if (canEvent.listData.Count <= 16)
                    {
                        return;
                    }
                    else
                    {
                        canEvent.listData.RemoveRange(0, 16);
                    }
                }
                else if (canEvent.listData[0] == 0x81)
                {
                    canEvent.listData.RemoveRange(0, 8);
                    if (canEvent.listData.Count == 0)
                        return;
                }
                if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
                {
                    CustomRecvDataEventArgs recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                    CSVFileHelper.WriteLogs("log", "博强接收", CSVFileHelper.ToHexStrFromByte(canEvent.listData.ToArray(), false), false);//记录所有接收到的消息
                    switch (canEvent.listData[0])
                    {
                        case 0xA0:
                            BqProtocol.BqInstance.nHandshakeFailure = 0;

                            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                m_statusBarInfo.OnlineStatus = "在线";
                                statusBrush.Color = m_green;
                                if (m_statusBarInfo.IsOnline == false)//连接成功，更新页面信息
                                {
                                    m_statusBarInfo.IsOnline = true;

                                    labTip.Content = "类型:" + ZLGInfo.DevType.ToString() +
                                             "    索引号: " + zlgFuc.zlgInfo.DevIndex.ToString() +
                                             "    通道号: " + zlgFuc.zlgInfo.DevChannel.ToString() +
                                             "    波特率: " + ZLGInfo.Baudrate.ToString();
                                    OnRaiseEepromWndUpdateEvent(null);
                                    OnRaiseMcuWndUpdateEvent(null);
                                    OnRaiseAdjustWndUpdateEvent(null);
                                    OnRaiseDebugWndUpdateEvent(null);
                                    OnRaiseBootTestWndUpdateEvent(null);
                                }

                            }), null);
                            break;

                        case 0xA1:  // bms
                            //recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                            OnRaiseReadBqBmsInfoEvent(recvEvent);
                            OnRaiseAdjustEvent(recvEvent);

                            BqProtocol.BqInstance.nHandshakeFailure = 0;
                            m_statusBarInfo.OnlineStatus = "在线";
                            statusBrush.Color = m_green;
                            if (m_statusBarInfo.IsOnline == false)//连接成功，更新页面信息
                            {
                                m_statusBarInfo.IsOnline = true;

                                labTip.Content = "类型:" + ZLGInfo.DevType.ToString() +
                                         "    索引号: " + zlgFuc.zlgInfo.DevIndex.ToString() +
                                         "    通道号: " + zlgFuc.zlgInfo.DevChannel.ToString() +
                                         "    波特率: " + ZLGInfo.Baudrate.ToString();
                                OnRaiseEepromWndUpdateEvent(null);
                                OnRaiseMcuWndUpdateEvent(null);
                                OnRaiseAdjustWndUpdateEvent(null);
                                OnRaiseDebugWndUpdateEvent(null);
                                OnRaiseBootTestWndUpdateEvent(null);
                            }
                            break;
                        case 0xA2:  // mcu
                            //recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                            OnRaiseReadMcuEvent(recvEvent);
                            break;
                        case 0xA3: // eeprom
                            //recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                            OnRaiseReadEepromEvent(recvEvent);
                            break;
                        case 0xA4:
                            OnRaiseReadOthersEvent(recvEvent);
                            break;
                        case 0xA5: // rtc
                            //recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                            OnRaiseReadBqRTCEvent(recvEvent);
                            break;
                        case 0xA6:
                            OnRaiseReadRecordEvent(recvEvent);// lipeng 2020.03.26处理备份数据的读取
                            //BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "recv", "H5Recv A6\r\n");
                            break;
                        case 0xA8:
                            OnRaiseReadAdjustParamEvent(recvEvent);
                            break;
                        case 0xAA:
                        case 0xAB:
                        case 0xAC:
                        case 0xAD:
                            OnRaiseReadProtectParamEvent(recvEvent);// lipeng 2020.5.14读保护参数
                            break;

                        case 0xB2:
                            OnRaiseWriteMcuEvent(recvEvent);
                            break;

                        case 0xB3:
                            OnRaiseWriteEepromEvent(recvEvent);
                            break;

                        case 0xB4:
                            OnRaiseWriteManufacturingInfoEvent(recvEvent);
                            break;

                        case 0xC1:
                            OnRaiseAdjustZeroCurrenEvent(recvEvent);
                            break;
                        case 0xC2:
                            OnRaiseAdjustRTCurrenEvent(recvEvent);
                            break;
                        case 0xB5:
                            OnRaiseAdjustRTCEvent(recvEvent);
                            break;

                        case 0xD0:
                        case 0xD1:
                        case 0xD2:
                        case 0xD3:
                        case 0xD4:
                        case 0xD5:
                        case 0xDB:
                            OnRaiseDebugEvent(recvEvent);
                            break;
                        case 0xD6:
                            OnRaiseEraseRecordEvent(recvEvent);
                            break;
                        case 0xD8:
                            OnRaiseRequireReadEepromEvent(recvEvent);
                            break;
                        case 0xD9:
                            OnRaiseRequireReadOthersEvent(recvEvent);
                            break;
                        case 0xDA:
                            OnRaiseReadBqBootInfoEvent(recvEvent);
                            break;
                        case 0xDC:
                            OnRaiseReadUIDEvent(recvEvent);
                            break;

                        case 0x10:
                            if(canEvent.listData[1] == 0xA2)
                            {
                                OnRaisePowerOnOffEvent(null);
                            }
                            break;
                        case 0x03:
                            if (canEvent.listData[1] == 0x56)
                            {
                                OnRequireRaiseReadDeviceInfoEvent(recvEvent);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    CSVFileHelper.WriteLogs("log", "滴滴接收", CSVFileHelper.ToHexStrFromByte(canEvent.listData.ToArray(), false), false);//记录所有接收到的消息
                    if (canEvent.listData[0] == 0x03)
                    {
                        if(canEvent.listData[1] == 0x02)
                        {
                            //if(DdProtocol.DdInstance.m_bIsStopCommunication)
                            //{
                            //    OnRaiseReadRegisterEvent(new CustomRecvDataEventArgs(canEvent.listData));//读寄存器
                            //}
                            //else
                            {
                                CustomRecvDataEventArgs recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                                OnRaiseReadDdRecordCountEvent(recvEvent);
                            }
                        }
                        else if(canEvent.listData[1] == 0xD2)
                        {
                            if (DdProtocol.DdInstance.m_bIsStopCommunication)
                            {
                                OnRaiseReadRegisterEvent(new CustomRecvDataEventArgs(canEvent.listData));//读寄存器
                            }
                            else
                            {
                                CustomRecvDataEventArgs recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                                OnRaiseReadDdBmsInfoEvent(recvEvent);
                                OnRaiseAdjustEvent(recvEvent);

                                DdProtocol.DdInstance.nReadSOHFailure = 0;

                                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    m_statusBarInfo.OnlineStatus = "在线";
                                    statusBrush.Color = m_green;
                                    if (m_statusBarInfo.IsOnline == false)
                                    {
                                        m_statusBarInfo.IsOnline = true;

                                        labTip.Content = "类型:" + ZLGInfo.DevType.ToString() +
                                                        "    索引号: " + zlgFuc.zlgInfo.DevIndex.ToString() +
                                                         "    通道号: " + zlgFuc.zlgInfo.DevChannel.ToString() +
                                                         "    波特率: " + ZLGInfo.Baudrate.ToString();
                                        OnRaiseEepromWndUpdateEvent(null);
                                        OnRaiseMcuWndUpdateEvent(null);
                                        OnRaiseAdjustWndUpdateEvent(null);
                                        OnRaiseDebugWndUpdateEvent(null);
                                        OnRaiseBootTestWndUpdateEvent(null);
                                    }
                                }), null);
                            }
                        }
                        else if (canEvent.listData[1] == 0x56)//读设备信息
                        {
                            if (DdProtocol.DdInstance.m_bIsStopCommunication)
                            {
                                OnRaiseReadRegisterEvent(new CustomRecvDataEventArgs(canEvent.listData));//读寄存器
                            }
                            else
                            {
                                CustomRecvDataEventArgs recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                                OnRaiseReadDeviceInfoEvent(recvEvent);
                            }
                        }
                        else if (canEvent.listData[1] == 0x04)  // 读RTC数据
                        {
                            if (DdProtocol.DdInstance.m_bIsStopCommunication)
                            {
                                OnRaiseReadRegisterEvent(new CustomRecvDataEventArgs(canEvent.listData));//读寄存器
                            }
                            else
                            {
                                CustomRecvDataEventArgs recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                                OnRaiseReadBqRTCEvent(recvEvent);
                            }
                        }
                        else if(canEvent.listData[1] == 0x48)
                        {
                            CustomRecvDataEventArgs recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                            OnRaiseReadDdRecordEvent(recvEvent);
                        }
                        else if (canEvent.listData[1] == 0x06)
                        {
                            CustomRecvDataEventArgs recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                            OnRaiseReadDdFeedbackInfoEvent(recvEvent);
                        }
                        else// 读寄存器
                        {
                            OnRaiseReadRegisterEvent(new CustomRecvDataEventArgs(canEvent.listData));
                        }
                    }
                    else if (canEvent.listData[0] == 0x10 )  
                    {
                        if(canEvent.listData[2] == 0x48 && canEvent.listData[1] == 0xA2)
                        {
                            OnRaiseAdjustDdRTCEvent(new CustomRecvDataEventArgs(canEvent.listData));// 校准滴滴RTC返回
                        }
                        else if(canEvent.listData[2] == 0x00 && canEvent.listData[1] == 0xA2)// 上下电
                        {
                            OnRaisePowerOnOffEvent(null);
                        }
                        else if (canEvent.listData[1] == 0xA2 && canEvent.listData[2] == 0x64)
                        {
                            OnRaiseSettingBatteryStatus(new CustomRecvDataEventArgs(canEvent.listData));
                        }
                        else if (canEvent.listData[1] == 0xA2 && canEvent.listData[2] == 0x68)
                        {
                            OnRaiseSettingBrakeStatus(new CustomRecvDataEventArgs(canEvent.listData));
                        }
                        else
                        {
                            OnRaiseWriteRegisterEvent(new CustomRecvDataEventArgs(canEvent.listData));//写寄存器
                        }

                    }
                    else if(canEvent.listData[0] == 0xD7)
                    {
                        CustomRecvDataEventArgs recvEvent = new CustomRecvDataEventArgs(canEvent.listData);
                        OnRaiseEraseDdRecordEvent(recvEvent);
                    }
                    else if (canEvent.listData[0] == 0xA7)
                    {
                        OnRaiseReadDdRecordEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                    else if(canEvent.listData[0] == 0xD2 || canEvent.listData[0] == 0xD0 || canEvent.listData[0] == 0xD4 || canEvent.listData[0] == 0xD5 || canEvent.listData[0] == 0xDB)
                    {
                        OnRaiseDebugEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                    else if(canEvent.listData[0] == 0xD8)
                    {
                        OnRaiseRequireReadEepromEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                    else if (canEvent.listData[0] == 0xD9)
                    {
                        OnRaiseRequireReadOthersEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                    else if(canEvent.listData[0] == 0xA2)
                    {
                        OnRaiseReadMcuEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                    else if(canEvent.listData[0] == 0xB2)
                    {
                        OnRaiseWriteMcuEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                    else if (canEvent.listData[0] == 0xA3)
                    {
                        OnRaiseReadEepromEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                    else if (canEvent.listData[0] == 0xA4)
                    {
                        OnRaiseReadOthersEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                    else if(canEvent.listData[0] == 0xA8)
                    {
                        OnRaiseReadAdjustParamEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                    else if(canEvent.listData[0] == 0xDA)
                    {
                        OnRaiseReadBqBootInfoEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                    else if (canEvent.listData[0] == 0xDC)
                    {
                        OnRaiseReadUIDEvent(new CustomRecvDataEventArgs(canEvent.listData));
                    }
                }
            }));
        }

        private void InitRecvDataEvenHandle()
        {
            RaiseReadBqBmsInfoEvent += ucBqBmsInfoWnd.HandleRecvBmsInfoDataEvent;
            RaiseReadBqBmsInfoEvent += ucDebugWnd.HandleRecvBmsInfoDataEvent;//调试界面增加BMS信息读取
            RaiseReadDdBmsInfoEvent += ucDdBmsInfoWnd.HandleRecvBmsInfoDataEvent;
            RaiseReadDeviceInfoEvent += ucDdBmsInfoWnd.HandleRecvDeviceInfoDataEvent;
            RequireRaiseReadDeviceInfoEvent += ucBqBmsInfoWnd.HandleRecvDeviceInfoDataEvent;
            RaiseReadEepromEvent += ucEepromWnd.HandleRecvEepromDataEvent;
            RaiseReadEepromEvent += ucDebugWnd.HandleRecvEepromDataEvent;
            RaiseReadMcuEvent += ucMcuWnd.HandleRecvMcuDataEvent;
            RaiseReadMcuEvent += ucBqBmsInfoWnd.HandleRecvMcuDataEvent;//信息界面增加MCU参数的读取
            RaiseAdjustEvent += ucAdjustWnd.HandleRecvBmsInfoDataEvent;
            RaiseReadRecordInfoEvent += ucRecordWnd.HandleReadRecordInfoDataEvent;
            RaiseEraseInfoEvent += ucRecordWnd.HandleEraseInfoDataEvent;
            RaiseReadBqRTCEvent += ucAdjustWnd.HandleReadBqRTCEvent;
            RaiseReadBqRTCEvent += ucDebugWnd.HandleReadBqRTCEvent;
            RaiseReadBqBootEvent += ucDebugWnd.HandleReadBqBootEvent;
            RaiseReadProtectParamEvent += ucProtectParamWnd.HandleRecvReadProtectParamEvent;

            RaiseWriteEepromEvent += ucEepromWnd.HandleWriteEepromDataEvent;
            RaiseWriteMcuEvent += ucMcuWnd.HandleWriteMcuDataEvent;
            RaiseWriteMcuEvent += ucBqBmsInfoWnd.HandleWriteMcuDataEvent;
            RaiseWriteManufacturingInformationEvent += ucBqBmsInfoWnd.HandleWriteManufacturingInfoEvent;
            RaiseWriteRegisterEvent += ucDebugWnd.HandleWriteRegisterEvent;
            RaiseReadRegisterEvent += ucDebugWnd.HandleReadRegisterEvent;

            RaiseAdjustRTCurrenEvent += ucAdjustWnd.HandleAdjustRTCurrenEvent;
            RaiseAdjustZeroCurrenEvent += ucAdjustWnd.HandleAdjustZeroCurrenEvent;
            RaiseAdjustRTCEvent += ucAdjustWnd.HandleAdjustRTCEvent;
            RaiseAdjustRTCEvent += ucDebugWnd.HandleAdjustRTCEvent;
            RaiseAdjustDdRTCEvent += ucDebugWnd.HandleAdjustRTCEvent;
            RaiseDebugEvent += ucDebugWnd.HandleDebugEvent;
            RaiseDebugEvent += ucBqBmsInfoWnd.HandleDebugEvent;

            RaiseEepromWndUpdateEvent += ucEepromWnd.HandleEepromWndUpdateEvent;
            RaiseMcuWndUpdateEvent += ucMcuWnd.HandleMcuWndUpdateEvent;
            RaiseAdjustWndUpdateEvent += ucAdjustWnd.HandleAdjustWndUpdateEvent;
            RaiseDebugWndUpdateEvent += ucDebugWnd.HandleDebugWndUpdateEvent;
            RaiseBootTestWndUpdateEvent += ucBootTestWnd.HandleBootTestWndUpdateEvent;

            BqProtocol.BqInstance.RaiseMenuBreakEvent += HandleRaiseMenuBreakEvent;
            DdProtocol.DdInstance.RaiseMenuBreakEvent += HandleRaiseMenuBreakEvent;

            RaiseReadDdRecordCountEvent += ucDdRecordWnd.HandleGetRecordCountEvent;
            RaiseReadDdRecordEvent += ucDdRecordWnd.HandleReadRecordInfoDataEvent;
            RaiseEraseDdRecordEvent += ucDdRecordWnd.HandleEraseInfoDataEvent;
            //RaisePowerOnOffEvent += ucDdBmsInfoWnd.HandleRaisePowerOnOffEvent;
            RaisePowerOnOffEvent += ucDebugWnd.HandleRaisePowerOnOffEvent;

            RaiseReadUIDEvent += ucDebugWnd.HandleReadUIDEvent;

            RaiseRequireReadEepromEvent += ucAFEWnd.HandleRecvEepromDataEvent;
            RaiseReadEepromEvent += ucAFEWnd.HandleRecvEepromDataEvent;
            RaiseRequireReadOthersEvent += ucAFEWnd.HandleRecvOthersDataEvent;
            RaiseReadOthersEvent += ucAFEWnd.HandleRecvOthersDataEvent;
            RaiseReadAdjustParamEvent += ucAFEWnd.HandleRecvAdjustParamEvent;
            RaiseReadDdBmsInfoEvent += ucBootTestWnd.HandleRecvBmsInfoDataEvent;
            RaiseDebugEvent += ucBootTestWnd.HandleSleepEvent;

            RaiseSettingBatteryStatusEvent += ucDebugWnd.HandleRecvSettingBatteryStatusEvent;
            RaiseSettingBrakeStatusEvent += ucDdBmsInfoWnd.HandleRecvSettingBrakeStatusEvent;
            RaiseReadDdFeedbackEvent += ucDdBmsInfoWnd.HandleRecvDDFeedbackInfoEvent;
        }

        public void HandleRaiseMenuBreakEvent(object sender, EventArgs e)
        {
            MenuBreak(true);//断开重连
            //BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "断开", "握手判断连接失败\r\n");
        }

        // lipeng 2020.04.02写入制造信息
        public virtual void OnRaiseWriteManufacturingInfoEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseWriteManufacturingInformationEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        // lipeng 2020.03.26处理备份数据的读取
        public virtual void OnRaiseReadRecordEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadRecordInfoEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        // lipeng 2020.03.31 擦除备份数据的读取
        public virtual void OnRaiseEraseRecordEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseEraseInfoEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        // lipeng 2020.03.31 擦除备份数据的读取
        public virtual void OnRaiseReadBqRTCEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadBqRTCEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        // lipeng 2020.03.31 校准RTC数据的读取
        public virtual void OnRaiseAdjustRTCEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseAdjustRTCEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        // lipeng 2020.03.31 校准滴滴RTC数据的读取
        public virtual void OnRaiseAdjustDdRTCEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseAdjustDdRTCEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseWriteRegisterEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseWriteRegisterEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual void OnRaiseReadRegisterEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadRegisterEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual void OnRaiseReadBqBootInfoEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadBqBootEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseReadUIDEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadUIDEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseRequireReadEepromEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseRequireReadEepromEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual void OnRaiseRequireReadOthersEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseRequireReadOthersEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual void OnRaiseReadOthersEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadOthersEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseReadProtectParamEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadProtectParamEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual void OnRaiseReadBqBmsInfoEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadBqBmsInfoEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseReadDdBmsInfoEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadDdBmsInfoEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseReadEepromEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadEepromEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseReadMcuEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadMcuEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }



        public virtual void OnRaiseAdjustEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseAdjustEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseWriteEepromEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseWriteEepromEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseWriteMcuEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseWriteMcuEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseAdjustRTCurrenEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseAdjustRTCurrenEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseAdjustZeroCurrenEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseAdjustZeroCurrenEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseDebugEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseDebugEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        public void OnRaiseEepromWndUpdateEvent(EventArgs e)
        {
            EventHandler handler = RaiseEepromWndUpdateEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnRaiseMcuWndUpdateEvent(EventArgs e)
        {
            EventHandler handler = RaiseMcuWndUpdateEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnRaiseAdjustWndUpdateEvent(EventArgs e)
        {
            EventHandler handler = RaiseAdjustWndUpdateEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnRaiseDebugWndUpdateEvent(EventArgs e)
        {
            EventHandler handler = RaiseDebugWndUpdateEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        public void OnRaiseBootTestWndUpdateEvent(EventArgs e)
        {
            EventHandler handler = RaiseBootTestWndUpdateEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaisePowerOnOffEvent(EventArgs e)
        {
            EventHandler handler = RaisePowerOnOffEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseReadDeviceInfoEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadDeviceInfoEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRequireRaiseReadDeviceInfoEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RequireRaiseReadDeviceInfoEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseReadDdRecordCountEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadDdRecordCountEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseReadDdRecordEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadDdRecordEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseEraseDdRecordEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseEraseDdRecordEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseReadAdjustParamEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadAdjustParamEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnRaiseReadDdFeedbackInfoEvent(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseReadDdFeedbackEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual void OnRaiseSettingBatteryStatus(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseSettingBatteryStatusEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual void OnRaiseSettingBrakeStatus(CustomRecvDataEventArgs e)
        {
            DelegateBoqiangH5 handler = RaiseSettingBrakeStatusEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
