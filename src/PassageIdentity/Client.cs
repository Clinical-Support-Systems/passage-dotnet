using Microsoft.Extensions.Logging;

namespace PassageIdentity
{
    public class PassageClient
    {
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
        }

        public PassageAuthentication Authentication
        {
            get
            {
                _authentication ??= new();
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