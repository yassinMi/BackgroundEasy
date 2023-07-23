using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Converters
{
    
    [ValueConversion(typeof(bool?),typeof(int))]
    public class NullableBoolToComboBoxIx : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as bool?) == null)
            {
                return -1;
            }
            if ((value as bool?).HasValue==false)
            {
                return -1;
            }
            return ((value as bool?).Value == false) ? 1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int ix = (int)value;
                return ix == 0 ? true as bool? : ix == 1 ? false as bool? : null as bool?;
            }
            catch
            {
                return null as bool?;
            }
        }
    }

   



}
