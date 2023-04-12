﻿using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Common.BulkImportExport
{
    public class PositionImportProcessor : ImportProcessor<PositionDto, PositionDtoValidator>
    {
        private readonly IPositionService _positionService;

        public PositionImportProcessor(IPositionService positionService) : base()
        {
            _positionService = positionService;
        }

        protected override async Task<ProcessedRowErrorLogEntry<PositionDto>> ProcessItem(PositionDto row)
        {
            var logEntry = new ProcessedRowErrorLogEntry<PositionDto>(row);

            if (row.Id != default)
            {
                var response = await _positionService.UpdatePositionAsync(row);
                if (response.Status != OperationStatus.Ok)
                {
                    logEntry.AddError(response.Message);
                }
            }
            else
            {
                var response = await _positionService.OpenPositionAsync(row);
                if (response.Status != OperationStatus.Ok)
                {
                    logEntry.AddError(response.Message);
                }

                logEntry.Data.Id = response.Response?.Id ?? default;
            }

            return logEntry;
        }
    }
}
