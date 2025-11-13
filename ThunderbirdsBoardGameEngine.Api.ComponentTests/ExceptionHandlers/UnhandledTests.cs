using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;   // IServiceCollection extensions
using Microsoft.Extensions.DependencyInjection.Extensions; // RemoveAll<T>()
using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.ExceptionHandlers
{
    public class UnhandledTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public UnhandledTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [MemberData(nameof(GetDisasterCardValidationExceptionData))]
        public async Task GetDisasterCards_WhennExceptionThrows_ReturnsProblemDetails(Exception exception, HttpStatusCode expectedStatusCode, string expectedTitle)
        {
            // Arrange
            var throwingFactory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IDisasterCardService>();
                    services.AddSingleton<IDisasterCardService>(new ThrowingService(exception));
                });
            });

            var client = throwingFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/disaster-cards");
            request.Headers.Add("X-API-Version", "1");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            
            Assert.NotNull(content);
            Assert.Equal((int)expectedStatusCode, content.Status);
            Assert.Equal(expectedTitle, content.Title);
            Assert.True(content.Extensions.ContainsKey("traceId"));
        }

        public static TheoryData<Exception, HttpStatusCode, string> GetDisasterCardValidationExceptionData()
        {
            return new TheoryData<Exception, HttpStatusCode, string>
            {
                { DisasterCardValidationException.Unknown(), HttpStatusCode.InternalServerError, "An unknown disaster card validation error occurred." },
                { CatalogDataAccessException.SourceUnreadable("file error"), HttpStatusCode.ServiceUnavailable, "The catalog data source could not be read" },
                { new ApplicationValidationException("Invalid request"), HttpStatusCode.InternalServerError, "Application validation failed" }
            };
        }

        private class ThrowingService : IDisasterCardService
        {
            public ThrowingService(Exception exception)
            {
                _exception = exception;
            }

            private readonly Exception _exception;

            ImmutableArray<DisasterCard> IDisasterCardService.GetAll()
            {
                throw _exception;
            }
        }
}
}
