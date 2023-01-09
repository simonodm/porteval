﻿using AutoFixture;
using AutoFixture.AutoMoq;
using Hangfire;
using Moq;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers.Extensions;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Services
{
    public class InstrumentServiceTests
    {
        [Fact]
        public async Task CreatingInstrument_AddsInstrumentToInstrumentRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();
            fixture.CreateDefaultExchangeRepositoryMock();

            var sut = fixture.Create<InstrumentService>();

            await sut.CreateInstrumentAsync(instrument);

            instrumentRepository.Verify(
                r => r.Add(It.Is<Instrument>(i =>
                    i.Name == instrument.Name &&
                    i.Exchange == instrument.Exchange &&
                    i.CurrencyCode == instrument.CurrencyCode &&
                    i.Symbol == instrument.Symbol &&
                    i.Type == instrument.Type &&
                    i.Note == instrument.Note
                )),
                Times.Once());
        }

        [Fact]
        public async Task CreatingInstrument_ReturnsCreatedInstrument_WhenSuccessfullyCreated()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentDto = fixture.Create<InstrumentDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();
            fixture.CreateDefaultExchangeRepositoryMock();

            var sut = fixture.Create<InstrumentService>();

            var createdInstrument = await sut.CreateInstrumentAsync(instrumentDto);
            Assert.Equal(createdInstrument.Name, instrumentDto.Name);
            Assert.Equal(createdInstrument.Symbol, instrumentDto.Symbol);
            Assert.Equal(createdInstrument.Note, instrumentDto.Note);
            Assert.Equal(createdInstrument.Exchange, instrumentDto.Exchange);
            Assert.Equal(createdInstrument.CurrencyCode, instrumentDto.CurrencyCode);
        }

        [Fact]
        public async Task CreatingInstrument_CreatesItsExchange_WhenExchangeDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var exchangeRepository = fixture.CreateDefaultExchangeRepositoryMock();
            exchangeRepository
                .Setup(e => e.ExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<InstrumentService>();
            await sut.CreateInstrumentAsync(instrument);

            exchangeRepository.Verify(
                r => r.Add(It.Is<Exchange>(e => e.Symbol == instrument.Exchange)),
                Times.Once()
            );
        }

        [Fact]
        public async Task CreatingInstrument_DoesNotCreateExchange_WhenExchangeExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();
            var exchangeRepository = fixture.CreateDefaultExchangeRepositoryMock();

            var sut = fixture.Create<InstrumentService>();
            await sut.CreateInstrumentAsync(instrument);

            exchangeRepository.Verify(
                r => r.Add(It.IsAny<Exchange>()),
                Times.Never()
            );
        }

        [Fact]
        public async Task CreatingInstrument_ThrowsException_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(c => c.ExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<InstrumentService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.CreateInstrumentAsync(instrument));
        }

        [Fact]
        public async Task UpdatingInstrument_UpdatesInstrumentInRepository_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<Instrument>();
            var updatedInstrumentDto = fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(instrument));

            fixture.CreateDefaultExchangeRepositoryMock();

            var sut = fixture.Create<InstrumentService>();
            await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            instrumentRepository.Verify(r => r.Update(
                    It.Is<Instrument>(i =>
                        i.Id == updatedInstrumentDto.Id &&
                        i.Name == updatedInstrumentDto.Name &&
                        i.Symbol == instrument.Symbol &&
                        i.Exchange == updatedInstrumentDto.Exchange &&
                        i.Note == updatedInstrumentDto.Note)),
                Times.Once()
            );
        }

        [Fact]
        public async Task UpdatingInstrument_ThrowsException_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var updatedInstrumentDto = fixture.Create<InstrumentDto>();

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult<Instrument>(null));
            instrumentRepository
                .Setup(i => i.ExistsAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<InstrumentService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () =>
                await sut.UpdateInstrumentAsync(updatedInstrumentDto));
        }

        [Fact]
        public async Task UpdatingInstrument_ReturnsUpdatedInstrument_WhenUpdatedSuccessfully()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<Instrument>();
            var updatedInstrumentDto = fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(instrument));

            fixture.CreateDefaultExchangeRepositoryMock();

            var sut = fixture.Create<InstrumentService>();
            var updatedInstrument = await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            Assert.Equal(updatedInstrumentDto.Id, updatedInstrument.Id);
            Assert.Equal(updatedInstrumentDto.Name, updatedInstrument.Name);
            Assert.Equal(updatedInstrumentDto.Exchange, updatedInstrument.Exchange);
            Assert.Equal(updatedInstrumentDto.Note, updatedInstrument.Note);
        }

        [Fact]
        public async Task UpdatingInstrument_AddsExchangeToRepository_WhenExchangeDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<Instrument>();
            var updatedInstrumentDto = fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(instrument));

            var exchangeRepository = fixture.CreateDefaultExchangeRepositoryMock();
            exchangeRepository
                .Setup(e => e.ExistsAsync(updatedInstrumentDto.Exchange))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<InstrumentService>();
            await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            exchangeRepository.Verify(r => r.Add(It.Is<Exchange>(e => e.Symbol == updatedInstrumentDto.Exchange)),
                Times.Once());
        }

        [Fact]
        public async Task UpdatingInstrument_DoesNotAddExchangeToRepository_WhenExchangeExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<Instrument>();
            var updatedInstrumentDto = fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(i => i.FindAsync(instrument.Id))
                .Returns(Task.FromResult(instrument));

            var exchangeRepository = fixture.Freeze<Mock<IExchangeRepository>>();
            exchangeRepository
                .Setup(e => e.ExistsAsync(updatedInstrumentDto.Exchange))
                .Returns(Task.FromResult(true));

            var sut = fixture.Create<InstrumentService>();
            await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            exchangeRepository.Verify(r => r.Add(It.IsAny<Exchange>()), Times.Never());
        }

        [Fact]
        public async Task DeletingInstrument_DeletesInstrumentFromRepository_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(i => i.ExistsAsync(It.Is<int>(id => id == instrumentId)))
                .Returns(Task.FromResult(true));

            fixture.CreateDefaultCurrencyRepositoryMock();
            fixture.CreateDefaultExchangeRepositoryMock();

            var sut = fixture.Create<InstrumentService>();

            await sut.DeleteAsync(instrumentId);

            instrumentRepository.Verify(r => r.DeleteAsync(instrumentId), Times.Once());
        }

        [Fact]
        public async Task DeletingInstrument_ThrowsException_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(i => i.ExistsAsync(instrumentId))
                .Returns(Task.FromResult(false));

            fixture.CreateDefaultCurrencyRepositoryMock();
            fixture.CreateDefaultExchangeRepositoryMock();

            var sut = fixture.Create<InstrumentService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.DeleteAsync(instrumentId));
        }
    }
}