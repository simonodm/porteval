using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PortEval.Tests.Integration.FinancialDataFetcherTests
{
    internal static class Helpers
    {
        public static HttpClient SetupMockHttpClientReturningResponse<T>(string uri, T response)
        {
            var serializedMockResponse = JsonConvert.SerializeObject(response);
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m =>
                        m.RequestUri.AbsoluteUri.StartsWith(uri)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(serializedMockResponse)
                });

            return new HttpClient(mockMessageHandler.Object);
        }
    }
}
