using Microsoft.Extensions.Logging;

namespace PassageIdentity
{
    public class PassageClient
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPassageConfig _passageConfig;
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

        public PassageClient(ILogger logger, IHttpClientFactory httpClientFactory, IPassageConfig passageConfig)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _passageConfig = passageConfig;
        }

        public PassageAuthentication Authentication
        {
            get
            {
                _authentication ??= new(_logger, _httpClientFactory, _passageConfig);
                return _authentication;
            }
        }

        public PassageManagement Management
        {
            get
            {
                _management ??= new();
                return _management;
            }
        }
    }
}
