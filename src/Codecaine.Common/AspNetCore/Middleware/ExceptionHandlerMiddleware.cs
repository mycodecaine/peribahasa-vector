using Codecaine.Common.Errors;
using Codecaine.Common.Exceptions;
using Codecaine.Common.Primitives.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;

namespace Codecaine.Common.AspNetCore.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">The delegate pointing to the next middleware in the chain.</param>
        /// <param name="logger">The logger.</param>
        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the exception handler middleware with the specified <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="httpContext">The HTTP httpContext.</param>
        /// <returns>The task that can be awaited by the next middleware.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                var request = await FormatRequest(httpContext);

                await _next(httpContext);

                var originalBodyStream = httpContext.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    httpContext.Response.Body = responseBody;
                    var response = await FormatResponse(httpContext.Response);
                    var message = $"Request: {request} Response: {response}";
                    _logger.LogInformation(message);
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred: {Message}", ex.Message);

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        /// <summary>
        /// Handles the specified <see cref="Exception"/> for the specified <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="httpContext">The HTTP httpContext.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>The HTTP response that is modified based on the exception.</returns>
        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            (var httpStatusCode, var errors) = GetHttpStatusCodeAndErrors(exception);

            httpContext.Response.ContentType = "application/json";

            httpContext.Response.StatusCode = (int)httpStatusCode;

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var response = JsonSerializer.Serialize(new ErrorResponse(errors), serializerOptions);

            await httpContext.Response.WriteAsync(response);
        }

        private static async Task<string> FormatRequest(HttpContext context)
        {
            var request = context.Request;
            var ipAddress = context.Connection.RemoteIpAddress;
            var bodyAsText = await ReadRequestBody(request);
            return $"{ipAddress} {request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private static async Task<string> ReadRequestBody(HttpRequest request)
        {
            if ((request.Method == HttpMethods.Post || request.Method == HttpMethods.Put) && request.ContentLength > 0)
            {
                request.EnableBuffering();

                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                string body = await reader.ReadToEndAsync();

                request.Body.Position = 0; // Rewind the stream for the next middleware
                return body;
            }

            return string.Empty;
        }

        private static async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"{response.StatusCode}: {body}";
        }

               
        private static (HttpStatusCode httpStatusCode, IReadOnlyCollection<Error>) GetHttpStatusCodeAndErrors(Exception exception) =>
            exception switch
            {
                ValidationException validationException => (HttpStatusCode.BadRequest, validationException.Errors),
                DomainException domainException => (HttpStatusCode.BadRequest, new[] { domainException.Error }),
                ArgumentException argumentException => (HttpStatusCode.BadRequest, new[] { new Error(GeneralErrors.UnProcessableRequest, argumentException.Message) }),
                AuthenticationException  => (HttpStatusCode.Unauthorized, new[] { AuthenticationErrors.UnauthorizedUser }),
                ApplicationLayerException serviceApplicationException => (HttpStatusCode.InternalServerError, new[] { serviceApplicationException.Error }),
                InfrastructureException serviceInfrastructureException => (HttpStatusCode.InternalServerError, new[] { serviceInfrastructureException.Error }),
                NotFoundException notFoundException => (HttpStatusCode.NotFound, new[] { notFoundException.Error }),
                CommonLibraryException commonLibraryException => (HttpStatusCode.InternalServerError, new[] { commonLibraryException.Error }),

                _ => (HttpStatusCode.InternalServerError, new[] { GeneralErrors.ServerError })
            };
    }
}
