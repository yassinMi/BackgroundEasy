using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Mi.Attached
{
    /// <summary>
    /// select rown upon left mouse down
    /// (not recommended (ruins the multi selection using shift/ctrl and is not complete(leaves selected rows ))
    /// solutions: make last column with="*" 
    /// </summary>
    public static class DataGridRowAttachedProperties
    {
        public static bool GetSelectRowOnRightDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectRowOnRightDownProperty);
        }

        public static void SetSelectRowOnRightDown(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectRowOnRightDownProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectRowOnRightDown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectRowOnRightDownProperty =
            DependencyProperty.RegisterAttached("SelectRowOnRightDown", typeof(bool), typeof(DataGridRowAttachedProperties), new PropertyMetadata(false, HndlValueChanged));

        private static void HndlValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRow r = d as DataGridRow;
            if (r == null) return;
            if ((bool)e.OldValue == false && (bool)e.NewValue == true)
            {
                //enable
                r.MouseLeftButtonDown += HndlLeftDown;
            }
            else if ((bool)e.OldValue == true && (bool)e.NewValue == false)
            {
                //disable
                r.MouseLeftButtonDown -= HndlLeftDown;
            }
        }

        private static void HndlLeftDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow r = sender as DataGridRow;
            if (r == null) return;
            r.IsSelected = true;
        }
    }

}