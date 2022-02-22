using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Business.Middleware
{
    public class ExciptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExciptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExciptionMiddleware(RequestDelegate next, ILogger<ExciptionMiddleware> logger, IHostEnvironment env)
        {
            _logger = logger;
            _next = next;
            _env = env;
        }

       public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = ("application/json");

                var response = _env.IsDevelopment() ?
                    new APIException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) :
                    new APIException(context.Response.StatusCode, "Internal Server Error");


                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
