using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
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
        private IFixture _fixture;
        private Mock<ICsvImportService> _importService;

        public CsvImportControllerTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _importService = _fixture.Freeze<Mock<ICsvImportService>>();
        }

        [Fact]
        public async Task GetAllImports_ReturnsAllImports()
        {
            var imports = _fixture.CreateMany<CsvTemplateImportDto>();

            _importService
                .Setup(m => m.GetAllImportsAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(imports));

            var sut = _fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = await sut.GetAllImports();

            _importService.Verify(m => m.GetAllImportsAsync(), Times.Once());
            Assert.Equal(imports, result.Value);
        }

        [Fact]
        public async Task GetImport_ReturnsMatchingImport_WhenImportExists()
        {
            var import = _fixture.Create<CsvTemplateImportDto>();

            _importService
                .Setup(m => m.GetImportAsync(import.ImportId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(import));

            var sut = _fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = await sut.GetImport(import.ImportId.ToString());

            _importService.Verify(m => m.GetImportAsync(import.ImportId), Times.Once());
            Assert.Equal(import, result.Value);
        }

        [Fact]
        public async Task GetImport_ReturnsNotFound_WhenImportDoesNotExist()
        {
            var importId = _fixture.Create<Guid>();

            _importService
                .Setup(m => m.GetImportAsync(importId))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<CsvTemplateImportDto>());

            var sut = _fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = await sut.GetImport(importId.ToString());

            _importService.Verify(m => m.GetImportAsync(importId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetImport_ReturnsBadRequest_WhenProvidedIdIsNotGuid()
        {
            var importId = "STR";

            _importService
                .Setup(m => m.GetImportAsync(It.IsAny<Guid>()))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<CsvTemplateImportDto>());

            var sut = _fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = await sut.GetImport(importId);

            _importService.Verify(m => m.GetImportAsync(It.IsAny<Guid>()), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void GetImportErrorLog_ReturnsErrorLogFile_WhenItExists()
        {
            var testString = _fixture.Create<string>();
            var importId = _fixture.Create<Guid>();

            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms, Encoding.UTF8);
            sw.Write(testString);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            _importService
                .Setup(m => m.TryGetErrorLog(importId))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse<Stream>(ms));

            var sut = _fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = sut.GetImportErrorLog(importId.ToString());

            _importService.Verify(m => m.TryGetErrorLog(importId), Times.Once());
            ControllerTestHelper.AssertFileStreamEqual(testString, result);
        }

        [Fact]
        public void GetImportErrorLog_ReturnsNotFound_WhenItDoesNotExist()
        {
            var importId = _fixture.Create<Guid>();

            _importService
                .Setup(m => m.TryGetErrorLog(importId))
                .Returns(OperationResponseHelper.GenerateNotFoundOperationResponse<Stream>());

            var sut = _fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = sut.GetImportErrorLog(importId.ToString());

            _importService.Verify(m => m.TryGetErrorLog(importId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }


        [Fact]
        public void GetImportErrorLog_ReturnsBadRequest_WhenProvidedIdIsNotGuid()
        {
            var importId = "STR";

            _importService
                .Setup(m => m.TryGetErrorLog(It.IsAny<Guid>()))
                .Returns(OperationResponseHelper.GenerateNotFoundOperationResponse<Stream>());

            var sut = _fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = sut.GetImportErrorLog(importId);

            _importService.Verify(m => m.TryGetErrorLog(It.IsAny<Guid>()), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetImportTemplate_ReturnsImportTemplate()
        {
            var templateType = _fixture.Create<TemplateType>();
            var testString = _fixture.Create<string>();

            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms, Encoding.UTF8);
            sw.Write(testString);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            _importService
                .Setup(m => m.GetCsvTemplate(templateType))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse<Stream>(ms));

            var sut = _fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            var result = sut.GetImportTemplate(templateType);

            _importService.Verify(m => m.GetCsvTemplate(templateType), Times.Once());
            ControllerTestHelper.AssertFileStreamEqual(testString, result);
        }

        [Fact]
        public async Task UploadFile_StartsImportingFile()
        {
            var templateType = _fixture.Create<TemplateType>();
            var testString = _fixture.Create<string>();

            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms, Encoding.UTF8);
            sw.Write(testString);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var formFile = new FormFile(ms, 0, ms.Length, "test_file", "test_file.csv");

            _importService
                .Setup(m => m.StartImportAsync(It.IsAny<Stream>(), templateType))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.Create<CsvTemplateImportDto>()));

            var sut = _fixture.Build<CsvImportController>().OmitAutoProperties().Create();

            await sut.UploadFile(formFile, templateType);

            _importService.Verify(m => m.StartImportAsync(It.IsAny<Stream>(), templateType), Times.Once());
        }
    }
}
