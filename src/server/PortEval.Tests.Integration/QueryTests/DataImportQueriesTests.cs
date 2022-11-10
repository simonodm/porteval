using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Services.Queries;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Domain.Models.Enums;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests
{
    [Collection("Integration test collection")]
    public class DataImportQueriesTests
    {
        private readonly IDataImportQueries _dataImportQueries;

        public DataImportQueriesTests(IntegrationTestFixture fixture)
        {
            var scope = fixture.Factory.Services.CreateScope();
            _dataImportQueries = scope.ServiceProvider.GetRequiredService<IDataImportQueries>();
        }

        [Fact]
        public async Task GetDataImports_ReturnsImportsFromDatabaseTable()
        {
            var queryResult = await _dataImportQueries.GetAllImports();

            var dataImports = queryResult.Response.ToList();

            Assert.Equal(QueryStatus.Ok,  queryResult.Status);
            Assert.Collection(dataImports,
                import =>
                {
                    Assert.Equal(Guid.Parse("4c0019c2-402f-41e8-9ddf-b3c98027e2d5"), import.ImportId);
                    Assert.Equal(TemplateType.Portfolios, import.TemplateType);
                    Assert.Equal(ImportStatus.Error, import.Status);
                    Assert.Equal("Internal error.", import.StatusDetails);
                    Assert.Equal(DateTime.UtcNow, import.Time, TimeSpan.FromHours(1));
                    Assert.False(import.ErrorLogAvailable);
                    Assert.True(string.IsNullOrEmpty(import.ErrorLogUrl));
                },
                import =>
                {
                    Assert.Equal(Guid.Parse("974c9b22-8276-4121-96ce-6bf3f0f70152"), import.ImportId);
                    Assert.Equal(TemplateType.Instruments, import.TemplateType);
                    Assert.Equal(ImportStatus.Finished, import.Status);
                    Assert.Equal(DateTime.UtcNow, import.Time, TimeSpan.FromHours(1));
                    Assert.False(import.ErrorLogAvailable);
                    Assert.True(string.IsNullOrEmpty(import.ErrorLogUrl));
                });
        }

        [Fact]
        public async Task GetDataImportById_ReturnsNotFound_WhenImportDoesNotExist()
        {
            var queryResult = await _dataImportQueries.GetImport(Guid.NewGuid());

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
            Assert.Null(queryResult.Response);
        }

        [Fact]
        public async Task GetDataImportById_ReturnsImport_WhenItExists()
        {
            var queryResult = await _dataImportQueries.GetImport(Guid.Parse("974c9b22-8276-4121-96ce-6bf3f0f70152"));

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.NotNull(queryResult.Response);
            Assert.Equal(TemplateType.Instruments, queryResult.Response.TemplateType);
            Assert.Equal(ImportStatus.Finished, queryResult.Response.Status);
        }
    }
}
