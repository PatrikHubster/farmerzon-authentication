using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AuthenticationErrorHandling.Response;
using AuthenticationErrorHandling.CustomException;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

using DTO = AuthenticationDataTransferModel;

namespace AuthenticationErrorHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exp)
            {
                await HandleExceptionAsync(context, exp);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exp)
        {
            HttpStatusCode code;
            var resultMessage = new ErrorResponse
            {
                Success = false
            };

            switch (exp)
            {
                case BadRequestException convertedException:
                    code = HttpStatusCode.BadRequest; // HTTP 400
                    resultMessage.Errors = convertedException.Messages;
                    break;
                case UnautherizedException convertedException:
                    code = HttpStatusCode.Unauthorized; // HTTP 401
                    resultMessage.Errors = convertedException.Messages;
                    break;
                case NotFoundException convertedException:
                    code = HttpStatusCode.NotFound; // HTTP 404
                    resultMessage.Errors = convertedException.Messages;
                    break;
                default:
                    code = HttpStatusCode.InternalServerError; // HTTP 500
                    resultMessage.Errors = new List<string> {"Ooops, something went wrong."};
                    break;
            }

            var jsonResult = JsonConvert.SerializeObject(resultMessage);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) code;
            return context.Response.WriteAsync(jsonResult);
        }
    }
}