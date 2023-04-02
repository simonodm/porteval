using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;

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
                await _positionService.UpdatePositionAsync(row);
            }
            else
            {
                var position = await _positionService.OpenPositionAsync(row);
                logEntry.Data.Id = position.Id;
            }

            return logEntry;
        }
    }
}
