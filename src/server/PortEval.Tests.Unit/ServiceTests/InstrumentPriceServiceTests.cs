using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.ServiceTests
{
    public class InstrumentPriceServiceTests
    {
        [Fact]
        public async Task AddingPricePoint_AddsPricePointToRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();

            var sut = fixture.Create<InstrumentPriceService>();

            await sut.AddPricePointAsync(priceDto);

            priceRepository.Verify(r => r.Add(It.Is<InstrumentPrice>(p =>
                p.InstrumentId == priceDto.InstrumentId &&
                p.Price == priceDto.Price &&
                Math.Abs(priceDto.Time.Ticks - p.Time.Ticks) <= TimeSpan.FromMinutes(1).Ticks
            )));
        }

        [Fact]
        public async Task AddingPricePoint_ThrowsException_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId))
                .ReturnsAsync(false);
            instrumentRepository
                .Setup(r => r.FindAsync(priceDto.InstrumentId))
                .ReturnsAsync((Instrument)null);
            fixture.CreateDefaultInstrumentPriceRepositoryMock();

            var sut = fixture.Create<InstrumentPriceService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.AddPricePointAsync(priceDto));
        }

        [Fact]
        public async Task AddingPriceIfNotExists_AddsPriceToRepository_WhenPriceDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync(false);
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Time))
                .ReturnsAsync(false);
            priceRepository
                .Setup(r => r.FindPriceByIdAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync((InstrumentPrice)null);
            priceRepository
                .Setup(r => r.FindPriceAtAsync(priceDto.InstrumentId, It.Is<DateTime>(dt => dt <= priceDto.Time)))
                .ReturnsAsync((InstrumentPrice)null);

            var sut = fixture.Create<InstrumentPriceService>();

            await sut.AddPriceIfNotExistsAsync(priceDto.InstrumentId, priceDto.Time, priceDto.Price);

            priceRepository.Verify(r => r.Add(It.Is<InstrumentPrice>(p =>
                p.InstrumentId == priceDto.InstrumentId &&
                p.Price == priceDto.Price &&
                Math.Abs(priceDto.Time.Ticks - p.Time.Ticks) <= TimeSpan.FromMinutes(1).Ticks
            )));
        }

        [Fact]
        public async Task AddingPriceIfNotExists_AddsPriceToRepository_IfExistingPriceIsDifferent()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync(false);
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Time))
                .ReturnsAsync(false);
            priceRepository
                .Setup(r => r.FindPriceByIdAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync((InstrumentPrice)null);
            priceRepository
                .Setup(r => r.FindPriceAtAsync(priceDto.InstrumentId, It.Is<DateTime>(dt => dt <= priceDto.Time)))
                .ReturnsAsync<int, DateTime, IInstrumentPriceRepository, InstrumentPrice>((id, dt) =>
                    new InstrumentPrice(dt, priceDto.Price + 1m, priceDto.InstrumentId));

            var sut = fixture.Create<InstrumentPriceService>();

            await sut.AddPriceIfNotExistsAsync(priceDto.InstrumentId, priceDto.Time, priceDto.Price);

            priceRepository.Verify(r => r.Add(It.Is<InstrumentPrice>(p =>
                p.InstrumentId == priceDto.InstrumentId &&
                p.Price == priceDto.Price &&
                Math.Abs(priceDto.Time.Ticks - p.Time.Ticks) <= TimeSpan.FromMinutes(1).Ticks
            )));
        }

        [Fact]
        public async Task AddingPriceIfNotExists_DoesNotAddPrice_WhenSamePriceExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync(false);
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Time))
                .ReturnsAsync(false);
            priceRepository
                .Setup(r => r.FindPriceByIdAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync((InstrumentPrice)null);
            priceRepository
                .Setup(r => r.FindPriceAtAsync(priceDto.InstrumentId, It.Is<DateTime>(dt => dt <= priceDto.Time)))
                .ReturnsAsync<int, DateTime, IInstrumentPriceRepository, InstrumentPrice>((id, dt) =>
                    new InstrumentPrice(dt.AddSeconds(-1), priceDto.Price, priceDto.InstrumentId));

            var sut = fixture.Create<InstrumentPriceService>();

            await sut.AddPriceIfNotExistsAsync(priceDto.InstrumentId, priceDto.Time, priceDto.Price);

            priceRepository.Verify(r => r.Add(It.IsAny<InstrumentPrice>()), Times.Never());
        }

        [Fact]
        public async Task AddingPriceIfNotExists_ThrowsException_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId))
                .ReturnsAsync(false);
            instrumentRepository
                .Setup(r => r.FindAsync(priceDto.InstrumentId))
                .ReturnsAsync((Instrument)null);
            fixture.CreateDefaultInstrumentPriceRepositoryMock();

            var sut = fixture.Create<InstrumentPriceService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () =>
                await sut.AddPriceIfNotExistsAsync(priceDto.InstrumentId, priceDto.Time, priceDto.Price));
        }

        [Fact]
        public async Task DeletingPrice_DeletesPriceFromRepository_WhenPriceExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var price = fixture.Create<InstrumentPrice>();
            var priceDto = fixture.Build<InstrumentPriceDto>()
                .With(p => p.Id, price.Id)
                .With(p => p.InstrumentId, price.InstrumentId)
                .With(p => p.Price, price.Price)
                .With(p => p.Time, price.Time)
                .Create();

            fixture.CreateDefaultInstrumentRepositoryMock();
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync(true);
            priceRepository
                .Setup(r => r.FindPriceByIdAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync(price);

            var sut = fixture.Create<InstrumentPriceService>();

            await sut.DeletePricePointByIdAsync(priceDto.InstrumentId, priceDto.Id);

            priceRepository.Verify(r => r.DeleteAsync(priceDto.InstrumentId, priceDto.Id), Times.Once());
        }

        [Fact]
        public async Task DeletingPrice_ThrowsException_WhenPriceDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync(false);
            priceRepository
                .Setup(r => r.FindPriceByIdAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync((InstrumentPrice)null);

            var sut = fixture.Create<InstrumentPriceService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () =>
                await sut.DeletePricePointByIdAsync(priceDto.InstrumentId, priceDto.Id));
        }
    }
}