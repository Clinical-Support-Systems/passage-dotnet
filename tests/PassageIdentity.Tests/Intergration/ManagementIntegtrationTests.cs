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

    [Fact]
    public async Task Can_Get_Apps()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        // Act
        var app = await client.Management.GetAppsAsync().ConfigureAwait(false);

        // Assert
        app.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x!.Any().ShouldNotBe(false)
        );
    }

    [Fact]
    public async Task Can_Get_Users()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        // Act
        var app = await client.Management.GetUsersAsync().ConfigureAwait(false);

        // Assert
        app.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x!.Any().ShouldNotBe(false)
        );
    }

    [Fact]
    public async Task Can_Get_User()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var identifier = "KjhfIloBeaflZGd5ipm6oMge";

        // Act
        var app = await client.Management.GetUserAsync(identifier).ConfigureAwait(false);

        // Assert
        app.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(identifier)
        );
    }

    [Fact]
    public async Task Can_Create_App()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var testApp = new App() { AllowedIdentifier = AllowedIdentifier.Email, AuthOrigin = new Uri("https://localhost:7294"), Name = "TestCreateApp", PublicSignup = false, RedirectUrl = new Uri("https://localhost:7294/signin-passage") };

        // Act
        var app = await client.Management.CreateAppAsync(testApp).ConfigureAwait(false);

        // Assert
        app.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull()
        );
    }

    [Fact]
    public async Task Can_Create_User()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var testUser = new User() { Email = "test@testy.com" };

        // Act
        var app = await client.Management.CreateUserAsync(testUser).ConfigureAwait(false);

        // Assert
        app.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull()
        );
    }


    [Fact]
    public async Task Can_Import_Users()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        byte[] bytes = null;
        using (var ms = new MemoryStream())
        {
            using (TextWriter tw = new StreamWriter(ms))
            {
                tw.WriteLine("test1@test.com");
                tw.WriteLine("test2@test.com");
                tw.Flush();
                ms.Position = 0;
                bytes = ms.ToArray();
            }

        }

        // Act
        var app = await client.Management.CreateUsersAsync(bytes).ConfigureAwait(false);

        // Assert
        app.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull()
        );
    }
}
