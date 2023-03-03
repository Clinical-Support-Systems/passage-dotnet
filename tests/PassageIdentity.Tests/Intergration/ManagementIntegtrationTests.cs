using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit.Abstractions;

namespace PassageIdentity.Tests.Intergration;

public class ManagementIntegtrationTests : IntegrationTestBase
{
    public ManagementIntegtrationTests(IHttpClientFactory httpClientFactory,
                                       IConfiguration configuration,
                                       ITestOutputHelper testOutputHelper)
        : base(httpClientFactory, configuration, testOutputHelper)
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
    public async Task Can_Get_ApiKeys()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        // Act
        var keys = await client.Management.GetApiKeysAsync().ConfigureAwait(false);

        // Assert
        keys.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x!.Any().ShouldNotBe(false),
            x => x.All(k => !string.IsNullOrEmpty(k.Id)).ShouldBeTrue(),
            x => x.All(k => !string.IsNullOrEmpty(k.Name)).ShouldBeTrue(),
            x => x.All(k => !string.IsNullOrEmpty(k.Role)).ShouldBeTrue(),
            x => x.All(k => !string.IsNullOrEmpty(k.KeyPrefix)).ShouldBeTrue(),
            x => x.All(k => k.CreatedAt > DateTime.MinValue).ShouldBeTrue()
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
            x => x!.Any().ShouldBeTrue()
        );
    }

    [Theory]
    [InlineData("KjhfIloBeaflZGd5ipm6oMge")]
    public async Task Can_Get_User(string identifier)
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

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

    [Theory]
    [InlineData("test1@test.com")]
    public async Task Can_Create_User(string emailAddress)
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var testUser = new User() { Email = emailAddress };

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

        var faker = new Faker();
        var bytes = Array.Empty<byte>();
        using (var ms = new MemoryStream())
        {
            using TextWriter tw = new StreamWriter(ms);
            for (var i = 0; i < faker.Random.Int(1, 3); i++)
            {
                var emailAddress = faker.Internet.Email();
                Output.WriteLine($"Adding {emailAddress}");
                tw.WriteLine(emailAddress);
            }
            tw.Flush();
            ms.Position = 0;
            bytes = ms.ToArray();
        }

        // Act
        bytes.ShouldSatisfyAllConditions(
            b => b.ShouldNotBeNull(),
            b => b.ShouldNotBe(Array.Empty<byte>())
        );
        var app = await client.Management.CreateUsersAsync(bytes).ConfigureAwait(false);

        // Assert
        app.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull()
        );
    }
}
