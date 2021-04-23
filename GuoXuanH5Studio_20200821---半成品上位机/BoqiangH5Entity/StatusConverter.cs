using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BoqiangH5Entity
{

     public enum ConnectStatus
     {
         Offline = 0,
         Online = 1
     }

     public class ConnectStatusConverter : IValueConverter
    {
        public event PropertyChangedEventHandler PropertyChanged;

        bool bIsOnline = false;
        public bool IsVisibility
        {
            get { return bIsOnline; }
            set
            {
                bIsOnline = value;
                              
            }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Boolean && (bool)value)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else
            {
                return new SolidColorBrush(Colors.Red);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion IValueConverter Members
    }

    public enum BalanceStatusEnum
    {
        No = 0,
        Yes = 1,        
    }

    [ValueConversion(typeof(BalanceStatusEnum), typeof(BitmapImage))]
    public class BalanceStatusToImgConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BalanceStatusEnum Status = (BalanceStatusEnum)value;
            switch (Status)
            {
                case BalanceStatusEnum.Yes:
                    return new BitmapImage(new Uri("pack://application:,,,/Images/balance.png"));
                case BalanceStatusEnum.No:
                    return null;                
                default:
                    throw new InvalidEnumArgumentException("BalanceStatusEnum 值超出预期范围");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
