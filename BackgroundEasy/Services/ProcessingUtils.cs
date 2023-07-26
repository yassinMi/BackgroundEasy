using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BackgroundEasy.Services
{
    public static class ProcessingUtils
    {
        public static System.Windows.Rect GetBgPositionRectangle(Size bgSize, Size pngSize, ContentAlignment alignment)
        {
            if (bgSize.Width < pngSize.Width || bgSize.Height < pngSize.Height)
            {
                throw new ArgumentException("bgSize should be greater than or equal to pngSize in both dimensions.");
            }

            int x, y;
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    x = 0;
                    y = 0;
                    break;
                case ContentAlignment.TopCenter:
                    x = (bgSize.Width - pngSize.Width) / 2;
                    y = 0;
                    break;
                case ContentAlignment.TopRight:
                    x = bgSize.Width - pngSize.Width;
                    y = 0;
                    break;
                case ContentAlignment.MiddleLeft:
                    x = 0;
                    y = (bgSize.Height - pngSize.Height) / 2;
                    break;
                case ContentAlignment.MiddleCenter:
                    x = -(bgSize.Width - pngSize.Width) / 2;
                    y = -(bgSize.Height - pngSize.Height) / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    x = bgSize.Width - pngSize.Width;
                    y = (bgSize.Height - pngSize.Height) / 2;
                    break;
                case ContentAlignment.BottomLeft:
                    x = 0;
                    y = bgSize.Height - pngSize.Height;
                    break;
                case ContentAlignment.BottomCenter:
                    x = (bgSize.Width - pngSize.Width) / 2;
                    y = bgSize.Height - pngSize.Height;
                    break;
                case ContentAlignment.BottomRight:
                    x = bgSize.Width - pngSize.Width;
                    y = bgSize.Height - pngSize.Height;
                    break;
                default:
                    throw new ArgumentException("Invalid ContentAlignment value.");
            }

            return new System.Windows.Rect(x, y, bgSize.Width, bgSize.Height);
        }

        public static BitmapImage GetResiezedImage(BitmapImage img, int newWidth, int newHeight)
        {
            double scaleX = (double)newWidth / img.PixelWidth;
            double scaleY = (double)newHeight / img.PixelHeight;
            double scale = Math.Min(scaleX, scaleY);

            int targetWidth = (int)(img.PixelWidth * scale);
            int targetHeight = (int)(img.PixelHeight * scale);

            var resizedImage = new TransformedBitmap(img, new System.Windows.Media.ScaleTransform(scale, scale));

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(resizedImage));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = memoryStream;
                result.EndInit();
                result.Freeze();

                return result;
            }
        }



        public static BitmapImage FlipImage(BitmapImage originalImage)
        {
            if (originalImage == null)
                throw new ArgumentNullException(nameof(originalImage));

            int width = originalImage.PixelWidth;
            int height = originalImage.PixelHeight;

            // Create a TransformedBitmap with a 90-degree rotation and horizontal flip.
            TransformedBitmap transformedBitmap = new TransformedBitmap(originalImage, new System.Windows.Media.TransformGroup
            {
                Children = new System.Windows.Media.TransformCollection
        {
            new System.Windows.Media.RotateTransform(90),
            new System.Windows.Media.ScaleTransform(-1, 1)
        }
            });

            // Create a new BitmapImage with the transformed image source.
            BitmapImage flippedImage = new BitmapImage();
            flippedImage.BeginInit();
            flippedImage.CacheOption = BitmapCacheOption.OnLoad;
            flippedImage.StreamSource = ConvertBitmapToMemoryStream(transformedBitmap);
            flippedImage.EndInit();
            flippedImage.Freeze(); // Freeze the image to improve performance.

            return flippedImage;
        }

        private static MemoryStream ConvertBitmapToMemoryStream(BitmapSource bitmapSource)
        {
            MemoryStream memoryStream = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder(); // You can use other encoders like JpegBitmapEncoder if needed.
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);
            return memoryStream;
        }


        /// <summary>
        /// the production as opposed o preview version of the utility
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Image ResizeImageProduction(Image originalImage, int newWidth, int newHeight)
        {
            // Create a new Bitmap with the desired dimensions
            Bitmap resizedImage = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Create a Graphics object from the resizedImage
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                // Set the interpolation mode to HighQualityBicubic for better image quality
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // Set the pixel offset mode to HighQuality for better rendering
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                // Set the smoothing mode to HighQuality for better smoothing of lines and curves
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                // Set the CompositingQuality to HighQuality for better compositing output
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                // Draw the original image on the Graphics object with the desired dimensions
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }

    }
}
