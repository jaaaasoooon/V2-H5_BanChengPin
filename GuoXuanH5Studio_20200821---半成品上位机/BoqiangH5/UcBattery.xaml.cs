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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BoqiangH5
{
    /// <summary>
    /// UcBattery.xaml 的交互逻辑
    /// </summary>
    public partial class UcBattery : UserControl
    {

        private double _maxSOC;                   //最大SOC  
        private double _minSOC;                   //最小SOC 
        private double _currentSOC;               //当前SOC  
        private RectangleGeometry rt;           //显示的区域  
        private PathGeometry maskGeometry;
        public UcBattery()
        {
            InitializeComponent();
        }

        public void Battery_Glasses(double maxsoc, double minsoc, double currentsoc)
        {
            //InitializeComponent();

            _maxSOC = maxsoc;
            _minSOC = minsoc;
            _currentSOC = currentsoc;
            labVal.Content = string.Format("{0}%",currentsoc);

            if (_currentSOC < 0)
                _currentSOC = 0;
            if (_currentSOC > 100)
                _currentSOC = 100;
            if (_currentSOC <= 10)
            {
                mask.Background = new SolidColorBrush(Colors.Red);
                labVal.Margin = new Thickness(-3, 0, 0, 0);
            }
            else if (_currentSOC > 10 && _currentSOC <= 20)
            {
                mask.Background = new SolidColorBrush(Colors.Yellow);
                labVal.Margin = new Thickness(-8, 0, 0, 0);
            }
            else
            {
                mask.Background = new SolidColorBrush(Colors.Green);
                if(_currentSOC == 100)
                {
                    labVal.Margin = new Thickness(-13, 0, 0, 0);
                }
                else
                {
                    labVal.Margin = new Thickness(-8, 0, 0, 0);
                }
            }
            maskGeometry = new PathGeometry();
            rt = new RectangleGeometry();
            rt.Rect = GetRectangle();
            maskGeometry = Geometry.Combine(maskGeometry, rt, GeometryCombineMode.Union, null);
            mask.Clip = maskGeometry;
            labVal.ToolTip = string.Format("当前SOC值为：{0}%", _currentSOC);
        }

        private Rect GetRectangle()
        {
            //return new Rect(0, 330 - 89 + 30 - (_currentT + 40) / (_maxT - _minT) * (330 - 97 - 30 + 5) + 30, 100, (_currentT + 40) / (_maxT - _minT) * (330 - 97 - 30 + 5));
            //return new Rect(0, 330 - 28 + 65 - ((_maxT - _minT) - (_maxT - _minT) * _currentT / 100), 100, (_maxT - _minT) * _currentT / 100);
            return new Rect(0, 100 - (_maxSOC - _minSOC) * _currentSOC / 100, 100, (_maxSOC - _minSOC) * _currentSOC / 100);
        }

        public double CurrentSOC
        {
            get { return _currentSOC; }
            set
            {
                double new_currentT = value / 100.0 * (_maxSOC - _minSOC) + _minSOC;

                if (_currentSOC == new_currentT)
                {
                    return;
                }
                else if (_currentSOC > new_currentT)     //下降  
                {
                    rt.Rect = GetRectangle();
                    maskGeometry = Geometry.Combine(maskGeometry, rt, GeometryCombineMode.Intersect, null);
                    mask.Clip = maskGeometry;

                }
                else if (_currentSOC < new_currentT)     //上升  
                {
                    rt.Rect = GetRectangle();
                    maskGeometry = Geometry.Combine(maskGeometry, rt, GeometryCombineMode.Union, null);
                    mask.Clip = maskGeometry;
                }

                _currentSOC = new_currentT;
            }
        }

        public double MinSOC
        {
            get { return _minSOC; }
            set { _minSOC = value; }
        }
        public double MaxSOC
        {
            get { return _maxSOC; }
            set { _maxSOC = value; }
        }
    }
}
