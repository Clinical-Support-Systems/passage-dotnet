using System.Collections.ObjectModel;
using System.Text.Json;
using Bogus;
using Shouldly;

namespace PassageIdentity.Tests.Unit;

public class SerializationTests
{
    [Fact]
    public void Can_Serialize_App()
    {
        var registrationFaker = new Faker<PassageRegistration>()
            .RuleFor(p => p.Id, f => f.Random.AlphaNumeric(24))
            .RuleFor(p => p.X, f => f.Random.Long(0, 10))
            .RuleFor(p => p.Y, f => f.Random.Long(0, 10))
            .RuleFor(p => p.Width, f => f.Random.Long(0, 10))
            .RuleFor(p => p.Height, f => f.Random.Long(0, 10));

        var profileFaker = new Faker<PassageProfile>()
            .RuleFor(p => p.Id, f => f.Random.AlphaNumeric(24))
            .RuleFor(p => p.X, f => f.Random.Long(0, 10))
            .RuleFor(p => p.Y, f => f.Random.Long(0, 10))
            .RuleFor(p => p.Width, f => f.Random.Long(0, 10))
            .RuleFor(p => p.Height, f => f.Random.Long(0, 10));

        var userMetadataFaker = new Faker<UserMetadata>()
            .RuleFor(u => u.Id, f => f.Random.AlphaNumeric(24))
            .RuleFor(u => u.FieldName, f => f.Database.Column())
            .RuleFor(u => u.Type, f => f.PickRandom<UserMetadataFieldType>())
            .RuleFor(u => u.FriendlyName, f => f.Lorem.Word())
            .RuleFor(u => u.Registration, f => f.Random.Bool())
            .RuleFor(u => u.Profile, f => f.Random.Bool());

        var layoutFaker = new Faker<Layouts>()
            .RuleFor(l => l.Profile, f => new Collection<PassageProfile>(profileFaker.GenerateBetween(1, 2)))
            .RuleFor(l => l.Registration, f => new Collection<PassageRegistration>(registrationFaker.GenerateBetween(1, 2)));

        var faker = new Faker<App>()
            .RuleFor(x => x.AllowedIdentifier, f => f.PickRandom<AllowedIdentifier>())
            .RuleFor(x => x.AuthOrigin, f => new Uri(f.Internet.Url()))
            .RuleFor(x => x.DefaultLanguage, f => f.Lorem.Word())
            .RuleFor(x => x.Ephemeral, f => f.Random.Bool())
            .RuleFor(x => x.Id, f => f.Random.AlphaNumeric(24))
            .RuleFor(x => x.Layouts, f => layoutFaker.Generate(1)[0])
            .RuleFor(x => x.LoginUrl, f => new Uri(f.Internet.Url()))
            .RuleFor(x => x.Name, f => f.Lorem.Word())
            .RuleFor(x => x.PublicSignup, f => f.Random.Bool())
            .RuleFor(x => x.RedirectUrl, f => new Uri(f.Internet.Url()))
            .RuleFor(x => x.RequiredIdentifier, f => f.Lorem.Word())
            .RuleFor(x => x.RequireEmailVerification, f => f.Random.Bool())
            .RuleFor(x => x.RequireIdentifierVerification, f => f.Random.Bool())
            .RuleFor(x => x.SessionTimeoutLength, f => f.Random.Long(15, 60))
            .RuleFor(x => x.UserMetadataSchema, f => new Collection<UserMetadata>(userMetadataFaker.GenerateBetween(1, 5)));

        var app = new PassageApp()
        {
            App = faker.Generate(1)[0]
        };

        var appSerialized = JsonSerializer.Serialize<PassageApp>(app, new JsonSerializerOptions() { WriteIndented = true });

        appSerialized.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNullOrEmpty(),
            x => x.ShouldContain(expected: app.App.Name)
        );
    }
}
