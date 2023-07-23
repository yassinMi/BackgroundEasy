﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BackgroundEasy.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class IsAutoUpdatedToGroupHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if ((bool)value)  return "Auto-Update Tasks";
            }
            catch
            {


            }

            return "Manual Tasks";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
}
