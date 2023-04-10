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

namespace PortEval.Tests.Unit.CoreTests.Services
{
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

            if (pi == null) return new NoSpecimen();

            if (pi.Member.DeclaringType != typeof(CsvImportService) ||
                pi.ParameterType != typeof(string) ||
                pi.Name != "storagePath")
                return new NoSpecimen();

            return _storagePath;
        }
    }

    public class CsvImportServiceTests
    {
        private readonly string _storagePath;

        public CsvImportServiceTests()
        {
            _storagePath = "/storage/";
        }

        public static IEnumerable<object[]> ImportData =>
            CsvImportTestHelper.TemplateTestData.Select(
                kv => new object[]
                {
                    string.Join(",", CsvImportTestHelper.TemplateExpectedHeaders[kv.Key]) + "\r\n" +
                    string.Join(",", kv.Value),
                    kv.Key
                }
            );

        [Fact]
        public async Task GetAllImportsAsync_ReturnsAllImports()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var imports = fixture.CreateMany<CsvTemplateImportDto>();

            var importQueriesMock = fixture.Freeze<Mock<IDataImportQueries>>();
            importQueriesMock
                .Setup(m => m.GetAllImportsAsync())
                .ReturnsAsync(imports);

            var sut = fixture.Create<CsvImportService>();

            var result = await sut.GetAllImportsAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(imports, result.Response);
        }

        [Fact]
        public async Task GetImportAsync_ReturnsCorrectImport_WhenItExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var import = fixture.Create<CsvTemplateImportDto>();

            var importQueriesMock = fixture.Freeze<Mock<IDataImportQueries>>();
            importQueriesMock
                .Setup(m => m.GetImportAsync(import.ImportId))
                .ReturnsAsync(import);

            var sut = fixture.Create<CsvImportService>();

            var result = await sut.GetImportAsync(import.ImportId);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(import, result.Response);
        }

        [Fact]
        public async Task GetImportAsync_ReturnsNotFound_WhenImportDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var importQueriesMock = fixture.Freeze<Mock<IDataImportQueries>>();
            importQueriesMock
                .Setup(m => m.GetImportAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<CsvTemplateImportDto>(null));

            var sut = fixture.Create<CsvImportService>();

            var result = await sut.GetImportAsync(Guid.NewGuid());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Theory]
        [MemberData(nameof(ImportData))]
        public async Task StartingImport_AddsImportToRepository(string data, TemplateType templateType)
        {
            var fixture = GetFixtureWithMockFileSystem();

            var importRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            importRepository
                .Setup(m => m.Add(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddDirectory(_storagePath);

            var sut = fixture.Create<CsvImportService>();

            await using var stream = GenerateStreamFromString(data);
            await sut.StartImportAsync(stream, templateType);

            importRepository.Verify(r => r.Add(It.Is<DataImport>(i => i.TemplateType == templateType)));
        }

        [Theory]
        [MemberData(nameof(ImportData))]
        public async Task StartingImport_EnqueuesImportJob(string data, TemplateType templateType)
        {
            var fixture = GetFixtureWithMockFileSystem();

            var importRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            importRepository
                .Setup(m => m.Add(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);
            var importQueries = fixture.Freeze<Mock<IDataImportQueries>>();
            importQueries
                .Setup(m => m.GetImportAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => fixture.Build<CsvTemplateImportDto>().With(i => i.ImportId, id).Create());
            var jobClient = fixture.Freeze<Mock<IBackgroundJobClient>>();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddDirectory(_storagePath);

            var sut = fixture.Create<CsvImportService>();

            await using var stream = GenerateStreamFromString(data);
            var importEntry = await sut.StartImportAsync(stream, templateType);

            jobClient.Verify(c => c.Create(
                It.Is<Job>(job =>
                    job.Method.Name == nameof(IDataImportJob.RunAsync) && (Guid)job.Args[0] == importEntry.Response.ImportId &&
                    job.Type.IsAssignableTo(typeof(IDataImportJob))),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public async Task GettingErrorLog_ReturnsStreamToErrorLog_WhenErrorLogExists()
        {
            var fixture = GetFixtureWithMockFileSystem();

            var guid = fixture.Create<Guid>();
            var content = fixture.Create<string>();

            fixture.Freeze<Mock<IDataImportRepository>>();
            fixture.Freeze<Mock<IBackgroundJobClient>>();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(Path.Combine(_storagePath, $"{guid}_log.csv"), content);

            var sut = fixture.Create<CsvImportService>();

            await using var stream = sut.TryGetErrorLog(guid).Response;
            using var sr = new StreamReader(stream);
            var actualContent = await sr.ReadToEndAsync();

            Assert.Equal(content, actualContent);
        }

        [Fact]
        public void GettingErrorLog_ReturnsError_WhenErrorLogDoesNotExist()
        {
            var fixture = GetFixtureWithMockFileSystem();

            var guid = fixture.Create<Guid>();

            fixture.Freeze<Mock<IDataImportRepository>>();
            fixture.Freeze<Mock<IBackgroundJobClient>>();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddDirectory(_storagePath);

            var sut = fixture.Create<CsvImportService>();

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
            var fixture = GetFixtureWithMockFileSystem();

            fixture.Freeze<Mock<IDataImportRepository>>();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddDirectory(_storagePath);

            var sut = fixture.Create<CsvImportService>();

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
            var fixture = GetFixtureWithMockFileSystem();
            var fileContent = fixture.Create<string>();

            fixture.Freeze<Mock<IDataImportRepository>>();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(Path.Combine(_storagePath, templateType.ToString().ToLower() + "_template.csv"),
                fileContent);

            var sut = fixture.Create<CsvImportService>();

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
}