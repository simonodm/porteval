﻿using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Features.Services.BulkImportExport;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.BulkImportExport
{
    public class PositionImportProcessorTests
    {
        [Fact]
        public async Task ProcessingImport_CreatesNewPosition_WhenNoIdIsPresent()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Time, DateTime.UtcNow)
                .Create();
            var positionService = fixture.CreateDefaultPositionServiceMock();
            var sut = fixture.Create<PositionImportProcessor>();

            await sut.ImportRecords(new List<PositionDto> { position });

            positionService.Verify(s => s.OpenPositionAsync(It.Is<PositionDto>(p =>
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
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Time, DateTime.UtcNow)
                .Create();
            var positionService = fixture.CreateDefaultPositionServiceMock();
            var sut = fixture.Create<PositionImportProcessor>();

            await sut.ImportRecords(new List<PositionDto> { position });

            positionService.Verify(s => s.UpdatePositionAsync(It.Is<PositionDto>(p =>
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
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.InstrumentId, 0)
                .Create();
            var positionService = fixture.CreateDefaultPositionServiceMock();
            var sut = fixture.Create<PositionImportProcessor>();

            await sut.ImportRecords(new List<PositionDto> { position });

            positionService.Verify(s => s.OpenPositionAsync(It.IsAny<PositionDto>()), Times.Never());
            positionService.Verify(s => s.UpdatePositionAsync(It.IsAny<PositionDto>()), Times.Never());
        }
    }
}