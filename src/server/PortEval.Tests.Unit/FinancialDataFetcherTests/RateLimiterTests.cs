using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortEval.FinancialDataFetcher;
using Xunit;

namespace PortEval.Tests.Unit.FinancialDataFetcherTests
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
