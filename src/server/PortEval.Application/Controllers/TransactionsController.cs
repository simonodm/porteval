using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("transactions")]
    [ApiController]
    public class TransactionsController : PortEvalControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions([FromQuery] TransactionFilters transactionFilters, [FromQuery] DateRangeParams dateRange)
        {
            var transactions = await _transactionService.GetTransactionsAsync(transactionFilters, dateRange);
            return GenerateActionResult(transactions);
        }

        // GET api/transactions/3
        [HttpGet("{transactionId}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int transactionId)
        {
            var transaction = await _transactionService.GetTransactionAsync(transactionId);
            return GenerateActionResult(transaction);
        }

        // POST api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> PostTransaction([FromBody] TransactionDto createRequest)
        {
            var createdTransaction = await _transactionService.AddTransactionAsync(createRequest);
            return GenerateActionResult(createdTransaction, nameof(GetTransaction), new { transactionId = createdTransaction.Response.Id });
        }

        // PUT api/transactions/3
        [HttpPut("{transactionId}")]
        public async Task<ActionResult<TransactionDto>> PutTransaction(int transactionId, [FromBody] TransactionDto updateRequest)
        {
            if (transactionId != updateRequest.Id)
            {
                return BadRequest("URL transaction id and request body transaction id don't match.");
            }

            var updatedTransaction = await _transactionService.UpdateTransactionAsync(updateRequest);
            return GenerateActionResult(updatedTransaction);
        }

        // DELETE api/transactions/3
        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            var response = await _transactionService.DeleteTransactionAsync(transactionId);
            return GenerateActionResult(response);
        }
    }
}
