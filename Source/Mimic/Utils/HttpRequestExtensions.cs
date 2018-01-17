using Microsoft.AspNetCore.Http;
using System.IO;

namespace Mimic.Utils
{
    /// <summary>
    /// Provides a set of extension methods for HttpRequests
    /// </summary>
    static class HttpRequestExtensions
    {
        /// <summary>
        /// Simplifies reading the request's body, which is a stream.
        /// </summary>
        public static string ReadBody(this HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Returns true when the HttpRequest has the specified method, 
        /// e.g. GET, POST, etc., and path, ex: /api/students. The test is 
        /// case insensitive.
        public static bool Matches(this HttpRequest request, string method, string path)
        {
            RequiresArgument.NotNullOrWhiteSpace(method, "method");
            RequiresArgument.NotNullOrWhiteSpace(path, "path");

            return request.Method.ToUpperInvariant() == method.ToUpperInvariant()
                && request.Path.ToString().ToUpperInvariant() == path.ToUpperInvariant();
        }
    }
}
