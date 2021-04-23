using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace BoqiangH5
{
    /// <summary>
    /// LoadingWnd.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWnd : Window
    {
        public System.Windows.Threading.DispatcherTimer timer = null;

        int _interval = 0;
        public LoadingWnd(int interval)
        {
            InitializeComponent();
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(OnTimer);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            loadingTime = DateTime.Now;
            _interval = interval;
            SetProgressText(string.Format("设备进入浅休眠，请等待：{0} 秒！",_interval));
        }
        //void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    //Thread.Sleep(5000);
        //    this.Close();
        //}
        DateTime loadingTime;

        int count = 0;
        private void OnTimer(object sender, EventArgs e)
        {
            count += 1;
            if(_interval - count >= 0)
            {
                SetProgressText(string.Format("设备进入浅休眠，请等待：{0} 秒！",_interval - count));
            }
            else
            {
                timer.Stop();
                count = 0;
                Close();
            }
        }
        public void SetProgressText(string desc)
        {

            //this.message.Content = desc;

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.message.Content = desc;
            }));


        }
    }
}
