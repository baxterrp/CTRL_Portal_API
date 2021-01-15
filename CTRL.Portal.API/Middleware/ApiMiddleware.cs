using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.Exceptions;
using CTRL.Portal.Data.DataExceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Middleware
{
    public class ApiMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiMiddleware(RequestDelegate next)
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

        private async Task WriteHttpContextResponse(HttpContext httpContext, Exception exception)
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

        private int GetHttpStatusCode(Exception exception)
        {
            HttpStatusCode code;

            switch (exception)
            {
                case ArgumentNullException _:
                case ArgumentException _:
                    code = HttpStatusCode.BadRequest;
                    break;
                case InvalidOperationException _:
                    code = HttpStatusCode.Conflict;
                    break;
                case InvalidLoginAttemptException _:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case ResourceNotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    break;
            }

            return (int)code;
        }
    }
}
