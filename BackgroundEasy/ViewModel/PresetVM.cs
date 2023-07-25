using BackgroundEasy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BackgroundEasy.Model;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Drawing;

namespace BackgroundEasy.ViewModel
{
    public class PresetVM:BaseViewModel
    {
        public PresetVM()
        {
            //time
        }

        public PresetVM(Preset p, MainVM mainVM)
        {
            this.Model = p;
            this.MainVm = mainVM;
            Name = Model.Name;
        }

        private Preset _Model;
        public Preset Model
        {
            set { _Model = value; notif(nameof(Model)); }
            get { return _Model; }
        }


        

        public ImageSource ThumbImageSource
        {
            
            get { return GetThumbImageSource(); }
        }

        public bool IsImageType { get { return Model.ImagePath != null; } }

        public ImageSource GetThumbImageSource()
        {
            
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            int height =40, width = 40;
            if (IsImageType)
            {
                var img = new BitmapImage(new Uri(Model.ImagePath));
                width = img.PixelWidth;
                height = img.PixelHeight;
                drawingContext.DrawImage(img, new Rect(0, 0, width, height));

            }
            else
            {
                System.Drawing.Color c =(System.Drawing.Color) new System.Drawing.ColorConverter().ConvertFromString(Model.SolidColorHex);

                drawingContext.DrawRectangle(new SolidColorBrush(Utils.MediaColorFromDrawingColor(c)), null, new Rect(0, 0, width, height));
            }


            drawingContext.Close();

            var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            renderTarget.Render(drawingVisual);
            return renderTarget;
        }

        private string _Name;
       

        public string Name
        {
            set { _Name = value; notif(nameof(Name)); }
            get { return _Name; }
        }

        public MainVM MainVm { get; private set; }
    }
}
