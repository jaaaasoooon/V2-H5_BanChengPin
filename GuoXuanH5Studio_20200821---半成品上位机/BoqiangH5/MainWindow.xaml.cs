using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using BoqiangH5.ISO15765;
using System.Windows.Media.Animation;
using BoqiangH5Entity;
using BoqiangH5.DDProtocol;
using BoqiangH5.BQProtocol;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.IO;

namespace BoqiangH5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static OperateType m_OperateType = OperateType.NoAction;

        public static StatusBarInfo m_statusBarInfo = new StatusBarInfo();

        public static ZLGFuction zlgFuc = DataLinkLayer.DllZLGFun;
        //public static ZLGCANFuction zlgCANFuc = DataLinkLayer.DllZLGCANFun;//20200401

        public static byte[] byRecvData = new byte[0xC8];

        public static Color m_green = Color.FromArgb(255, 100, 255, 137);
        public static Color m_red = Color.FromArgb(255, 251, 1, 1);
        public static Color m_black = Color.FromArgb(255, 60, 60, 60);
        public static Color m_white = Color.FromArgb(255, 255, 255, 255);
                
        SolidColorBrush defBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x51, 0x5C, 0x66));  
        SolidColorBrush enterBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x2F, 0x36, 0x3C));  
        SolidColorBrush selectBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x23, 0x26, 0x29));
        SolidColorBrush brush = new SolidColorBrush(Colors.Silver);
        public static bool bIsBreak = false;

        string strSelectMenu = "gridMenuInfo";

        public MainWindow()
        {
            InitializeComponent();

            InitRecvDataEvenHandle();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppLayProtocol.Initialize();
            AppLayProtocol.RaiseAppLayerProtocolEvent += HandlerAppLayerProtocolEvent;

            this.labOnLine.DataContext = m_statusBarInfo;

            menuConnect.IsEnabled = true;
            menuBreak.IsEnabled = false;
            menuSetting.IsEnabled = true;
   
            zlgFuc = (ZLGFuction)FindResource("ZLGCAN");
            zlgFuc.RaiseZLGRecvDataEvent += HandlerZLGRecvDataEvent;

            //zlgCANFuc = (ZLGCANFuction)FindResource("ZLGCANFD");
            //zlgCANFuc.RaiseZLGRecvDataEvent += HandlerZLGRecvDataEvent;


            gridMenuInfo.Background = selectBrush;

            if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
            {
                ucBqBmsInfoWnd.Visibility = Visibility.Visible;
                ucBqBmsInfoWnd.ShallowSleepEvent += Sleep_Clicked;//点击浅休眠事件
                ucBqBmsInfoWnd.DeepSleepEvent += Sleep_Clicked;//点击深休眠事件
                //ucBqBmsInfoWnd.DetectionVersionEvent += DetectionVersion_Clicked;
                ucDdBmsInfoWnd.Visibility = Visibility.Hidden;
            }
            else
            {
                ucBqBmsInfoWnd.Visibility = Visibility.Hidden;
                ucDdBmsInfoWnd.Visibility = Visibility.Visible;
                ucDdBmsInfoWnd.PassSOCEvent += OnPassSOCEvent;
                ucDdBmsInfoWnd.PassUTCEvent += OnPassUTCEvent;
                ucDebugWnd.ShallowSleepEvent += Sleep_Clicked;//点击浅休眠事件
                ucDebugWnd.DeepSleepEvent += Sleep_Clicked;//点击深休眠事件
            }
            ucBqBmsInfoWnd.RefreshStatusEvent += OnRefreshStatusEvent;
            ucDdBmsInfoWnd.RefreshStatusEvent += OnRefreshStatusEvent;
            ucBootTestWnd.RequireReadDeviceInfoEvent += OnRequireReadDeviceInfoEvent;
            ucBootTestWnd.RequireReadBootInfoEvent += OnRequireReadBootInfoEvent;
            ucBootTestWnd.RequireReadUIDInfoEvent += OnRequireReadUIDInfoEvent;
            ucDebugWnd.GetUIDEvent += OnGetUIDEvent;
            ucDebugWnd.GetBootInfoEvent += OnGetBootInfoEvent;
            ucDdBmsInfoWnd.GetDeviceInfoEvent += OnGetDeviceInfoEvent;
            ucBqBmsInfoWnd.GetDeviceInfoEvent += OnGetDeviceInfoEvent;

            ucDdRecordWnd.RequireReadUIDInfoEvent += OnRequireReadUIDInfoEvent;
            ucDdRecordWnd.RequireReadDeviceInfoEvent += OnRequireReadDeviceInfoEvent;
            ucDdRecordWnd.RequireReadMCUInfoEvent += OnRequireReadMcuInfoEvent;
            ucMcuWnd.ExportMCUMsgEvent += OnExportMCUMsgEvent;
        }
        //private void OnReadDeviceOverEvent(object sender,EventArgs<bool> e)
        //{
        //    ucBqBmsInfoWnd.ShowDetection(e.Args);
        //}
        private void OnPassSOCEvent(object sender, EventArgs<string> e)
        {
            ucDebugWnd.SetSOCValue(e.Args);
        }
        private void OnPassUTCEvent(object sender, EventArgs<string> e)
        {
            if(SelectCANWnd.m_H5Protocol == H5Protocol.DI_DI)
            {
                if(ucAdjustWnd.cbIsRefresh.IsChecked == true)
                {
                    ucDebugWnd.SetUTCValue(e.Args);
                }
            }
        }
        public void OnRefreshStatusEvent(object sender,EventArgs<List<bool>> e)
        {
            List<bool> list = e.Args;
            if(list[0] == true)
            {
                epWarning.Fill = new SolidColorBrush(Colors.Yellow);
                tbWarning.Foreground = new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                epWarning.Fill = new SolidColorBrush(Colors.White);
                tbWarning.Foreground = new SolidColorBrush(Colors.White);
            }

            if (list[1] == true)
            {
                epProtect.Fill = new SolidColorBrush(Colors.Red);
                tbProtect.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                epProtect.Fill = new SolidColorBrush(Colors.White);
                tbProtect.Foreground = new SolidColorBrush(Colors.White);
            }

            if (list[0] == false && list[1] == false)
            {
                epNormal.Fill = new SolidColorBrush(Colors.LightGreen);
                tbNormal.Foreground = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                epNormal.Fill = new SolidColorBrush(Colors.White);
                tbNormal.Foreground = new SolidColorBrush(Colors.White);
            }
        }
        public void OnRequireReadDeviceInfoEvent(object sender,EventArgs e)
        {
            if(SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
            {
                ucBqBmsInfoWnd.RequireReadDeviceInfo();
            }
            else
            {
                ucDdBmsInfoWnd.RequireReadDeviceInfo();
            }
        }

        public void OnRequireReadMcuInfoEvent(object sender,EventArgs e)
        {
            ucMcuWnd.RequireReadMcuMsg();
        }
        public void OnExportMCUMsgEvent(object sender,EventArgs<string> e)
        {
            ucDdRecordWnd.SetMCUMsg(e.Args);
        }
        public void OnRequireReadBootInfoEvent(object sender, EventArgs e)
        {
            ucDebugWnd.RequireReadBootInfo();
        }
        public void OnRequireReadUIDInfoEvent(object sender, EventArgs e)
        {
            ucDebugWnd.RequireReadUID();
        }
        public void OnGetUIDEvent(object sender, EventArgs<string> e)
        {
            ucBootTestWnd.SetUID(e.Args);
            ucDdRecordWnd.SetUID(e.Args);
        }
        public void OnGetBootInfoEvent(object sender, EventArgs<List<string>> e)
        {
            ucBootTestWnd.SetBootInfo(e.Args);
        }
        public void OnGetDeviceInfoEvent(object sender, EventArgs<List<string>> e)
        {
            ucBootTestWnd.SetDeviceInfo(e.Args);
            ucDdRecordWnd.SetDeviceInfo(e.Args);
        }
        //点击休眠，调用断开函数，断开与BMS的连接
        private void Sleep_Clicked(object sender,EventArgs e)
        {
            MenuBreak(false);
            //BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "断开", "收到休眠命令\r\n");
        }

        //private void DetectionVersion_Clicked(object sender,EventArgs e)
        //{
        //    ucMcuWnd.DetectionVersion();
        //}
        //private void OnDetectionVersionOKEvent(object sender, EventArgs<bool> e)
        //{
        //    ucBqBmsInfoWnd.DetectionVersionOK(e.Args);
        //}
        private void Window_Closed(object sender, EventArgs e)
        {
            zlgFuc.StopDevice();

            this.Close();
        }


         public void HandleRaiseCloseEvent(object sender, EventArgs e)
        {
             Window_Closed(this, null);
        }

        private void HandlerAppLayerProtocolEvent(object sender, EventArgs e)
        {
            var appLayerEvent = e as AppLayerEvent;
            if (appLayerEvent == null)
            {
                return;
            }
            switch (appLayerEvent.eventType)
            {
                case AppEventType.AppSendEvent:
                    break;
                case AppEventType.AppReceiveEvent:

                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        treatRecvFrame(appLayerEvent);
                    }));
                    break;
                case AppEventType.Other:
                    break;
                default:
                    break;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (null == menuItem)
                return;

            switch (menuItem.Name)
            {
                case "menuConnect":
                    MenuConnect();
                    break;

                case "menuBreak":
                    MenuBreak(false);
                    //BoqiangH5Repository.CSVFileHelper.WriteLogs("log", "断开", "点击断开按钮\r\n");
                    break;
                case "menuControl":
                    BmsControl();
                    break;

                case "menuCommunicateRecord":
                                        
                    break;

                case "menuRealtimeRecords":
                    break;

                case "menuDTCInfo":
                    break;

                case "menuAdjustAndControl":
                    break;

                case "menuPackInfo":               
                    break;

                case "menuSetting":
                    if(m_statusBarInfo.IsOnline == true)
                    {
                        MessageBox.Show("请断开连接再进行设置！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        CAN_Setting();
                    }
                    break;

                case "menuLanguageEn":

                    break;

                case "menuUpdateSystem":
                   
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        private void MenuConnect()
        {
            int ret = zlgFuc.RunDevice();
            if (ret == 1)
            {
                labTip.Content = "打开ZLG接口卡失败，请检查设备连接！";
                return;
            }
            else if (ret == 2)
            {
                labTip.Content = "初始化CAN通道失败，请检查设备连接！";
                return;
            }
            else if (ret == 3)
            {
                labTip.Content = "启动CAN通道失败，请检查设备连接！";
                return;
            }

            bIsBreak = false;

            menuConnect.IsEnabled = false;
            TextBlock tb = (TextBlock)menuConnect.Header;
            tb.Foreground = new SolidColorBrush(Colors.White);
            menuBreak.IsEnabled = true;
            tb = (TextBlock)menuBreak.Header;
            tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 200, 0, 0));
            //menuConnect.Icon = new Image() { Source = new  BitmapImage( new Uri("Images/start_gray.png", UriKind.RelativeOrAbsolute) )};
            //menuBreak.Icon = new Image() { Source = new BitmapImage(new Uri("Images/stop_red.png", UriKind.RelativeOrAbsolute)) };
            connectBrush.Color = m_white;
            breakBrush.Color = m_red;
            //m_statusBarInfo.IsOnline = true;
            statusBrush.Color = m_red;
            m_statusBarInfo.OnlineStatus = "离线";
            labTip.Content = "类型:" + ZLGInfo.DevType.ToString() +
                             "    索引号: " + zlgFuc.zlgInfo.DevIndex.ToString() +
                             "    通道号: " + zlgFuc.zlgInfo.DevChannel.ToString() +
                             "    波特率: " + ZLGInfo.Baudrate.ToString();
            if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
            {
                // SOH 
                //BqProtocol.BqInstance.SetTimerHandshake();//2020-05-27取消握手指令，改为直接发送读取命令
                BqProtocol.BqInstance.m_bIsStopCommunication = false;
                BqProtocol.BqInstance.m_bIsTest = false;
                BqProtocol.BqInstance.SetTimer();
                ThreadPool.QueueUserWorkItem(BqProtocol.BqInstance.ThreadReadMasterTeleData);
                //ThreadPool.QueueUserWorkItem(BqProtocol.BqInstance.ThreadReadProtectParam);
            }
            else
            {
                // SOH 
                //DdProtocol.DdInstance.SetTimerReadSOH();//2020-05-27取消握手指令，改为直接发送读取命令
                DdProtocol.DdInstance.m_bIsStopCommunication = false;
                DdProtocol.DdInstance.m_bIsFlag = false;
                ThreadPool.QueueUserWorkItem(DdProtocol.DdInstance.ThreadReadMasterTeleData);
            }
            ucEepromWnd.isSetPassword = false;
            ucMcuWnd.isSetPassword = false;
            //OnRaiseEepromWndUpdateEvent(null);
            //OnRaiseMcuWndUpdateEvent(null);
            //OnRaiseAdjustWndUpdateEvent(null);
            //OnRaiseDebugWndUpdateEvent(null);

            //连接成功，保留当天日志并插入分割，删除其他日志
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"log";
            string logPath = AppDomain.CurrentDomain.BaseDirectory + @"log\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            if (Directory.Exists(dir))
            {
                string[] fileSystemEntries = Directory.GetFileSystemEntries(dir);
                for (int i = 0; i < fileSystemEntries.Length; i++)
                {
                    string file = fileSystemEntries[i];
                    if (File.Exists(file))
                    {
                        if (file != logPath)
                        {
                            File.Delete(file);
                        }
                        else
                        {
                            StreamWriter sw = new StreamWriter(logPath, true, System.Text.Encoding.Default);
                            for (int j = 0; j < 5; j++)
                            {
                                sw.WriteLine("\r\n");
                            }
                            sw.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuBreak(bool isReconnection)
        {
            //if (!m_statusBarInfo.IsOnline || bIsBreak)
            //{
            //    return;
            //}

            //bIsBreak = true;

            if (isReconnection == false)
            {
                bIsBreak = true;
                if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
                    BqProtocol.BqInstance.StopTimer();
                //else
                //    DdProtocol.DdInstance.StopTimerReadSOH();

                zlgFuc.StopDevice();
                zlgFuc.zlgInfo.IsRecFrame = false;

                menuConnect.IsEnabled = true;
                TextBlock tb = (TextBlock)menuConnect.Header;
                tb.Foreground = new SolidColorBrush(Colors.LightGreen);
                menuBreak.IsEnabled = false;
                tb = (TextBlock)menuBreak.Header;
                tb.Foreground = new SolidColorBrush(Colors.White);
                menuSetting.IsEnabled = true;
                //menuConnect.Icon = new Image() { Source = new BitmapImage(new Uri("Images/start_green.png", UriKind.RelativeOrAbsolute)) };
                //menuBreak.Icon = new Image() { Source = new BitmapImage(new Uri("Images/stop_gray.png", UriKind.RelativeOrAbsolute)) };
                labTip.Content = "";
                m_statusBarInfo.OnlineStatus = "断开";
                statusBrush.Color = m_black;
                breakBrush.Color = m_white;
                connectBrush.Color = m_green;
            }
            else
            {
                m_statusBarInfo.OnlineStatus = "离线";
                statusBrush.Color = m_red;
                labTip.Content = "系统连接断开，正在重新连接......";
            }
            m_statusBarInfo.IsOnline = false;
            epNormal.Fill = new SolidColorBrush(Colors.White);
            tbNormal.Foreground = new SolidColorBrush(Colors.White);
            epProtect.Fill = new SolidColorBrush(Colors.White);
            tbProtect.Foreground = new SolidColorBrush(Colors.White);
            epWarning.Fill = new SolidColorBrush(Colors.White);
            tbWarning.Foreground = new SolidColorBrush(Colors.White);

            if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
            {
                ucBqBmsInfoWnd.SetOffLineUIStatus();
            }
            else
            {
                // bms
                ucDdBmsInfoWnd.SetOffLineUIStatus();
            }
            ucEepromWnd.isSetPassword = false;
            ucMcuWnd.isSetPassword = false;
            OnRaiseEepromWndUpdateEvent(null);
            OnRaiseMcuWndUpdateEvent(null);
            OnRaiseAdjustWndUpdateEvent(null);
            OnRaiseDebugWndUpdateEvent(null);
            OnRaiseBootTestWndUpdateEvent(null);
        }

        private void CAN_Setting()
        {
            zlgFuc = DataLinkLayer.DllZLGFun;
            SelectCANWnd settingWindow = new SelectCANWnd();
            settingWindow.RaiseCloseEvent += HandleRaiseCloseEvent;
            settingWindow.ShowDialog();
            //settingWindow.Activate();
        }

        private void BmsControl()
        {
            ControlWnd cw = new ControlWnd();
            cw.ShowDialog();

        }


        private Storyboard CommMaskStoryBoard;
        public enum CommunicationStatus { Txd = 0, RcvOk = 1, RcvError = 2 };

        private void BlinkCommStatus(CommunicationStatus which)
        {                
            if (which == CommunicationStatus.Txd)
                CommMaskStoryBoard = InitBlinkLedStoryBoard("commAnimatedBrush", Colors.LightGreen, Colors.Black, 0.5);
            else if (which == CommunicationStatus.RcvOk)
                CommMaskStoryBoard = InitBlinkLedStoryBoard("commAnimatedBrush", Colors.LightGreen, Colors.Black, 0.5);
            else
                CommMaskStoryBoard = InitBlinkLedStoryBoard("commAnimatedBrush", Colors.Red, Colors.Black, 0.5);

            CommMaskStoryBoard.Begin(this);
            
        }

        private Storyboard InitBlinkLedStoryBoard(string tarName, Color lightOnlColor, Color lightOfflColor, double seconds)
        {
            ColorAnimationUsingKeyFrames colorAnimation = new ColorAnimationUsingKeyFrames();
            colorAnimation.Duration = TimeSpan.FromSeconds(1);
            colorAnimation.FillBehavior = FillBehavior.Stop;

            colorAnimation.KeyFrames.Add(new DiscreteColorKeyFrame(lightOnlColor, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));

            colorAnimation.KeyFrames.Add(new DiscreteColorKeyFrame(lightOfflColor, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(seconds))));

            Storyboard.SetTargetName(colorAnimation, tarName);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));

            // Create a storyboard to apply the animation.
            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(colorAnimation);

            return myStoryboard;
        }         

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            byte[] arrLen = BitConverter.GetBytes(0x1001);
            byte[] bytes = System.Text.Encoding.Default.GetBytes("1001");

            RequestFrameTestWnd reqFrameWnd = new RequestFrameTestWnd();
            reqFrameWnd.Show();
        }      

        private void gridMenu_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid gridMenu = sender as Grid;
            if(gridMenu == null)
            {
                return;
            }           

            switch(gridMenu.Name)
            {
                case "gridMenuInfo":
                    if (SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
                    {
                        ucBqBmsInfoWnd.Visibility = Visibility.Visible;
                        ucDdBmsInfoWnd.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        ucBqBmsInfoWnd.Visibility = Visibility.Hidden;
                        ucDdBmsInfoWnd.Visibility = Visibility.Visible;
                    }
                    //ucDdBmsInfoWnd.Visibility = Visibility.Visible;
                    ucEepromWnd.Visibility = Visibility.Hidden;
                    ucMcuWnd.Visibility = Visibility.Hidden;
                    ucAdjustWnd.Visibility = Visibility.Hidden;
                    ucDebugWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.Visibility = Visibility.Hidden;
                    ucRecordWnd.Visibility = Visibility.Hidden;
                    ucDdRecordWnd.Visibility = Visibility.Hidden;
                    ucAFEWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.StartOrStopTimer(true);
                    ucBootTestWnd.Visibility = Visibility.Hidden;

                    gridMenuBoot.Background = defBrush;
                    gridMenuEeprom.Background = defBrush;
                    gridMenuMcu.Background = defBrush;
                    gridMenuRecord.Background = defBrush;
                    gridMenuAdjust.Background = defBrush;
                    gridMenuDebug.Background = defBrush;
                    gridMenuParam.Background = defBrush;
                    gridMenuAFE.Background = defBrush;
                    break;

                case "gridMenuParam":
                    ucBqBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucDdBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucEepromWnd.Visibility = Visibility.Hidden;
                    ucMcuWnd.Visibility = Visibility.Hidden;
                    ucRecordWnd.Visibility = Visibility.Hidden;
                    ucAdjustWnd.Visibility = Visibility.Hidden;
                    ucDebugWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.Visibility = Visibility.Visible;
                    ucDdRecordWnd.Visibility = Visibility.Hidden;
                    ucAFEWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.StartOrStopTimer(false);
                    ucBootTestWnd.Visibility = Visibility.Hidden;

                    gridMenuBoot.Background = defBrush;
                    gridMenuInfo.Background = defBrush;
                    gridMenuMcu.Background = defBrush;
                    gridMenuRecord.Background = defBrush;
                    gridMenuAdjust.Background = defBrush;
                    gridMenuDebug.Background = defBrush;
                    gridMenuEeprom.Background = defBrush;
                    gridMenuAFE.Background = defBrush;
                    break;

                case "gridMenuEeprom":
                    ucBqBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucDdBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucEepromWnd.Visibility = Visibility.Visible;
                    ucMcuWnd.Visibility = Visibility.Hidden;
                    ucRecordWnd.Visibility = Visibility.Hidden;
                    ucAdjustWnd.Visibility = Visibility.Hidden;
                    ucDebugWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.Visibility = Visibility.Hidden;
                    ucDdRecordWnd.Visibility = Visibility.Hidden;
                    ucAFEWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.StartOrStopTimer(true);
                    ucBootTestWnd.Visibility = Visibility.Hidden;

                    gridMenuBoot.Background = defBrush;
                    gridMenuInfo.Background = defBrush;
                    gridMenuMcu.Background = defBrush;
                    gridMenuRecord.Background = defBrush;
                    gridMenuAdjust.Background = defBrush;
                    gridMenuDebug.Background = defBrush;
                    gridMenuParam.Background = defBrush;
                    gridMenuAFE.Background = defBrush;
                    ucEepromWnd.isSetPassword = true;
                    OnRaiseEepromWndUpdateEvent(null);
                    ucProtectParamWnd.StartOrStopTimer(true);
                    break;

                case "gridMenuMcu":
                    ucBqBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucDdBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucEepromWnd.Visibility = Visibility.Hidden;
                    ucMcuWnd.Visibility = Visibility.Visible;
                    ucRecordWnd.Visibility = Visibility.Hidden;
                    ucAdjustWnd.Visibility = Visibility.Hidden;
                    ucDebugWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.Visibility = Visibility.Hidden;
                    ucDdRecordWnd.Visibility = Visibility.Hidden;
                    ucAFEWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.StartOrStopTimer(true);
                    ucBootTestWnd.Visibility = Visibility.Hidden;

                    gridMenuBoot.Background = defBrush;
                    gridMenuInfo.Background = defBrush;
                    gridMenuEeprom.Background = defBrush;
                    gridMenuRecord.Background = defBrush;
                    gridMenuAdjust.Background = defBrush;
                    gridMenuDebug.Background = defBrush;
                    gridMenuParam.Background = defBrush;
                    gridMenuAFE.Background = defBrush;
                    ucMcuWnd.isSetPassword = true;
                    OnRaiseMcuWndUpdateEvent(null);
                    break;

                case "gridMenuRecord":
                    if(SelectCANWnd.m_H5Protocol == H5Protocol.BO_QIANG)
                    {
                        ucRecordWnd.Visibility = Visibility.Visible;
                        ucDdRecordWnd.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        ucRecordWnd.Visibility = Visibility.Hidden;
                        ucDdRecordWnd.Visibility = Visibility.Visible;
                    }
                    ucBqBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucDdBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucEepromWnd.Visibility = Visibility.Hidden;
                    ucMcuWnd.Visibility = Visibility.Hidden;

                    ucAdjustWnd.Visibility = Visibility.Hidden;
                    ucDebugWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.Visibility = Visibility.Hidden;
                    ucAFEWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.StartOrStopTimer(true);
                    ucBootTestWnd.Visibility = Visibility.Hidden;

                    gridMenuBoot.Background = defBrush;
                    gridMenuInfo.Background = defBrush;
                    gridMenuEeprom.Background = defBrush;
                    gridMenuMcu.Background = defBrush;
                    gridMenuAdjust.Background = defBrush;
                    gridMenuDebug.Background = defBrush;
                    gridMenuParam.Background = defBrush;
                    gridMenuAFE.Background = defBrush;
                    break;

                case "gridMenuAdjust":
                    ucBqBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucDdBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucEepromWnd.Visibility = Visibility.Hidden;
                    ucMcuWnd.Visibility = Visibility.Hidden;
                    ucRecordWnd.Visibility = Visibility.Hidden;
                    ucAdjustWnd.Visibility = Visibility.Visible;
                    ucDebugWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.Visibility = Visibility.Hidden;
                    ucDdRecordWnd.Visibility = Visibility.Hidden;
                    ucAFEWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.StartOrStopTimer(true); 
                    ucBootTestWnd.Visibility = Visibility.Hidden;

                    gridMenuBoot.Background = defBrush;
                    gridMenuInfo.Background = defBrush;
                    gridMenuEeprom.Background = defBrush;
                    gridMenuMcu.Background = defBrush;
                    gridMenuRecord.Background = defBrush;
                    gridMenuDebug.Background = defBrush;
                    gridMenuParam.Background = defBrush;
                    gridMenuAFE.Background = defBrush;
                    OnRaiseAdjustWndUpdateEvent(null);
                    break;

                case "gridMenuDebug":
                    ucBqBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucDdBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucEepromWnd.Visibility = Visibility.Hidden;
                    ucMcuWnd.Visibility = Visibility.Hidden;
                    ucRecordWnd.Visibility = Visibility.Hidden;
                    ucAdjustWnd.Visibility = Visibility.Hidden;
                    ucDebugWnd.Visibility = Visibility.Visible;
                    ucProtectParamWnd.Visibility = Visibility.Hidden;
                    ucDdRecordWnd.Visibility = Visibility.Hidden;
                    ucAFEWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.StartOrStopTimer(true);
                    ucBootTestWnd.Visibility = Visibility.Hidden;

                    gridMenuBoot.Background = defBrush;
                    gridMenuInfo.Background = defBrush;
                    gridMenuEeprom.Background = defBrush;
                    gridMenuMcu.Background = defBrush;
                    gridMenuRecord.Background = defBrush;
                    gridMenuAdjust.Background = defBrush;
                    gridMenuParam.Background = defBrush;
                    gridMenuAFE.Background = defBrush;
                    OnRaiseDebugWndUpdateEvent(null);
                    break;

                case "gridMenuAFE":
                    ucBqBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucDdBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucEepromWnd.Visibility = Visibility.Hidden;
                    ucMcuWnd.Visibility = Visibility.Hidden;
                    ucRecordWnd.Visibility = Visibility.Hidden;
                    ucAdjustWnd.Visibility = Visibility.Hidden;
                    ucDebugWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.Visibility = Visibility.Hidden;
                    ucDdRecordWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.StartOrStopTimer(true);
                    ucAFEWnd.Visibility = Visibility.Visible;
                    ucBootTestWnd.Visibility = Visibility.Hidden;

                    gridMenuBoot.Background = defBrush;
                    gridMenuInfo.Background = defBrush;
                    gridMenuEeprom.Background = defBrush;
                    gridMenuMcu.Background = defBrush;
                    gridMenuRecord.Background = defBrush;
                    gridMenuAdjust.Background = defBrush;
                    gridMenuParam.Background = defBrush;
                    gridMenuDebug.Background = defBrush;
                    break;
                case "gridMenuBoot":
                    ucBqBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucDdBmsInfoWnd.Visibility = Visibility.Hidden;
                    ucEepromWnd.Visibility = Visibility.Hidden;
                    ucMcuWnd.Visibility = Visibility.Hidden;
                    ucRecordWnd.Visibility = Visibility.Hidden;
                    ucAdjustWnd.Visibility = Visibility.Hidden;
                    ucDebugWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.Visibility = Visibility.Hidden;
                    ucDdRecordWnd.Visibility = Visibility.Hidden;
                    ucProtectParamWnd.StartOrStopTimer(true);
                    ucAFEWnd.Visibility = Visibility.Hidden;
                    ucBootTestWnd.Visibility = Visibility.Visible;

                    gridMenuInfo.Background = defBrush;
                    gridMenuEeprom.Background = defBrush;
                    gridMenuMcu.Background = defBrush;
                    gridMenuRecord.Background = defBrush;
                    gridMenuAdjust.Background = defBrush;
                    gridMenuParam.Background = defBrush;
                    gridMenuDebug.Background = defBrush;
                    gridMenuAFE.Background = defBrush;
                    OnRaiseBootTestWndUpdateEvent(null);
                    break;
                default:
                    break;
            }

            gridMenu.Background = selectBrush; 
            strSelectMenu = gridMenu.Name;
        }

        private void gridMenu_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Grid gridMenu = sender as Grid;
            if (gridMenu == null)
            {
                return;
            }

            gridMenu.Background = enterBrush;
        }

        private void gridMenu_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Grid gridMenu = sender as Grid;
            if (gridMenu == null)
            {
                return;
            }

            if (strSelectMenu == gridMenu.Name)
            {
                gridMenu.Background = selectBrush;
            }
            else
            {
                gridMenu.Background = defBrush; 
            }
        }

        //private void BtnMin_Click(object sender, RoutedEventArgs e)
        //{
        //    this.WindowState = WindowState.Minimized;
        //}

        //private void BtnClose_Click_1(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}
    }
}
