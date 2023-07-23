using BackgroundEasy.Model;
using BackgroundEasy.Services;
using BackgroundEasy.View;
using BackgroundEasy.ViewModel;
using Mi.View;
using Mi.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BackgroundEasy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MainVM vm = new MainVM();
            vm.AboutWindowOpenRequested += (s, e) =>
            {
                var win = new AboutWindow();

                win.Owner = this;
                win.ShowDialog();
            };

            //ModifyTheme(ConfigService.Instance.IsDarkTheme);
            Topmost = ConfigService.Instance.TopMost;
            DataContext = vm;
            InitializeComponent();
            

            ConfigService.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ConfigService.IsDarkTheme))
                {
                    //ModifyTheme(ConfigService.Instance.IsDarkTheme);
                }

                else if (e.PropertyName == nameof(ConfigService.IsDetailsPanelOpen))
                {

                }
                else if (e.PropertyName == nameof(ConfigService.TopMost))
                {
                    Topmost = ConfigService.Instance.TopMost;
                }
                
            };
            RetrieveGridSplittersState();



            lastIsDetailsOpenState = !ConfigService.Instance.IsDetailsPanelOpen;//to force a toggle op
           

            if (ConfigService.Instance.IsWindowMaximized)
            {
                WindowState = WindowState.Maximized;//it defaults to restoed
            }
            StateChanged += HndlStateChanged;
            Closing += HndlClosing;

            //# promp handeling
            CoreUtils.PromptRequested += (s, p) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var w = new PromptWindow();
                    w.Owner = App.Current.MainWindow;
                    PromptWindowVM pwvm = new PromptWindowVM(p.PromptContent);
                    w.DataContext = pwvm;
                    w.ShowDialog();
                    Debug.WriteLine("nvokde");
                    p.PromptResponseHandler(pwvm.Result);
                    //(App.Current.MainWindow as MainWindow).InvokePrompt();

                });

                return;

            };


        }

        /// <summary>
        /// the point at which user clicked on the deails spliter relative to this wi
        /// </summary>
        Point _originPoint;

        public int SplitterMargin_details { get; set; } = 1;

        private void HndlClosing(object sender, CancelEventArgs e)
        {
            SaveGridSplittersState();
            MainVM mv = DataContext as MainVM;
            if (mv == null) return;
            e.Cancel = mv.OnClose();
            return;
        }

        private void SaveGridSplittersState()
        {
            //we don't have to , the app saves cofig which in turn has the serializable sizes ictionary
            Services.ConfigService.Instance.Save();
        }

        private void RetrieveGridSplittersState()
        {
            this.LastFolupDetailsWidth = new GridLength(ConfigService.Instance.GridSplittersState["FolupDetailsWidth"] ?? 300);
            this.LastDetailsHeight = ConfigService.Instance.GridSplittersState["DetailsWidth"] == null ? new GridLength(389, GridUnitType.Star) : new GridLength(ConfigService.Instance.GridSplittersState["DetailsWidth"].Value);
        }


       

        private void HndlStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                ConfigService.Instance.IsWindowMaximized = true;
            }
            else if (WindowState == WindowState.Normal)
            {
                ConfigService.Instance.IsWindowMaximized = false;
            }
        }

        bool lastIsPluginsPanelOpenState = true;
        GridLength LastPluginsPanelColWidth;

        bool lastIsFolupDetailsOpenState = false;


        //
        private void ToggleFolupDetails(bool isOpen)
        {
            if (lastIsFolupDetailsOpenState != isOpen)
            {
                lastIsFolupDetailsOpenState = isOpen;
                if (isOpen)
                {
                  
                }
                else
                {
                  
                }
            }
        }


        bool lastIsDetailsOpenState = false;
        GridLength LastDetailsHeight;
        GridLength LastFolupDetailsWidth;

        

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //the close menu item
            Close();
        }

      

        private void ColorZone_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainVM vm = DataContext as MainVM;

            var cr = (sender as MaterialDesignThemes.Wpf.ColorZone);
            //cr.ContextMenu.IsOpen = true;

            if (vm != null)
            {

            }
            e.Handled = true;
        }

        private void ColorZone_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var cr = (sender as MaterialDesignThemes.Wpf.ColorZone);
            cr.ContextMenu.IsOpen = true;
            e.Handled = true;
        }

    }
}
