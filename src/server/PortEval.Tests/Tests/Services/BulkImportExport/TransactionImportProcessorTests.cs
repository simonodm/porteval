﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.BulkImportExport;
using PortEval.Tests.Extensions;
using Xunit;

namespace PortEval.Tests.Tests.Services.BulkImportExport
{
    public class TransactionImportProcessorTests
    {
        [Fact]
        public async Task ProcessingImport_CreatesNewTransaction_WhenNoIdIsPresent()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Build<TransactionDto>()
                .With(t => t.Id, 0)
                .With(t => t.Time, DateTime.UtcNow)
                .Create();
            var transactionService = fixture.CreateDefaultTransactionServiceMock();
            var sut = fixture.Create<TransactionImportProcessor>();

            await sut.ImportRecords(new List<TransactionDto> { transaction });

            transactionService.Verify(s => s.AddTransactionAsync(It.Is<TransactionDto>(t =>
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
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Build<TransactionDto>()
                .With(t => t.Time, DateTime.UtcNow)
                .Create();
            var transactionService = fixture.CreateDefaultTransactionServiceMock();
            var sut = fixture.Create<TransactionImportProcessor>();

            await sut.ImportRecords(new List<TransactionDto> { transaction });

            transactionService.Verify(s => s.UpdateTransactionAsync(It.Is<TransactionDto>(t =>
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
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Build<TransactionDto>()
                .With(t => t.Amount, 0)
                .With(t => t.Time, DateTime.UtcNow)
                .Create();
            var transactionService = fixture.CreateDefaultTransactionServiceMock();
            var sut = fixture.Create<TransactionImportProcessor>();

            await sut.ImportRecords(new List<TransactionDto> { transaction });

            transactionService.Verify(s => s.AddTransactionAsync(It.IsAny<TransactionDto>()), Times.Never());
            transactionService.Verify(s => s.UpdateTransactionAsync(It.IsAny<TransactionDto>()), Times.Never());
        }
    }
}