﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Features.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ITransactionQueries _transactionQueries;
        private readonly IMapper _mapper;

        public TransactionsController(ITransactionService transactionService, ITransactionQueries transactionQueries, IMapper mapper)
        {
            _transactionService = transactionService;
            _transactionQueries = transactionQueries;
            _mapper = mapper;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions([FromQuery] TransactionFilters transactionFilters, [FromQuery] DateRangeParams dateRange)
        {
            var transactions = await _transactionQueries.GetTransactions(transactionFilters, dateRange);
            if (transactions.Status == QueryStatus.NotFound)
            {
                return NotFound("Not found.");
            }

            return transactions.Response.ToList();
        }

        // GET api/transactions/3
        [HttpGet("{transactionId}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int transactionId)
        {
            var transaction = await _transactionQueries.GetTransaction(transactionId);
            if (transaction.Status == QueryStatus.NotFound)
            {
                return NotFound($"Transaction {transactionId} not found.");
            }

            return transaction.Response;
        }

        // POST api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> PostTransaction([FromBody] TransactionDto createRequest)
        {
            var createdTransaction = await _transactionService.AddTransactionAsync(createRequest);

            return CreatedAtAction("GetTransaction",
                new { transactionId = createdTransaction.Id },
                _mapper.Map<TransactionDto>(createdTransaction));
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

            return _mapper.Map<TransactionDto>(updatedTransaction);
        }

        // DELETE api/transactions/3
        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            await _transactionService.DeleteTransactionAsync(transactionId);
            return Ok();
        }
    }
}
