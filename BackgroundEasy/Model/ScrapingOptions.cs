using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundEasy.Model
{
    public class ScrapingOptions
    {
        public string UrlTemplate { get; set; }
        public string OutputFilenameTemplate { get; set; }
        public int? RequestsDelayMs { get; set; }
        
        /// <summary>
        /// images location
        /// </summary>
        public string DumpDir { get; set; }
       
        /// <summary>
        /// if true, existing files wont be downloaded, otherwise they will be replaced (if the download fails they won't be replaced or deleted)
        /// </summary>
        public bool SkipExisting { get; set; }

    }
}
