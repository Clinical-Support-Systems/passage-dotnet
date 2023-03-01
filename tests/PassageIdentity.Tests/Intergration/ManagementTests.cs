using Microsoft.Extensions.Configuration;

namespace PassageIdentity.Tests.Intergration;

public class ManagementIntegtrationTests : IntegrationTestBase
{
    public ManagementIntegtrationTests(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
    {
    }

    [Fact]
    public void Test1()
    {

    }
}
