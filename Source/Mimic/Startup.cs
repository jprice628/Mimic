using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mimic.Utils;

namespace Mimic
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // This method gets called by the runtime. It can be used to add 
            // services to the container.
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // TODO: Need to figure out logging.
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // This is where the magic happens. MVC has been stripped out, and
            // the following pipeline has been put into place to handle incoming 
            // requests.
            app.Use(GlobalExceptionHandler.Middleware);
            app.Use(ServiceFunctions.AddService);
            app.Use(ServiceFunctions.QueryService);
            app.Use(ServiceFunctions.DeleteService);
            app.Use(ServiceFunctions.DeleteAllServices);
            app.Use(ServiceFunctions.InvokeService);
        }
    }
}
