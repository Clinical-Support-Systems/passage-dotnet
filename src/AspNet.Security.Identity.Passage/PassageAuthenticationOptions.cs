using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNet.Security.Identity.Passage
{
    public class PassageAuthenticationOptions : RemoteAuthenticationOptions
    {
        public PassageAuthenticationOptions()
        {
            CallbackPath = new PathString("/signin-passage");
        }

        public string? ApiKey { get; set; }
        public string? AppId { get; set; }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties>? StateDataFormat { get; set; }
    }
}
