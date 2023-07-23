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
    /// Interaction logic for ScrapingProgressWindow.xaml
    /// </summary>
    public partial class ScrapingProgressWindow : Window
    {
        public ScrapingProgressWindow()
        {
            InitializeComponent();
            Closing += h_closing;
            
        }

        private void h_closing(object sender, CancelEventArgs e)
        {
            
            ViewModel.ScrapingProgressVM mv = DataContext as ViewModel.ScrapingProgressVM;
            if (mv == null) return;
            e.Cancel = mv.OnClose();
            return;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if(this.WindowState == WindowState.Minimized)
            {
                if(Owner!=null)
                Owner.WindowState = WindowState.Minimized;
            }
        }
    }
}
