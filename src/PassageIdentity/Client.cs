using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace PassageIdentity
{
    public class PassageClient
    {
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
    }
}
