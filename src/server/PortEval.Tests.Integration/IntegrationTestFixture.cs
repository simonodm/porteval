using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application;
using PortEval.Infrastructure;
using System;

namespace PortEval.Tests.Functional
{
    public class IntegrationTestFixture : IDisposable
    {
        internal readonly IntegrationTestFactory<Program> Factory;

        public IntegrationTestFixture()
        {
            Factory = new IntegrationTestFactory<Program>();
            Factory.InitializeAsync().Wait();
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PortEvalDbContext>();
            db.Database.EnsureDeleted();
            db.Database.Migrate();

            var seeder = new IntegrationTestDataSeeder(db);
            seeder.SeedDatabase().Wait();
        }

        public void Dispose()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PortEvalDbContext>();
            db.Database.EnsureDeleted();
            Factory.DisposeAsync().Wait();
        }
    }
}
