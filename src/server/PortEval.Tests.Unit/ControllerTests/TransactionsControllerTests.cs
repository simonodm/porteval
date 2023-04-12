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
        private IFixture _fixture;
        private Mock<ITransactionService> _transactionService;

        public TransactionsControllerTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _transactionService = _fixture.Freeze<Mock<ITransactionService>>();
        }

        [Fact]
        public async Task GetTransactions_ReturnsTransactions_WhenPositionExists()
        {
            var filters = _fixture.Create<TransactionFilters>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var transactions = _fixture.CreateMany<TransactionDto>();

            _transactionService
                .Setup(m => m.GetTransactionsAsync(filters, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(transactions));

            var sut = _fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactions(filters, dateRange);

            _transactionService.Verify(m => m.GetTransactionsAsync(filters, dateRange));
            Assert.Equal(transactions, result.Value);
        }


        [Fact]
        public async Task GetTransactions_ReturnsNotFound_WhenQueryReturnsNotFound()
        {
            var filters = _fixture.Create<TransactionFilters>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _transactionService
                .Setup(m => m.GetTransactionsAsync(filters, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<TransactionDto>>());

            var sut = _fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactions(filters, dateRange);

            _transactionService.Verify(m => m.GetTransactionsAsync(filters, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetTransaction_ReturnsTransaction_WhenTransactionExists()
        {
            var transaction = _fixture.Create<TransactionDto>();

            _transactionService
                .Setup(m => m.GetTransactionAsync(transaction.Id))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(transaction));

            var sut = _fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransaction(transaction.Id);

            _transactionService.Verify(m => m.GetTransactionAsync(transaction.Id));
            Assert.Equal(transaction, result.Value);
        }

        [Fact]
        public async Task GetTransaction_ReturnsNotFound_TransactionDoesNotExist()
        {
            var transactionId = _fixture.Create<int>();

            _transactionService
                .Setup(m => m.GetTransactionAsync(transactionId))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<TransactionDto>());

            var sut = _fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransaction(transactionId);

            _transactionService.Verify(m => m.GetTransactionAsync(transactionId));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostTransaction_CreatesTransaction()
        {
            var transaction = _fixture.Create<TransactionDto>();

            _transactionService
                .Setup(m => m.AddTransactionAsync(transaction))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(transaction));

            var sut = _fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            await sut.PostTransaction(transaction);

            _transactionService.Verify(m => m.AddTransactionAsync(transaction));
        }

        [Fact]
        public async Task PutTransaction_UpdatesTransaction()
        {
            var transaction = _fixture.Create<TransactionDto>();

            _transactionService
                .Setup(m => m.UpdateTransactionAsync(transaction))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(transaction));

            var sut = _fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            await sut.PutTransaction(transaction.Id, transaction);

            _transactionService.Verify(m => m.UpdateTransactionAsync(transaction));
        }

        [Fact]
        public async Task PutTransaction_ReturnsBadRequest_WhenQueryParamIdDoesNotMatchBodyId()
        {
            var transaction = _fixture.Create<TransactionDto>();

            var sut = _fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.PutTransaction(transaction.Id + 1, transaction);

            _transactionService.Verify(m => m.UpdateTransactionAsync(transaction), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteTransaction_DeletesTransaction()
        {
            var transactionId = _fixture.Create<int>();

            _transactionService
                .Setup(m => m.DeleteTransactionAsync(transactionId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse());

            var sut = _fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            await sut.DeleteTransaction(transactionId);

            _transactionService.Verify(m => m.DeleteTransactionAsync(transactionId));
        }
    }
}
