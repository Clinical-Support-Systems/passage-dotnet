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
        var keys = await client.Management.GetAPIKeysAsync().ConfigureAwait(false);

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
        var app = await client.Management.GetUsersAsync(new ListPassageUsersQuery { }).ConfigureAwait(false);

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
    public async Task Can_Update_User()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var identifier = "KjhfIloBeaflZGd5ipm6oMge";

        // Act
        var user = await client.Management.GetUserAsync(identifier).ConfigureAwait(false);

        var updatedApp = await client.Management.UpdateUserAsync(user).ConfigureAwait(false);

        // Assert
        user.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(identifier)
        );
    }

    [Fact]
    public async Task Can_Activate_User()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var identifier = "8kIAbF46fIee5jMuan3m9LO5";

        // Act
        var user = await client.Management.ActivateUserAsync(identifier).ConfigureAwait(false);

        // Assert
        user.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(identifier),
            x => x.Status.ShouldBe(UserStatus.Active)
        );
    }

    [Fact]
    public async Task Can_Deactivate_User()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var identifier = "8kIAbF46fIee5jMuan3m9LO5";

        // Act
        var user = await client.Management.DeactivateUserAsync(identifier).ConfigureAwait(false);

        // Assert
        user.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(identifier),
            x => x.Status.ShouldBe(UserStatus.Inactive)
        );
    }

    [Fact]
    public async Task Can_Delete_User()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var identifier = "8kIAbF46fIee5jMuan3m9LO5";

        // Act
        await client.Management.DeleteUserAsync(identifier).ConfigureAwait(false);

        var users = await client.Management.GetUsersAsync(new ListPassageUsersQuery { }).ConfigureAwait(false);

        // Assert
        users.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Any(s => s.Id == identifier).ShouldBe(false)
        );
    }

    [Fact]
    public async Task Can_Revoke_User_Tokens()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var identifier = "8kIAbF46fIee5jMuan3m9LO5";

        // Act
        await client.Management.RevokeTokensAsync(identifier).ConfigureAwait(false);

        // Assert
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
    public async Task Can_Update_App()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var app = await client.Management.GetAppAsync().ConfigureAwait(false);

        // Act
        var updatedApp = await client.Management.UpdateAppAsync(app).ConfigureAwait(false);

        // Assert
        app.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBeEquivalentTo(app.Id)
        );
    }


    [Fact]
    public async Task Can_Delete_App()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var appId = "";

        // Act
        await client.Management.DeleteAppAsync(appId).ConfigureAwait(false);

        // Assert
    }

    [Theory]
    [InlineData("testy@test.com")]
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

    [Fact]
    public async Task Can_Claim_Test_App()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var appName = "";

        // Act
        await client.Management.ClaimTestAppAsync(appName).ConfigureAwait(false);

        // Assert
        
    }

    [Fact]
    public async Task Can_Get_APIKeys()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        // Act
        var apiKeys = await client.Management.GetAPIKeysAsync().ConfigureAwait(false);

        // Assert
        apiKeys.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Count().ShouldBeGreaterThan(0)
        );
    }


    [Fact]
    public async Task Can_Create_APIKey()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        // Act
        var apiKey = await client.Management.CreateAPIKey("test").ConfigureAwait(false);

        // Assert
        apiKey.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Name.ShouldBeEquivalentTo("test")
        );
    }

    [Fact]
    public async Task Can_Delete_APIKey()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var apiKey = "";

        // Act
        await client.Management.DeleteAPIKeyAsync(apiKey).ConfigureAwait(false);


        var keys = await client.Management.GetAPIKeysAsync().ConfigureAwait(false);

        // Assert
        keys.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Any(k => k.Id == apiKey).ShouldBe(false)
        );
    }


    [Fact]
    public async Task Can_Get_Admins()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        // Act
        var admins = await client.Management.GetAdminsAsync().ConfigureAwait(false);


        // Assert
        admins.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Count().ShouldBeGreaterThan(0)
        );
    }

    [Fact]
    public async Task Can_Get_Admin()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var adminId = "";

        // Act
        var admin = await client.Management.GetAdminAsync(adminId).ConfigureAwait(false);


        // Assert
        admin.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Admin.Id.ShouldBeEquivalentTo(adminId)
        );
    }

    [Fact]
    public async Task Can_Create_Admin()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var email = "testAdmin";

        // Act
        var admin = await client.Management.CreateAdminAsync(email).ConfigureAwait(false);


        // Assert
        admin.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Email.ShouldBeEquivalentTo(email)
        );
    }

    [Fact]
    public async Task Can_Delete_Admin()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var adminId = "";

        // Act
         await client.Management.DeleteAdminAsync(adminId).ConfigureAwait(false);

        var admins = await client.Management.GetAdminsAsync().ConfigureAwait(false);

        // Assert
        admins.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Any(k => k.Id == adminId).ShouldBe(false)
        );
    }

    [Fact]
    public async Task Can_Get_PaginatedEvents()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var client = new PassageClient(logger, HttpClientFactory, PassageConfig);

        var adminId = "";

        // Act
        var events = await client.Management.GetPaginatedEventsAsync(new PassagePaginatedEventsQuery { }).ConfigureAwait(false);

        // Assert
        events.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Count().ShouldBeGreaterThan(0)
        );
    }
}
