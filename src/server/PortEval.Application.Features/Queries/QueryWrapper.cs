namespace PortEval.Application.Features.Queries
{
    public class QueryWrapper<TDbResponse>
    {
        public string Query { get; set; }
        public object Params { get; set; }
    }
}