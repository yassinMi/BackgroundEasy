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
    /// Interaction logic for PresetView.xaml
    /// </summary>
    public partial class PresetView : UserControl
    {
        public PresetView()
        {
            InitializeComponent();

        }

        private void Images_section_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.PresetVM vm = this.DataContext as ViewModel.PresetVM;
            if (vm != null)
            {
                vm.SelectCommand.Execute(null);
            }
        }
    }


}
