using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace PassageIdentity.Tests.Intergration;

public class AuthIntegrationTests : IntegrationTestBase
{
    public AuthIntegrationTests(IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
        : base(httpClientFactory, configuration)
    {
    }

    [Fact]
    public async Task Can_Get_App()
    {
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var app = await client.Authentication.GetApp();

        app.ShouldSatisfyAllConditions(x => x.ShouldNotBeNull());
    }
}
