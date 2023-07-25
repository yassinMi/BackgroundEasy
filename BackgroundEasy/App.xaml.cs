using MaterialDesignThemes.Wpf;
using Mi.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using BackgroundEasy.Services;
using System.Windows.Threading;

namespace BackgroundEasy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// not to be used by the end user, this enables testing 
        /// </summary>
        public string UserDataDirectory { get; set; }

        private const string AppName = "BackgroundEasy-d9ec4936ba72be5c53393e1d";
        private const string UniqueEventName = "BackgroundEasy-beb3b717a818bf55d81c3d9c";
        private EventWaitHandle eventWaitHandle;

        private Mutex _mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            ApplicationInfo.OnAppStartup();

            base.OnStartup(e);

        }
      

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ConfigService.Instance.Save();
        }
        public List<string> CommandLineArgs { get; set; } = new List<string>();


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-GB");
            DispatcherUnhandledException += hndlUnhandeledExceptions;
            Thread.CurrentThread.Name = "ui thread";

            Debug.WriteLine("started");
            //CoreUtils.WriteLine($"app started [{DateTime.Now}]");

            CommandLineArgs.AddRange(e.Args);
            Debug.WriteLine(string.Join(",", CommandLineArgs));

            //InitAme();
            
            Debug.WriteLine("setting up workspace..");
            //# verify the dependencies
            try
            {
                //curl.exe verification here (if required)
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, ApplicationInfo.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(4);
            }
            


            bool isOnwned;

            this._mutex = new Mutex(true, AppName, out isOnwned);
            this.eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);
            GC.KeepAlive(this._mutex);

            if (isOnwned)
            {
                var thread = new Thread(() => {
                    while (this.eventWaitHandle.WaitOne())
                    {
                        Current.Dispatcher.BeginInvoke((Action)(() => {
                            ShowMainWindow();
                        }));
                    }
                });
                thread.IsBackground = true;
                thread.Start();
                ShowMainWindow();

                //PropertyingService.Instance.RunWorkerAsync();



                return;

            }

            this.eventWaitHandle.Set();
            this.Shutdown();





        }

        private void hndlUnhandeledExceptions(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            CoreUtils.WriteLine($"[{DateTime.Now}] app unhandeled exception:  {e.Exception}");
#if DEBUG
            MessageBox.Show($"DispatcherUnhandledException:{Environment.NewLine}{Environment.NewLine}{e.Exception.ToString()}", ApplicationInfo.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);

#else
           MessageBox.Show($"DispatcherUnhandledException:{Environment.NewLine}{Environment.NewLine}{e.Exception.Message}", ApplicationInfo.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
#endif

            e.Handled = true;
            Shutdown();

        }
        private void InitAme()
        {

            //AvalonMaterialEdit.TextEditorAssist.GetFieldDescriptorsName = (f) => { return (f as FieldDescriptor).PropName; };
            /* AvalonMaterialEdit.CompletionDataSql.GetCompletionDataSqlFromFieldDescriptor = (fo) => {
                 var f = fo as FieldDescriptor;
                 var res = new AvalonMaterialEdit.CompletionDataSql(f);
                 res.Text = f.PropName;
                 var sp = new System.Windows.Controls.StackPanel();
                 var tb = new System.Windows.Controls.TextBlock(new System.Windows.Documents.Run(f.PropName));
                 tb.Margin = new Thickness(4, 4, 4, 4);
                 TextOptions.SetTextFormattingMode(tb, TextFormattingMode.Display);
                 TextOptions.SetTextFormattingMode(sp, TextFormattingMode.Display);

                 System.Windows.Controls.Image i = new System.Windows.Controls.Image();
                 //i.Source = (System.Windows.Media.Imaging.BitmapImage)new Converters.FieldTypeToIconConverter().Convert(f.FieldType, typeof(ImageSource), null, System.Globalization.CultureInfo.InvariantCulture);
                 //res.Image = (System.Windows.Media.Imaging.BitmapImage)new Converters.FieldTypeToIconConverter().Convert(f.FieldType, typeof(ImageSource), null, System.Globalization.CultureInfo.InvariantCulture);

                 i.VerticalAlignment = VerticalAlignment.Center;
                 tb.VerticalAlignment = VerticalAlignment.Center;
                 i.Height = 16;
                 i.Width = 16;
                 sp.Orientation = System.Windows.Controls.Orientation.Horizontal;
                 //sp.Children.Add(i);
                 sp.Children.Add(tb);
                 res.Content = sp;

                 res.Description = $"{f.PropName}";

                 return res;

             };
             */

        }
        public static Window FindOpenWindowByType(Type targetType)
        {
            foreach (var item in Application.Current.Windows)
            {
                if (item.GetType() == targetType)
                {
                    return item as Window;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ShowMainWindow()
        {
            MainWindow maybeMainWindowRef = (MainWindow)FindOpenWindowByType(typeof(MainWindow));
            if (maybeMainWindowRef != null)
            {
                if (maybeMainWindowRef.WindowState == System.Windows.WindowState.Minimized)
                    maybeMainWindowRef.WindowState = System.Windows.WindowState.Normal;
                maybeMainWindowRef.Activate();
                maybeMainWindowRef.Topmost = true;
                maybeMainWindowRef.Topmost = false;
                maybeMainWindowRef.Focus();
            }
            else
            {
                MainWindow newMainWindow = new MainWindow();
                Application.Current.MainWindow = newMainWindow;
                newMainWindow.Show();
            }

        }

        

    }
}
