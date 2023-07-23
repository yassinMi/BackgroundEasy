using BackgroundEasy.ViewModel;
using MaterialDesignThemes.Wpf;
using Mi.Common;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContextChanged += (s, e) =>
            {
                var vm = DataContext as ViewModel.SettingsVM;
                if (vm != null)
                {
                    vm.CloseRequested += (se, ev) =>
                    {
                        Close();
                    };
                }

            };
            
            
        }

       

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
