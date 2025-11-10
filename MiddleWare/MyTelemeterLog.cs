using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomMiddleWare.MiddleWare
{
    public class MyTelemeterLog : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // context..Services.AddOpenTelemetry().ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName)).WithTracing(tracking => tracking.AddAspNetCoreInstrumentation().AddConsoleExporter());

            return next(context);
        }
    }
}
