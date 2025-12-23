using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions
{
    /// <summary>
    /// Assertion helpers for validating <see cref="ProblemDetails"/> responses
    /// </summary>
    public static class ProblemDetailsAssertions
    {
        /// <summary>
        /// Asserts that the specified HTTP response represents a 400 Bad Request with a problem details payload
        /// matching the expected title.
        /// </summary>
        /// <remarks>This method verifies that the response has a status code of 400 Bad Request, a
        /// content type of 'application/problem+json', and a problem details object with the specified title and a
        /// 'traceId' extension. Intended for use in test scenarios to validate error responses conform to expected API
        /// error formats.</remarks>
        /// <param name="response">The HTTP response message to validate. Must not be null.</param>
        /// <param name="expectedTitle">The expected value of the problem details title. The assertion fails if the actual title does not match this
        /// value.</param>
        /// <returns>A task that represents the asynchronous assertion operation.</returns>
        public static async Task<ProblemDetails> AssertBadRequestAsync(HttpResponseMessage response, string expectedTitle)
        {
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status400BadRequest, problem!.Status);
            Assert.Equal(expectedTitle, problem.Title);
            Assert.True(problem.Extensions.ContainsKey("traceId"));

            return problem;
        }

        /// <summary>
        /// Asserts that the specified HTTP response represents a validation error and that the expected fields are
        /// present in the error details.
        /// </summary>
        /// <remarks>This method verifies that the response has a Bad Request status and contains a
        /// problem details payload with validation errors for the specified fields. Intended for use in test scenarios
        /// to ensure that validation errors are correctly reported by the API.</remarks>
        /// <param name="problem">The problem details object to validate. Must not be null.</param>
        /// <param name="expectedFields">The names of the fields that are expected to appear in the validation error details.</param>
        /// <returns>A task that represents the asynchronous assertion operation.</returns>
        public static void AssertValidationErrors(ProblemDetails problem, params string[] expectedFields)
        {
            Assert.Equal("Request validation failed.", problem.Title);
            Assert.True(problem.Extensions.ContainsKey("errors"));

            var errorsElement = Assert.IsType<JsonElement>(problem.Extensions["errors"]);

            var errors = JsonSerializer.Deserialize<Dictionary<string, string[]>>(errorsElement.GetRawText());

            Assert.NotNull(errors);
            Assert.NotEmpty(errors);
        }
    }
}
