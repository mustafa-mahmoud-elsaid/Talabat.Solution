using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                //take an action with request

                await _next.Invoke(httpContext);

                //take an action with response
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";
                var response = _env.IsDevelopment() ? new ApiExceptionHandler(500, ex.Message, ex.StackTrace)
                                : new ApiExceptionHandler(500);
                var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response , options);

                await httpContext.Response.WriteAsync(json);
            }


        }
    }
}
