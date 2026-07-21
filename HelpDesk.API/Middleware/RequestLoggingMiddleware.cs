using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HelpDesk.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = context.Request.Path;

            _logger.LogInformation("HTTP {Method} {Path} started", method, path);

            try
            {
                await _next(context);
                
                sw.Stop();
                var statusCode = context.Response.StatusCode;
                _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms", method, path, statusCode, sw.ElapsedMilliseconds);
            }
            catch
            {
                sw.Stop();
                _logger.LogInformation("HTTP {Method} {Path} failed in {Elapsed}ms", method, path, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
