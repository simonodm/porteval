using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models
{
    /// <summary>
    /// Represents a paginated response of multiple objects.
    /// </summary>
    /// <typeparam name="TPayload">Type of response's DTOs.</typeparam>
    [SwaggerSchema("Represents a paginated response of multiple objects.")]
    public class PaginatedResponse<TPayload>
    {
        [SwaggerSchema("Page number.")]
        public int Page { get; private set; }

        [SwaggerSchema("Amount of entities in the page.")]
        public int Count { get; private set; }

        [SwaggerSchema("Total count of entities available.")]
        public int TotalCount { get; private set; }

        [SwaggerSchema("Paginated data.")]
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
