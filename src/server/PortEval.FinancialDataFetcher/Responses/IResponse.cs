﻿using PortEval.FinancialDataFetcher.Models;

namespace PortEval.FinancialDataFetcher.Responses
{
    /// <summary>
    /// Represents an operation's response.
    /// </summary>
    public interface IResponse
    {
        public StatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
