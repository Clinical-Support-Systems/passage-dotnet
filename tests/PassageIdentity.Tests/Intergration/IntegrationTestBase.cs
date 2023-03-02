using Microsoft.Extensions.Configuration;

namespace PassageIdentity.Tests.Intergration;

public abstract class IntegrationTestBase
{
    protected IntegrationTestBase(IHttpClientFactory httpClientFactory,
                               IConfiguration configuration)
    {
        HttpClientFactory = httpClientFactory;
        Configuration = configuration;

        PassageConfig = new PassageConfig(Configuration.GetValue<string>("Passage:AppId")) { ApiKey = Configuration.GetValue("Passage:ApiKey", string.Empty) };
    }

    public IConfiguration Configuration { get; }
    public IHttpClientFactory HttpClientFactory { get; }
    public PassageConfig PassageConfig { get; private set; }
}
