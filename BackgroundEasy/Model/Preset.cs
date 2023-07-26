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
        public string ImagePath { get; set; }
        public string SolidColorHex { get; set; }
        public string Name { get; set; }
        public static Preset FromBackground(Background bg,string name)
        {
            return new Model.Preset()
            {
                ImagePath = bg.IsImageType ? bg.BackgroundImagePath : null,
                Name = name,
                SolidColorHex = bg.IsImageType ? null : Utils.HexConverter(bg.BackgroundColor),
            };
        }

        internal Color TryGetColor()
        {
            return Utils.HexToColor(SolidColorHex);
        }
    }
}
