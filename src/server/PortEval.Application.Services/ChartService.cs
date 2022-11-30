using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services
{
    /// <inheritdoc cref="IChartService" />
    public class ChartService : IChartService
    {
        private readonly IChartRepository _chartRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public ChartService(IChartRepository chartRepository, ICurrencyRepository currencyRepository, IPositionRepository positionRepository,
            IPortfolioRepository portfolioRepository, IInstrumentRepository instrumentRepository)
        {
            _chartRepository = chartRepository;
            _currencyRepository = currencyRepository;
            _positionRepository = positionRepository;
            _portfolioRepository = portfolioRepository;
            _instrumentRepository = instrumentRepository;
        }

        /// <inheritdoc cref="IChartService.CreateChartAsync" />
        public async Task<Chart> CreateChartAsync(ChartDto options)
        {
            await ValidateCurrencyCode(options);
            ValidateFrequency(options);

            var dateRange = GenerateDateRange(options);
            var typeConfig = GenerateTypeConfiguration(options);

            var chart = new Chart(options.Name, dateRange, typeConfig);
            chart.ReplaceLines(await ConvertLineDtosToLineEntities(chart, options.Lines));
            _chartRepository.Add(chart);

            await _chartRepository.UnitOfWork.CommitAsync();
            return chart;
        }

        /// <inheritdoc cref="IChartService.UpdateChartAsync" />
        public async Task<Chart> UpdateChartAsync(ChartDto options)
        {
            var chart = await _chartRepository.FindAsync(options.Id);
            if (chart == null)
            {
                throw new ItemNotFoundException($"Chart {options.Id} does not exist.");
            }

            await ValidateCurrencyCode(options);
            ValidateFrequency(options);

            var dateRange = GenerateDateRange(options);
            var typeConfig = GenerateTypeConfiguration(options);

            chart.Rename(options.Name);
            chart.SetDateRange(dateRange);
            chart.SetConfiguration(typeConfig);
            chart.ReplaceLines(await ConvertLineDtosToLineEntities(chart, options.Lines));
            chart.IncreaseVersion();

            _chartRepository.Update(chart);

            await _chartRepository.UnitOfWork.CommitAsync();
            return chart;
        }

        /// <inheritdoc cref="IChartService.DeleteChartAsync" />
        public async Task DeleteChartAsync(int id)
        {
            if (!(await _chartRepository.ExistsAsync(id)))
            {
                throw new ItemNotFoundException($"Chart {id} does not exist.");
            }

            await _chartRepository.DeleteAsync(id);
            await _chartRepository.UnitOfWork.CommitAsync();
        }

        /// <summary>
        /// Checks if currency code is required for the given chart type and if so, checks whether an existing currency was supplied in the DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <exception cref="ItemNotFoundException">Thrown if no currency with the supplied code exists.</exception>
        /// <returns>A task representing the asynchronous validation operation.</returns>
        private async Task ValidateCurrencyCode(ChartDto options)
        {
            if (options.Type == ChartType.Price || options.Type == ChartType.Profit ||
                options.Type == ChartType.AggregatedProfit)
            {
                if (!(await _currencyRepository.ExistsAsync(options.CurrencyCode)))
                {
                    throw new ItemNotFoundException($"Currency {options.CurrencyCode} not found.");
                }
            }
        }

        /// <summary>
        /// Checks if frequency is required for the given chart type and if so, checks whether frequency was supplied in the DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <exception cref="OperationNotAllowedException">Thrown if no frequency is set in the DTO.</exception>
        private void ValidateFrequency(ChartDto options)
        {
            if (options.Type != ChartType.AggregatedPerformance && options.Type != ChartType.AggregatedProfit) return;
            if (options.Frequency == null)
            {
                throw new OperationNotAllowedException("Chart frequency must be set for aggregated charts.");
            }
        }

        /// <summary>
        /// Generates the chart's date range configuration based on the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <exception cref="OperationNotAllowedException">Thrown if invalid date range parameters are supplied.</exception>
        /// <returns>A <see cref="ChartDateRange">ChartDateRange</see> instance containing the supplied configuration.</returns>
        private ChartDateRange GenerateDateRange(ChartDto options)
        {
            if (options.IsToDate != null && (bool)options.IsToDate)
            {
                if (options.ToDateRange != null) return new ChartDateRange(options.ToDateRange);
                throw new OperationNotAllowedException(
                    $"{nameof(options.ToDateRange)} cannot be null when {nameof(options.IsToDate)} is true.");
            }

            if (options.DateRangeStart == null || options.DateRangeEnd == null)
            {
                throw new OperationNotAllowedException(
                    $"{nameof(options.DateRangeStart)} and {nameof(options.DateRangeEnd)} must be set when {nameof(options.IsToDate)} is not set to true.");
            }

            return new ChartDateRange((DateTime)options.DateRangeStart, (DateTime)options.DateRangeEnd);
        }

        /// <summary>
        /// Generates the chart's type configuration based on the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <exception cref="OperationNotAllowedException">Thrown if an unknown type was supplied in the DTO.</exception>
        /// <returns>A <see cref="ChartTypeSettings">ChartTypeSettings</see> instance containing the supplied configuration.</returns>
        private ChartTypeSettings GenerateTypeConfiguration(ChartDto options)
        {
            var chartTypeConfig = options.Type switch
            {
                ChartType.Price => ChartTypeSettings.PriceChart(options.CurrencyCode),
                ChartType.Profit => ChartTypeSettings.ProfitChart(options.CurrencyCode),
                ChartType.Performance => ChartTypeSettings.PerformanceChart(),
                ChartType.AggregatedProfit => ChartTypeSettings.AggregatedProfitChart(options.CurrencyCode,
                    (AggregationFrequency)options.Frequency),
                ChartType.AggregatedPerformance => ChartTypeSettings.AggregatedPerformanceChart(
                    (AggregationFrequency)options.Frequency),
                _ => throw new OperationNotAllowedException($"Unrecognized chart type: {options.Type}.")
            };

            return chartTypeConfig;
        }

        /// <summary>
        /// Converts chart line DTOs to chart line entities.
        /// </summary>
        /// <param name="chart">Target chart.</param>
        /// <param name="lineDtos">Chart line DTOs.</param>
        /// <returns>A task representing the asynchronous conversion operation. Task result contains converted chart line entities.</returns>
        private async Task<IEnumerable<ChartLine>> ConvertLineDtosToLineEntities(Chart chart, IEnumerable<ChartLineDto> lineDtos)
        {
            var lineEntities = new List<ChartLine>();
            foreach (var dto in lineDtos)
            {
                var generatedEntity = await GenerateLineAsync(chart, dto);
                lineEntities.Add(generatedEntity);
            }

            return lineEntities;
        }

        /// <summary>
        /// Generates a chart line entity from a chart line DTO.
        /// </summary>
        /// <param name="chart">Chart to create the line for.</param>
        /// <param name="lineDto">Chart line DTO.</param>
        /// <exception cref="OperationNotAllowedException">Thrown if an unrecognized chart line type was supplied in <c>lineDto</c></exception>
        /// <returns>A task representing the asynchronous creation operation. Task result contains the newly created chart line entity.</returns>
        private async Task<ChartLine> GenerateLineAsync(Chart chart, ChartLineDto lineDto)
        {
            return lineDto.Type switch
            {
                ChartLineType.Portfolio => await GeneratePortfolioLineAsync(chart, lineDto),
                ChartLineType.Position => await GeneratePositionLineAsync(chart, lineDto),
                ChartLineType.Instrument => await GenerateInstrumentLineAsync(chart, lineDto),
                _ => throw new OperationNotAllowedException($"Unrecognized chart line type: {lineDto.Type}")
            };
        }

        /// <summary>
        /// Creates a new portfolio chart line.
        /// </summary>
        /// <param name="chart">Chart to create the line for.</param>
        /// <param name="lineDto">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains newly created chart line entity.</returns>
        private async Task<ChartLinePortfolio> GeneratePortfolioLineAsync(Chart chart, ChartLineDto lineDto)
        {
            if (lineDto.PortfolioId == null)
            {
                throw new OperationNotAllowedException($"{nameof(lineDto.PortfolioId)} cannot be null.");
            }

            int portfolioId = (int)lineDto.PortfolioId;

            if (!(await _portfolioRepository.ExistsAsync(portfolioId)))
            {
                throw new ItemNotFoundException($"Portfolio {portfolioId} does not exist.");
            }

            return new ChartLinePortfolio(chart.Id, lineDto.Width, lineDto.Dash, lineDto.Color, portfolioId);
        }

        /// <summary>
        /// Creates a new position chart line.
        /// </summary>
        /// <param name="chart">Chart to create the line for.</param>
        /// <param name="lineDto">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains newly created chart line entity.</returns>
        private async Task<ChartLine> GeneratePositionLineAsync(Chart chart, ChartLineDto lineDto)
        {
            if (lineDto.PositionId == null)
            {
                throw new OperationNotAllowedException("Position id must be set to create a position chart line.");
            }

            int positionId = (int)lineDto.PositionId;

            var position = await _positionRepository.FindAsync(positionId);
            if (position == null)
            {
                throw new ItemNotFoundException($"Position {positionId} does not exist.");
            }

            return new ChartLinePosition(chart.Id, lineDto.Width, lineDto.Dash, lineDto.Color, positionId);
        }

        /// <summary>
        /// Creates a new instrument chart line.
        /// </summary>
        /// <param name="chart">Chart to create the line for.</param>
        /// <param name="lineDto">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains newly created chart line entity.</returns>
        private async Task<ChartLine> GenerateInstrumentLineAsync(Chart chart, ChartLineDto lineDto)
        {
            if (lineDto.InstrumentId == null)
            {
                throw new OperationNotAllowedException("Instrument id must be set to create an instrument chart line.");
            }

            int instrumentId = (int)lineDto.InstrumentId;

            if (!(await _instrumentRepository.ExistsAsync(instrumentId)))
            {
                throw new ItemNotFoundException($"Instrument {instrumentId} does not exist.");
            }

            return new ChartLineInstrument(chart.Id, lineDto.Width, lineDto.Dash, lineDto.Color, instrumentId);
        }
    }
}
