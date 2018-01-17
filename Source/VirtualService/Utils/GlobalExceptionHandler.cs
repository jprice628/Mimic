using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace VirtualService.Utils
{
    /// <summary>
    /// Provides general exception handling for the service. The Middleware 
    /// function defined in this class should be the first middleware defined 
    /// when configuring the service (See the Startup.cs).
    /// </summary>
    public class GlobalExceptionHandler
    {
        public static async Task Middleware(HttpContext context, Func<Task> next)
        {
            try
            {
                // Have to await this or the exceptions won't get caught by 
                // this try-catch block.
                await next();
            }
            catch(BadRequestException ex)
            {
                await context.Response.BadRequest(ex);
            }
            catch (Exception ex)
            {
                await context.Response.InternalServerError(ex);
            }
        }
    }
}
