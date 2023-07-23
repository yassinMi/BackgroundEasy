

using System;
using System.Diagnostics;
using System.Windows;

namespace Mi.Attached{
    public static class PartScreenAttachedProperties
    {




        public static Common.IPartScreenReferenceViewModel GetTargetViewModel(DependencyObject obj)
        {
            return (Common.IPartScreenReferenceViewModel)obj.GetValue(TargetViewModelProperty);
        }

        public static void SetTargetViewModel(DependencyObject obj, Common.IPartScreenReferenceViewModel value)
        {
            obj.SetValue(TargetViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for TargetViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetViewModelProperty =
            DependencyProperty.RegisterAttached("TargetViewModel", typeof(Common.IPartScreenReferenceViewModel), typeof(PartScreenAttachedProperties), new PropertyMetadata(null, TargetViewModelChanged));





        private static void TargetViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement t = d as FrameworkElement;
           
            if (t == null) { Debug.WriteLine("PartScreenAttachedProperties: sender FrameworkElement null"); return; }
            var vm = GetTargetViewModel(t) as Mi.Common.IPartScreenReferenceViewModel;
            if (vm == null) { Debug.WriteLine($"PartScreenAttachedProperties: sender {t} IPartScreenReferenceViewModel datacontext {t.DataContext} null"); return; }
            if (e.OldValue != null && e.NewValue == null)
            {
                //#disble
                vm.PartScreenReference = null;
                

            }
            else if (e.OldValue == null && e.NewValue != null)
            {
                //# enable
                vm.PartScreenReference = t;
                

            }
        }


      


    }


}

namespace Mi.Common
{
    public interface IPartScreenReferenceViewModel
    {
        FrameworkElement PartScreenReference { get; set; }
    }
}


