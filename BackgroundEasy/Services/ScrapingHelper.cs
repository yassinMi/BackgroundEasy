using BackgroundEasy.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public async Task<int> ScrapeMessages(string[] items, ScrapingOptions Options, Action<ScrapingProgressReport> chunckCallback)
        {
            int successCC = 0;
            foreach (var item in items)
            {
                
                string filenameAndExt = CoreUtils.SanitizeFileName(Options.OutputFilenameTemplate.Replace("{sku}", item));
                string fullpath = System.IO.Path.Combine(Options.DumpDir, filenameAndExt);
                string url = Options.UrlTemplate.Replace("{sku}", item);
                long imgBytes = 0;
                if (Options.SkipExisting && File.Exists(fullpath))
                {
                    imgBytes = new FileInfo(fullpath).Length;
                    chunckCallback(new ScrapingProgressReport() { ExistingImages = 1, FailedImages = 0, ImagesSize = (long)imgBytes, ProcessedImages = 1 });
                    continue;
                }
                //at this point file does not exist or re-downloading is required anyway
                byte[] img_data = null;
                bool threwException = true;
                try
                {
                    if ((Options.RequestsDelayMs ?? 0) > 0)
                    {
                        await Task.Delay(Options.RequestsDelayMs.Value);
                    }
                    //img_data = await FakeRequest(url).ConfigureAwait(false);
                    img_data = await hc.GetByteArrayAsync(url).ConfigureAwait(false);

                    imgBytes = img_data.Length;
                    threwException = false;
                }
                catch (Exception err)
                {
                }
                

                if (threwException)
                {
                    var existsPreviously = File.Exists(fullpath);
                    if (existsPreviously)
                    {
                        imgBytes = new FileInfo(fullpath).Length;
                    }
                    chunckCallback(new ScrapingProgressReport() { ExistingImages = existsPreviously ? 1 : 0, FailedImages = 1, ImagesSize = existsPreviously ? imgBytes : 0, NewImages = 0, ProcessedImages = 1 });
                    continue;
                }
                File.WriteAllBytes(fullpath, (img_data)); //throwig here should abort the task
                chunckCallback(new ScrapingProgressReport() { ExistingImages = 0, FailedImages = 0, ImagesSize = imgBytes, NewImages = 1, ProcessedImages = 1 });

            }


            return successCC;
        }

        private async Task< byte[]> FakeRequest(string url)
        {
            CoreUtils.WriteLine($"dl: {url}");
             await Task.Delay(1).ConfigureAwait(false);
            var size =(int)( Utils.Rnd() * 500000);
            return Enumerable.Range(0, size).Select(s=>new byte()).ToArray();
        }
        public async Task<int> ScrapeMessages_fake_impl(string[] items, ScrapingOptions Options, Action<ScrapingProgressReport> chunckCallback)
        {
            int successCC = 0;
            foreach (var item in items)
            {
                if ((Options.RequestsDelayMs ?? 0) > 0)
                {
                    await Task.Delay(Options.RequestsDelayMs.Value);
                }
                bool isExisting = Utils.Rnd() > 0.8;
                bool isFailed = (isExisting == false) && Utils.Rnd() > 0.9;
                int suc = (isExisting || !isFailed) ? 1 : 0;
                successCC += suc;
                chunckCallback(new ScrapingProgressReport() { NewImages = (!isExisting && !isFailed) ? 1 : 0, ImagesSize = 4878, FailedImages = (!isExisting && isFailed) ? 1 : 0, ProcessedImages = 1 });
            }


            return successCC;
        }
    }

    /// <summary>
    /// a chunck of progress, not the agregate of the entire task
    /// </summary>
    public struct ScrapingProgressReport
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
