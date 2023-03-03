using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace PassageIdentity.Tests.Intergration;

public abstract class IntegrationTestBase
{
    protected IntegrationTestBase(IHttpClientFactory httpClientFactory,
                                  IConfiguration configuration,
                                  ITestOutputHelper testOutputHelper)
    {
        HttpClientFactory = httpClientFactory;
        Configuration = configuration;
        Output = testOutputHelper;
        PassageConfig = new PassageConfig(Configuration.GetValue<string>("Passage:AppId")) { ApiKey = Configuration.GetValue("Passage:ApiKey", string.Empty) };
    }

    public IConfiguration Configuration { get; }
    public IHttpClientFactory HttpClientFactory { get; }
    public ITestOutputHelper Output { get; }
    public PassageConfig PassageConfig { get; private set; }
}
