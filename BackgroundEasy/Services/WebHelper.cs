using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BackgroundEasy.Services;

namespace Mi.Utils
{
    public interface IWebHelper
    {
        string GetPageTextSync(string pageUrl);

        Task<string> GetPageText(string pageUrl);
    }

    public class WebHelper:  HttpClient, IWebHelper
    {
        public const string USER_AGENT = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36";
        public const string UA2 = " Mozilla/5.0 (compatible; AcmeInc/1.0)";
        public const string UA3 = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

        public WebHelper(HttpClientHandler hndler):base(hndler)
        {

        }
        public WebHelper()
        {

        }

        static WebHelper _instance = null;
        public static WebHelper instance
        {
            get
            {
                if (_instance == null)
                {
                    HttpClientHandler hnder = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.Deflate 
                    };
                    //hnder.serve
                      
                    if(UserSettings.Current.WebClientProtocol== WebClientProtocol.Tls)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    }
                    else if (UserSettings.Current.WebClientProtocol == WebClientProtocol.Tls12)
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                    else if (UserSettings.Current.WebClientProtocol == WebClientProtocol.Default)
                    {

                    }

                    //# temp to fix ss
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.Expect100Continue = true;
                    //ServicePointManager.DefaultConnectionLimit = 100;

                    Trace.WriteLine(ServicePointManager.SecurityProtocol);
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    //#

                    _instance = new WebHelper(hnder);

                    if (_instance.DefaultRequestHeaders.UserAgent.TryParseAdd(UA2))
                    {
                        Debug.WriteLine("can be parsed");
                    }
                    _instance.DefaultRequestHeaders.UserAgent.ParseAdd(UA2);
                    _instance.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    _instance.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
                    _instance.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"104\", \" Not A;Brand\";v=\"99\", \"Google Chrome\";v=\"104\"");
                    _instance.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    _instance.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                    _instance.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
                    _instance.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
                    _instance.DefaultRequestHeaders.Add("sec-fetch-site", "none");//none/same-origin
                    _instance.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
                    _instance.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");


                    //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                }
                return _instance;
            }
        } 
        public string GetPageTextSync(string pageUrl)
        {
           
            var t = this.GetStringAsync(pageUrl);
            Exception erorr = null;
            string result = null;
            var res =t.ContinueWith(te => {
                AggregateException exception = t.Exception;
                Debug.WriteLine($"ContinueWith faulted: {exception}, inner: {exception?.InnerException}");
                erorr= exception.InnerException;
            }, TaskContinuationOptions.OnlyOnFaulted) ;
            
            var f =t.ContinueWith( tt=> {
                Debug.WriteLine("ContinueWith true");
                result= tt.Result;
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            try
            {
                Task.WaitAll(res,f);
            }
            catch (Exception)
            {

                
            }
            
            if (erorr != null)
            {
                throw erorr;
            }
            else {
                if (result == null)
                {
                    throw new Exception("unknown result and error");
                }
                else
                {
                    return result;
                }
            }
           
            
        }

        public Task<string> GetPageText(string pageUrl)
        {
            return this.GetStringAsync(pageUrl);
        }
    }
}
