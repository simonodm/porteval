using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using Xunit;

namespace PortEval.Tests.Unit.Helpers
{
    internal static class ControllerTestHelper
    {
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
