using PortEval.Application.Models.QueryParams;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    public abstract class ChartPointGeneratorBase : IDisposable
    {
        private readonly IEnumerator<DateRangeParams> _rangeEnumerator;
        private DateRangeParams _currentRange;

        protected ChartPointGeneratorBase(IEnumerable<DateRangeParams> ranges)
        {
            _rangeEnumerator = ranges.GetEnumerator();
            _rangeEnumerator.MoveNext();
        }

        public DateTime? GetNextChartPointTime()
        {
            if (_rangeEnumerator.Current == null)
            {
                if (_currentRange != null)
                {
                    var lastRange = _currentRange;
                    _currentRange = null;
                    return lastRange.To;
                }

                return null;
            }

            _currentRange = _rangeEnumerator.Current;
            _rangeEnumerator.MoveNext();
            return _currentRange.From;
        }

        public virtual void Dispose()
        {
            _rangeEnumerator?.Dispose();
        }
    }
}
