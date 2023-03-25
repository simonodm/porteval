using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class TransactionsControllerTests
    {
        [Fact]
        public async Task GetTransactions_ReturnsTransactions_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var filters = fixture.Create<TransactionFilters>();
            var dateRange = fixture.Create<DateRangeParams>();
            var transactions = fixture.CreateMany<TransactionDto>();

            var transactionQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            transactionQueries
                .Setup(m => m.GetTransactions(filters, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(transactions));

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactions(filters, dateRange);

            transactionQueries.Verify(m => m.GetTransactions(filters, dateRange));
            Assert.Equal(transactions, result.Value);
        }


        [Fact]
        public async Task GetTransactions_ReturnsNotFound_WhenQueryReturnsNotFound()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var filters = fixture.Create<TransactionFilters>();
            var dateRange = fixture.Create<DateRangeParams>();

            var transactionQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            transactionQueries
                .Setup(m => m.GetTransactions(filters, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<TransactionDto>>());

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactions(filters, dateRange);

            transactionQueries.Verify(m => m.GetTransactions(filters, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetTransaction_ReturnsTransaction_WhenTransactionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var transactionQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            transactionQueries
                .Setup(m => m.GetTransaction(transaction.Id))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(transaction));

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransaction(transaction.Id);

            transactionQueries.Verify(m => m.GetTransaction(transaction.Id));
            Assert.Equal(transaction, result.Value);
        }

        [Fact]
        public async Task GetTransaction_ReturnsNotFound_TransactionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactionId = fixture.Create<int>();

            var transactionQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            transactionQueries
                .Setup(m => m.GetTransaction(transactionId))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<TransactionDto>());

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransaction(transactionId);

            transactionQueries.Verify(m => m.GetTransaction(transactionId));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostTransaction_CreatesTransaction()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var transactionService = fixture.Freeze<Mock<ITransactionService>>();
            transactionService
                .Setup(m => m.AddTransactionAsync(transaction))
                .ReturnsAsync(fixture.Create<Transaction>());

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            await sut.PostTransaction(transaction);

            transactionService.Verify(m => m.AddTransactionAsync(transaction));
        }

        [Fact]
        public async Task PutTransaction_UpdatesTransaction()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var transactionService = fixture.Freeze<Mock<ITransactionService>>();

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            await sut.PutTransaction(transaction.Id, transaction);

            transactionService.Verify(m => m.UpdateTransactionAsync(transaction));
        }

        [Fact]
        public async Task PutTransaction_ReturnsBadRequest_WhenQueryParamIdDoesNotMatchBodyId()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var transactionService = fixture.Freeze<Mock<ITransactionService>>();

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.PutTransaction(transaction.Id + 1, transaction);

            transactionService.Verify(m => m.UpdateTransactionAsync(transaction), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTransaction_DeletesTransaction()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactionId = fixture.Create<int>();

            var transactionService = fixture.Freeze<Mock<ITransactionService>>();

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            await sut.DeleteTransaction(transactionId);

            transactionService.Verify(m => m.DeleteTransactionAsync(transactionId));
        }
    }
}
