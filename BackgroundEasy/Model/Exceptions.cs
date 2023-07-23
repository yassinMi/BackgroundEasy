using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundEasy.Model
{
    public class BadInputFileException : Exception
    {
        public BadInputFileException(string msg):base(msg)
        {

        }
    }

    public class ScrapingException : Exception
    {

        public ScrapingException(string PageUrl, string msg) : base(msg)
        {
            TagetUrl = PageUrl;
        }
        public ScrapingException(string PageUrl, string msg, Exception inner) : base(msg, inner)
        {
            TagetUrl = PageUrl;
        }
        string TagetUrl { get; set; }

        public override string ToString()
        {
            return base.ToString() + System.Environment.NewLine + $"TargetUrl:'{TagetUrl}'";
        }
    }
    /// <summary>
    /// the exception that is thrown when a request fails with 404 or simlar
    /// </summary>
    public class ResourceNotFoundException : ScrapingException
    {
        public ResourceNotFoundException(string ResourceUrl) : base(ResourceUrl, $"Image not found")
        {

        }
        public ResourceNotFoundException(string ResourceUrl, Exception inner) : base(ResourceUrl, $"Image not found", inner)
        {
            Data["url"] = ResourceUrl;
        }
    }
    
    
    /// <summary>
    /// throwing this exception inside a user command code tells the outer most catch block to show the message directely in a warning box (the message must be user friendly)
    /// </summary>
    public class InvalidUserOperationException : Exception
    {
        public InvalidUserOperationException(string message) : base(message) { }
    }

    /// <summary>
    /// fired to indicate that the task was unseccusfull due to connectivity issue (e,g offline)
    /// </summary>
    public class NetworkFailureException : Exception
    {
        public NetworkFailureException(string message) : base(message)
        {

        }
    }
    /// <summary>
    /// fired to indicate that the task was unseccusfull due to full disk
    /// </summary>
    public class DiskSpaceException : Exception
    {
        public DiskSpaceException(string message) : base(message)
        {

        }
    }

}
