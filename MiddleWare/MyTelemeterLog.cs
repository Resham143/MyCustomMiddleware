using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace CustomMiddleWare.MiddleWare
{
    public class MyTelemeterLog(ILogger<MyTelemeterLog> _logger) : IMiddleware
    {

        private const int MaxBodySize = 10 * 1024; // 10 KB limit
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string requestBody = string.Empty;
            if (context.Request.ContentLength > 0 && context.Request.ContentType?.Contains("application/json") == true)
            {
                context.Request.EnableBuffering(); // Allow reading the body multiple times
                using (var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0; // Reset stream position for subsequent readers
                }

                // Limit body size if necessary
                const int maxBodySize = 10 * 1024; // 10 KB
                if (requestBody.Length > maxBodySize)
                {
                    requestBody = requestBody.Substring(0, maxBodySize) + "... (truncated)";
                }
            }

            // Get current span and add attribute
            var currentSpan = Tracer.CurrentSpan; // Assuming you have a Tracer initialized
            if (currentSpan != null && !string.IsNullOrEmpty(requestBody))
            {
                currentSpan.SetAttribute("http.request.body", requestBody);
            }


            await ProcessResponseCapture(currentSpan, context, next);
        }

        private async Task<string> ReadResponseBodyAsync(Stream responseBodyStream)
        {
            responseBodyStream.Position = 0;
            using var reader = new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            return (body.Length > MaxBodySize) ? body.Substring(0, MaxBodySize) + "... (truncated)" : body;
        }

        private async Task ProcessResponseCapture(TelemetrySpan span,HttpContext context, RequestDelegate next)
        {
            // --- 2. Capture Response Body ---
        // Temporarily replace the response stream with a MemoryStream to capture the response
        var originalResponseBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        // Call the next middleware(s) in the pipeline, which will write to our MemoryStream
        await next(context);

        // Read the captured response body from the MemoryStream
        string responseBody = await ReadResponseBodyAsync(memoryStream);
        if (!string.IsNullOrEmpty(responseBody))
        {
            span.SetAttribute("http.response.body", responseBody);
        }

        // Copy the captured response back to the original response stream for the client
        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(originalResponseBodyStream);

        // Restore the original response body stream
        context.Response.Body = originalResponseBodyStream;
        }
    }
}
