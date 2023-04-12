using PortEval.DataFetcher;
using System;
using Xunit;

namespace PortEval.Tests.Unit.DataFetcherTests
{
    public class RateLimiterTests
    {
        [Fact]
        public void AllowRequest_ReturnsFalse_WhenLimitIsExceeded()
        {
            var rateLimiter = new RateLimiter(TimeSpan.FromDays(30), 2);

            var first = rateLimiter.AllowRequest();
            var second = rateLimiter.AllowRequest();
            var third = rateLimiter.AllowRequest();

            Assert.True(first);
            Assert.True(second);
            Assert.False(third);
        }
    }
}
