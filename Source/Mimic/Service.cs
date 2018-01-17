using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Mimic.Utils;

namespace Mimic
{
    /// <summary>
    /// Represents a virtual service that can be invoked from the outside 
    /// world via HTTP. During normal usage of the application, callers 
    /// (esp. automated tests) add, invoke, query, and delete these via HTTP.
    /// 
    /// Methods on this class are thread-safe.
    /// </summary>
    public class Service
    {
        private readonly string method;
        private readonly string path;
        private readonly string bodyFilter;
        private readonly ServiceResponse response;

        private readonly object thisLock;
        private int callCount;
        private string lastRequestBody;

        public Guid Id { get; private set; }

        /// <summary>
        /// Returns the number of times this service has been invoked and 
        /// content of the last message body used to invoke the service.
        /// </summary>
        public ServiceStats Stats
        {
            get
            {
                lock (thisLock)
                {
                    return new ServiceStats()
                    {
                        CallCount = callCount,
                        LastRequestBody = lastRequestBody
                    };
                }
            }
        }

        /// <summary>
        /// Creates a new service using the specified service description. See 
        /// the ServiceDesc class for more information.
        /// </summary>
        public Service(ServiceDesc desc)
        {
            RequiresArgument.NotNullOrWhiteSpace(desc.Method, "Method");
            RequiresArgument.NotNullOrWhiteSpace(desc.Path, "Path");
            RequiresArgument.NotNullOrWhiteSpace(desc.ContentType, "ContentType");
            RequiresArgument.NotNullOrWhiteSpace(desc.StatusCode, "StatusCode");
            RequiresArgument.NotNull(desc.Body, "Body");

            thisLock = new object();
            Id = ParseId(desc.Id);
            // The method and path values are use for string matching, so 
            //convert them to upper invariant.
            method = desc.Method.ToUpperInvariant();
            path = desc.Path.ToUpperInvariant();
            bodyFilter = desc.BodyContains;
            response = new ServiceResponse()
            {
                Body = desc.Body,
                ContentType = desc.ContentType,
                StatusCode = ParseStatusCode(desc.StatusCode)
            };
            callCount = 0;
            lastRequestBody = string.Empty;
        }

        /// <summary>
        /// Returns true if the specified service will respond to the same 
        /// requests as this one.
        /// </summary>
        public bool HandlesSameRequests(Service other)
        {
            RequiresArgument.NotNull(other, "other");

            return method == other.method &&
                path == other.path &&
                bodyFilter == other.bodyFilter;
        }

        /// <summary>
        /// Returns true when the service's method, path, and body filter 
        /// match the specified request and body.
        /// </summary>
        public bool MatchesRequest(HttpRequest request, string body)
        {
            // This method is called in a loop. While is it possible to read 
            // the body from the request itself, it would be very inefficient. 
            // For this reason, the body is required as a separate argument 
            // to the method.
            RequiresArgument.NotNull(request, "request");
            RequiresArgument.NotNull(body, "body");

            return method == request.Method 
                && path == WebUtility.UrlDecode(request.Path + request.QueryString).ToUpperInvariant() 
                && MatchesBodyFilter(body);
        }

        /// <summary>
        /// Writes the services predefined response to the specified 
        /// HttpResponse.
        /// </summary>
        public async Task WriteResponseTo(HttpResponse response)
        {
            RequiresArgument.NotNull(response, "response");

            response.StatusCode = this.response.StatusCode;
            response.ContentType = this.response.ContentType;
            await response.WriteAsync(this.response.Body);
        }

        /// <summary>
        /// Updates the service's Stats to reflect the fact that the service 
        /// has been invoked.
        /// </summary>
        public void RecordCall(string requestBody)
        {
            RequiresArgument.NotNull(requestBody, "requestBody");

            lock (thisLock)
            {
                callCount++;
                lastRequestBody = requestBody;
            }
        }

        private static int ParseStatusCode(string str)
        {
            if (Int32.TryParse(str, out int statusCode))
            {
                return statusCode;
            }
            else
            {
                throw new ArgumentException("Unable to parse status code value.");
            }
        }

        private static Guid ParseId(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return Guid.NewGuid();
            }
            else if (Guid.TryParse(id, out Guid result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Provided service ID is not a GUID.");
            }
        }

        private bool MatchesBodyFilter(string body)
        {
            if (string.IsNullOrWhiteSpace(bodyFilter))
            {                
                return true;
            }
            else
            {
                return body.Contains(bodyFilter);
            }
        }
    }
}
