using Xunit;

namespace PortEval.Tests.Integration.QueryTests;

[CollectionDefinition("Query test collection")]
public class QueryTestCollection : ICollectionFixture<QueryTestFixture>
{
}