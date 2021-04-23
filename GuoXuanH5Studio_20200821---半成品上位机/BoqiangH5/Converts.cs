using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Windows;
using BoqiangH5Entity;

namespace BoqiangH5
{
    #region 数组内容转换
    public class ContentToEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> subFuctionArrary = new List<string>();
            if (parameter != null && value != null)
            {
                if (parameter.ToString() == "read")
                {
                    subFuctionArrary = (List<string>)value;
                }
                else if(parameter.ToString()=="clear")
                {
                    subFuctionArrary = new List<string>();
                }
                
            }
            return subFuctionArrary;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region  可变精度转换器 DigitViewConverter
    [ValueConversion(typeof(float[]), typeof(String))]
    public class TextViewConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;
            
            int sid = System.Convert.ToInt16(values[0].ToString().Substring(1, 2), 16);

            if (sid != 0x14)//不是清除指令
            {
                if (values[1] != null)
                {
                    int subFuction = System.Convert.ToInt16(values[1].ToString().Substring(1, 2), 16);
                    if (subFuction != 0x04) //不是读取DTC指令
                    {
                        result = string.Format("{0:X2} {1:X2} {2:X2}", sid, subFuction,9);
                    }
                    else
                    {
                        if (values[2] != null)
                        {
                            int IdentifierHighbyte = System.Convert.ToInt32(values[2].ToString().Substring(1, 2), 16);
                            int IdentifierMiddlebyte = System.Convert.ToInt32(values[2].ToString().Substring(3, 2), 16);
                            int IdentifierLowbyte = System.Convert.ToInt32(values[2].ToString().Substring(5, 2), 16);
                            result = string.Format("{0:X2} {1:X2} {2:X2} {3:X2} {4:X2}", sid, subFuction, IdentifierHighbyte, IdentifierMiddlebyte, IdentifierLowbyte);
                        }
                    }
                }
            }
            else
            {
                if (values[2] != null)
                {
                    int IdentifierHighbyte = System.Convert.ToInt32(values[2].ToString().Substring(1, 2), 16);
                    int IdentifierMiddlebyte = System.Convert.ToInt32(values[2].ToString().Substring(3, 2), 16);
                    int IdentifierLowbyte = System.Convert.ToInt32(values[2].ToString().Substring(5, 2), 16);
                    result = string.Format("{0:X2} {1:X2} {2:X2} {3:X2}", sid, IdentifierHighbyte, IdentifierMiddlebyte, IdentifierLowbyte);
                }
            }            

            return result;

        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    #endregion

    #region 转换成十六进制字符串以及其他转换
    public class HexToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string _str = "";
            if (parameter != null && value != null)
            {
                if (parameter.ToString() == "timer")
                {
                    _str = String.Format("{0:X2}", value);
                }
                else if (parameter.ToString() == "Acc")
                {
                    _str = String.Format("{0:X8}", value);// System.Convert.ToString(System.Convert.ToUInt32(value), 16).PadLeft(8, '0');
                }
                else if (parameter.ToString() == "baudRate")
                {
                    _str = (int)value + "Kbps";
                }
                else if (parameter.ToString() == "servicedId")
                {
                    _str = string.Format("{0:X2}", System.Convert.ToByte(value.ToString().Substring(1, 2), 16));
                }
                else if (parameter.ToString() == "subFuction")
                {
                    _str = string.Format("{0:X2}", System.Convert.ToByte(value.ToString().Substring(1, 2), 16));

                }
                else if (parameter.ToString() == "identifier")
                {
                    _str += string.Format("{0:X2}", System.Convert.ToByte(value.ToString().Substring(1, 2), 16))+" ";
                    _str += string.Format("{0:X2}", System.Convert.ToByte(value.ToString().Substring(3, 2), 16))+" ";
                    _str += string.Format("{0:X2}", System.Convert.ToByte(value.ToString().Substring(5, 2), 16))+" ";
                }

            }
            return _str;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value.ToString() != null)
            {
                if (parameter.ToString() == "timer")
                {
                    byte _byte = System.Convert.ToByte(value.ToString(), 16);
                    return _byte;
                }
                else if (parameter.ToString() == "Acc")
                {
                    UInt32 _uint32 = System.Convert.ToUInt32(value.ToString(), 16);
                    return _uint32;
                }
                else if (parameter.ToString() == "baudRate")
                {
                    BaudRate _baud = (BaudRate)System.Convert.ToInt32(value.ToString().Substring(0, value.ToString().Length - 4));
                    return _baud;
                }                 
            }
            return null;
        }
    }
    #endregion

    #region 字节转换成整型以及下拉框默认选择索引
    public class ByteToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int _int = 0;
            if (parameter != null)
            {
                if (parameter.ToString() == "index")
                {
                    return _int;
                }
            }
            if (value != null)
            {
                _int = System.Convert.ToUInt16(value);
            }
            return _int;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte state = 0;
            Byte.TryParse(value.ToString(),out state);
            //if(Byte.Parse(value.ToString())>0)
            //state = System.Convert.ToByte(value);
            return state;
        }
    }
    #endregion

    #region 极值标题转换
    [ValueConversion(typeof(float), typeof(String))]
    public class ExtremumValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;
            string name = (string)values[0];
            float value = (float)values[1];

            if (name != null)
            {
                if (name.Contains("电压"))
                {
                    result = string.Format("{0}:C{1}", name, value);
                }
                else
                {
                    result = string.Format("{0}:T{1}", name, value);
                }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    #endregion

    #region  可变精度转换器 DigitViewConverter
    [ValueConversion(typeof(float[]), typeof(String))]
    public class DigitViewConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string result;

            if ((string)parameter == "Decimal")
            {
                Decimal val = (Decimal)values[0];
                Decimal scale = (Decimal)values[1];

                if (scale == 0.001M)
                    result = String.Format("{0:D3}", val);
                else if (scale == 0.01M)
                    result = String.Format("{0:D2}", val);
                else if (scale == 0.1M)
                    result = String.Format("{0:D1}", val);
                else
                    result = String.Format("{0:D0}", val);
            }
            else
            {
                float val = (float)values[0];
                float scale = (float)values[1];

                if (scale == 0.001F)
                    result = String.Format("{0:F3}", val);
                else if (scale == 0.01F)
                    result = String.Format("{0:F2}", val);
                else if (scale == 0.1F)
                    result = String.Format("{0:F1}", val);
                else
                    result = String.Format("{0:F0}", val);
            }

            if (values.Length == 3)
            {
                result += ' ' + (string)values[2];
            }

            return result;

        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    #endregion

    #region  状态颜色转换器 StatusColorConverter
    [ValueConversion(typeof(string), typeof(Brush))]
    public class StatusColorConverter : IValueConverter
    {
        public Brush ProtectOnBrush { get; set; }
        public Brush WarnOnBrush { get; set; }
        public Brush NormalOnBrush { get; set; }
        public Brush AFEOnBrush { get; set; }
        public Brush BalanceOnBrush { get; set; }
        public Brush OtherOnBrush { get; set; }
        public Brush OffBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = (string)value;
            Brush brush;

            switch (status)
            {
                case "ProtectOn":
                    brush = ProtectOnBrush; break;

                case "WarnOn":
                    brush = WarnOnBrush; break;

                case "NormalOn":
                    brush = NormalOnBrush; break;

                case "AFEOn":
                    brush = AFEOnBrush; break;

                case "BalanceOn":
                    brush = BalanceOnBrush; break;

                case "OtherOn":
                    brush = OtherOnBrush; break;

                default:
                    brush = OffBrush; break;
            }

            return brush;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    #endregion    

    #region 取反转换器
    public class ReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = !(bool)value;
            return state;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = !(bool)value;
            return state;
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "DownLoad")
            {
                if ((bool)value)
                {
                    return Visibility.Hidden;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            else if (parameter.ToString() == "Cancle")
            {
                if ((bool)value)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
            else
            {
                if ((bool)value)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    #endregion


    #region 发送，停止等按键使能convert
    public class FlagToIsEnableConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool _flag = false;
            if (parameter != null)
            {
                try
                {
                    if (parameter.ToString() == "send")
                    {
                        if ((bool)values[0] && (bool)values[1])
                        {
                            _flag = true;
                        }
                    }
                    else
                    {
                        if ((bool)values[0]&&!(bool)values[1])
                        {
                            _flag = true;
                        }
                    }
                }
                catch (Exception)
                {

                    //为什么在从TabControl中删除Item的时候回执行此Convert
                }

            }
            return _flag;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region 表格数据增加时让选中最后一行从而实现滚动条自动下拉
    public class RowToFocusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int _focusRow = (int)value;
            _focusRow--;
            return _focusRow;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region 元素可视转换器 BoolenToVisibility

    [ValueConversion((typeof(Boolean)), typeof(Visibility))]
    public class BoolenToVisibilityConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;
            if ((Boolean)value)
            {
                visibility = Visibility.Visible;
            }
            else
            {
                visibility = Visibility.Hidden;
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    #endregion
}
