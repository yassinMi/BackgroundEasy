using BackgroundEasy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace BackgroundEasy.View
{



    
    public class ScoreFactorItemEditingTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement elem = container as FrameworkElement;
            DataTemplate Range = ((DataTemplate)elem.FindResource("RangeMapScoreFactorItemEditingTemplate"));
            DataTemplate Map = ((DataTemplate)elem.FindResource("MapScoreFactorItemEditingTemplate"));
            /*if (item != null && item is Model.RangeMapScoreFactorItem)//nt type when implemented
            {
                return Range;
            }
            else if (item != null && item is Model.MapScoreFactorItem)//nt type when implemented
            {
                return Map;
            }
            */
            return null;

        }
    }



   

}
