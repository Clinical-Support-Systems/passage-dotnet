using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace PassageIdentity.Tests.Intergration;

public class ManagementIntegtrationTests : IntegrationTestBase
{
    public ManagementIntegtrationTests(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
    {
    }

    [Fact]
    public async Task Can_Get_App()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        // Act
        var app = await client.Management.GetAppAsync().ConfigureAwait(false);

        // Assert
        app.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x!.Id.ShouldNotBeNullOrEmpty(),
            x => x!.Id.ShouldBe(PassageConfig.AppId)
        );
    }
}
