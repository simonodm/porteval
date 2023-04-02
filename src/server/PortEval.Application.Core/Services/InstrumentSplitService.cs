using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Application.Core.Services
{
    /// <inheritdoc cref="IInstrumentSplitService" />
    public class InstrumentSplitService : IInstrumentSplitService
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentSplitRepository _splitRepository;

        public InstrumentSplitService(IInstrumentRepository instrumentRepository,
            IInstrumentSplitRepository splitRepository)
        {
            _instrumentRepository = instrumentRepository;
            _splitRepository = splitRepository;
        }

        /// <inheritdoc />
        public async Task<InstrumentSplit> CreateSplitAsync(InstrumentSplitDto options)
        {
            var instrument = await _instrumentRepository.FindAsync(options.InstrumentId);
            if (instrument == null)
            {
                throw new ItemNotFoundException($"Instrument {options.InstrumentId} does not exist.");
            }

            var ratio = new SplitRatio(options.SplitRatioDenominator, options.SplitRatioNumerator);
            var newSplit = InstrumentSplit.Create(instrument, options.Time, ratio);

            _splitRepository.Add(newSplit);
            await _splitRepository.UnitOfWork.CommitAsync();

            return newSplit;
        }

        /// <inheritdoc />
        public async Task<InstrumentSplit> UpdateSplitAsync(int instrumentId, InstrumentSplitDto options)
        {
            var split = await _splitRepository.FindAsync(options.Id);
            if (split == null)
            {
                throw new ItemNotFoundException($"Split {options.Id} does not exist.");
            }

            if (split.InstrumentId != instrumentId)
            {
                throw new OperationNotAllowedException(
                    $"Split {options.Id} does not belong to instrument {instrumentId}.");
            }

            if (split.ProcessingStatus == InstrumentSplitProcessingStatus.Processed &&
                options.Status == InstrumentSplitProcessingStatus.RollbackRequested)
            {
                split.Rollback();
                split.IncreaseVersion();
                _splitRepository.Update(split);
                await _splitRepository.UnitOfWork.CommitAsync();
            }

            return split;
        }
    }
}
