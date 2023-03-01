using Microsoft.Extensions.Logging;
using NSubstitute;

namespace PassageIdentity.Tests
{
    public class AuthenticationTests
    {
        [Fact]
        public void Can_Get_App()
        {
            var logger = Substitute.For<ILogger>();
            //var client = new PassageClient(logger, )
        }
    }
}
