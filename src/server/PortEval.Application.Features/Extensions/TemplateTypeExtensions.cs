using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Enums;
using System;

namespace PortEval.Application.Features.Extensions
{
    internal static class TemplateTypeExtensions
    {
        public static Type GetRowType(this TemplateType templateType)
        {
            switch (templateType)
            {
                case TemplateType.Portfolios:
                    return typeof(PortfolioDto);
                case TemplateType.Positions:
                    return typeof(PositionDto);
                case TemplateType.Instruments:
                    return typeof(InstrumentDto);
                case TemplateType.Prices:
                    return typeof(InstrumentPriceDto);
                case TemplateType.Transactions:
                    return typeof(TransactionDto);
                default:
                    throw new Exception($"Unknown template type: {templateType}");
            }
        }
    }
}
