using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace PassageIdentity.Tests;

public class AuthenticationTests
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthenticationTests(IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [Fact]
    public async Task Can_Get_App()
    {
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, _httpClientFactory, new PassageConfig(_configuration.GetValue<string>("Passage:AppId")) { ApiKey = _configuration.GetValue<string>("Passage:ApiKey", string.Empty) });

        var app = await client.Authentication.GetApp();

        app.ShouldSatisfyAllConditions(x => x.ShouldNotBeNull());
    }
}
