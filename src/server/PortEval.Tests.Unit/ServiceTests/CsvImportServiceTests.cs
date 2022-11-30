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
using PortEval.Application.Services;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.ServiceTests
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

        [Theory]
        [MemberData(nameof(ImportData))]
        public async Task StartingImport_AddsImportToRepository(string data, TemplateType templateType)
        {
            var fixture = GetFixtureWithMockFileSystem();

            var importRepository = fixture.CreateDefaultDataImportRepositoryMock();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddDirectory(_storagePath);

            var sut = fixture.Create<CsvImportService>();

            await using var stream = GenerateStreamFromString(data);
            await sut.StartImport(stream, templateType);

            importRepository.Verify(r => r.Add(It.Is<DataImport>(i => i.TemplateType == templateType)));
        }

        [Theory]
        [MemberData(nameof(ImportData))]
        public async Task StartingImport_ReturnsMatchingDataImportEntry(string data, TemplateType templateType)
        {
            var fixture = GetFixtureWithMockFileSystem();

            fixture.CreateDefaultDataImportRepositoryMock();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddDirectory(_storagePath);

            var sut = fixture.Create<CsvImportService>();

            await using var stream = GenerateStreamFromString(data);
            var importEntry = await sut.StartImport(stream, templateType);

            Assert.Equal(templateType, importEntry.TemplateType);
            Assert.Equal(ImportStatus.Pending, importEntry.Status);
        }

        [Theory]
        [MemberData(nameof(ImportData))]
        public async Task StartingImport_EnqueuesImportJob(string data, TemplateType templateType)
        {
            var fixture = GetFixtureWithMockFileSystem();

            fixture.CreateDefaultDataImportRepositoryMock();
            var jobClient = fixture.Freeze<Mock<IBackgroundJobClient>>();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddDirectory(_storagePath);

            var sut = fixture.Create<CsvImportService>();

            await using var stream = GenerateStreamFromString(data);
            var importEntry = await sut.StartImport(stream, templateType);

            jobClient.Verify(c => c.Create(
                It.Is<Job>(job =>
                    job.Method.Name == "Run" && ((DataImport)job.Args[0]).Id == importEntry.Id &&
                    job.Type.IsAssignableTo(typeof(IDataImportJob))),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public async Task GettingErrorLog_ReturnsStreamToErrorLog_WhenErrorLogExists()
        {
            var fixture = GetFixtureWithMockFileSystem();

            var guid = fixture.Create<Guid>();
            var content = fixture.Create<string>();

            fixture.CreateDefaultDataImportRepositoryMock();
            fixture.Freeze<Mock<IBackgroundJobClient>>();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(Path.Combine(_storagePath, $"{guid}_log.csv"), content);

            var sut = fixture.Create<CsvImportService>();

            await using var stream = sut.TryGetErrorLog(guid);
            using var sr = new StreamReader(stream);
            var actualContent = await sr.ReadToEndAsync();

            Assert.Equal(content, actualContent);
        }

        [Fact]
        public void GettingErrorLog_ReturnsNull_WhenErrorLogDoesNotExist()
        {
            var fixture = GetFixtureWithMockFileSystem();

            var guid = fixture.Create<Guid>();

            fixture.CreateDefaultDataImportRepositoryMock();
            fixture.Freeze<Mock<IBackgroundJobClient>>();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddDirectory(_storagePath);

            var sut = fixture.Create<CsvImportService>();

            var stream = sut.TryGetErrorLog(guid);

            Assert.Null(stream);
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

            fixture.CreateDefaultDataImportRepositoryMock();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddDirectory(_storagePath);

            var sut = fixture.Create<CsvImportService>();

            await using var templateStream = sut.GetCsvTemplate(templateType);
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

            fixture.CreateDefaultDataImportRepositoryMock();
            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(Path.Combine(_storagePath, templateType.ToString().ToLower() + "_template.csv"),
                fileContent);

            var sut = fixture.Create<CsvImportService>();

            await using var templateStream = sut.GetCsvTemplate(templateType);
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