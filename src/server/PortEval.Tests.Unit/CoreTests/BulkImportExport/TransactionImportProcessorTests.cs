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
    public class TransactionImportProcessorTests
    {
        private IFixture _fixture;
        private Mock<ITransactionService> _transactionService;

        public TransactionImportProcessorTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _transactionService = _fixture.Freeze<Mock<ITransactionService>>();
        }

        [Fact]
        public async Task ProcessingImport_CreatesNewTransaction_WhenNoIdIsPresent()
        {
            var transaction = _fixture.Build<TransactionDto>()
                .With(t => t.Id, 0)
                .With(t => t.Time, DateTime.UtcNow)
                .Create();
            var sut = _fixture.Create<TransactionImportProcessor>();

            await sut.ImportRecordsAsync(new List<TransactionDto> { transaction });

            _transactionService.Verify(s => s.AddTransactionAsync(It.Is<TransactionDto>(t =>
                t.Id == default &&
                t.PortfolioId == transaction.PortfolioId &&
                t.PositionId == transaction.PositionId &&
                t.Amount == transaction.Amount &&
                t.Time == transaction.Time &&
                t.Price == transaction.Price &&
                t.Note == transaction.Note
            )), Times.Once());
        }

        [Fact]
        public async Task ProcessingImport_UpdatesTransaction_WhenIdIsPresent()
        {
            var transaction = _fixture.Build<TransactionDto>()
                .With(t => t.Time, DateTime.UtcNow)
                .Create();
            var sut = _fixture.Create<TransactionImportProcessor>();

            await sut.ImportRecordsAsync(new List<TransactionDto> { transaction });

            _transactionService.Verify(s => s.UpdateTransactionAsync(It.Is<TransactionDto>(t =>
                t.Id == transaction.Id &&
                t.PortfolioId == transaction.PortfolioId &&
                t.PositionId == transaction.PositionId &&
                t.Amount == transaction.Amount &&
                t.Time == transaction.Time &&
                t.Price == transaction.Price &&
                t.Note == transaction.Note
            )), Times.Once());
        }

        [Fact]
        public async Task ProcessingImport_DoesNothing_WhenTransactionFailsValidation()
        {
            var transaction = _fixture.Build<TransactionDto>()
                .With(t => t.Amount, 0)
                .With(t => t.Time, DateTime.UtcNow)
                .Create();
            var sut = _fixture.Create<TransactionImportProcessor>();

            await sut.ImportRecordsAsync(new List<TransactionDto> { transaction });

            _transactionService.Verify(s => s.AddTransactionAsync(It.IsAny<TransactionDto>()), Times.Never());
            _transactionService.Verify(s => s.UpdateTransactionAsync(It.IsAny<TransactionDto>()), Times.Never());
        }
    }
}