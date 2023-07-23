using MahApps.Metro.IconPacks;
using Mi.UI;
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

    /// <summary>
   
    /// </summary>
    [ValueConversion(typeof(PromptType),typeof(MahApps.Metro.IconPacks.PackIconFontAwesomeKind))]
    public class PromptTypeToFontAwesomeIconKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(! (value is PromptType))
            {
                return PackIconFontAwesomeKind.ExclamationTriangleSolid;
                
            }
            PromptType t = (PromptType) value;
            switch (t)
            {
                case PromptType.Error:
                    return PackIconFontAwesomeKind.SadTearSolid;
                case PromptType.Warning:
                    return PackIconFontAwesomeKind.ExclamationTriangleSolid;
                case PromptType.Information:
                    return PackIconFontAwesomeKind.InfoCircleSolid;
                case PromptType.Question:
                    return PackIconFontAwesomeKind.QuestionSolid;
                default:
                    return PackIconFontAwesomeKind.ExclamationTriangleSolid;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    


}
