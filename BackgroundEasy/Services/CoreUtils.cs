
using Mi.Common;
using Mi.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Trinet.Core.IO.Ntfs;

namespace BackgroundEasy.Services
{

    

  


    //@configurator way
    public class Synchronizer<T>
    {
        private Dictionary<T, object> locks;
        private object myLock;

        public Synchronizer()
        {
            locks = new Dictionary<T, object>();
            myLock = new object();
        }

        public object this[T index]
        {
            get
            {
                lock (myLock)
                {
                    object result;
                    if (locks.TryGetValue(index, out result))
                        return result;

                    result = new object();
                    locks[index] = result;
                    return result;
                }
            }
        }
    }
    public static class CoreUtils
    {


        public static CookieCollection GetAllCookiesFromHeader(string strHeader, string strHost)
        {
            ArrayList al = new ArrayList();
            CookieCollection cc = new CookieCollection();
            if (strHeader != string.Empty)
            {
                al = ConvertCookieHeaderToArrayList(strHeader);
                cc = ConvertCookieArraysToCookieCollection(al, strHost);
            }
            return cc;
        }


        private static ArrayList ConvertCookieHeaderToArrayList(string strCookHeader)
        {
            strCookHeader = strCookHeader.Replace("\r", "");
            strCookHeader = strCookHeader.Replace("\n", "");
            string[] strCookTemp = strCookHeader.Split(',');
            ArrayList al = new ArrayList();
            int i = 0;
            int n = strCookTemp.Length;
            while (i < n)
            {
                if (strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    al.Add(strCookTemp[i] + "," + strCookTemp[i + 1]);
                    i = i + 1;
                }
                else
                {
                    al.Add(strCookTemp[i]);
                }
                i = i + 1;
            }
            return al;
        }


        private static CookieCollection ConvertCookieArraysToCookieCollection(ArrayList al, string strHost)
        {
            CookieCollection cc = new CookieCollection();

            int alcount = al.Count;
            string strEachCook;
            string[] strEachCookParts;
            for (int i = 0; i < alcount; i++)
            {
                strEachCook = al[i].ToString();
                strEachCookParts = strEachCook.Split(';');
                int intEachCookPartsCount = strEachCookParts.Length;
                string strCNameAndCValue = string.Empty;
                string strPNameAndPValue = string.Empty;
                string strDNameAndDValue = string.Empty;
                string[] NameValuePairTemp;
                Cookie cookTemp = new Cookie();

                for (int j = 0; j < intEachCookPartsCount; j++)
                {
                    if (j == 0)
                    {
                        strCNameAndCValue = strEachCookParts[j];
                        if (strCNameAndCValue != string.Empty)
                        {
                            int firstEqual = strCNameAndCValue.IndexOf("=");
                            string firstName = strCNameAndCValue.Substring(0, firstEqual);
                            string allValue = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1));
                            cookTemp.Name = firstName;
                            cookTemp.Value = allValue;
                        }
                        continue;
                    }
                    if (strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty)
                        {
                            NameValuePairTemp = strPNameAndPValue.Split('=');
                            if (NameValuePairTemp[1] != string.Empty)
                            {
                                cookTemp.Path = NameValuePairTemp[1];
                            }
                            else
                            {
                                cookTemp.Path = "/";
                            }
                        }
                        continue;
                    }

                    if (strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strDNameAndDValue = strEachCookParts[j];
                        if (strDNameAndDValue != string.Empty)
                        {
                            NameValuePairTemp = strDNameAndDValue.Split('=');

                            if (NameValuePairTemp[1] != string.Empty)
                            {
                                cookTemp.Domain = NameValuePairTemp[1];
                            }
                            else
                            {
                                cookTemp.Domain = strHost;
                            }
                        }
                        continue;
                    }
                }

                if (cookTemp.Path == string.Empty)
                {
                    cookTemp.Path = "/";
                }
                if (cookTemp.Domain == string.Empty)
                {
                    cookTemp.Domain = strHost;
                }
                cc.Add(cookTemp);
            }
            return cc;
        }


        /// <summary>
        /// removes the data streams that flags the file as downloaded form the internet
        /// the data is
        /// [ZoneTransfer]
        /// ZoneId=3
        /// the stream name is Zone.Identfier
        //// can be read using command more < myFile.dll:Zone.Identfier
        //// can be written using (echo [ZoneTransfer] && echo ZoneId=3) > myFile.dll:Zone.Identfier 
        /// </summary>
        /// <param name="file"></param>
        public static void UnblockFile(FileInfo file)
        {
            file.DeleteAlternateDataStream("Zone.Identifier");
        }

        //i needed this backing delegate to alter the default event behaviour preventing accidentaly adding more than one subscriber
        //it's always only one subscriber: for tests a dummy one,
        //at runtime a real subscriper is added and is performing the UI interactions.
        private static EventHandler<PromptRequestEventArgs> promptRequested;
        
        public static event EventHandler<PromptRequestEventArgs> PromptRequested {
            add { promptRequested = value; }
            remove { promptRequested = null; }
        }
        
        public static void RequestPrompt(PromptContent promptContent, Action<string> responseHandler)
        {
            if (promptRequested == null)
            {
                responseHandler("default");
                return;
            }
            promptRequested.Invoke(null, new PromptRequestEventArgs(promptContent, responseHandler));
        }
        

        const string AuxiliaryTask_Query_Separator = "`,";
        public static bool TryParseAuxiliaryTaskQuery(string q, out string header, out string[] parameters)
        {
            var all = Regex.Split(q,AuxiliaryTask_Query_Separator);
            if(all.Count()<2)
            {
                header = null; parameters = null;
                return false;
            }
            header = all.First();
            parameters = all.Skip(1).ToArray();
            return true;
        }
        public static string FormatAuxiliaryTaskQuery(string header, params string[] parameters)
        {
            var auxiliaryTaskQueryBuilder = new StringBuilder();
            auxiliaryTaskQueryBuilder.Append(header);//auxiliary task type identifier
            auxiliaryTaskQueryBuilder.Append(AuxiliaryTask_Query_Separator);
            auxiliaryTaskQueryBuilder.Append(string.Join(AuxiliaryTask_Query_Separator,parameters));
            return auxiliaryTaskQueryBuilder.ToString();
        }

        static string logFile  = ApplicationInfo.DOCUMENTS +"\\log.txt";
        public static void WriteLine(string line)
        {
            Trace.WriteLine(line);
            File.AppendAllLines(logFile, new string[] { line });
        }
        
       
        public static readonly string MAIN_PATH = Path.GetDirectoryName(
           System.Reflection.Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// used by the function that determines plugin compatibility based on the verison decaled as assemply attribute
        /// </summary>
        public static bool IsCompatibleSemVer(string apiVersion, string pluginTargetApiVersion)
        {
            Version api = new Version(apiVersion);
            Version plugin = new Version(pluginTargetApiVersion);
            Debug.WriteLine($"comparing versions: api:{api.Major}.{api.Minor}.{api.Build} , plugin:{plugin.Major}.{plugin.Minor}.{plugin.Build}");
            //# plugin is more recent than core: not compatible
            if (api < plugin) return false;

            //# core is more recent than plugin : ~semVer rules 

            //before 0.2.x exclusvly breaking changes are implied with patch level increase
            //before major version 1 exclusvly and after v0.2.x inclusively breaking changes are impllied with minor increase,
            //after major 1 inclusivly breaking changes are impllied with major increase
            if (api.Major == 0 && plugin.Major == 0)
            {
                if (api.Minor < 2 && plugin.Minor < 2)
                {
                    //case1
                    return api.Build == plugin.Build;//'build' is semver patch
                }
                else
                {
                    //case2
                    return api.Minor == plugin.Minor;
                }
            }
            //case3
            return (api.Major == plugin.Major);

        }

        private static readonly Regex InvalidFileRegex = new Regex(
    string.Format("[{0}]", Regex.Escape(@"<>:""/\|?*")));


        public static string SanitizeFileName(string fileName)
        {
            return InvalidFileRegex.Replace(fileName, string.Empty);
        }

        //from @Philip Rieck resp
        public static string CamelCaseToUIText(string camelCase)
        {
            string res=  Regex.Replace(camelCase, @"(?<a>(?<!^)((?:[A-Z][a-z])|(?:(?<!^[A-Z]+)[A-Z0-9]+(?:(?=[A-Z][a-z])|$))|(?:[0-9]+)))", @" ${a}");
            res = res.Substring(0,1).ToUpper()+ res.Substring(1);
            return res;
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);


                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }



        /// <summary>
        /// only required by downloadOrReadFromObject, returns a filesystem-friendly hash
        /// </summary>
        public static string getUniqueLinkHash(string businessLink)
        {
            return CreateMD5(businessLink);
        }


        

    }
}
