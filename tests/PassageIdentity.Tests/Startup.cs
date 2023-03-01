using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PassageIdentity.Tests;

public class Startup
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient(PassageConsts.NamedClient);

        var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.Development.json", optional: true)
            .Build();

        services.AddSingleton<IConfiguration>(config);
    }
}
