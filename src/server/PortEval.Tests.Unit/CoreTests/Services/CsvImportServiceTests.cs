using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services;

internal class CsvImportServiceStoragePathArg : ISpecimenBuilder
{
    private readonly string _storagePath;

    public CsvImportServiceStoragePathArg(string storagePath)
    {
        _storagePath = storagePath;
    }

    public object Create(object request, ISpecimenContext context)
    {
        var pi = request as ParameterInfo;

        if (pi == null)
        {
            return new NoSpecimen();
        }

        if (pi.Member.DeclaringType != typeof(CsvImportService) ||
            pi.ParameterType != typeof(string) ||
            pi.Name != "storagePath")
        {
            return new NoSpecimen();
        }

        return _storagePath;
    }
}

public class CsvImportServiceTests
{
    public static IEnumerable<object[]> ImportData =>
        CsvImportTestHelper.TemplateTestData.Select(
            kv => new object[]
            {
                string.Join(",", CsvImportTestHelper.TemplateExpectedHeaders[kv.Key]) + "\r\n" +
                string.Join(",", kv.Value),
                kv.Key
            }
        );

    private readonly Mock<IBackgroundJobClient> _backgroundJobClient;
    private readonly Mock<IDataImportQueries> _dataImportQueries;
    private readonly Mock<IDataImportRepository> _dataImportRepository;
    private readonly MockFileSystem _fileSystem;
    private readonly IFixture _fixture;
    private readonly string _storagePath;

    public CsvImportServiceTests()
    {
        _storagePath = "/storage/";
        _fixture = GetFixtureWithMockFileSystem();
        _dataImportQueries = _fixture.CreateDefaultDataImportQueriesMock();
        _dataImportRepository = _fixture.CreateDefaultDataImportRepositoryMock();
        _backgroundJobClient = _fixture.Freeze<Mock<IBackgroundJobClient>>();
        _fileSystem = _fixture.Freeze<MockFileSystem>();
    }

    [Fact]
    public async Task GetAllImportsAsync_ReturnsAllImports()
    {
        var imports = _fixture.CreateMany<CsvTemplateImportDto>();

        _dataImportQueries
            .Setup(m => m.GetAllImportsAsync())
            .ReturnsAsync(imports);

        var sut = _fixture.Create<CsvImportService>();

        var result = await sut.GetAllImportsAsync();

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(imports, result.Response);
    }

    [Fact]
    public async Task GetImportAsync_ReturnsCorrectImport_WhenItExists()
    {
        var import = _fixture.Create<CsvTemplateImportDto>();

        _dataImportQueries
            .Setup(m => m.GetImportAsync(import.ImportId))
            .ReturnsAsync(import);

        var sut = _fixture.Create<CsvImportService>();

        var result = await sut.GetImportAsync(import.ImportId);

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(import, result.Response);
    }

    [Fact]
    public async Task GetImportAsync_ReturnsNotFound_WhenImportDoesNotExist()
    {
        _dataImportQueries
            .Setup(m => m.GetImportAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult<CsvTemplateImportDto>(null));

        var sut = _fixture.Create<CsvImportService>();

        var result = await sut.GetImportAsync(Guid.NewGuid());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Theory]
    [MemberData(nameof(ImportData))]
    public async Task StartingImport_AddsImportToRepository(string data, TemplateType templateType)
    {
        _fileSystem.AddDirectory(_storagePath);

        var sut = _fixture.Create<CsvImportService>();

        await using var stream = GenerateStreamFromString(data);
        await sut.StartImportAsync(stream, templateType);

        _dataImportRepository.Verify(r => r.Add(It.Is<DataImport>(i => i.TemplateType == templateType)));
    }

    [Theory]
    [MemberData(nameof(ImportData))]
    public async Task StartingImport_EnqueuesImportJob(string data, TemplateType templateType)
    {
        _fileSystem.AddDirectory(_storagePath);

        var sut = _fixture.Create<CsvImportService>();

        _dataImportQueries
            .Setup(m => m.GetImportAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => _fixture.Build<CsvTemplateImportDto>().With(i => i.ImportId, id).Create());

        await using var stream = GenerateStreamFromString(data);
        var importEntry = await sut.StartImportAsync(stream, templateType);

        _backgroundJobClient.Verify(c => c.Create(
            It.Is<Job>(job =>
                job.Method.Name == nameof(IDataImportJob.RunAsync) &&
                (Guid)job.Args[0] == importEntry.Response.ImportId &&
                job.Type.IsAssignableTo(typeof(IDataImportJob))),
            It.IsAny<EnqueuedState>()));
    }

    [Fact]
    public async Task GettingErrorLog_ReturnsStreamToErrorLog_WhenErrorLogExists()
    {
        var guid = _fixture.Create<Guid>();
        var content = _fixture.Create<string>();

        _fileSystem.AddFile(Path.Combine(_storagePath, $"{guid}_log.csv"), content);

        var sut = _fixture.Create<CsvImportService>();

        await using var stream = sut.TryGetErrorLog(guid).Response;
        using var sr = new StreamReader(stream);
        var actualContent = await sr.ReadToEndAsync();

        Assert.Equal(content, actualContent);
    }

    [Fact]
    public void GettingErrorLog_ReturnsError_WhenErrorLogDoesNotExist()
    {
        var guid = _fixture.Create<Guid>();

        _fileSystem.AddDirectory(_storagePath);

        var sut = _fixture.Create<CsvImportService>();

        var response = sut.TryGetErrorLog(guid);

        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Theory]
    [InlineData(TemplateType.Instruments)]
    [InlineData(TemplateType.Portfolios)]
    [InlineData(TemplateType.Positions)]
    [InlineData(TemplateType.Transactions)]
    [InlineData(TemplateType.Prices)]
    public async Task GettingTemplate_ReturnsStreamToCorrectTemplate_WhenTemplateFileDoesNotExist(
        TemplateType templateType)
    {
        _fileSystem.AddDirectory(_storagePath);

        var sut = _fixture.Create<CsvImportService>();

        await using var templateStream = sut.GetCsvTemplate(templateType).Response;
        using var sr = new StreamReader(templateStream);

        var templateContentFirstLine = await sr.ReadLineAsync();

        Assert.Equal(string.Join(",", CsvImportTestHelper.TemplateExpectedHeaders[templateType]),
            templateContentFirstLine);
    }

    [Theory]
    [InlineData(TemplateType.Instruments)]
    [InlineData(TemplateType.Portfolios)]
    [InlineData(TemplateType.Positions)]
    [InlineData(TemplateType.Transactions)]
    [InlineData(TemplateType.Prices)]
    public async Task GettingTemplate_ReturnsStreamToExistingTemplateFile_WhenTemplateFileExists(
        TemplateType templateType)
    {
        var fileContent = _fixture.Create<string>();

        _fileSystem.AddFile(Path.Combine(_storagePath, templateType.ToString().ToLower() + "_template.csv"),
            fileContent);

        var sut = _fixture.Create<CsvImportService>();

        await using var templateStream = sut.GetCsvTemplate(templateType).Response;
        using var sr = new StreamReader(templateStream);

        var actualContent = await sr.ReadToEndAsync();

        Assert.Equal(fileContent, actualContent);
    }

    private IFixture GetFixtureWithMockFileSystem()
    {
        var fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        fixture.Customizations.Add(new CsvImportServiceStoragePathArg(_storagePath));
        fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

        return fixture;
    }

    private static Stream GenerateStreamFromString(string str)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(str);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}