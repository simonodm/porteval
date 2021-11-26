using System;

namespace PortEval.Domain.Exceptions
{
    public abstract class PortEvalException : Exception
    {
        protected PortEvalException(string message) : base(message)
        {

        }
    }
}
