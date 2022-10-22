namespace PortEval.Domain.Exceptions
{
    public class OperationNotAllowedException : PortEvalException
    {
        public OperationNotAllowedException(string message) : base(message)
        {

        }
    }
}
