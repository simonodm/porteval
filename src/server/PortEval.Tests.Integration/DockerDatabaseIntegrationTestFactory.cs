using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PortEval.Application.Extensions;
using PortEval.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration
{
    public class DockerDatabaseIntegrationTestFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
        where TProgram : class
    {
        private IConfiguration _configuration;
        private readonly TestcontainerDatabase _container;

        public DockerDatabaseIntegrationTestFactory()
        {
            // Generic ContainerBuilder is being deprecated, however there is no specialized builder available for MSSQL as of Feb 2023
            // Can possibly be solved using a custom builder/module, but currently it's not necessary
            _container = new ContainerBuilder<MsSqlTestcontainer>()
                .WithDatabase(new MsSqlTestcontainerConfiguration
                {
                    Password = "localdevpassword#123",
                })
                .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
                .WithCleanUp(true)
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var additionalConfigurationKeys = new Dictionary<string, string>
            {
                { "PortEvalDb:ConnectionString", _container.ConnectionString }
            };

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddEnvironmentVariables();
                config.AddInMemoryCollection(additionalConfigurationKeys);
                _configuration = config.Build();
            });

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<PortEvalDbConnectionCreator>();
                services.RemoveAll<PortEvalDbContext>();
                services.ConfigureDapper();
                services.ConfigureDbContext(_configuration);
                services.AddQueries();
                services.AddSingleton<JobStorage>(_ => new SqlServerStorage(_container.ConnectionString));
            });
        }

        public async Task InitializeAsync() => await _container.StartAsync();

        public new async Task DisposeAsync()
        {
            await _container.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}
