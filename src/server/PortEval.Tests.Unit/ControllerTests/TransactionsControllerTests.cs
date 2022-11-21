using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class TransactionsControllerTests
    {
        [Fact]
        public async Task GetTransactions_ReturnsTransactionsFromQueries_WhenQueryReturnsData()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var filters = fixture.Create<TransactionFilters>();
            var dateRange = fixture.Create<DateRangeParams>();
            var transactions = fixture.CreateMany<TransactionDto>();

            var instrumentQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            instrumentQueries
                .Setup(m => m.GetTransactions(filters, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(transactions));

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactions(filters, dateRange);

            instrumentQueries.Verify(m => m.GetTransactions(filters, dateRange));
            Assert.Equal(transactions, result.Value);
        }


        [Fact]
        public async Task GetTransactions_ReturnsNotFound_WhenQueryReturnsNotFound()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var filters = fixture.Create<TransactionFilters>();
            var dateRange = fixture.Create<DateRangeParams>();

            var instrumentQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            instrumentQueries
                .Setup(m => m.GetTransactions(filters, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<TransactionDto>>());

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactions(filters, dateRange);

            instrumentQueries.Verify(m => m.GetTransactions(filters, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetTransaction_ReturnsTransactionFromQueries_WhenQueryReturnsData()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            var transaction = fixture.Create<TransactionDto>();

            var instrumentQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            instrumentQueries
                .Setup(m => m.GetTransaction(transaction.Id))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(transaction));

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransaction(transaction.Id);

            instrumentQueries.Verify(m => m.GetTransaction(transaction.Id));
            Assert.Equal(transaction, result.Value);
        }

        [Fact]
        public async Task GetTransaction_ReturnsNotFound_WhenQueryReturnsNotFound()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactionId = fixture.Create<int>();

            var instrumentQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            instrumentQueries
                .Setup(m => m.GetTransaction(transactionId))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<TransactionDto>());

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.GetTransaction(transactionId);

            instrumentQueries.Verify(m => m.GetTransaction(transactionId));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostTransaction_CreatesTransactionUsingTransactionService()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var instrumentTransactionService = fixture.Freeze<Mock<ITransactionService>>();
            instrumentTransactionService
                .Setup(m => m.AddTransactionAsync(transaction))
                .ReturnsAsync(fixture.Create<Transaction>());

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            await sut.PostTransaction(transaction);

            instrumentTransactionService.Verify(m => m.AddTransactionAsync(transaction));
        }

        [Fact]
        public async Task PutTransaction_UpdatesTransactionUsingTransactionService()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var instrumentTransactionService = fixture.Freeze<Mock<ITransactionService>>();

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            await sut.PutTransaction(transaction.Id, transaction);

            instrumentTransactionService.Verify(m => m.UpdateTransactionAsync(transaction));
        }

        [Fact]
        public async Task PutTransaction_ReturnsBadRequest_WhenQueryParamIdDoesNotMatchBodyId()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var instrumentTransactionService = fixture.Freeze<Mock<ITransactionService>>();

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            var result = await sut.PutTransaction(transaction.Id + 1, transaction);

            instrumentTransactionService.Verify(m => m.UpdateTransactionAsync(transaction), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteTransaction_DeletesTransactionUsingTransactionService()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactionId = fixture.Create<int>();

            var instrumentTransactionService = fixture.Freeze<Mock<ITransactionService>>();

            var sut = fixture.Build<TransactionsController>().OmitAutoProperties().Create();

            await sut.DeleteTransaction(transactionId);

            instrumentTransactionService.Verify(m => m.DeleteTransactionAsync(transactionId));
        }
    }
}
