using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
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

            var transactionService = fixture.Freeze<Mock<ITransactionService>>();
            transactionService
                .Setup(m => m.GetTransactionsAsync(filters, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(transactions));

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactions(filters, dateRange);

            transactionService.Verify(m => m.GetTransactionsAsync(filters, dateRange));
            Assert.Equal(transactions, result.Value);
        }


        [Fact]
        public async Task GetTransactions_ReturnsNotFound_WhenQueryReturnsNotFound()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var filters = fixture.Create<TransactionFilters>();
            var dateRange = fixture.Create<DateRangeParams>();

            var transactionService = fixture.Freeze<Mock<ITransactionService>>();
            transactionService
                .Setup(m => m.GetTransactionsAsync(filters, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<TransactionDto>>());

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactions(filters, dateRange);

            transactionService.Verify(m => m.GetTransactionsAsync(filters, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetTransaction_ReturnsTransaction_WhenTransactionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var transactionService = fixture.Freeze<Mock<ITransactionService>>();
            transactionService
                .Setup(m => m.GetTransactionAsync(transaction.Id))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(transaction));

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransaction(transaction.Id);

            transactionService.Verify(m => m.GetTransactionAsync(transaction.Id));
            Assert.Equal(transaction, result.Value);
        }

        [Fact]
        public async Task GetTransaction_ReturnsNotFound_TransactionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactionId = fixture.Create<int>();

            var transactionService = fixture.Freeze<Mock<ITransactionService>>();
            transactionService
                .Setup(m => m.GetTransactionAsync(transactionId))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<TransactionDto>());

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransaction(transactionId);

            transactionService.Verify(m => m.GetTransactionAsync(transactionId));
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
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(transaction));

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
            transactionService
                .Setup(m => m.UpdateTransactionAsync(transaction))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(transaction));

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
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteTransaction_DeletesTransaction()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactionId = fixture.Create<int>();

            var transactionService = fixture.Freeze<Mock<ITransactionService>>();
            transactionService
                .Setup(m => m.DeleteTransactionAsync(transactionId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse());

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            await sut.DeleteTransaction(transactionId);

            transactionService.Verify(m => m.DeleteTransactionAsync(transactionId));
        }
    }
}
