using Mi.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundEasy.Services
{

    
    public enum CachePolicy {None = 0,
        CacheElements = 1,
        CachePages = 2,
        CacheAll = CacheElements|CachePages,
         }
    public enum WebClientProtocol { Default, Tls12, Tls12http1,
        Tls
    }
    /// <summary>
    /// auto updating file when dirty
    /// </summary>
    public class UserSettings
    {
       
        static UserSettings current;
        public static UserSettings Current
        {
            get
            {
                if (current == null)
                {
                    current = loadOrFactory();
                }
                return current;
            }

        }

        private void OnDirty()
        {
            save();
        }

        private CachePolicy _CachePolicy;
        public CachePolicy CachePolicy
        {
            get { return _CachePolicy; }
            set
            {
                if (_CachePolicy != value)
                {
                    _CachePolicy = value;
                    if (lock_save == false)
                        OnDirty();
                }

            }
        }



        private bool _IsCachePages;
        /// <summary>
        /// used as binding helpers (values are fed to the main enum prop
        /// </summary>
        [JsonIgnore]

        public bool IsCachePages
        {
            set
            {
                if (value == false)//removing flag
                {
                    CachePolicy = CachePolicy & (~CachePolicy.CachePages);
                }
                else if (value == true)
                {
                    CachePolicy = CachePolicy | CachePolicy.CachePages;
                }
            }
            get { return CachePolicy.HasFlag(CachePolicy.CachePages); }
        }


        private bool _IsCacheElements;
        /// <summary>
        /// used as binding helpers (values are fed to the main enum prop
        /// </summary>
        [JsonIgnore]
        public bool IsCacheElements
        {
            set
            {
                if (value == false)//removing flag
                {
                    CachePolicy = CachePolicy & (~CachePolicy.CacheElements);
                }
                else if (value == true)//adding flag
                {
                    CachePolicy = CachePolicy | CachePolicy.CacheElements;
                }
            }
            get { return CachePolicy.HasFlag(CachePolicy.CacheElements); }
        }



        private WebClientProtocol _WebClientProtocol;
        /// <summary>
        /// setting true will avoid saving.. workaround a stackoverflowexcepion
        /// </summary>
        static bool lock_save = false;
        /// <summary>
        /// temporary workaround
        /// </summary>
        public WebClientProtocol WebClientProtocol {
            get { return _WebClientProtocol; }
            set
            {
                if (_WebClientProtocol != value)
                {
                    _WebClientProtocol = value;
                    if(lock_save==false)
                    OnDirty();
                }

            }
        }
        private static UserSettings getFactoryUserSettings()
        {
            
            UserSettings s = new UserSettings();
            s._CachePolicy = CachePolicy.None;//it is immoprtant to acess backing fiels to avoid stackoverflowexception
            s._WebClientProtocol = WebClientProtocol.Tls12;
            return s;
        }

        static object lock_ = new object();
        public static readonly string DOCUMENTS = ApplicationInfo.DOCUMENTS;
        public static readonly string DOCUMENTS_SETTINGS = DOCUMENTS + "\\" + "Settings";
        static string SETTINGS_FILE = DOCUMENTS_SETTINGS + "\\user.settings.json";

        /// <summary>
        /// load file or make new file with factory settings, 
        /// </summary>
        /// <returns></returns>
        private static UserSettings loadOrFactory()
        {
            //#if file exists and valid
            lock (lock_)
            {
                lock_save = true;
                try
                {
                    var r = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SETTINGS_FILE));
                    lock_save = false;
                    return r;
                }
                catch (Exception)
                {
                    var s = getFactoryUserSettings();
                    string s_str = Newtonsoft.Json.JsonConvert.SerializeObject(s);
                    File.WriteAllText(SETTINGS_FILE, s_str);
                    lock_save = false;
                    return s;
                }
            }
            

            //else
        }
        /// <summary>
        /// saves current into file
        /// </summary>
        public void save()
        {
            lock (lock_)
            {
                string s_str = Newtonsoft.Json.JsonConvert.SerializeObject(Current);
                File.WriteAllText(SETTINGS_FILE, s_str);
            }
        }
    }
}
