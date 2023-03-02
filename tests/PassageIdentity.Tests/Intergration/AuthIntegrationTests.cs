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

    [Fact]
    public async Task Get_User()
    {
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var user = await client.Authentication.GetUserAsync("byter@me.com");

        user.ShouldSatisfyAllConditions(x => x.ShouldNotBeNull());
    }

    [Fact]
    public async Task Get_MagicLink_ValidEmail()
    {
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var magicLink = await client.Authentication.GetMagicLinkAsync("byter@me.com");

        magicLink.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x?.Id.ShouldNotBeNullOrEmpty()
        );
    }

    [Fact]
    public async Task Get_MagicLink_ValidPhone()
    {
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var magicLink = await client.Authentication.GetMagicLinkAsync("byter@me.com");

        magicLink.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x?.Id.ShouldNotBeNullOrEmpty()
        );
    }
}
