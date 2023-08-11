using Mi.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BackgroundEasy.Model;

namespace BackgroundEasy.Services
{
    [Serializable]
    /// <summary>
    /// the MVVM alternative for the Config class
    /// singletone
    /// </summary>
    public class ConfigService : INotifyPropertyChanged
    {
        [NonSerialized]
        private static ConfigService _instance;
        public static ConfigService Instance
        {
            get
            { if (_instance != null) return _instance;
              else { _instance = ConfigService.Load();
                    Trace.WriteLine("ConfigService Load ended");

                    return _instance; }
            }
        }

        [NonSerialized]
        private static ConfigService _factoryInstance;
        public static ConfigService Factory
        {
            get
            {
                if (_factoryInstance == null) 
                {
                    _factoryInstance = ConfigService.FactoryConfig();
                }
                return _factoryInstance;
            }
        }

        private ConfigService()
        {
           

        }



        #region props


        private bool _IsWindowMaximized=false;
        public bool IsWindowMaximized
        {
            set { _IsWindowMaximized = value; notif(nameof(IsWindowMaximized)); }
            get { return _IsWindowMaximized; }
        }



        private bool _IsDarkTheme;
        public bool IsDarkTheme
        {
            set { _IsDarkTheme = value; notif(nameof(IsDarkTheme)); }
            get { return _IsDarkTheme; }
        }

       
       

        private bool _TopMost;
        public bool TopMost
        {
            set { _TopMost = value; notif(nameof(TopMost)); }
            get { return _TopMost; }
        }



      



      

        private bool _IsDetailsPanelOpen=true;
        public bool IsDetailsPanelOpen
        {
            set { _IsDetailsPanelOpen = value; notif(nameof(IsDetailsPanelOpen)); }
            get { return _IsDetailsPanelOpen; }
        }

        



        



        private DateTime? _LastRun;
        public DateTime? LastRun
        {
            set { _LastRun = value; notif(nameof(LastRun)); }
            get { return _LastRun; }
        }


        


        


        
        



        private GridSpliterStateDictionary _GridSplittersState = new GridSpliterStateDictionary();
        public GridSpliterStateDictionary GridSplittersState
        {
            set { _GridSplittersState = value; notif(nameof(GridSplittersState)); }
            get { return _GridSplittersState; }
        }







        //user settings




       

       




        


        private string _OutputFilenameTemplate=@"{ImageName}.jpg";
        public string OutputFilenameTemplate
        {
            set { _OutputFilenameTemplate = value; notif(nameof(OutputFilenameTemplate)); }
            get { return _OutputFilenameTemplate; }
        }


        private string _BgPlacement = "Contain";
        public string BgPlacement
        {
            set { _BgPlacement = value; notif(nameof(BgPlacement)); }
            get { return _BgPlacement; }
        }



        //last user task input 




        private string _LastUserBackgroundImagePath;
        public string LastUserBackgroundImagePath
        {
            set { _LastUserBackgroundImagePath = value; notif(nameof(LastUserBackgroundImagePath)); }
            get { return _LastUserBackgroundImagePath; }
        }


        private string _LastUserOutputDirectory = null;
        public string LastUserOutputDirectory
        {
            set { _LastUserOutputDirectory = value; notif(nameof(LastUserOutputDirectory)); }
            get { return _LastUserOutputDirectory; }
        }


       



        private bool _LastUserShouldSkipExisting=true;
        public bool LastUserShouldSkipExisting
        {
            set { _LastUserShouldSkipExisting = value; notif(nameof(LastUserShouldSkipExisting)); }
            get { return _LastUserShouldSkipExisting; }
        }


        private System.Windows.Media.Color _LastUserColorPickerValue = System.Windows.Media.Colors.LightSeaGreen;
        public System.Windows.Media.Color LastUserColorPickerValue
        {
            set { _LastUserColorPickerValue = value; notif(nameof(LastUserColorPickerValue)); }
            get { return _LastUserColorPickerValue; }
        }




        private double _LastUserToleranceSliderValue = 0;
        public double LastUserToleranceSliderValue
        {
            set { _LastUserToleranceSliderValue = value; notif(nameof(LastUserToleranceSliderValue)); }
            get { return _LastUserToleranceSliderValue; }
        }






        private string _LastUserSelectedPreviewExample="Portrait - small";
        public string LastUserSelectedPreviewExample
        {
            set { _LastUserSelectedPreviewExample = value; notif(nameof(LastUserSelectedPreviewExample)); }
            get { return _LastUserSelectedPreviewExample; }
        }


        private string _LastUserSelectedPresetName;
        public string LastUserSelectedPresetName
        {
            set { _LastUserSelectedPresetName = value; notif(nameof(LastUserSelectedPresetName)); }
            get { return _LastUserSelectedPresetName; }
        }


        private int _LastUserSelectedTabIx=0;
        public int LastUserSelectedTabIx
        {
            set { _LastUserSelectedTabIx = value; notif(nameof(LastUserSelectedTabIx)); }
            get { return _LastUserSelectedTabIx; }
        }




        private string[] _LastUserBackgroundImagesPaths;
        public string[] LastUserBackgroundImagesPaths
        {
            set { _LastUserBackgroundImagesPaths = value; notif(nameof(LastUserBackgroundImagesPaths)); }
            get { return _LastUserBackgroundImagesPaths; }
        }






        #endregion props


        /// <summary>
        /// ts
        /// </summary>
        /// <param name="saveAS"></param>
        /// <returns></returns>
        public ConfigService Save(string saveAS = null)
        {
            lock (this)
            {
                if (saveAS == null) saveAS = ApplicationInfo.APP_CONFIG_FILE;
                using (var stream = File.Open(saveAS, FileMode.Create))
                {
                    sr.Serialize(stream, this);
                }
                Trace.WriteLine("configservice: saved");
                //MainWindow.ShowMessage("Settings Saved");
                return this;
            }
        }


        /// <summary>
        /// attemps to load the xml config file, if file is missing the factory config is automatically saved and returned
        /// throws unhandled exceptions if file deserialization fails
        /// </summary>
        /// <param name="ConfigFile">if not specified, the MI.APP_CONFIG_FILE_V2 is used  </param>
        /// <returns></returns>
        private static ConfigService Load(string ConfigFile = null)
        {
            Trace.WriteLine("ConfigService Load called");
            if (ConfigFile == null) ConfigFile = ApplicationInfo.APP_CONFIG_FILE;
            if (!File.Exists(ConfigFile))
            {
                return ConfigService.FactoryConfig().Save();
            }
            using (var stream = File.OpenRead(ConfigFile))
            {
                return sr.Deserialize(stream) as ConfigService;
            }
           
        }


        /// <summary>
        /// makes up a new config object, used to reset factory setting when requested by the user or when the xml conf is missing or is corrupt
        /// NOTE: this doesnt change the Instance, but rathr creates and returns a new object
        /// </summary>
        private static ConfigService FactoryConfig()
        {
            var fc = new ConfigService();
            return fc;
        }


   




        static XmlSerializer sr = new XmlSerializer(typeof(ConfigService));

        public event PropertyChangedEventHandler PropertyChanged;
        private void notif(string propertyName)
        {
            Debug.WriteLine("ConfigServce Instance is dirty");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }

    

}
