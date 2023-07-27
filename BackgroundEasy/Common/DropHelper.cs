using DragDropLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mi.Common
{
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
