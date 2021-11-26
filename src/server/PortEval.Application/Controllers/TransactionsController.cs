using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Queries.Interfaces;
using PortEval.Application.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Queries;

namespace PortEval.Application.Controllers
{
    [Route("api/positions/{positionId}/transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ITransactionQueries _transactionQueries;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TransactionsController(ITransactionService transactionService, ITransactionQueries transactionQueries, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _transactionService = transactionService;
            _transactionQueries = transactionQueries;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger(typeof(TransactionsController));
        }

        // GET: api/portfolios/5/positions/1/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions(int positionId, [FromQuery] DateRangeParams dateRange)
        {
            _logger.LogInformation($"Position {positionId} transactions requested.");

            var transactions = await _transactionQueries.GetPositionTransactions(positionId, dateRange);
            if (transactions.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found.");
            }

            return transactions.Response.ToList();
        }

        // GET api/portfolios/5/positions/1/transactions/3
        [HttpGet("{transactionId}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int positionId, int transactionId)
        {
            _logger.LogInformation($"Transaction {transactionId} of position {positionId} requested.");

            var transaction = await _transactionQueries.GetTransaction(positionId, transactionId);
            if (transaction.Status == QueryStatus.NotFound)
            {
                return NotFound($"Transaction {transactionId} of position {positionId} not found.");
            }

            return transaction.Response;
        }

        // POST api/portfolios/5/positions/1/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> PostTransaction(int positionId, [FromBody] TransactionDto createRequest)
        {
            _logger.LogInformation($"Creating transaction for position {positionId}.");

            var createdTransaction = await _transactionService.AddTransactionAsync(createRequest);

            return CreatedAtAction("GetTransaction",
                new { positionId, transactionId = createdTransaction.Id },
                _mapper.Map<TransactionDto>(createdTransaction));
        }

        // PUT api/portfolios/5/positions/1/transactions/3
        [HttpPut("{transactionId}")]
        public async Task<ActionResult<TransactionDto>> PutTransaction(int positionId, int transactionId, [FromBody] TransactionDto updateRequest)
        {
            _logger.LogInformation($"Updating transaction {transactionId} of position {positionId}.");

            if (positionId != updateRequest.PositionId)
            {
                return BadRequest("URL position id and request body position id don't match.");
            }

            if (transactionId != updateRequest.Id)
            {
                return BadRequest("URL transaction id and request body transaction id don't match.");
            }

            var updatedTransaction = await _transactionService.UpdateTransactionAsync(updateRequest);

            return _mapper.Map<TransactionDto>(updatedTransaction);
        }

        // DELETE api/portfolios/5/positions/1/transactions/3
        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> DeleteTransaction(int positionId, int transactionId)
        {
            _logger.LogInformation($"Deleting transaction {transactionId} of position {positionId}.");

            await _transactionService.DeleteTransactionAsync(positionId, transactionId);
            return Ok();
        }
    }
}
