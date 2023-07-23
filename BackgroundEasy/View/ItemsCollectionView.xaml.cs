using DragDropLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BackgroundEasy.View
{
    /// <summary>
    /// Interaction logic for ItemsCollectionView.xaml
    /// </summary>
    public partial class ItemsCollectionView : UserControl
    {
        public ItemsCollectionView()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.Drop += rop;
            this.DragEnter += ragEnter;
            this.DragOver += ragover;
            this.DragLeave += leav;
        }



        private void leav(object sender, DragEventArgs e)
        {
            Trace.WriteLine($"leav {sender}");
            var vm = this.DataContext as ViewModel.MainVM;
            if (vm != null)
            {
                vm.IsInDropFileState = false;
            }
            DropHelper.DragLeave();
        }

        private void ragover(object sender, DragEventArgs e)
        {
            DropHelper.DragOver(e, this, e.GetPosition(this));
        }

        private void ragEnter(object sender, DragEventArgs e)
        {
            Trace.WriteLine($"ragEnter {sender}");
            var vm = this.DataContext as ViewModel.MainVM;
            if (vm == null)
            {
                return;
            }
            string type_destnation_name = "Images";

            //this.DragEnter -= ragEnter;
            var fs = e.Data.GetFormats();
            if (fs.Contains(DataFormats.FileDrop))
            {

                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var csv = files.Where(f => System.IO.Path.GetExtension(f).ToLower() == ".csv");
                var txt = files.Where(f => System.IO.Path.GetExtension(f).ToLower() == ".txt");
                var json = files.Where(f => System.IO.Path.GetExtension(f).ToLower() == ".json");
                var allowed_extensions_cc = csv.Count()+txt.Count()+json.Count();
                if (allowed_extensions_cc > 0)
                {

                    vm.IsInDropFileState = true;

                    e.Effects = DragDropEffects.Copy;

                    e.Data.SetDropDescription(DropImageType.Copy, "Add To %1", type_destnation_name);
                }
                else
                {
                    e.Data.SetDropDescription(DropImageType.None, null, null);
                    e.Effects = DragDropEffects.None;


                }

            }
            else
            {
                e.Data.SetDropDescription(DropImageType.None, "Cannot place here", type_destnation_name);

            }

            DropHelper.DragEnter(e, this, new Point());
            // DropHelper.DragEnter( App.Current.MainWindow,e);

        }

        private void rop(object sender, DragEventArgs e)
        {
            var vm = this.DataContext as ViewModel.MainVM;
            if (vm != null)
            {
                vm.IsInDropFileState = false;
            }
            DropHelper.DragDrop(e, this, e.GetPosition(this));

        }


    }



    public class DropHelper
    {
        public static void DragEnter(DragEventArgs e, IInputElement relv, Point p)
        {
            Win32Point wp = p.ToWin32Point();
            IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
            dropHelper.DragEnter(IntPtr.Zero, (System.Runtime.InteropServices.ComTypes.IDataObject)e.Data, ref wp, (int)e.Effects);
        }
        public static void DragEnter(Window w, DragEventArgs e)
        {
            IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
            dropHelper.DragEnter(w, e.Data, e.GetPosition(w), e.Effects);
        }

        public static void DragOver(DragEventArgs e, IInputElement relv, Point p)
        {
            Win32Point wp = p.ToWin32Point();
            IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
            dropHelper.DragOver(ref wp, (int)e.Effects);
        }

        public static void DragLeave()
        {
            IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
            dropHelper.DragLeave();
        }

        public static void DragDrop(DragEventArgs e, IInputElement relv, Point p)
        {
            Win32Point wp = p.ToWin32Point();
            IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
            dropHelper.Drop((System.Runtime.InteropServices.ComTypes.IDataObject)e.Data, ref wp, (int)e.Effects);
        }
    }


}
