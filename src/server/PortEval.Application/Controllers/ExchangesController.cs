using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExchangesController : ControllerBase
    {
        private readonly IExchangeQueries _exchangeQueries;

        public ExchangesController(IExchangeQueries exchangeQueries)
        {
            _exchangeQueries = exchangeQueries;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExchangeDto>>> GetKnownExchanges()
        {
            var result = await _exchangeQueries.GetKnownExchanges();

            return result.Response.ToList();
        }
    }
}
