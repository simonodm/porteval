using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Infrastructure;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests;

[Collection("Query test collection")]
public class PortfolioQueriesTests
{
    private readonly int _firstPortfolioId;
    private readonly IPortfolioQueries _portfolioQueries;
    private readonly int _secondPortfolioId;

    public PortfolioQueriesTests(QueryTestFixture fixture)
    {
        using var scope = fixture.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PortEvalDbContext>();
        _portfolioQueries = scope.ServiceProvider.GetRequiredService<IPortfolioQueries>();

        _firstPortfolioId = context.Portfolios.First(p => p.Name == "Portfolio 1").Id;
        _secondPortfolioId = context.Portfolios.First(p => p.Name == "Portfolio 2").Id;
    }

    [Fact]
    public async Task GetPortfolios_ReturnsAllPortfolios()
    {
        var queryResult = await _portfolioQueries.GetAllPortfoliosAsync();

        Assert.Collection(queryResult, AssertIsFirstPortfolio, AssertIsSecondPortfolio);
    }

    [Fact]
    public async Task GetPortfolio_ReturnsCorrectPortfolio_WhenPortfolioExists()
    {
        var queryResult = await _portfolioQueries.GetPortfolioAsync(_firstPortfolioId);

        AssertIsFirstPortfolio(queryResult);
    }

    [Fact]
    public async Task GetPortfolio_ReturnsNull_WhenPortfolioDoesNotExist()
    {
        var queryResult = await _portfolioQueries.GetPortfolioAsync(-1);

        Assert.Null(queryResult);
    }

    private void AssertIsFirstPortfolio(PortfolioDto p)
    {
        Assert.Equal(_firstPortfolioId, p.Id);
        Assert.Equal("Portfolio 1", p.Name);
        Assert.Equal("USD", p.CurrencyCode);
        Assert.Equal("Test note 1", p.Note);
    }

    private void AssertIsSecondPortfolio(PortfolioDto p)
    {
        Assert.Equal(_secondPortfolioId, p.Id);
        Assert.Equal("Portfolio 2", p.Name);
        Assert.Equal("EUR", p.CurrencyCode);
        Assert.Equal("Test note 2", p.Note);
    }
}