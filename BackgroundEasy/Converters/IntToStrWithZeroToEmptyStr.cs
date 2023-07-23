﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Converters
{
   
    /// <summary>

    /// </summary>
    [ValueConversion(typeof(int),typeof(string))]
    public class IntToStrWithZeroToEmptyStr : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is int))
            {
                return (((int)value==0)?"":value);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

       
    }

    
  

}
