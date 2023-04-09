using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using Xunit;
using PortEval.Application.Core;

namespace PortEval.Tests.Unit.Helpers
{
    internal static class ControllerTestHelper
    {
        public static OperationResponse<T> GenerateSuccessfulQueryResponse<T>(T result)
        {
            return new OperationResponse<T>
            {
                Status = OperationStatus.Ok,
                Response = result
            };
        }

        public static OperationResponse<T> GenerateNotFoundQueryResponse<T>()
        {
            return new OperationResponse<T>
            {
                Status = OperationStatus.NotFound,
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
