using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundEasy.Services
{
    /// <summary>
    /// rised by XLSXUtils when attempting to load data from a sheet with missing expected headers
    /// </summary>
    public class XLSXBadHeaderException:Exception
    {
        public XLSXBadHeaderException(string[] missingHeaders):base($"Excel sheet is missing the folowing headers {string.Join(", ",missingHeaders)}")
        {
            MissingHeaders = missingHeaders;
        }
        string[] MissingHeaders { get; set; }
    }
    /// <summary>
    /// rised by XLSXUtils when attempting to load data from a file which cannot be processed usng ExcelPackage ctor
    /// </summary>
    public class XLSXBadFileException : Exception
    {
        public XLSXBadFileException(string file, Exception inner) : base($"The specified excel file could not be proceced: {inner.Message}",inner)
        {

        }
    }

}
