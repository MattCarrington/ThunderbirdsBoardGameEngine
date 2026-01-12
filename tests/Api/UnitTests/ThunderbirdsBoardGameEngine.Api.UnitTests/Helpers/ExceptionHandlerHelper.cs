using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Text;
using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Helpers
{
    public static class ExceptionHandlerHelper
    {
        private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

        public static IProblemDetailsService CreateProblemsDetailService()
        {
            var problemDetailsService = Substitute.For<IProblemDetailsService>();

            problemDetailsService.WriteAsync(Arg.Any<ProblemDetailsContext>())
                    .Returns(callInfo =>
                {
                    var context = callInfo.Arg<ProblemDetailsContext>();
                    var runtimeType = context.ProblemDetails.GetType();
                    var json = JsonSerializer.Serialize(context.ProblemDetails, runtimeType, Json);
                    var bytes = Encoding.UTF8.GetBytes(json);

                    context.HttpContext.Response.ContentType = "application/problem+json; charset=utf-8";

                    return new ValueTask(context.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length));
                });

            return problemDetailsService;
        }

        public static async Task<(bool Handled, int Status, string ContentType, TProblemDetails Body)>
            InvokeAsync<TProblemDetails>(IExceptionHandler handler, Exception ex) where TProblemDetails : ProblemDetails
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var handled = await handler.TryHandleAsync(httpContext, ex, CancellationToken.None);

            httpContext.Response.Body.Position = 0;

            TProblemDetails? body = null;

            if (httpContext.Response.Body.Length > 0)
            {
                body = await JsonSerializer.DeserializeAsync<TProblemDetails>(httpContext.Response.Body, Json);
            }

            return (handled,
                    httpContext.Response.StatusCode,
                    httpContext.Response.ContentType ?? string.Empty,
                    body!);
        }

        public static async Task<(bool Handled, int Status, string ContentType, ProblemDetails Body)>
            InvokeAsync(IExceptionHandler handler, Exception ex)
        {
            return await InvokeAsync<ProblemDetails>(handler, ex);
        }
    }
}
