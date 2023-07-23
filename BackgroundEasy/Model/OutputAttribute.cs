using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mi.Common.Model
{
    /// <summary>
    /// idicate the field is to be (possibly) included in user output sheet
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =true)]
    
    public class OutputAttribute:Attribute
    {
        public OutputAttribute()
        {
                
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name">the name in excel header</param>
        public OutputAttribute(string friendlyName)
        {
            this.UserName = friendlyName;
        }
        /// <summary>
        /// use this ctor for value types, the passed string format is used by other mi helper classes to format the output cells
        /// </summary>
        /// <param name="Name">the name in excel header</param>
        /// <param name="Name">this used to compile dynamic method v2 in case of datetime you can pass "dd/MM/yyyy hh:mm:ss tt"</param>
        public OutputAttribute(string friendlyName, string formatString)
        {
            this.FormatString = formatString;
        }

        public string FormatString { get; internal set; }

        /// <summary>
        /// user-friedly header can have spaces
        /// </summary>
        public string UserName { get; set; }
    }
}
