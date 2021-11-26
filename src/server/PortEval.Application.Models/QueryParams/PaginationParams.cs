using System;

namespace PortEval.Application.Models.QueryParams
{
    /// <summary>
    /// Represents pagination query parameters (page size and limit).
    /// </summary>
    public class PaginationParams
    {
        public int Limit
        {
            get => _limit;
            set => _limit = Math.Min(value, 300);
        }

        public int Page { get; set; } = 1;

        private int _limit = 10;
    }
}
