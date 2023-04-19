using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Infrastructure.Repositories;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests;

public class ExchangeRepositoryTests : RepositoryTestBase
{
    private readonly IExchangeRepository _exchangeRepository;

    public ExchangeRepositoryTests()
    {
        _exchangeRepository = new ExchangeRepository(DbContext);
    }

    [Fact]
    public async Task ListAllAsync_ReturnsAllExchanges()
    {
        var nasdaq = new Exchange("NASDAQ", "NASDAQ");
        var nyse = new Exchange("NYSE", "New York Stock Exchange");

        DbContext.Add(nasdaq);
        DbContext.Add(nyse);
        await DbContext.SaveChangesAsync();

        var exchanges = await _exchangeRepository.ListAllAsync();

        Assert.Collection(exchanges, exchange => AssertExchangesAreEqual(nasdaq, exchange),
            exchange => AssertExchangesAreEqual(nyse, exchange));
    }

    [Fact]
    public async Task FindAsync_ReturnsCorrectExchange()
    {
        var nasdaq = new Exchange("NASDAQ", "NASDAQ");
        DbContext.Add(nasdaq);
        await DbContext.SaveChangesAsync();

        var exchange = await _exchangeRepository.FindAsync("NASDAQ");

        AssertExchangesAreEqual(nasdaq, exchange);
    }

    [Fact]
    public async Task Add_CreatesNewExchange()
    {
        var nasdaq = new Exchange("NASDAQ", "NASDAQ");

        _exchangeRepository.Add(nasdaq);
        await _exchangeRepository.UnitOfWork.CommitAsync();

        var createdExchange = DbContext.Exchanges.FirstOrDefault();

        AssertExchangesAreEqual(nasdaq, createdExchange);
    }

    [Fact]
    public async Task Update_UpdatesExchange()
    {
        var nasdaq = new Exchange("NASDAQ", "NASDAQ");
        DbContext.Add(nasdaq);
        await DbContext.SaveChangesAsync();

        nasdaq.Rename("TEST");

        _exchangeRepository.Update(nasdaq);
        await _exchangeRepository.UnitOfWork.CommitAsync();

        var updatedExchange = DbContext.Exchanges.FirstOrDefault(n => n.Symbol == "NASDAQ");

        AssertExchangesAreEqual(nasdaq, updatedExchange);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenExchangeExists()
    {
        DbContext.Add(new Exchange("NASDAQ", "NASDAQ"));
        await DbContext.SaveChangesAsync();

        var exists = await _exchangeRepository.ExistsAsync("NASDAQ");

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenExchangeDoesNotExist()
    {
        var exists = await _exchangeRepository.ExistsAsync("NASDAQ");

        Assert.False(exists);
    }

    private void AssertExchangesAreEqual(Exchange expected, Exchange actual)
    {
        Assert.Equal(expected?.Symbol, actual?.Symbol);
        Assert.Equal(expected?.Name, actual?.Name);
    }
}