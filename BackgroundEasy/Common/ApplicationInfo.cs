//v1
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BackgroundEasy;

namespace Mi.Common
{


    /// <summary>
    /// contains the info in the about view and other app info 
    /// uncomment stuff that's relevent to the project
    /// </summary>
    public class ApplicationInfo
    {

        public ApplicationInfo()
        {
            // Environment.CurrentDirectory
        }

        public static bool IsDev { get; set; } = false;
        public static string APP_DEV_NAME = "BackgroundEasy"; //used in creating app data directory and such

        //these fields are to be displayed to the user 
        public static string APP_TITLE { get; } = "BackgroundEasy";
        public static string APP_SUB_TITLE { get; } = "© - copyright todo - ";
        public static string APP_SHORT_DESCRIPTION { get; } = "A tool to Download Images From Website";
        public static string APP_VERSION_NOTE { get; } = "";
        /// <summary>
        /// raw
        /// </summary>
        public static string APP_VERSION_PART { get; } = "0.1.1";

        public static string APP_VERSION { get; } = APP_VERSION_PART + (IsDev ? " [dev]" : " (19-07-2023)");
        //public static string APP_DEVELOPER_NAME { get; set; } = "";
        //public static string APP_GUI_DESIGNER_NAME { get; set; } = "";
        //public static string APP_GITHUB_URL { get; set; } = "https://github.com/yassinMi/";
        public static string APP_DEVELOPER_EMAIL { get; set; } = "DIR16CAT17@gmail.com";

        /// <summary>
        /// the absolute path where the exe lives
        /// </summary>
        public static readonly string MAIN_PATH = Path.GetDirectoryName(
           System.Reflection.Assembly.GetExecutingAssembly().Location);
        //
        //public static readonly string COPYRIGHT_FILE = MAIN_PATH + "\\"  + "COPYRIGHT.rtf";
        public static readonly string FACTORY_SETTINGS_DIR = MAIN_PATH + "\\" + "FactorySettings";

        public static readonly string APP_DATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + APP_DEV_NAME;
        public static readonly string DOCUMENTS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + APP_DEV_NAME;
        public static readonly string DOCUMENTS_SETTINGS = DOCUMENTS + "\\" + "Settings";
        /// <summary>
        /// where tasks files reside (json files) under my documents/appname/
        /// </summary>
        public static readonly string DOCUMENTS_TASKS = DOCUMENTS + "\\" + "Tasks";
        public static readonly string DATABASE_FILE = DOCUMENTS + "\\" + "Data.db";
        //
        //public static string DEFAULT_GLOBAL_OUTPUT_DIR = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\"+APP_DEV_NAME+" Output";
        public static readonly string APP_CONFIG_FILE = APP_DATA + "\\" + APP_DEV_NAME + ".config.xml";
        internal static readonly string ERRORS_LOG_FILE = APP_DATA + @"\Errors.log";
        /// <summary>
        /// defines the UI names of the main application classes in a single place so that they can be easily modified
        /// </summary>
        public static UINames UIN_PRIMARY { get; } = (UINames)"sku,Images";




        public class UINames:IFormattable
        {
            /// <summary>
            /// the args are case insensitive
            /// </summary>
            /// <param name="singular"></param>
            /// <param name="plural"></param>
            public UINames(string singular, string plural)
            {
                Plural= plural.ToUpper().Substring(0, 1) + plural.ToLower().Substring(1);
                Singular = singular.ToUpper().Substring(0, 1) + singular.ToLower().Substring(1);
                this.plural = plural.ToLower();
                this.singular = singular.ToLower();
            }
            /// <summary>
            /// this ctor uses args as is
            /// </summary>
            /// <param name="singular"></param>
            /// <param name="plural"></param>
            /// <param name="Singlar"></param>
            /// <param name="Plural"></param>
            public UINames(string singular, string plural, string Singlar, string Plural)
            {
                this.Plural = Plural;
                this.Singular = Singlar;
                this.plural = plural;
                this.singular = singular;
            }
            public string Plural { get;  }
            public string plural { get;  }
            public string Singular { get;  }
            public string singular { get; }
            public static explicit operator UINames(string s)
            {
                var split = s.Split(',');
                if (split.Length==2) return new UINames(split[0], split[1]);
                return new UINames(s, s + "s");
            }
            public override string ToString()
            {
                return ToString("S", null);
            }

            static string AddDashBeforeChar(string str,char c)
            {
                var ix = str.ToLower().IndexOf(c);
                if (ix == -1) return str;
                return str.Insert(ix, "_");
            }
           /// <summary>
           /// s , sa , sam 
           /// </summary>
           /// <param name="format"></param>
           /// <param name="formatProvider"></param>
           /// <returns></returns>
            public string ToString(string format, IFormatProvider formatProvider)
            {
                if (string.IsNullOrWhiteSpace(format)) format = "S";

                string res;
                switch (format.First().ToString())
                {
                    case "S":
                        res =  Singular; break;
                    case "s":
                        res =  singular; break;
                    case "P":
                        res =  Plural; break;
                    case "p":
                        res =  plural; break;
                    default:
                        res =  Singular; break;
                }
                if (format.Length == 1) return res;
                if (format.Length == 2 && format[1] == 'a') return "_" + res;
                if (format.Length == 3 && format[1] == 'a') return AddDashBeforeChar(res, format[2]);
                return res;
            }
        }
        internal static void OnAppStartup()
        {
            if (Directory.Exists(APP_DATA) == false)
            {
                Directory.CreateDirectory(APP_DATA);
            }
            
            Directory.CreateDirectory(DOCUMENTS_SETTINGS);
           
            
           
           
        }
    }





}
