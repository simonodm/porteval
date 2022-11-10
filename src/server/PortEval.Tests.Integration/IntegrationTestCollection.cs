using Xunit;

namespace PortEval.Tests.Integration
{
    [CollectionDefinition("Integration test collection")]
    public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture>
    {
    }
}
