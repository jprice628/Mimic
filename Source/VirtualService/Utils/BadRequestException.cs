using System;

namespace VirtualService.Utils
{
    /// <summary>
    /// A type of exception that can be thrown when the service receives a bad 
    /// request. This type of exception is recognized by the 
    /// GlobalExceptionHandler so that the correct HTTP status code and message 
    /// is returned.
    /// </summary>
    public class BadRequestException : Exception
    {
        public BadRequestException()
        {
        }

        public BadRequestException(string message)
            : base(message)
        {
        }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
