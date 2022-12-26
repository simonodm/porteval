using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application;
using PortEval.Infrastructure;
using System;

namespace PortEval.Tests.Integration.QueryTests
{
    public class QueryTestFixture : IDisposable
    {
        internal readonly DockerDatabaseIntegrationTestFactory<Program> Factory;

        public QueryTestFixture()
        {
            Factory = new DockerDatabaseIntegrationTestFactory<Program>();
            Factory.InitializeAsync().Wait();
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PortEvalDbContext>();
            db.Database.EnsureDeleted();
            db.Database.Migrate();

            var seeder = new DbContextDataSeeder(db);
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
