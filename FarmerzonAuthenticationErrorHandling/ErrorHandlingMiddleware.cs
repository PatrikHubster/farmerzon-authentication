using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FarmerzonAuthenticationDataTransferModel;
using FarmerzonAuthenticationErrorHandling.CustomException;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

 namespace FarmerzonAuthenticationErrorHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exp)
            {
                await HandleExceptionAsync(context, exp, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exp, 
            ILogger<ErrorHandlingMiddleware> logger)
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
                    logger.LogError(exp, "This error was captured by the error handling middleware.");
                    break;
            }

            var jsonResult = JsonConvert.SerializeObject(resultMessage);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) code;
            return context.Response.WriteAsync(jsonResult);
        }
    }
}