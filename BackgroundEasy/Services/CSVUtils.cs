
using CsvHelper;
//using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundEasy.Services
{
    public static class CSVUtils
    {
        

        public static void CSVAppendRecords(string filename, IEnumerable records)
        {
            var cof = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            using (var writer = new StreamWriter(filename, true))
            using (var csv = new CsvWriter(writer, cof))
            {
                csv.WriteRecords(records);
            }
        }
        public static void CSVOverwriteRecords(string filename, IEnumerable records)
        {
            using (var writer = new StreamWriter(filename))
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }

        public static void CSVWriteRecords(string filename, IEnumerable records, bool appendOrOverwrite)
        {
            if (appendOrOverwrite) CSVAppendRecords(filename, records);
            else CSVOverwriteRecords(filename, records);
        }




        public static IEnumerable<T> parseCSVfile<T>(string path)
        {
            Debug.WriteLine("parsing csv: " + path);
            bool fileExixsts = File.Exists(path);
            Trace.Assert(fileExixsts, $"file '{path}' doesnt exist");
            if (fileExixsts == false)
            {
                return null;
            }
            List<T> list = new List<T>();
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                list = csv.GetRecords<T>().ToList();
            }

            return list;
        }
        /// <summary>
        /// returns null when bad data or missing file
        /// </summary>
        /// <param name="recordType"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<object> parseCSVfile(Type recordType, string path)
        {
            Debug.WriteLine("parsing csv: " + path);
            bool fileExixsts = File.Exists(path);
            Trace.Assert(fileExixsts, $"file '{path}' doesnt exist");
            if (fileExixsts == false)
            {
                return null;
            }
            List<object> list = new List<object>();
            using (var reader = new StreamReader(path))
                try
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        list = csv.GetRecords(recordType).ToList();
                    }
                }
                catch (Exception err) when (err is HeaderValidationException 
                || err is CsvHelper.MissingFieldException)
                {
                    return null;
                }
            

            return list;
        }

    }
}
