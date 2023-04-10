using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Domain.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests
{
    [Collection("Query test collection")]
    public class DataImportQueriesTests
    {
        private readonly IDataImportQueries _dataImportQueries;

        public DataImportQueriesTests(QueryTestFixture fixture)
        {
            var scope = fixture.Factory.Services.CreateScope();
            _dataImportQueries = scope.ServiceProvider.GetRequiredService<IDataImportQueries>();
        }

        [Fact]
        public async Task GetDataImports_ReturnsImportsFromDatabaseTable()
        {
            var queryResult = await _dataImportQueries.GetAllImportsAsync();

            var dataImports = queryResult.ToList();
            
            Assert.Collection(dataImports,
                import =>
                {
                    Assert.Equal(Guid.Parse("974c9b22-8276-4121-96ce-6bf3f0f70152"), import.ImportId);
                    Assert.Equal(TemplateType.Instruments, import.TemplateType);
                    Assert.Equal(ImportStatus.Finished, import.Status);
                    Assert.Equal(DateTime.UtcNow, import.Time, TimeSpan.FromHours(1));
                    Assert.False(import.ErrorLogAvailable);
                    Assert.True(string.IsNullOrEmpty(import.ErrorLogUrl));
                },
                import =>
                {
                    Assert.Equal(Guid.Parse("4c0019c2-402f-41e8-9ddf-b3c98027e2d5"), import.ImportId);
                    Assert.Equal(TemplateType.Portfolios, import.TemplateType);
                    Assert.Equal(ImportStatus.Error, import.Status);
                    Assert.Equal("Internal error.", import.StatusDetails);
                    Assert.Equal(DateTime.UtcNow.AddHours(-6), import.Time, TimeSpan.FromHours(1));
                    Assert.False(import.ErrorLogAvailable);
                    Assert.True(string.IsNullOrEmpty(import.ErrorLogUrl));
                });
        }

        [Fact]
        public async Task GetDataImportById_ReturnsNull_WhenImportDoesNotExist()
        {
            var queryResult = await _dataImportQueries.GetImportAsync(Guid.NewGuid());
            
            Assert.Null(queryResult);
        }

        [Fact]
        public async Task GetDataImportById_ReturnsImport_WhenItExists()
        {
            var queryResult = await _dataImportQueries.GetImportAsync(Guid.Parse("974c9b22-8276-4121-96ce-6bf3f0f70152"));
            
            Assert.NotNull(queryResult);
            Assert.Equal(TemplateType.Instruments, queryResult.TemplateType);
            Assert.Equal(ImportStatus.Finished, queryResult.Status);
        }
    }
}
