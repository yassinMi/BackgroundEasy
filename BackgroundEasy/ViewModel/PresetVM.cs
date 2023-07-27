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
using System.Windows.Input;
using Mi.Common;

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
            set { _Model = value;

                notif(nameof(Model));
                notif(nameof(ThumbImageSource));
                notif(nameof(IsImgsCCVisible));
                notif(nameof(IsImageType));

            }
            get { return _Model; }
        }



        private bool _IsSelected;
        /// <summary>
        /// currently selected (used)
        /// </summary>
        public bool IsSelected
        {
            set { _IsSelected = value; notif(nameof(IsSelected)); }
            get { return _IsSelected; }
        }


        public bool IsImgsCCVisible
        {
            get { return Model.ImagePaths!=null; }
        }






        public ImageSource ThumbImageSource
        {
            
            get { return GetThumbImageSource(); }
        }

        public bool IsImageType { get { return Model.IsImageType; } }

        public ImageSource GetThumbImageSource()
        {
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            int height =30, width = 30;
            if (IsImageType)
            {
                var img = ProcessingUtils.CreateThumbnail(Model.ImagePath??Model.ImagePaths?.FirstOrDefault());
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


        public event EventHandler SelectRequest;
        public ICommand SelectCommand { get { return new MICommand(hndlSelectCommand, canExecuteSelectCommand); } }

        private bool canExecuteSelectCommand()
        {
            return true;
        }

        private void hndlSelectCommand()
        {
            try
            {
                SelectRequest?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception err)
            {

                MainVm.ReportErr(err);
            }
        }

        public event EventHandler DeleteRequest;
        public ICommand DeleteCommand { get { return new MICommand(hndlDeleteCommand, canExecuteDeleteCommand); } }

        private bool canExecuteDeleteCommand()
        {
            return true;
        }

        private void hndlDeleteCommand()
        {
            try
            {
                DeleteRequest?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception err)
            {
                MainVm.ReportErr(err);
            }
        }





    }
}
