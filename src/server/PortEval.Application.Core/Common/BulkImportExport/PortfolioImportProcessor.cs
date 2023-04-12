﻿using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Common.BulkImportExport
{
    public class PortfolioImportProcessor : ImportProcessor<PortfolioDto, PortfolioDtoValidator>
    {
        private readonly IPortfolioService _portfolioService;

        public PortfolioImportProcessor(IPortfolioService portfolioService) : base()
        {
            _portfolioService = portfolioService;
        }

        protected override async Task<ProcessedRowErrorLogEntry<PortfolioDto>> ProcessItem(PortfolioDto row)
        {
            var logEntry = new ProcessedRowErrorLogEntry<PortfolioDto>(row);

            if (row.Id == default)
            {
                var response = await _portfolioService.CreatePortfolioAsync(row);
                if (response.Status != OperationStatus.Ok)
                {
                    logEntry.AddError(response.Message);
                }
                logEntry.Data.Id = response.Response?.Id ?? default;
            }
            else
            {
                var response = await _portfolioService.UpdatePortfolioAsync(row);
                if (response.Status != OperationStatus.Ok)
                {
                    logEntry.AddError(response.Message);
                }
            }

            return logEntry;
        }
    }
}