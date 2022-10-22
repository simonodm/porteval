namespace PortEval.Application.Services.Queries
{
    public class QueryWrapper<TDbResponse>
    {
        public string Query { get; set; }
        public object Params { get; set; }
    }
}