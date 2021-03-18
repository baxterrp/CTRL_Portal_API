using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CTRL.Portal.Middleware
{
    public class ApiPortalMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiPortalMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                await WriteHttpContextResponse(httpContext, exception);
            }
        }

        private static async Task WriteHttpContextResponse(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;
            httpContext.Response.StatusCode = GetHttpStatusCode(exception);

            var apiErrorResponse = new ApiResponseContract
            {
                Status = (HttpStatusCode)httpContext.Response.StatusCode,
                Message = exception.Message
            };

            var stringifiedApiException = JsonConvert.SerializeObject(apiErrorResponse);


            await httpContext.Response.WriteAsync(stringifiedApiException);
        }

        private static int GetHttpStatusCode(Exception exception) =>
            (int)(exception switch
            {
                ArgumentNullException _ or ArgumentException _ => HttpStatusCode.BadRequest,
                InvalidOperationException _ => HttpStatusCode.Conflict,
                InvalidLoginAttemptException _ => HttpStatusCode.Unauthorized,
                ResourceNotFoundException _ => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError,
            });
    }
}
