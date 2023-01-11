using System.Text;

namespace PortEval.Infrastructure.FinancialDataFetcher
{
    public class QueryUrlBuilder
    {
        private readonly StringBuilder _urlBuilder = new StringBuilder();
        private bool _queryParamsExist = false;

        public QueryUrlBuilder(string baseUrl)
        {
            _urlBuilder.Append(baseUrl);
        }

        public void AddQueryParam(string name, string value)
        {
            if (name == null || value == null) return;

            _urlBuilder.Append(_queryParamsExist ? '&' : '?');
            _urlBuilder.Append($"{name}={value}");

            _queryParamsExist = true;
        }

        public override string ToString()
        {
            return _urlBuilder.ToString();
        }
    }
}
