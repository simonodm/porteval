using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure.Repositories;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests
{
    public class DataImportRepositoryTests : RepositoryTestBase
    {
        private readonly IDataImportRepository _dataImportRepository;

        public DataImportRepositoryTests() : base()
        {
            _dataImportRepository = new DataImportRepository(DbContext);
        }

        [Fact]
        public async Task ListAllAsync_ReturnsAllDataImports()
        {
            var first = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Portfolios);
            var second = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Instruments, ImportStatus.Finished, "errorMessage");

            DbContext.Add(first);
            DbContext.Add(second);
            await DbContext.SaveChangesAsync();

            var dataImports = await _dataImportRepository.ListAllAsync();

            Assert.Collection(dataImports, dataImport => AssertDataImportsAreEqual(first, dataImport),
                dataImport => AssertDataImportsAreEqual(second, dataImport));
        }

        [Fact]
        public async Task Add_CreatesNewDataImport()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Instruments, ImportStatus.Finished, "errorMessage");

            _dataImportRepository.Add(dataImport);
            await _dataImportRepository.UnitOfWork.CommitAsync();

            var createdDataImport = DbContext.Imports.FirstOrDefault();

            AssertDataImportsAreEqual(dataImport, createdDataImport);
        }

        [Fact]
        public async Task Update_UpdatesDataImport()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Instruments, ImportStatus.Pending, "errorMessage");
            DbContext.Add(dataImport);
            await DbContext.SaveChangesAsync();

            dataImport.ChangeStatus(ImportStatus.Error);
            dataImport.AddErrorLog("/home/root/errorLogs");

            _dataImportRepository.Update(dataImport);
            await _dataImportRepository.UnitOfWork.CommitAsync();

            var updatedDataImport = DbContext.Imports.FirstOrDefault();

            AssertDataImportsAreEqual(dataImport, updatedDataImport);
        }

        [Fact]
        public async Task DeleteAsync_DeletesDataImport()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Instruments, ImportStatus.Finished, "errorMessage");
            DbContext.Add(dataImport);
            await DbContext.SaveChangesAsync();

            await _dataImportRepository.DeleteAsync(dataImport.Id);
            await _dataImportRepository.UnitOfWork.CommitAsync();

            var dataImportDeleted = !DbContext.Imports.Any();

            Assert.True(dataImportDeleted);
        }

        [Fact]
        public async Task Delete_DeletesDataImport()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Instruments, ImportStatus.Finished, "errorMessage");
            DbContext.Add(dataImport);
            await DbContext.SaveChangesAsync();

            _dataImportRepository.Delete(dataImport);
            await _dataImportRepository.UnitOfWork.CommitAsync();

            var dataImportDeleted = !DbContext.Imports.Any();

            Assert.True(dataImportDeleted);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenDataImportExists()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Instruments, ImportStatus.Finished, "errorMessage");
            DbContext.Imports.Add(dataImport);
            await DbContext.SaveChangesAsync();

            var exists = await _dataImportRepository.ExistsAsync(dataImport.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenDataImportDoesNotExist()
        {
            var exists = await _dataImportRepository.ExistsAsync(Guid.NewGuid());

            Assert.False(exists);
        }

        private void AssertDataImportsAreEqual(DataImport expected, DataImport actual)
        {
            Assert.Equal(expected?.Time, actual?.Time);
            Assert.Equal(expected?.Status, actual?.Status);
            Assert.Equal(expected?.TemplateType, actual?.TemplateType);
            Assert.Equal(expected?.ErrorLogAvailable, actual?.ErrorLogAvailable);
            Assert.Equal(expected?.ErrorLogPath, actual?.ErrorLogPath);
        }
    }
}
