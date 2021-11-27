using System;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.QueryParams
{
    /// <summary>
    /// Represents pagination query parameters (page size and limit).
    /// </summary>
    [SwaggerSchema("Configures pagination of the response.")]
    public class PaginationParams
    {
        [SwaggerSchema("Determines how many entities are provided in the page. Maximum value is 300.")]
        public int Limit
        {
            get => _limit;
            set => _limit = Math.Min(value, 300);
        }

        [SwaggerSchema("Page number.")]
        public int Page { get; set; } = 1;

        private int _limit = 10;
    }
}
