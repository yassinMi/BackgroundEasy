using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mi.Common.Attached
{
    public  class DropNativeUXHelper
    {








        public static string GetChannel(DependencyObject obj)
        {
            return (string)obj.GetValue(ChannelProperty);
        }

        public static void SetChannel(DependencyObject obj, string value)
        {
            obj.SetValue(ChannelProperty, value);
        }

        // Using a DependencyProperty as the backing store for Channel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChannelProperty =
            DependencyProperty.RegisterAttached("Channel", typeof(string), typeof(DropNativeUXHelper), new PropertyMetadata(null));




        public static string GetEntitiesPluralName(DependencyObject obj)
        {
            return (string)obj.GetValue(EntitiesPluralNameProperty);
        }

        public static void SetEntitiesPluralName(DependencyObject obj, string value)
        {
            obj.SetValue(EntitiesPluralNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for EntitiesPluralName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EntitiesPluralNameProperty =
            DependencyProperty.RegisterAttached("EntitiesPluralName", typeof(string), typeof(DropNativeUXHelper), new PropertyMetadata("files"));




        public static string GetDestinationLabel(DependencyObject obj)
        {
            return (string)obj.GetValue(DestinationLabelProperty);
        }

        public static void SetDestinationLabel(DependencyObject obj, string value)
        {
            obj.SetValue(DestinationLabelProperty, value);
        }

        // Using a DependencyProperty as the backing store for DestinationLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DestinationLabelProperty =
            DependencyProperty.RegisterAttached("DestinationLabel", typeof(string), typeof(DropNativeUXHelper), new PropertyMetadata("collection"));



        public static string GetAllowedFileExtenstionsCommaSeparated(DependencyObject obj)
        {
            return (string)obj.GetValue(AllowedFileExtenstionsCommaSeparatedProperty);
        }

        public static void SetAllowedFileExtenstionsCommaSeparated(DependencyObject obj, string value)
        {
            obj.SetValue(AllowedFileExtenstionsCommaSeparatedProperty, value);
        }

        // Using a DependencyProperty as the backing store for AllowedFileExtenstionsCommaSeparated.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowedFileExtenstionsCommaSeparatedProperty =
            DependencyProperty.RegisterAttached("AllowedFileExtenstionsCommaSeparated", typeof(string), typeof(DropNativeUXHelper), new PropertyMetadata(".*"));



        public static bool GetAcceptFileDrop(DependencyObject obj)
        {
            return (bool)obj.GetValue(AcceptFileDropProperty);
        }

        public static void SetAcceptFileDrop(DependencyObject obj, bool value)
        {
            obj.SetValue(AcceptFileDropProperty, value);
        }

        // Using a DependencyProperty as the backing store for AcceptFileDrop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcceptFileDropProperty =
            DependencyProperty.RegisterAttached("AcceptFileDrop", typeof(bool), typeof(DropNativeUXHelper), new PropertyMetadata(false,AcceptFileDropProperty_Ch));

        private static void AcceptFileDropProperty_Ch(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement el = d as FrameworkElement;
            if (el == null) return;
            if ((bool)e.NewValue == true && (bool)e.OldValue == false)
            {
                //activate
                el.AllowDrop = true;
                el.Drop += h_drop;
                el.DragEnter += h_dragEnter;
                el.DragOver += h_dragover;
                el.DragLeave += h_leave;
            }
            else if ((bool)e.NewValue == false && (bool)e.OldValue == true)
            {
                //disable
                el.AllowDrop = false;
                el.Drop -= h_drop;
                el.DragEnter -= h_dragEnter;
                el.DragOver -= h_dragover;
                el.DragLeave -= h_leave;
            }
        }



        private static void h_leave(object sender, DragEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            var vm = el.DataContext as INativeDropUXVm;
            if (vm != null)
            {
                vm.OnIsInDropFileStateChanged(GetChannel(el), false);
            }
            DropHelper.DragLeave();
        }

        private static void h_dragover(object sender, DragEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            DropHelper.DragOver(e, el, e.GetPosition(el));
        }

        private static void h_dragEnter(object sender, DragEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;

            var vm = el?.DataContext as INativeDropUXVm;
            if (vm == null)
            {
                return;
            }
            string destnation_name = GetDestinationLabel(el)??"collection";

            //this.DragEnter -= ragEnter;
            var fs = e.Data.GetFormats();
            if (fs.Contains(DataFormats.FileDrop))
            {

                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                var allowedExts = GetAllowedFileExtenstionsCommaSeparated(el) ?? ".*";
                IEnumerable<string> filteredFiles;
                if (allowedExts == ".*")
                {
                    filteredFiles = files;
                }
                else
                {
                    var exts = allowedExts.Split(',');
                    filteredFiles = files.Where(f=> exts.Any(ext=> System.IO.Path.GetExtension(f).Equals(ext, StringComparison.OrdinalIgnoreCase)) );
                }
                var allowed_extensions_cc = filteredFiles.Count();
                if (allowed_extensions_cc > 0)
                {
                    vm.OnIsInDropFileStateChanged(GetChannel(el), true);
                    e.Effects = DragDropEffects.Copy;
                    e.Data.SetDropDescription(DropImageType.Copy, "Add To %1", destnation_name);
                }
                else
                {
                    e.Data.SetDropDescription(DropImageType.None, null, null);
                    e.Effects = DragDropEffects.None;
                }
            }
            else
            {
                e.Data.SetDropDescription(DropImageType.None, "Cannot place here", GetEntitiesPluralName(el)??"files");
            }

            DropHelper.DragEnter(e, el, new Point());
            // DropHelper.DragEnter( App.Current.MainWindow,e);
        }

        private static void h_drop(object sender, DragEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            var vm = el.DataContext as INativeDropUXVm;
            if (vm != null)
            {
                vm.OnIsInDropFileStateChanged(GetChannel(el), false);
            }
            DropHelper.DragDrop(e, el, e.GetPosition(el));
        }

    }

    public interface INativeDropUXVm
    {
        void OnIsInDropFileStateChanged(string channel,bool value);
    }

}
