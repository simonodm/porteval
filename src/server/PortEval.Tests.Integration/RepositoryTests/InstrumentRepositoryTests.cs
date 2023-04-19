using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure.Repositories;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests;

public class InstrumentRepositoryTests : RepositoryTestBase
{
    private readonly IInstrumentRepository _instrumentRepository;

    public InstrumentRepositoryTests()
    {
        _instrumentRepository = new InstrumentRepository(DbContext);

        DbContext.Add(new Currency("USD", "US Dollar", "$", true));
        DbContext.Add(new Exchange("NASDAQ", "NASDAQ"));
        DbContext.SaveChanges();
    }

    [Fact]
    public async Task ListAllAsync_ReturnsAllInstruments()
    {
        var first = new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        var second = new Instrument("Bitcoin USD", "BTCUSD", null, InstrumentType.CryptoCurrency, "USD", "");

        DbContext.Add(first);
        DbContext.Add(second);
        await DbContext.SaveChangesAsync();

        var instruments = await _instrumentRepository.ListAllAsync();

        Assert.Collection(instruments, instrument => AssertInstrumentsAreEqual(first, instrument),
            instrument => AssertInstrumentsAreEqual(second, instrument));
    }

    [Fact]
    public async Task FindAsync_ReturnsCorrectInstrument()
    {
        var instrument = new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        DbContext.Add(instrument);
        await DbContext.SaveChangesAsync();

        var foundInstrument = await _instrumentRepository.FindAsync(instrument.Id);

        AssertInstrumentsAreEqual(instrument, foundInstrument);
    }

    [Fact]
    public async Task Add_CreatesNewInstrument()
    {
        var instrument = new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

        _instrumentRepository.Add(instrument);
        await _instrumentRepository.UnitOfWork.CommitAsync();

        var createdInstrument = DbContext.Instruments.FirstOrDefault();

        AssertInstrumentsAreEqual(instrument, createdInstrument);
    }

    [Fact]
    public async Task Update_UpdatesInstrument()
    {
        var instrument = new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        DbContext.Add(instrument);
        await DbContext.SaveChangesAsync();

        instrument.Rename("TEST");
        instrument.SetNote("TEST");

        _instrumentRepository.Update(instrument);
        await _instrumentRepository.UnitOfWork.CommitAsync();

        var updatedInstrument = DbContext.Instruments.FirstOrDefault();

        AssertInstrumentsAreEqual(instrument, updatedInstrument);
    }

    [Fact]
    public async Task DeleteAsync_DeletesInstrument()
    {
        var instrument = new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        DbContext.Add(instrument);
        await DbContext.SaveChangesAsync();

        await _instrumentRepository.DeleteAsync(instrument.Id);
        await _instrumentRepository.UnitOfWork.CommitAsync();

        var instrumentDeleted = !DbContext.Instruments.Any();

        Assert.True(instrumentDeleted);
    }

    [Fact]
    public async Task Delete_DeletesInstrument()
    {
        var instrument = new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        DbContext.Add(instrument);
        await DbContext.SaveChangesAsync();

        _instrumentRepository.Delete(instrument);
        await _instrumentRepository.UnitOfWork.CommitAsync();

        var instrumentDeleted = !DbContext.Instruments.Any();

        Assert.True(instrumentDeleted);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenInstrumentExists()
    {
        var instrument = new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        DbContext.Instruments.Add(instrument);
        await DbContext.SaveChangesAsync();

        var exists = await _instrumentRepository.ExistsAsync(instrument.Id);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenInstrumentDoesNotExist()
    {
        var exists = await _instrumentRepository.ExistsAsync(1);

        Assert.False(exists);
    }

    private void AssertInstrumentsAreEqual(Instrument expected, Instrument actual)
    {
        Assert.Equal(expected?.Symbol, actual?.Symbol);
        Assert.Equal(expected?.Name, actual?.Name);
        Assert.Equal(expected?.CurrencyCode, actual?.CurrencyCode);
        Assert.Equal(expected?.Exchange, actual?.Exchange);
        Assert.Equal(expected?.Type, actual?.Type);
        Assert.Equal(expected?.Note, actual?.Note);
    }
}