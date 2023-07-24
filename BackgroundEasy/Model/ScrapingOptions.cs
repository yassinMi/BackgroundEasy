using BackgroundEasy.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundEasy.Model
{
    public class ProcessingOptions
    {
        /// <summary>
        /// supported macros are: {ImageName} 
        /// </summary>
        public string OutputFilenameTemplate { get; set; }
        
        /// <summary>
        /// images location
        /// </summary>
        public string DumpDir { get; set; }
       
        /// <summary>
        /// if true, existing files wont be processed, otherwise they will be replaced (if the processing fails they won't be replaced or deleted)
        /// </summary>
        public bool SkipExisting { get; set; }
        public Background Background { get; set; }

    }
}
