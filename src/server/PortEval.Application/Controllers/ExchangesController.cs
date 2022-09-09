using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Services.Queries.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExchangesController : ControllerBase
    {
        private readonly IInstrumentQueries _instrumentQueries;
        private readonly ILogger _logger;

        public ExchangesController(IInstrumentQueries instrumentQueries, ILoggerFactory loggerFactory)
        {
            _instrumentQueries = instrumentQueries;
            _logger = loggerFactory.CreateLogger(typeof(ExchangesController));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExchangeDto>>> GetKnownExchanges()
        {
            _logger.LogInformation("Known exchanges requested.");

            var result = await _instrumentQueries.GetKnownExchanges();

            return result.Response.ToList();
        }
    }
}
