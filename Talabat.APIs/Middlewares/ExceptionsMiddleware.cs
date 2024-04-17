using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlewares
{
    public class ExceptionsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionsMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionsMiddleware(RequestDelegate next , ILogger<ExceptionsMiddleware> loggerFactory , IWebHostEnvironment env)
        {
            _next = next;
            _logger = loggerFactory;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext httpcontext)
        {
            try
            {
                await _next.Invoke(httpcontext);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                httpcontext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                httpcontext.Response.ContentType = "application/json";

                var response = _env.IsDevelopment() ?
                    new ApiExceptionsResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                    : new ApiExceptionsResponse((int)HttpStatusCode.InternalServerError);
                var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                httpcontext.Response.WriteAsync(json);
            }

        }
    }
}
