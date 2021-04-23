using BoqiangH5.BQProtocol;
using BoqiangH5Entity;
using BoqiangH5Repository;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Text;
using System.Collections.ObjectModel;

namespace BoqiangH5
{
    /// <summary>
    /// UserCtrlRecord.xaml 的交互逻辑
    /// </summary>
    public partial class UserCtrlRecord : UserControl
    {
        public static ObservableCollection<H5RecordInfo> m_ListRecordsInfo = new ObservableCollection<H5RecordInfo>();


        public UserCtrlRecord()
        {
            InitializeComponent();
            InitBqBmsInfoWnd();
        }

        private void InitBqBmsInfoWnd()
        {
            recordInfoDic.Clear();
            string strConfigFile = System.AppDomain.CurrentDomain.BaseDirectory + @"ProtocolFiles\bq_h5_record_info.xml";
            XmlHelper.LoadBackupRecordConfig(strConfigFile, recordInfoDic, recordTypeDic, packInfoList, batteryInfoList);
        }

        Dictionary<string, string> recordInfoDic = new Dictionary<string, string>();
        Dictionary<string, string> recordTypeDic = new Dictionary<string, string>();
        List<Tuple<string, string, string>> packInfoList = new List<Tuple<string, string, string>>();
        List<Tuple<string, string, string>> batteryInfoList = new List<Tuple<string, string, string>>();
        private void ucRecordInfo_Loaded(object sender, RoutedEventArgs e)
        {
            m_ListRecordsInfo.Clear();
            dataGridRecord.Items.Clear();
            dataGridRecord.ItemsSource = m_ListRecordsInfo;
            if(SelectCANWnd.m_H5Protocol == H5Protocol.DI_DI)
            {
                btnClear.IsEnabled = false;
                btnReadData.IsEnabled = false;
                btnSave.IsEnabled = false;
            }
            btnStopRead.IsEnabled = false;
        }

        bool isRead = false;
        private void btnReadData_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MainWindow.m_statusBarInfo.IsOnline)
            {
                if (isRead == false)
                {
                    m_ListRecordsInfo.Clear();
                    dataGridRecord.Items.Refresh();
                    isRead = false;

                    BqProtocol.bReadBqBmsResp = true;
                    BqProtocol.BqInstance.m_bIsStopCommunication = true;
                    BqProtocol.BqInstance.ReadRecordData(0);
                    btnStopRead.IsEnabled = true;
                    btnReadData.IsEnabled = false;
                    isRead = true;
                    isStopRead = false;
                    //preTime = DateTime.Now;
                    ////300ms没收到数据，认为失败，重读当前数据
                    //Task.Factory.StartNew(() =>
                    //{
                    //    int count = 0;
                    //    while (isRead)
                    //    {
                    //        if ((DateTime.Now - preTime) > new TimeSpan(0, 0, 0, 0, 250))
                    //        {
                    //            BqProtocol.bReadBqBmsResp = true;
                    //            BqProtocol.BqInstance.ReadRecordData(2);
                    //            preTime = DateTime.Now;
                    //            count++;
                    //        }
                    //        if(count > 2)
                    //        {
                    //            break;
                    //        }
                    //    }
                    //});
                }
                else
                {
                    MessageBox.Show("正在读取备份数据，请先停止读取，再进行操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("系统未连接，请连接后再进行操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.Filter = "csv files(*.csv)|*.csv";
                dlg.FileName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                //dlg.InitialDirectory = "D:\\";
                dlg.AddExtension = false;
                dlg.RestoreDirectory = true;
                System.Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string desFilePath = dlg.FileName.ToString();
                    if (File.Exists(desFilePath))
                    {
                        File.Delete(desFilePath);
                    }
                    CSVFileHelper.SaveRecordDataCSV(m_ListRecordsInfo, desFilePath);
                    MessageBox.Show("备份数据保存成功！");
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool isErase = false;
        private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MainWindow.m_statusBarInfo.IsOnline)
            {
                if (MessageBoxResult.Yes == MessageBox.Show("确定要擦除备份数据？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information))
                {
                    isErase = false;
                    BqProtocol.bReadBqBmsResp = true;
                    BqProtocol.BqInstance.EraseRecord();
                    isErase = true;
                }
            }
            else
            {
                MessageBox.Show("系统未连接，请连接后再进行操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        //DateTime preTime;
        bool isStopRead = false;

        public void HandleReadRecordInfoDataEvent(object sender, CustomRecvDataEventArgs e)
        {
            try
            {
                if (isStopRead)
                {
                    isRead = false;
                    BqProtocol.BqInstance.m_bIsStopCommunication = false;
                    return;
                }
                if(isRead)
                {
                    H5RecordInfo info = ReadRecordInfo(e.RecvMsg);
                    if (info != null)
                    {
                        //preTime = DateTime.Now;
                        if (info.RecordInfo == 0x00)
                        {
                            isRead = false;
                            btnStopRead.IsEnabled = false;
                            btnReadData.IsEnabled = true;
                            MessageBox.Show("没有备份数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else if (info.RecordInfo == 0x01 || info.RecordInfo == 0x02)
                        {
                            info.Index = m_ListRecordsInfo.Count + 1;
                            m_ListRecordsInfo.Add(info);
                            BqProtocol.bReadBqBmsResp = true;
                            BqProtocol.BqInstance.ReadRecordData(1);
                        }
                        else if (info.RecordInfo == 0x03)
                        {
                            info.Index = m_ListRecordsInfo.Count + 1;
                            m_ListRecordsInfo.Add(info);
                            isRead = false;
                            MessageBox.Show("备份数据读取完成！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            BqProtocol.BqInstance.m_bIsStopCommunication = false;
                            btnStopRead.IsEnabled = false;
                            btnReadData.IsEnabled = true;
                        }
                        else
                        {
                            BqProtocol.bReadBqBmsResp = true;
                            BqProtocol.BqInstance.ReadRecordData(2);
                        }
                    }
                    #region
                    //if (info != null)
                    //{
                    //    if (info.RecordInfo == 0x00)
                    //    {
                    //        MessageBox.Show("没有备份数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    //    }
                    //    else if (info.RecordInfo == 0x01)
                    //    {
                    //        isNewData = true;
                    //        isEnd = false;
                    //        info.Index = m_ListRecordsInfo.Count + 1;
                    //        m_ListRecordsInfo.Add(info);
                    //        getDataEvent?.Invoke(this, new EventArgs<bool>(false));
                    //        Task.Factory.StartNew(() =>
                    //        {
                    //            while (true)
                    //            {
                    //                if(isStopRead)
                    //                {
                    //                    isEnd = true;
                    //                    BqProtocol.BqInstance.m_bIsStopCommunication = false;
                    //                    isRead = false;
                    //                    break;
                    //                }

                    //                if (isEnd == true)
                    //                {
                    //                    break;
                    //                }

                    //                if (isNewData)
                    //                {
                    //                    isNewData = false;
                    //                    preTime = DateTime.Now;
                    //                    BqProtocol.bReadBqBmsResp = true;
                    //                    BqProtocol.BqInstance.ReadRecordData(1);
                    //                }
                    //                else
                    //                {
                    //                    DateTime currentTime = DateTime.Now;
                    //                    if (currentTime - preTime > new TimeSpan(0, 0, 0,0,300))
                    //                    {
                    //                        isNewData = true;
                    //                    }

                    //                    if(currentTime - preTime > new TimeSpan(0,0,5))//超过5秒还未读到返回数据，则结束读取
                    //                    {
                    //                        isEnd = true;
                    //                        BqProtocol.BqInstance.m_bIsStopCommunication = false;
                    //                        isRead = false;
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //            isEnd = false;//读取结束，将标志位标为false，以备下次读取
                    //        });
                    //    }
                    //    else if (info.RecordInfo == 0x02)
                    //    {
                    //        isEnd = false;
                    //        isNewData = true;
                    //        info.Index = m_ListRecordsInfo.Count + 1;
                    //        m_ListRecordsInfo.Add(info);

                    //        getDataEvent?.Invoke(this, new EventArgs<bool>(false));
                    //    }
                    //    else if (info.RecordInfo == 0x03)
                    //    {
                    //        isEnd = true;
                    //        info.Index = m_ListRecordsInfo.Count + 1;
                    //        m_ListRecordsInfo.Add(info);
                    //        getDataEvent?.Invoke(this, new EventArgs<bool>(true));
                    //        MessageBox.Show("备份数据读取完成！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    //        BqProtocol.BqInstance.m_bIsStopCommunication = false;
                    //        isRead = false;
                    //    }
                    //    else
                    //    {
                    //        System.Threading.Thread.Sleep(10);
                    //        BqProtocol.bReadBqBmsResp = true;
                    //        BqProtocol.BqInstance.ReadRecordData(2);
                    //    }
                    //}
                    #endregion
                }

            }
            catch (Exception ex)
            {
                btnReadData.IsEnabled = true;
                MessageBox.Show(ex.Message);
            }
        }

        public void HandleEraseInfoDataEvent(object sender, CustomRecvDataEventArgs e)
        {
            if(isErase)
            {
                BqProtocol.bReadBqBmsResp = true;
                if (e.RecvMsg[0] == 0xD6)
                {
                    MessageBox.Show("备份数据擦除成功！", "擦除提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    m_ListRecordsInfo.Clear();
                    //dataGridRecord.Items.Clear();
                }
                else
                {
                    MessageBox.Show("备份数据擦除失败！", "擦除提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                isErase = false;
            }
        }

        private void btnStopRead_Click(object sender, RoutedEventArgs e)
        {
            isStopRead = true;
            isRead = false;
            btnReadData.IsEnabled = true;
            btnStopRead.IsEnabled = false;
            BqProtocol.BqInstance.m_bIsStopCommunication = false;
        }
    }
}
