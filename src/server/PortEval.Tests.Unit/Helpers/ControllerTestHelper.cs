using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Features.Queries;
using System.IO;
using System.Text;
using Xunit;

namespace PortEval.Tests.Unit.Helpers
{
    internal static class ControllerTestHelper
    {
        public static QueryResponse<T> GenerateSuccessfulQueryResponse<T>(T result)
        {
            return new QueryResponse<T>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        public static QueryResponse<T> GenerateNotFoundQueryResponse<T>()
        {
            return new QueryResponse<T>
            {
                Status = QueryStatus.NotFound,
                Response = default
            };
        }

        public static void AssertFileContentEqual(string expectedContent, IActionResult actionResult)
        {
            Assert.IsAssignableFrom<FileContentResult>(actionResult);
            Assert.Equal(expectedContent, Encoding.UTF8.GetString(((FileContentResult)actionResult).FileContents));
        }

        public static void AssertFileStreamEqual(string expectedContent, IActionResult actionResult)
        {
            Assert.IsAssignableFrom<FileStreamResult>(actionResult);
            using var stream = ((FileStreamResult)actionResult).FileStream;
            using var sr = new StreamReader(stream);
            Assert.Equal(expectedContent, sr.ReadToEnd());
        }
    }
}
