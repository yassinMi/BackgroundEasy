using BackgroundEasy.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace BackgroundEasy.Services
{
    public class ScrapingHelper
    {


        static ScrapingHelper()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        }
        public static string headersSeparator = ";";
        static HttpClient hc = new HttpClient();

        /// <summary>
        /// parses validates ane applies the headers sring e.g. key: value; keyB: ValueB
        /// NOTE: the separaor is not hardcoded <see cref="headersSeparator"/>
        /// </summary>
        /// <param name="headersStr"></param>
        public void ConfigureCustomHeaders(string headersStr)
        {
            if (string.IsNullOrWhiteSpace(headersStr))
            {
                hc.DefaultRequestHeaders.Clear();
                return;
            }


            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();

            // Split the headers string into individual lines
            string[] headerLines = Regex.Split(headersStr, $@"(?<!\\){headersSeparator}");

            // Parse and add each header to HttpClient's DefaultRequestHeaders
            foreach (string headerLine in headerLines)
            {
                string header = headerLine.Replace($@"\{headersSeparator}", headersSeparator).Trim();
                int separatorIndex = header.IndexOf(':');
                if (separatorIndex > 0 && separatorIndex < header.Length - 1)
                {
                    string headerName = header.Substring(0, separatorIndex).Trim();
                    string headerValue = header.Substring(separatorIndex + 1).Trim();
                    if (string.IsNullOrWhiteSpace(headerName))
                    {
                        throw new Exception("empty header name");
                    }
                    lst.Add(new KeyValuePair<string, string>(headerName, headerValue));
                }
            }

            //apply
            hc.DefaultRequestHeaders.Clear();
            foreach (var h in lst)
            {
                hc.DefaultRequestHeaders.Add(h.Key, h.Value);
            }


        }

        public async Task<int> ScrapeMessages(string[] items, ProcessingOptions Options, Action<ProcessingProgressReport> chunckCallback)
        {
            int successCC = 0;
            foreach (var item in items)
            {
                string inputpath = item;
                string inputImageNameWithoutExt = Path.GetFileNameWithoutExtension(inputpath);
                string outpImageNameAndExt = CoreUtils.SanitizeFileName(Options.OutputFilenameTemplate.Replace("{ImageName}", inputImageNameWithoutExt));

                string outputpath = System.IO.Path.Combine(Options.DumpDir, outpImageNameAndExt);
                long imgBytes = 0;
                if (Options.SkipExisting && File.Exists(outputpath))
                {
                    imgBytes = new FileInfo(outputpath).Length;
                    chunckCallback(new ProcessingProgressReport() { ExistingImages = 1, FailedImages = 0, ImagesSize = (long)imgBytes, ProcessedImages = 1 });
                    continue;
                }
                //at this point file does not exist or re-downloading is required anyway
                byte[] img_data = null;
                bool threwException = true;
                try
                {

                    //img_data = await FakeRequest(url).ConfigureAwait(false);
                    var bg = Options.Background;

                    var img = File.ReadAllBytes(inputpath);
                    BackgroundLayeringOptions opts = new BackgroundLayeringOptions();
                    img_data = await AddBackgroundToImage(img, bg, opts).ConfigureAwait(false);

                    imgBytes = img_data.Length;
                    threwException = false;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }


                if (threwException)
                {
                    var existsPreviously = File.Exists(outputpath);
                    if (existsPreviously)
                    {
                        imgBytes = new FileInfo(outputpath).Length;
                    }
                    chunckCallback(new ProcessingProgressReport() { ExistingImages = existsPreviously ? 1 : 0, FailedImages = 1, ImagesSize = existsPreviously ? imgBytes : 0, NewImages = 0, ProcessedImages = 1 });
                    continue;
                }
                File.WriteAllBytes(outputpath, (img_data)); //throwig here should abort the task
                chunckCallback(new ProcessingProgressReport() { ExistingImages = 0, FailedImages = 0, ImagesSize = imgBytes, NewImages = 1, ProcessedImages = 1 });

            }


            return successCC;
        }

        private async Task<byte[]> FakeRequest(string url)
        {
            CoreUtils.WriteLine($"dl: {url}");
            await Task.Delay(1).ConfigureAwait(false);
            var size = (int)(Utils.Rnd() * 500000);
            return Enumerable.Range(0, size).Select(s => new byte()).ToArray();
        }


        public async Task<byte[]> AddBackgroundToImage(byte[] image, Background bg, BackgroundLayeringOptions opts)
        {
            //# validate params
            var backgroundColor = bg.BackgroundColor;
            // Load the PNG image and the background image
            using (Image pngImage = Image.FromStream(new MemoryStream(image)))//leak

            // Create a new bitmap to combine the images
            using (Bitmap combinedImage = new Bitmap(pngImage.Width, pngImage.Height))
            {
                // Set the background color for the combined image
                using (Graphics g = Graphics.FromImage(combinedImage))
                {

                    g.Clear(bg.BackgroundColor);

                }

                // Draw the background image on the combined image
                if (bg.BackgroundImage != null )
                    using (Image backgroundImage = Image.FromStream(new MemoryStream(bg.BackgroundImage)))
                    using (Graphics g = Graphics.FromImage(combinedImage))
                    {
                        g.DrawImage(backgroundImage, 0, 0, backgroundImage.Width, backgroundImage.Height);
                    }

                // Draw the PNG image on top of the background
                using (Graphics g = Graphics.FromImage(combinedImage))
                {
                    g.DrawImage(pngImage, 0, 0, pngImage.Width, pngImage.Height);
                }

                // Save the combined image to the specified output path
                using (MemoryStream ms = new MemoryStream())
                {
                    combinedImage.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }


            }
        }


    }

    public class Background
    {
        public Color BackgroundColor { get; set; }
        public byte[] BackgroundImage { get; internal set; }
    }
    public class BackgroundLayeringOptions
    {

    }

    /// <summary>
    /// a chunck of progress, not the agregate of the entire task
    /// </summary>
    public struct ProcessingProgressReport
    {
        /// <summary>
        /// successfully downloaded images
        /// </summary>
        public int NewImages { get; set; }
        public int ExistingImages { get; set; }
        /// <summary>
        /// failed to download
        /// </summary>
        public int FailedImages { get; set; }
        /// <summary>
        /// the size of the existing or successfully downloaded images
        /// </summary>
        public long ImagesSize { get; set; }
        /// <summary>
        /// sine the last update, not the total 
        /// </summary>
        public int ProcessedImages { get; internal set; }
    }
}
