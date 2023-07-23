

using System;
using System.Diagnostics;
using System.Windows;

namespace Mi.Attached{
    public static class DropFilesAtachedProperties
    {


        public static string GetDropFilesKey(DependencyObject obj)
        {
            return (string)obj.GetValue(DropFilesKeyProperty);
        }

        public static void SetDropFilesKey(DependencyObject obj, string value)
        {
            obj.SetValue(DropFilesKeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for DropFilesKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropFilesKeyProperty =
            DependencyProperty.RegisterAttached("DropFilesKey", typeof(string), typeof(DropFilesAtachedProperties), new PropertyMetadata(null, DropFilesKeyChanged));

        private static void DropFilesKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement taget = d as FrameworkElement;
            if (taget == null) return;
            if (e.OldValue != null && e.NewValue == null)
            {
                //#disble
                taget.Drop -= hndlTagetDrop;
            }
            else if (e.OldValue == null && e.NewValue != null)
            {
                //# enable
                taget.Drop += hndlTagetDrop;

            }
        }

        private static void hndlTagetDrop(object sender, DragEventArgs e)
        {
            
            var t = sender as FrameworkElement;
            if (t == null) { Debug.WriteLine("hndlTagetDrop: sender FrameworkElement null");return; }
            var vm = t.DataContext as Mi.Common.IDropFilesTarget;
            if (vm == null) { Debug.WriteLine($"hndlTagetDrop: sender {sender} IDropFilesTarget datacontext {t.DataContext} null"); return; }
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    vm.HndlDropFiles(GetDropFilesKey(t), (string[])e.Data.GetData(DataFormats.FileDrop));
                }));
            }
            e.Handled = false;
        }


    }


}

namespace Mi.Common
{
    public interface IDropFilesTarget
    {
        void HndlDropFiles(string senderKey, string[] files);
    }
}


