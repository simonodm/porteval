using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure.Repositories;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class InstrumentPriceServiceTests
    {
        [Fact]
        public async Task GetInstrumentPricesAsync_ReturnsAllInstrumentPricesInRange_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            var prices = fixture.CreateMany<InstrumentPriceDto>();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Instrument(
                    id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<InstrumentType>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
                );
            instrumentRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentPricesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(prices);

            var sut = fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPricesAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(prices, result.Response);
        }

        [Fact]
        public async Task GetInstrumentPricesAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Instrument(
                    id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<InstrumentType>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
                );

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);
            instrumentQueries
                .Setup(m => m.GetInstrumentPricesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((IEnumerable<InstrumentPriceDto>)null);

            var sut = fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPricesAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetInstrumentPricesPageAsync_ReturnsInstrumentPricePage_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceCount = 30;
            var takeCount = 15;
            var prices = fixture.CreateMany<InstrumentPriceDto>(priceCount);
            var pricesToTake = prices.Take(15);
            var paginationParams = fixture.Build<PaginationParams>()
                .With(p => p.Page, 2)
                .With(p => p.Limit, 15)
                .Create();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentPricesPageAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), paginationParams, It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(pricesToTake);
            instrumentQueries
                .Setup(m => m.GetInstrumentPriceCountAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(priceCount);

            var sut = fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPricesPageAsync(fixture.Create<int>(), paginationParams, fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(priceCount, result.Response.TotalCount);
            Assert.Equal(takeCount, result.Response.Count);
            Assert.Equal(paginationParams.Page, result.Response.Page);
            Assert.Equal(pricesToTake, result.Response.Data);
        }

        [Fact]
        public async Task GetInstrumentPricesPageAsync_ReturnsInstrumentPricePage_WhenInstrumentExistsAndCompressedIsTrue()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceCount = 30;
            var takeCount = 15;
            var prices = fixture.CreateMany<InstrumentPriceDto>(priceCount);
            var pricesToTake = prices.Take(15);
            var paginationParams = fixture.Build<PaginationParams>()
                .With(p => p.Page, 2)
                .With(p => p.Limit, 15)
                .Create();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Instrument(
                    id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<InstrumentType>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
                );
            instrumentRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            instrumentPriceRepository
                .Setup(m => m.Add(It.IsAny<InstrumentPrice>()))
                .Returns<InstrumentPrice>(p => p);

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentPricesPageCompressedAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), paginationParams, It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(pricesToTake);
            instrumentQueries
                .Setup(m => m.GetInstrumentPriceCompressedCountAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(priceCount);

            var sut = fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPricesPageAsync(fixture.Create<int>(), paginationParams, fixture.Create<DateRangeParams>(), true);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(priceCount, result.Response.TotalCount);
            Assert.Equal(takeCount, result.Response.Count);
            Assert.Equal(paginationParams.Page, result.Response.Page);
            Assert.Equal(pricesToTake, result.Response.Data);
        }

        [Fact]
        public async Task GetInstrumentPricesPageAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPricesPageAsync(fixture.Create<int>(), fixture.Create<PaginationParams>(), fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetInstrumentPriceAsync_ReturnsPrice_WhenItExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var price = fixture.Create<InstrumentPriceDto>();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.Add(It.IsAny<Instrument>()))
                .Returns<Instrument>(i => i);
            instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Instrument(
                    id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<InstrumentType>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
                );
            instrumentRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentPriceAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(price);

            var sut = fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPriceAsync(price.InstrumentId, price.Time);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(price, result.Response);
        }

        [Fact]
        public async Task GetInstrumentPriceAsync_ReturnsNotFound_WhenPriceDoesntExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentPriceAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync((InstrumentPriceDto)null);

            var sut = fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPriceAsync(fixture.Create<int>(), fixture.Create<DateTime>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task AddingPricePoint_AddsPricePointToRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Instrument(
                    id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<InstrumentType>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
                );
            var priceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Time))
                .ReturnsAsync(false);
            priceRepository
                .Setup(r => r.Add(It.IsAny<InstrumentPrice>()))
                .Returns<InstrumentPrice>(p => p);

            var sut = fixture.Create<InstrumentPriceService>();

            await sut.AddPricePointAsync(priceDto);

            priceRepository.Verify(r => r.Add(It.Is<InstrumentPrice>(p =>
                p.InstrumentId == priceDto.InstrumentId &&
                p.Price == priceDto.Price &&
                Math.Abs(priceDto.Time.Ticks - p.Time.Ticks) <= TimeSpan.FromMinutes(1).Ticks
            )));
        }

        [Fact]
        public async Task AddingPricePoint_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId))
                .ReturnsAsync(false);
            instrumentRepository
                .Setup(r => r.FindAsync(priceDto.InstrumentId))
                .ReturnsAsync((Instrument)null);
            fixture.Freeze<Mock<IInstrumentPriceRepository>>();

            var sut = fixture.Create<InstrumentPriceService>();

            var response = await sut.AddPricePointAsync(priceDto);
            Assert.Equal(OperationStatus.NotFound, response.Status);
        }

        [Fact]
        public async Task AddingPricePoint_ReturnsError_WhenPriceAlreadyExistsAtTime()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Instrument(
                    id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<InstrumentType>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
                );
            var priceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Time))
                .ReturnsAsync(true);

            var sut = fixture.Create<InstrumentPriceService>();

            var response = await sut.AddPricePointAsync(priceDto);
            Assert.Equal(OperationStatus.Error, response.Status);
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

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Instrument(
                    id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<InstrumentType>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
                );
            var priceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
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
        public async Task DeletingPrice_ReturnsNotFound_WhenPriceDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var priceDto = fixture.Create<InstrumentPriceDto>();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Instrument(
                    id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<InstrumentType>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
                );
            var priceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            priceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync(false);
            priceRepository
                .Setup(r => r.FindPriceByIdAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync((InstrumentPrice)null);

            var sut = fixture.Create<InstrumentPriceService>();
            var response = await sut.DeletePricePointByIdAsync(priceDto.InstrumentId, priceDto.Id);

            Assert.Equal(OperationStatus.NotFound, response.Status);
        }
    }
}