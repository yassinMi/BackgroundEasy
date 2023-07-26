using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Threading;
using System.IO;
using System.ComponentModel;
using BackgroundEasy.ViewModel;
using BackgroundEasy.Model;
using Microsoft.Win32;
using System.Net;
using System.Net.Http;
using Mi.Common;
using System.Reflection;
//using OfficeOpenXml;
using System.Windows.Media;
using System.Drawing;

namespace BackgroundEasy.Services
{

    /// <summary>
    /// some things here must have their own wraper class e.g SysTray controllers
    /// </summary>
    public static class Utils
    {


        static System.Drawing.ColorConverter cc = new System.Drawing.ColorConverter();
        public static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        internal static System.Drawing.Color HexToColor(string solidColorHex)
        {
            if (string.IsNullOrWhiteSpace(solidColorHex)) throw new ArgumentException($"color hex invalid: '{solidColorHex}'");
            return (System.Drawing.Color) cc.ConvertFromString(solidColorHex);
        }

        /// <summary>
        /// compute the next name for a collection with a numbered naming patteren, eg "myFile - copy(1)" "myFile - copy(2)"
        /// </summary>
        /// <param name="colection">the existig collection, if null or empty the first ame will be constructed</param>
        /// <param name="decode">extract number from an object from the collections. it is safe to throw exceptions, they will be igored</param>
        /// <param name="encode">convert the resulling number to the name to be returned, this must ot throw excepions</param>
        /// <param name="firstIx">0 or 1</param>
        public static string GetNextNumberedName<T>(IEnumerable<T> colection, Func<T, int> decode, Func<int, string> encode, int firstIx = 0)
        {

            int nextIx = firstIx;
            if (colection == null || !colection.Any()) return encode(nextIx);
            foreach (var item in colection)
            {
                try
                {
                    int ix = decode(item);
                    nextIx = ix + 1;
                }
                catch (Exception)
                {
                }
            }
            return encode(nextIx);
        }

        public static System.Drawing.Color DrawingColorFromMediaColor(System.Windows.Media.Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }
        public static System.Windows.Media.Color MediaColorFromDrawingColor(System.Drawing.Color c)
        {
            return  System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        /// <summary>
        /// compute the next name for a collection with a numbered naming patteren, eg "myFile - copy(1)" "myFile - copy(2)"
        /// </summary>
        /// <param name="colection">the existig collection of names, if null or empty the first ame will be constructed</param>
        /// <param name="prefix"> eg: "myFile - copy_" to acheieve the pattern: "myFile - copy_1" "myFile - copy_2"
        /// <param name="firstIx">0 or 1</param>
        public static string GetNextNumberedName(IEnumerable<string> colection, string prefix, int firstIx = 0)
        {

            int nextIx = firstIx;
            if (colection == null || !colection.Any()) return prefix + (nextIx.ToString());
            foreach (var item in colection)
            {
                try
                {
                    if (!item.StartsWith(prefix)) continue;
                    int ix = int.Parse(item.Substring(prefix.Length));
                    nextIx = ix + 1;
                }
                catch (Exception)
                {
                }
            }
            return prefix + (nextIx.ToString());
        }


        /// <summary>
        /// returns default local chromuim location chromium\win64-{982053}\chrome-win\chrome.exe 
        /// NOTE the version number in brackets can by any. in  case multiple versions exists it automatically gets the firt one with actual executable
        /// if more versions exists it picks the first one (by the fs enumeration)
        /// </summary>
        /// <returns></returns>
        internal static string TryGetLocalChromiumExePath()
        {
            string root = Path.Combine(ApplicationInfo.MAIN_PATH, @"chromium\");
            if (!Directory.Exists(root)) return null;
            //the "win64-982053"-like direcories
            var versions = Directory.GetDirectories(root);
            foreach (var v in versions)
            {
                string exe = Path.Combine(v, @"\chrome-win\chrome.exe");
                if (File.Exists(exe)) return exe;
            }

            return null;
        }


        
        
        /// <summary>
        /// returns typical chrome exe location on windows (returned file may or may not exist)
        /// </summary>
        /// <returns></returns>
        internal static string TryGetSystemChromeExePath()
        {
            string pf = Path.Combine(Environment.GetEnvironmentVariable("ProgramW6432"),
            @"Google\Chrome\Application\chrome.exe");
            Debug.WriteLine($"trying system chrome path: {pf}");
            // just in case the 
            if (!File.Exists(pf) && Environment.Is64BitOperatingSystem)
            {
                pf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
             @"Google\Chrome\Application\chrome.exe");
                Debug.WriteLine($"trying system chrome path: {pf}");
            }
            return pf;
        }



        

        public static T SelectMin<T>(IEnumerable<T> coll, Func<T, int> tr)
        {
            var reff = coll.FirstOrDefault();
            if (reff == null) return reff;
            foreach (var i in coll)
            {
                if (tr(i) < tr(reff)) reff = i;
            }
            return reff;
        }
        public static T SelectMax<T>(this IEnumerable<T> coll, Func<T, IComparable> tr)
        {
            var reff = coll.FirstOrDefault();
            if (reff == null) return reff;
            foreach (var i in coll)
            {
                
                if (tr(i).CompareTo( tr(reff))==1) reff = i;
            }
            return reff;
        }

        public static double ParseDblOrZero(string str)
        {
            double res;
            double.TryParse(str??"", out res);
            return res;
        }
        public static double? ParseDblNullable(string str)
        {
            double res;
            if (double.TryParse(str ?? "", out res))
                return res;
            else return null;
        }
        public static int ParseIntOrZero(string str)
        {
            int res;
            int.TryParse(str ?? "", out res);
            return res;
        }
        public static int? ParseIntNullable(string str)
        {
            int res;
            if (int.TryParse(str ?? "", out res))
                return res;
            else return null;
        }
        public static decimal ParseDecOrZero(string str)
        {
            decimal res;
            decimal.TryParse(str ?? "", out res);
            return res;
        }
        public static Uri ParseUriAbsOrNull(string str, UriKind kind = UriKind.Absolute)
        {
            Uri res = null;
            Uri.TryCreate(str ?? "", kind, out res);
            return res;
        }
        static Random r= new Random((int)DateTime.Now.Ticks);
        /// <summary>
        /// next double
        /// </summary>
        /// <returns></returns>
        public static double Rnd()
        {
            return r.NextDouble();
        }

        public static string TcatStr(string inp, int maxLength)
        {
            if (inp == null) return inp;
            if (inp.Length <= maxLength) return inp;
            return inp.Substring(0, maxLength) + "…";
        }

        public static string DateFormatUpload = "yyyyMMdd";
        static Utils()
        {
            try
            {
                wc = new WebClient() { Encoding = Encoding.UTF8 };
                //wc_headers = new WebHeaderCollection();
                //wc_headers.Add(HttpRequestHeader.UserAgent, new System.Net.Http.Headers.ProductInfoHeaderValue("QPortal", Assembly.GetAssembly(typeof(Utils)).GetName().Version.ToString()).ToString());
                
            }
            catch (Exception err)
            {

                Debug.WriteLine(err);
            }
           
        }
        

        static WebHeaderCollection wc_headers;
        static WebClient wc;

        //@deepee1
        public static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }


        /// <summary>
        /// bring the main window to front, creating new one if needed
        /// <param name="callback">action to run after complete initilization, thevm is passed</param>
        /// </summary>
        public static void ShowMainWindow(Action<MainVM> callback=null)
        {
            MainWindow maybeMainWindowRef =(MainWindow) FindOpenWindowByType(typeof(MainWindow));
            if (maybeMainWindowRef != null)
            {
                if (maybeMainWindowRef.WindowState == System.Windows.WindowState.Minimized)
                    maybeMainWindowRef.WindowState = System.Windows.WindowState.Normal;
                maybeMainWindowRef.Activate();
                maybeMainWindowRef.Topmost = true;
                maybeMainWindowRef.Topmost = false;
                maybeMainWindowRef.Focus();
                callback?.Invoke(maybeMainWindowRef.DataContext as MainVM);
                 }
            else
            {
                MainWindow newMainWindow = new MainWindow();
                Application.Current.MainWindow = newMainWindow;
                MainVM vm = newMainWindow.DataContext as MainVM;
                if (callback != null)
                {
                  
                }
                newMainWindow.Show();
                
                
            }
           
        }

        
        internal static void ShowSettingsWindow()
        {
            /*
            var mm = new SettingsWindowViewModel(ConfigService.Instance);

            CLWSettingsWindow csw = new CLWSettingsWindow() { DataContext = mm };
            mm.OnRequestClose += (ss, ee) =>
            {
                csw?.Close();
            };

            csw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            csw.ShowDialog();
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
        /// asynchronously starts a backgroundworker that creates and starts a process with the filename specified, 
        /// handels errors log
        /// </summary>
        /// <param name="fileToOpen"></param>
        internal static void TryOpenFileAsync(string fileToOpen)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s,e) =>{
                
                    Process.Start(fileToOpen);
                    e.Result = true;
                
            };
            bg.RunWorkerAsync();
            
        }
    }
}
