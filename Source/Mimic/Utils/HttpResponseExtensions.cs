using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Mimic.Utils
{
    /// <summary>
    /// Provides a set of extension methods for HttpResponses
    /// </summary>
    static class HttpResponseExtensions
    {
        /// <summary>
        /// Makes the response an OK (200) response and sets it's content.
        /// </summary>
        public static async Task Ok(this HttpResponse response, string content)
        {
            RequiresArgument.NotNullOrWhiteSpace(content, "message");

            response.StatusCode = 200;
            response.ContentType = "text/plain";
            await response.WriteAsync(content);
        }

        /// <summary>
        /// Makes the response an OK (200) response with no content.
        /// </summary>
        public static void Ok(this HttpResponse response)
        {
            response.StatusCode = 200;
        }

        /// <summary>
        /// Makes the response a NOT FOUND (404) response with no content.
        /// </summary>
        public static void NotFound(this HttpResponse response)
        {
            response.StatusCode = 404;
        }

        /// <summary>
        /// Makes the response a BAD REQUEST (400) and writes the exception 
        /// information to it.
        public static Task BadRequest(this HttpResponse response, Exception ex)
        {
            RequiresArgument.NotNull(ex, "ex");

            response.StatusCode = 400;
            response.ContentType = "text/plain";
            var msg = BuildExceptionMessage(ex);
            return response.WriteAsync(msg);
        }

        /// <summary>
        /// Makes the response a INTERNAL SERVER ERROR (500) and writes the 
        /// exception information to it.
        public static Task InternalServerError(this HttpResponse response, Exception ex)
        {
            RequiresArgument.NotNull(ex, "ex");

            response.StatusCode = 500;
            response.ContentType = "text/plain";
            var msg = BuildExceptionMessage(ex);
            return response.WriteAsync(msg);
        }

        private static string BuildExceptionMessage(Exception ex)
        {
            // We don't want to return full exceptions across the wire. The 
            // message is enough, but we want the messages from all of the 
            // inner exceptions too.
            var message = new StringBuilder();
            message.AppendLine(ex.Message);
            var innerException = ex.InnerException;
            while (innerException != null)
            {
                message
                    .Append("Inner exception: ")
                    .AppendLine(innerException.Message);
                innerException = innerException.InnerException;
            }
            return message.ToString();
        }
    }
}
