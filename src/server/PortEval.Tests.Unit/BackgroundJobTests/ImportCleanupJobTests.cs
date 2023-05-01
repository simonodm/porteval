using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Moq;
using PortEval.Application.Core.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests;

public class ImportCleanupJobTests
{
    private readonly Mock<IDataImportRepository> _dataImportRepository;
    private readonly MockFileSystem _fileSystem;
    private readonly IFixture _fixture;

    public ImportCleanupJobTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));
        _dataImportRepository = _fixture.CreateDefaultDataImportRepositoryMock();
        _fileSystem = _fixture.Freeze<MockFileSystem>();
    }

    [Fact]
    public async Task Run_RemovesImportDatabaseEntriesOlderThan24h()
    {
        var importsToRemove = new List<DataImport>
        {
            new(Guid.NewGuid(), DateTime.UtcNow.AddHours(-24), TemplateType.Portfolios),
            new(Guid.NewGuid(), DateTime.UtcNow.AddHours(-48), TemplateType.Portfolios),
            new(Guid.NewGuid(), DateTime.UtcNow.AddHours(-72), TemplateType.Portfolios)
        };
        var imports = new List<DataImport>
        {
            new(Guid.NewGuid(), DateTime.UtcNow.AddHours(-5), TemplateType.Portfolios),
            new(Guid.NewGuid(), DateTime.UtcNow.AddHours(-3), TemplateType.Portfolios)
        };

        imports.AddRange(importsToRemove);

        _dataImportRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(imports);

        var sut = _fixture.Create<ImportCleanupJob>();

        await sut.RunAsync();

        foreach (var import in importsToRemove)
        {
            _dataImportRepository.Verify(m => m.DeleteAsync(import.Id), Times.Once());
        }
    }

    [Fact]
    public async Task Run_RemovesImportFilesOlderThan24h()
    {
        var storagePath = "/storage/";

        var importsToRemove = new List<DataImport>
        {
            new(Guid.NewGuid(), DateTime.UtcNow.AddHours(-24), TemplateType.Portfolios),
            new(Guid.NewGuid(), DateTime.UtcNow.AddHours(-48), TemplateType.Portfolios),
            new(Guid.NewGuid(), DateTime.UtcNow.AddHours(-72), TemplateType.Portfolios)
        };
        var imports = new List<DataImport>
        {
            new(Guid.NewGuid(), DateTime.UtcNow.AddHours(-5), TemplateType.Portfolios),
            new(Guid.NewGuid(), DateTime.UtcNow.AddHours(-3), TemplateType.Portfolios)
        };

        _fileSystem.AddDirectory(storagePath);

        imports.AddRange(importsToRemove);
        foreach (var import in imports)
        {
            import.AddErrorLog(Path.Combine(storagePath, $"{import.Id}_log.csv"));
            _fileSystem.AddFile(import.ErrorLogPath, import.Id.ToString());
        }

        _dataImportRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(imports);

        var sut = _fixture.Create<ImportCleanupJob>();

        await sut.RunAsync();

        foreach (var import in importsToRemove)
        {
            Assert.False(_fileSystem.FileExists(import.ErrorLogPath));
        }
    }
}