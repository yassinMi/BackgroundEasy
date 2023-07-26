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
        /// <summary>
        /// accessing this infers the format from the <see cref="OutputFilenameTemplate"/> defaultig to png
        /// </summary>
        public System.Drawing.Imaging.ImageFormat FromatFromTemplate { get
            {
                string ext = OutputFilenameTemplate?.Split('.').LastOrDefault();
                if(ext==null) return System.Drawing.Imaging.ImageFormat.Png; 
                if (ext.Equals("png", StringComparison.OrdinalIgnoreCase))
                {
                    return System.Drawing.Imaging.ImageFormat.Png;
                }
                else if (ext.Equals("jpg", StringComparison.OrdinalIgnoreCase))
                {
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
                }
                else if (ext.Equals("jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
                }
                else
                {
                    return System.Drawing.Imaging.ImageFormat.Png;
                }
            } }

    }
}
