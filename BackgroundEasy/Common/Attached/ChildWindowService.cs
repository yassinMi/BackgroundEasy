using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mi.Common.Attached
{
    public static class ChildWindowService
    {
        public static bool GetActivateParentOnClose(DependencyObject obj)
        {
            return (bool)obj.GetValue(ActivateParentOnCloseProperty);
        }

        public static void SetActivateParentOnClose(DependencyObject obj, bool value)
        {
            obj.SetValue(ActivateParentOnCloseProperty, value);
        }

        // Using a DependencyProperty as the backing store for ActivateParentOnClose.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActivateParentOnCloseProperty =
            DependencyProperty.RegisterAttached("ActivateParentOnClose", typeof(bool), typeof(ChildWindowService), new PropertyMetadata(false,ActivateParentOnCloseChanged));

        private static void ActivateParentOnCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window w = d as Window;
            if (w == null) return;

            if((bool)e.OldValue==false && (bool)e.NewValue== true)
            {
                //activate
                w.Closed += hndlClosed;
            }
            else if ((bool)e.OldValue == true && (bool)e.NewValue == false)
            {
                //deactivate
                w.Closed -= hndlClosed;
            }
        }

        private static void hndlClosed(object sender, EventArgs e)
        {
            Window w = sender as Window;
            if (w?.Owner == null) return;
            var parent = w.Owner as Window;
            parent?.Activate();
        }
    }
}
