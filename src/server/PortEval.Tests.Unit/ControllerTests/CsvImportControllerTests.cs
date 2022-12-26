using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class CsvImportControllerTests
    {
        [Fact]
        public async Task GetAllImports_ReturnsAllImports()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var imports = fixture.CreateMany<CsvTemplateImportDto>();

            var importQueries = fixture.Freeze<Mock<IDataImportQueries>>();
            importQueries
                .Setup(m => m.GetAllImports())
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(imports));

            var sut = fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = await sut.GetAllImports();

            importQueries.Verify(m => m.GetAllImports(), Times.Once());
            Assert.Equal(imports, result.Value);
        }

        [Fact]
        public async Task GetImport_ReturnsMatchingImport_WhenImportExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var import = fixture.Create<CsvTemplateImportDto>();

            var importQueries = fixture.Freeze<Mock<IDataImportQueries>>();
            importQueries
                .Setup(m => m.GetImport(import.ImportId))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(import));

            var sut = fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = await sut.GetImport(import.ImportId.ToString());

            importQueries.Verify(m => m.GetImport(import.ImportId), Times.Once());
            Assert.Equal(import, result.Value);
        }

        [Fact]
        public async Task GetImport_ReturnsNotFound_WhenImportDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var importId = fixture.Create<Guid>();

            var importQueries = fixture.Freeze<Mock<IDataImportQueries>>();
            importQueries
                .Setup(m => m.GetImport(importId))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<CsvTemplateImportDto>());

            var sut = fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = await sut.GetImport(importId.ToString());

            importQueries.Verify(m => m.GetImport(importId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetImport_ReturnsBadRequest_WhenProvidedIdIsNotGuid()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var importId = "STR";

            var importQueries = fixture.Freeze<Mock<IDataImportQueries>>();
            importQueries
                .Setup(m => m.GetImport(It.IsAny<Guid>()))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<CsvTemplateImportDto>());

            var sut = fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = await sut.GetImport(importId);

            importQueries.Verify(m => m.GetImport(It.IsAny<Guid>()), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void GetImportErrorLog_ReturnsErrorLogFile_WhenItExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var testString = fixture.Create<string>();
            var importId = fixture.Create<Guid>();

            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms, Encoding.UTF8);
            sw.Write(testString);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var importService = fixture.Freeze<Mock<ICsvImportService>>();
            importService
                .Setup(m => m.TryGetErrorLog(importId))
                .Returns(ms);

            var sut = fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = sut.GetImportErrorLog(importId.ToString());

            importService.Verify(m => m.TryGetErrorLog(importId), Times.Once());
            ControllerTestHelper.AssertFileStreamEqual(testString, result);
        }

        [Fact]
        public void GetImportErrorLog_ReturnsNotFound_WhenItDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var importId = fixture.Create<Guid>();

            var importService = fixture.Freeze<Mock<ICsvImportService>>();
            importService
                .Setup(m => m.TryGetErrorLog(importId))
                .Returns((Stream)null);

            var sut = fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = sut.GetImportErrorLog(importId.ToString());

            importService.Verify(m => m.TryGetErrorLog(importId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }


        [Fact]
        public void GetImportErrorLog_ReturnsBadRequest_WhenProvidedIdIsNotGuid()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var importId = "STR";

            var importService = fixture.Freeze<Mock<ICsvImportService>>();
            importService
                .Setup(m => m.TryGetErrorLog(It.IsAny<Guid>()))
                .Returns((Stream)null);

            var sut = fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = sut.GetImportErrorLog(importId);

            importService.Verify(m => m.TryGetErrorLog(It.IsAny<Guid>()), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetImportTemplate_ReturnsImportTemplate()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var templateType = fixture.Create<TemplateType>();
            var testString = fixture.Create<string>();

            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms, Encoding.UTF8);
            sw.Write(testString);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var importService = fixture.Freeze<Mock<ICsvImportService>>();
            importService
                .Setup(m => m.GetCsvTemplate(templateType))
                .Returns(ms);

            var sut = fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = sut.GetImportTemplate(templateType);

            importService.Verify(m => m.GetCsvTemplate(templateType), Times.Once());
            ControllerTestHelper.AssertFileStreamEqual(testString, result);
        }

        [Fact]
        public void UploadFile_StartsImportingFile()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var templateType = fixture.Create<TemplateType>();
            var testString = fixture.Create<string>();

            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms, Encoding.UTF8);
            sw.Write(testString);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var formFile = new FormFile(ms, 0, ms.Length, "test_file", "test_file.csv");

            var importService = fixture.Freeze<Mock<ICsvImportService>>();

            var sut = fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = sut.UploadFile(formFile, templateType);

            importService.Verify(m => m.StartImport(It.IsAny<Stream>(), templateType), Times.Once());
        }
    }
}
