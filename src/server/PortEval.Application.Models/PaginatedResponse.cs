using System.Collections.Generic;

namespace PortEval.Application.Models
{
    /// <summary>
    /// Represents a paginated response of multiple objects.
    /// </summary>
    /// <typeparam name="TPayload">Type of response's DTOs.</typeparam>
    public class PaginatedResponse<TPayload>
    {
        public int Page { get; private set; }
        public int Count { get; private set; }
        public int TotalCount { get; private set; }
        public IEnumerable<TPayload> Data { get; private set; }

        public PaginatedResponse(int page, int count, int totalCount, IEnumerable<TPayload> data)
        {
            Page = page;
            Count = count;
            TotalCount = totalCount;
            Data = data;
        }
    }
}
