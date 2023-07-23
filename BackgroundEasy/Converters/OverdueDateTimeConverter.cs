using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using BackgroundEasy.Model;

namespace Converters
{
    [ValueConversion(typeof(bool?), typeof(int))]
    public class OverdueDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime?)
            {
                var v = value as DateTime?;
                if (v == null) return null;
                if (v.Value > DateTime.Now) return (System.Windows.Media.SolidColorBrush)Application.Current.FindResource("FolUpNextActionStatus.Ok.Brush");
                else  return (System.Windows.Media.SolidColorBrush)Application.Current.FindResource("FolUpNextActionStatus.Overdue.Brush");
            }
            else if (value is DateTime)
            {
                var v = (DateTime)value;
                if (v > DateTime.Now) return (System.Windows.Media.SolidColorBrush)Application.Current.FindResource("FolUpNextActionStatus.Ok.Brush");
                else return (System.Windows.Media.SolidColorBrush)Application.Current.FindResource("FolUpNextActionStatus.Overdue.Brush");
            }
            
            else { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
