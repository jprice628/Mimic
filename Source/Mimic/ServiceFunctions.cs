using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mimic.Utils;

namespace Mimic
{
    /// <summary>
    /// Provides a set of middleware functions for manipulating services.
    /// </summary>
    public static class ServiceFunctions
    {
        private static Func<ParseContext, Task> ParseServiceDescPipeline = InitParseServiceDescPipeline();
        private static ServiceCollection services = new ServiceCollection();

        /// <summary>
        /// Allows callers to add a service via HTTP requests by providing a 
        /// specially formatted service description.
        /// </summary>
        public static async Task AddService(HttpContext context, Func<Task> next)
        {
            RequiresArgument.NotNull(context, "context");
            RequiresArgument.NotNull(next, "next");

            if (!context.Request.Matches("POST", "/__vs/services"))
            {
                await next.Invoke();
                return;
            }

            var serviceDesc = await ParseServiceDescription(context.Request);
            RequestRequires.NotNullOrWhiteSpace(serviceDesc.Method, "An HTTP method, i.e. GET, POST, PUT, DELETE, etc., must be provided");
            RequestRequires.NotNullOrWhiteSpace(serviceDesc.Path, "A path must be provided. For example /api/myResources/53");

            var service = new Service(serviceDesc);
            if (services.TryAdd(service))
            {
                await context.Response.Ok(service.Id.ToString());
            }
            else
            {
                throw new BadRequestException("Unable to add service. A service with the same ID or the same request filter already exists.");
            }
        }

        /// <summary>
        /// Allows callers to query a service via HTTP to obtain information such as
        /// the number of times the service has been invoked and the body of the
        /// last request message that was used to invoke the service.
        /// </summary>
        public static async Task QueryService(HttpContext context, Func<Task> next)
        {
            RequiresArgument.NotNull(context, "context");
            RequiresArgument.NotNull(next, "next");

            if (context.Request.Method.ToUpperInvariant() != "GET" ||
                !IsAServicePath(context.Request.Path, out Guid serviceId))
            {
                await next.Invoke();
                return;
            }

            if (services.TryGetById(serviceId, out Service service))
            {
                var stats = service.Stats;
                var responseBody = new StringBuilder()
                    .AppendLine($"CallCount: {stats.CallCount}")
                    .AppendLine()
                    .AppendLine("LastRequestBody:")
                    .Append(stats.LastRequestBody)
                    .ToString();
                await context.Response.Ok(responseBody);
            }
            else
            {
                context.Response.NotFound();
            }
        }

        /// <summary>
        /// Allows callers to delete a service via HTTP using the service's ID.
        /// </summary>
        public static Task DeleteService(HttpContext context, Func<Task> next)
        {
            RequiresArgument.NotNull(context, "context");
            RequiresArgument.NotNull(next, "next");

            if (context.Request.Method.ToUpperInvariant() != "DELETE" ||
                !IsAServicePath(context.Request.Path, out Guid serviceId))
            {
                return next.Invoke();
            }

            services.TryRemove(serviceId);
            context.Response.Ok();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Allow callers to delete all services via HTTP. This is useful for 
        /// cleaning up after a set of tests have been run.
        /// </summary>
        public static Task DeleteAllServices(HttpContext context, Func<Task> next)
        {
            RequiresArgument.NotNull(context, "context");
            RequiresArgument.NotNull(next, "next");

            if (!context.Request.Matches("DELETE", "/__vs/services"))
            {
                return next.Invoke();
            }

            services.Clear();
            context.Response.Ok();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Allows callers to invoke services that have been added.
        /// </summary>
        public static async Task InvokeService(HttpContext context, Func<Task> next)
        {
            RequiresArgument.NotNull(context, "context");
            RequiresArgument.NotNull(next, "next");

            string body = context.Request.ReadBody();

            if (services.TryGetWhere(x => x.MatchesRequest(context.Request, body), out Service service))
            {
                service.RecordCall(body);
                await service.WriteResponseTo(context.Response);
            }
            else
            {
                context.Response.NotFound();
            }
        }

        private static Func<ParseContext, Task> InitParseServiceDescPipeline()
        {
            // When a caller wants to add a service, they provide a specially 
            // formatted text in the body of their HTTP request. This text is 
            // then passed through the pipeline defined below. With each invocation, 
            // the pipeline reads the next line from the text and processes it.
            var pipeline = new Pipeline<ParseContext>()
                .Use(ParseFunctions.ReadLine)
                .Use(ParseFunctions.ParseBody)
                .Use(ParseFunctions.IgnoreCommentsAndBlankLines)
                .Use(ParseFunctions.ParseSetting)
                .Build();

            return pipeline;
        }

        private static async Task<ServiceDesc> ParseServiceDescription(HttpRequest request)
        {
            var reader = new StreamReader(request.Body);
            try
            {
                var serviceDesc = new ServiceDesc();
                var ctx = new ParseContext(reader, serviceDesc);

                while (!reader.EndOfStream)
                {
                    await ParseServiceDescPipeline.Invoke(ctx);
                }

                return serviceDesc;
            }
            catch (Exception ex)
            {
                throw new BadRequestException("Unable to parse service description.", ex);
            }
            finally
            {
                ((IDisposable)reader).Dispose();
            }
        }

        private static bool IsAServicePath(string path, out Guid serviceId)
        {
            serviceId = Guid.Empty;

            string pattern = "/__vs/services/(.*)";
            var match = Regex.Match(path, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return Guid.TryParse(match.Groups[1].Value, out serviceId);
            }
            else
            {
                return false;
            }
        }
    }
}
