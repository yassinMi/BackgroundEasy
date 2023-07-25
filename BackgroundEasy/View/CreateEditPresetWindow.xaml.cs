using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BackgroundEasy.View
{
    /// <summary>
    /// Interaction logic for CreateEditPresetWindow.xaml
    /// </summary>
    public partial class CreateEditPresetWindow : Window
    {
        public CreateEditPresetWindow()
        {
            InitializeComponent();
            this.DataContextChanged += hDcchange;
        }
        private void hDcchange(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as Mi.Common.ViewModel.BaseWindowViewModel;
            if (vm == null) return;
            vm.CloseWindowRequest += hnlReqestDlgClose_Creation;
        }


        protected override void OnClosing(CancelEventArgs e)
        {

            var vm = this.DataContext as Mi.Common.ViewModel.BaseWindowViewModel;
            if (vm == null) return;
            vm.HandleClosing(e);
            if (e.Cancel == false)
            {
                vm.CloseWindowRequest -= hnlReqestDlgClose_Creation;
            }
        }
        private void hnlReqestDlgClose_Creation(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
