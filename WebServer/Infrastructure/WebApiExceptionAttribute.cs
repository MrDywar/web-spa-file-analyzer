using Autofac.Integration.WebApi;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace WebServer.Infrastructure
{
    public class WebApiExceptionAttribute : IAutofacExceptionFilter
    {
        public Task OnExceptionAsync(HttpActionExecutedContext context, CancellationToken cancellationToken)
        {
            var exceptionInfo = GetExceptionInfo(context.Exception);

#if DEBUG
            context.Response = context.ActionContext.Request.CreateErrorResponse(
                            exceptionInfo.StatusCode,
                            exceptionInfo.Message,
                            context.Exception);
#else
            context.Response = context.ActionContext.Request.CreateErrorResponse(
                            exceptionInfo.StatusCode,
                            exceptionInfo.Message);
#endif

            return Task.CompletedTask;
        }

        private ExceptionInfo GetExceptionInfo(Exception exception)
        {
            if (exception is FileNotFoundException)
            {
                return new ExceptionInfo(HttpStatusCode.NotFound, exception.Message);
            }

            if (exception is BusinessLogicException
                || exception is ValidationException)
            {
                return new ExceptionInfo(HttpStatusCode.BadRequest, exception.Message);
            }

            return new ExceptionInfo(HttpStatusCode.InternalServerError, null);
        }

        private class ExceptionInfo
        {
            public ExceptionInfo(HttpStatusCode statusCode, string message)
            {
                StatusCode = statusCode;
                Message = message;
            }

            public HttpStatusCode StatusCode { get; set; }
            public string Message { get; set; }
        }
    }
}