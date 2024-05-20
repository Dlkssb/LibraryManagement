using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;



namespace LibraryManagement.WebAPI.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {

            

                var response = context.Response;
                response.ContentType = "application/json";
               var responseModel = new Response<string>() { Success = false, Message = error?.Message };

                switch (error)
                {
                    case BadRequestException ex:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;

                        break;

                    case ValidationException ex:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.ValidationErrors = ex.ValdationErrors;
                        break;

                    case NotFoundException ex:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case SqlException ex:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Message = ex.Message;
                        break;
                    case AuthenticationException ex:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;

                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.Message = "Something went wrong, Please try again \n  " + error.Message;
                        break;
                }
                var result = JsonSerializer.Serialize(responseModel);

                await response.WriteAsync(result);
            }
        }
    }
}

