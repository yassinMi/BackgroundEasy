using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BackgroundEasy.Attached
{
    public class TextBoxAttachedProperties
    {


        public static bool GetUseUpDownNavigation(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseUpDownNavigationProperty);
        }

        public static void SetUseUpDownNavigation(DependencyObject obj, bool value)
        {
            obj.SetValue(UseUpDownNavigationProperty, value);
        }

        // Using a DependencyProperty as the backing store for UseUpDownNavigation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseUpDownNavigationProperty =
            DependencyProperty.RegisterAttached("UseUpDownNavigation", typeof(bool), typeof(TextBoxAttachedProperties), new PropertyMetadata(false, OnUseUpDownNavigationPropertyChanged));

        private static void OnUseUpDownNavigationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = d as TextBox;
            if (tb == null) return;
            if((bool)e.NewValue ==true && (bool)e.OldValue == false)
            {
                //activate
                tb.PreviewKeyDown += HndlTbPreviewKeyDown;
            }
            else if ((bool)e.NewValue == false && (bool)e.OldValue == true)
            {
                //disable
                //activate
                tb.PreviewKeyDown -= HndlTbPreviewKeyDown;
            }

        }

        private static void HndlTbPreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;
            if(e.Key== Key.Up)
            {
                tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
            }
            else if (e.Key == Key.Down)
            {
                tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
        }
    }
}
