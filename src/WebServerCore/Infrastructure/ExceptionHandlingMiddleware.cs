using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace WebServerCore.Infrastructure
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _isDevelopment;

        public ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _isDevelopment = env.IsDevelopment();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex) when (!httpContext.Response.HasStarted)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var exInfo = GetExceptionInfo(ex);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)exInfo.code;

            var result = _isDevelopment ?
                JsonConvert.SerializeObject(new { message = exInfo.message, exception = ex })
                : JsonConvert.SerializeObject(new { message = exInfo.message });

            return context.Response.WriteAsync(result);
        }

        private (HttpStatusCode code, string message) GetExceptionInfo(Exception exception)
        {
            if (exception is FileNotFoundException)
            {
                return (HttpStatusCode.NotFound, exception.Message);
            }

            if (exception is BusinessLogicException
                || exception is ValidationException)
            {
                return (HttpStatusCode.BadRequest, exception.Message);
            }

            return (HttpStatusCode.InternalServerError, null);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
