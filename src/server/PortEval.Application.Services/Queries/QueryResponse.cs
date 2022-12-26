namespace PortEval.Application.Features.Queries
{
    public class QueryResponse<T>
    {
        public QueryStatus Status { get; init; }
        public T Response { get; init; }
    }
}
