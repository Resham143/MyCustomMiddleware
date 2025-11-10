using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CustomMiddleWare.Middlewares
{
    public class MyCustomMiddleware(ILogger<MyCustomMiddleware> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate requestDelegate)
        {
            logger.LogInformation("Middleware task begin!");

            await requestDelegate(context);

            logger.LogInformation("Middleware task end");

        }
    }
}
