using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class InstrumentPriceServiceTests
    {
        private IFixture _fixture;
        private Mock<IInstrumentRepository> _instrumentRepository;
        private Mock<IInstrumentQueries> _instrumentQueries;
        private Mock<IInstrumentPriceRepository> _instrumentPriceRepository;

        public InstrumentPriceServiceTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
            _instrumentQueries = _fixture.CreateDefaultInstrumentQueriesMock();
            _instrumentPriceRepository = _fixture.CreateDefaultInstrumentPriceRepositoryMock();
        }

        [Fact]
        public async Task GetInstrumentPricesAsync_ReturnsAllInstrumentPricesInRange_WhenInstrumentExists()
        {
            var prices = _fixture.CreateMany<InstrumentPriceDto>();

            _instrumentQueries
                .Setup(m => m.GetInstrumentPricesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(prices);

            var sut = _fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPricesAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(prices, result.Response);
        }

        [Fact]
        public async Task GetInstrumentPricesAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            _instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((Instrument)null);
            _instrumentRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            _instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);
            _instrumentQueries
                .Setup(m => m.GetInstrumentPricesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((IEnumerable<InstrumentPriceDto>)null);

            var sut = _fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPricesAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetInstrumentPricesPageAsync_ReturnsInstrumentPricePage_WhenInstrumentExists()
        {
            var priceCount = 30;
            var takeCount = 15;
            var prices = _fixture.CreateMany<InstrumentPriceDto>(priceCount);
            var pricesToTake = prices.Take(15);
            var paginationParams = _fixture.Build<PaginationParams>()
                .With(p => p.Page, 2)
                .With(p => p.Limit, 15)
                .Create();

            _instrumentQueries
                .Setup(m => m.GetInstrumentPricesPageAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), paginationParams, It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(pricesToTake);
            _instrumentQueries
                .Setup(m => m.GetInstrumentPriceCountAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(priceCount);

            var sut = _fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPricesPageAsync(_fixture.Create<int>(), paginationParams, _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(priceCount, result.Response.TotalCount);
            Assert.Equal(takeCount, result.Response.Count);
            Assert.Equal(paginationParams.Page, result.Response.Page);
            Assert.Equal(pricesToTake, result.Response.Data);
        }

        [Fact]
        public async Task GetInstrumentPricesPageAsync_ReturnsInstrumentPricePage_WhenInstrumentExistsAndCompressedIsTrue()
        {
            var priceCount = 30;
            var takeCount = 15;
            var prices = _fixture.CreateMany<InstrumentPriceDto>(priceCount);
            var pricesToTake = prices.Take(15);
            var paginationParams = _fixture.Build<PaginationParams>()
                .With(p => p.Page, 2)
                .With(p => p.Limit, 15)
                .Create();

            _instrumentQueries
                .Setup(m => m.GetInstrumentPricesPageCompressedAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), paginationParams, It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(pricesToTake);
            _instrumentQueries
                .Setup(m => m.GetInstrumentPriceCompressedCountAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(priceCount);

            var sut = _fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPricesPageAsync(_fixture.Create<int>(), paginationParams, _fixture.Create<DateRangeParams>(), true);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(priceCount, result.Response.TotalCount);
            Assert.Equal(takeCount, result.Response.Count);
            Assert.Equal(paginationParams.Page, result.Response.Page);
            Assert.Equal(pricesToTake, result.Response.Data);
        }

        [Fact]
        public async Task GetInstrumentPricesPageAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            _instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);
            _instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((Instrument)null);
            _instrumentRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var sut = _fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPricesPageAsync(_fixture.Create<int>(), _fixture.Create<PaginationParams>(), _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetInstrumentPriceAsync_ReturnsPrice_WhenItExists()
        {
            var price = _fixture.Create<InstrumentPriceDto>();

            _instrumentQueries
                .Setup(m => m.GetInstrumentPriceAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(price);

            var sut = _fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPriceAsync(price.InstrumentId, price.Time);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(price, result.Response);
        }

        [Fact]
        public async Task GetInstrumentPriceAsync_ReturnsNotFound_WhenPriceDoesntExist()
        {
            _instrumentQueries
                .Setup(m => m.GetInstrumentPriceAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync((InstrumentPriceDto)null);

            var sut = _fixture.Create<InstrumentPriceService>();

            var result = await sut.GetInstrumentPriceAsync(_fixture.Create<int>(), _fixture.Create<DateTime>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task AddingPricePoint_AddsPricePointToRepository_WhenWellFormed()
        {
            var priceDto = _fixture.Create<InstrumentPriceDto>();

            _instrumentPriceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Time))
                .ReturnsAsync(false);
            _instrumentPriceRepository
                .Setup(r => r.Add(It.IsAny<InstrumentPrice>()))
                .Returns<InstrumentPrice>(p => p);

            var sut = _fixture.Create<InstrumentPriceService>();

            await sut.AddPricePointAsync(priceDto);

            _instrumentPriceRepository.Verify(r => r.Add(It.Is<InstrumentPrice>(p =>
                p.InstrumentId == priceDto.InstrumentId &&
                p.Price == priceDto.Price &&
                Math.Abs(priceDto.Time.Ticks - p.Time.Ticks) <= TimeSpan.FromMinutes(1).Ticks
            )));
        }

        [Fact]
        public async Task AddingPricePoint_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var priceDto = _fixture.Create<InstrumentPriceDto>();

            _instrumentRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId))
                .ReturnsAsync(false);
            _instrumentRepository
                .Setup(r => r.FindAsync(priceDto.InstrumentId))
                .ReturnsAsync((Instrument)null);

            var sut = _fixture.Create<InstrumentPriceService>();

            var response = await sut.AddPricePointAsync(priceDto);
            Assert.Equal(OperationStatus.NotFound, response.Status);
        }

        [Fact]
        public async Task AddingPricePoint_ReturnsError_WhenPriceAlreadyExistsAtTime()
        {
            var priceDto = _fixture.Create<InstrumentPriceDto>();

            var sut = _fixture.Create<InstrumentPriceService>();

            var response = await sut.AddPricePointAsync(priceDto);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task DeletingPrice_DeletesPriceFromRepository_WhenPriceExists()
        {
            var price = _fixture.Create<InstrumentPrice>();
            var priceDto = _fixture.Build<InstrumentPriceDto>()
                .With(p => p.Id, price.Id)
                .With(p => p.InstrumentId, price.InstrumentId)
                .With(p => p.Price, price.Price)
                .With(p => p.Time, price.Time)
                .Create();

            _instrumentPriceRepository
                .Setup(r => r.FindPriceByIdAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync(price);

            var sut = _fixture.Create<InstrumentPriceService>();

            await sut.DeletePricePointByIdAsync(priceDto.InstrumentId, priceDto.Id);

            _instrumentPriceRepository.Verify(r => r.DeleteAsync(priceDto.InstrumentId, priceDto.Id), Times.Once());
        }

        [Fact]
        public async Task DeletingPrice_ReturnsNotFound_WhenPriceDoesNotExist()
        {
            var priceDto = _fixture.Create<InstrumentPriceDto>();

            _instrumentPriceRepository
                .Setup(r => r.ExistsAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync(false);
            _instrumentPriceRepository
                .Setup(r => r.FindPriceByIdAsync(priceDto.InstrumentId, priceDto.Id))
                .ReturnsAsync((InstrumentPrice)null);

            var sut = _fixture.Create<InstrumentPriceService>();
            var response = await sut.DeletePricePointByIdAsync(priceDto.InstrumentId, priceDto.Id);

            Assert.Equal(OperationStatus.NotFound, response.Status);
        }
    }
}