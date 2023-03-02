using Microsoft.Extensions.Logging;

namespace PassageIdentity;

public class PassageClient
{
    private readonly IPassageConfig _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private PassageAuthentication? _authentication;

    private PassageManagement? _management;

    public PassageClient(ILogger logger, IHttpClientFactory httpClientFactory)
                        : this(logger, httpClientFactory, new PassageConfig())
    {
    }

    public PassageClient(string appId, ILogger logger, IHttpClientFactory httpClientFactory)
        : this(logger, httpClientFactory, new PassageConfig(appId))
    {
        //httpClientFactory.CreateClient(PassageConsts.NamedClient)
    }

    public PassageClient(ILogger logger, IHttpClientFactory httpClientFactory, IPassageConfig config)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public PassageAuthentication Authentication
    {
        get
        {
            _authentication ??= new(_logger, _httpClientFactory, _config);
            return _authentication;
        }
    }

    public PassageManagement Management
    {
        get
        {
            _management ??= new(_logger, _httpClientFactory, _config);
            return _management;
        }
    }
}
