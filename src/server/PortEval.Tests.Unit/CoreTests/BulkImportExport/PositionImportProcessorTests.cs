using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.Common.BulkImportExport;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.BulkImportExport
{
    public class PositionImportProcessorTests
    {
        private IFixture _fixture;
        private Mock<IPositionService> _positionService;

        public PositionImportProcessorTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _positionService = _fixture.Freeze<Mock<IPositionService>>();
        }

        [Fact]
        public async Task ProcessingImport_CreatesNewPosition_WhenNoIdIsPresent()
        {
            var position = _fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Time, DateTime.UtcNow)
                .Create();
            var sut = _fixture.Create<PositionImportProcessor>();

            await sut.ImportRecordsAsync(new List<PositionDto> { position });

            _positionService.Verify(s => s.OpenPositionAsync(It.Is<PositionDto>(p =>
                p.Id == default &&
                p.PortfolioId == position.PortfolioId &&
                p.Amount == position.Amount &&
                p.Time == position.Time &&
                p.Price == position.Price &&
                p.Note == position.Note
            )), Times.Once());
        }

        [Fact]
        public async Task ProcessingImport_UpdatesPosition_WhenIdIsPresent()
        {
            var position = _fixture.Build<PositionDto>()
                .With(p => p.Time, DateTime.UtcNow)
                .Create();
            var sut = _fixture.Create<PositionImportProcessor>();

            await sut.ImportRecordsAsync(new List<PositionDto> { position });

            _positionService.Verify(s => s.UpdatePositionAsync(It.Is<PositionDto>(p =>
                p.Id == position.Id &&
                p.PortfolioId == position.PortfolioId &&
                p.Amount == position.Amount &&
                p.Time == position.Time &&
                p.Price == position.Price &&
                p.Note == position.Note
            )), Times.Once());
        }

        [Fact]
        public async Task ProcessingImport_DoesNothing_WhenPositionFailsValidation()
        {
            var position = _fixture.Build<PositionDto>()
                .With(p => p.InstrumentId, 0)
                .Create();
            var sut = _fixture.Create<PositionImportProcessor>();

            await sut.ImportRecordsAsync(new List<PositionDto> { position });

            _positionService.Verify(s => s.OpenPositionAsync(It.IsAny<PositionDto>()), Times.Never());
            _positionService.Verify(s => s.UpdatePositionAsync(It.IsAny<PositionDto>()), Times.Never());
        }
    }
}