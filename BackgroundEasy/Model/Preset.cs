using BackgroundEasy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BackgroundEasy.Model
{
    public class Preset
    {
        public bool IsImageType { get { return ImagePath != null || ImagePaths != null; } }
        public string ImagePath { get; set; }
        public string[] ImagePaths { get; set; }
        public string SolidColorHex { get; set; }
        public string Name { get; set; }
        public static Preset FromBackground(Background bg,string name)
        {
            return new Model.Preset()
            {
                ImagePath = bg.IsImageType ? bg.BackgroundImagePath : null,
                ImagePaths = bg.IsImageType ? bg.BackgroundImagePaths : null,
                Name = name,
                SolidColorHex = bg.IsImageType ? null : Utils.HexConverter(bg.BackgroundColor),
            };
        }

        internal Color TryGetColor()
        {
            return Utils.HexToColor(SolidColorHex);
        }
        /// <summary>
        /// example: |c:\ile.txt|c:\file.ts
        /// </summary>
        /// <returns></returns>
        public string GetStorageImagePathOrPaths()
        {
            if (ImagePath != null) return ImagePath;
            if (ImagePaths != null) return "|"+ string.Join("|", ImagePaths);
            return null;
        }
        /// <summary>
        /// example: |c:\ile.txt|c:\file.ts
        /// </summary>
        /// <returns></returns>
        public void SetStorageImagePathOrPaths(string storageStr)
        {
            if(string.IsNullOrEmpty(storageStr))
            {
                ImagePath = null; ImagePaths = null; return;
            }
            else if (storageStr.StartsWith("|"))
            {
                ImagePath = null; ImagePaths = storageStr.Split('|').ToArray(); return;
            }
            else 
            {
                ImagePath = storageStr; ImagePaths = null;
            }
            
        }
    }
}
