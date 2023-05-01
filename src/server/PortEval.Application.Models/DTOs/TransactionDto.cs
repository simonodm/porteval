using System;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents a transaction of a given position.")]
public class TransactionDto
{
    [SwaggerSchema("Transaction identifier.", ReadOnly = true)]
    public int Id { get; set; }

    [SwaggerSchema("Identifier of the position to which this transaction belongs.")]
    public int PositionId { get; set; }

    [SwaggerSchema("Identifier of the portfolio to which this transaction's position belongs.")]
    public int PortfolioId { get; set; }

    [SwaggerSchema("Parent instrument of the transaction.")]
    public InstrumentDto Instrument { get; set; }

    [SwaggerSchema("Time of transaction.")]
    public DateTime Time { get; set; }

    [SwaggerSchema(
        "Number of instruments purchased/sold. Positive value represents a purchase, negative value represents a sale.")]
    public decimal Amount { get; set; }

    [SwaggerSchema("Value at which the instruments were purchased/sold.")]
    public decimal Price { get; set; }

    [SwaggerSchema("Transaction note.")]
    public string Note { get; set; }
}