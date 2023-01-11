using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using PortEval.Infrastructure;
using System;

namespace PortEval.Tests.Integration.RepositoryTests
{
    public class RepositoryTestBase : IDisposable
    {
        internal PortEvalDbContext DbContext { get; set; }

        public RepositoryTestBase()
        {
            DbContext = new PortEvalDbContext(new DbContextOptionsBuilder<PortEvalDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
                new Mock<IMediator>().Object);
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
