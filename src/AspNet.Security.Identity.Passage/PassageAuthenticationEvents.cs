using Microsoft.AspNetCore.Authentication;

namespace AspNet.Security.Identity.Passage
{
    public class PassageAuthenticationEvents : RemoteAuthenticationEvents
    {
        /// <summary>
        /// Defines a notification invoked when the user is authenticated by the identity provider.
        /// </summary>
        public Func<PassageAuthenticatedContext, Task> OnAuthenticated { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Defines a notification invoked when the user is authenticated by the identity provider.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task Authenticated(PassageAuthenticatedContext context) => OnAuthenticated(context);

        /// <summary>
        /// Gets or sets the function that is invoked when the Authenticated method is invoked.
        /// </summary>
        public Func<PassageAuthenticationCreatingTicketContext, Task> OnCreatingTicket { get; set; } = _ => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the delegate that is invoked when the ApplyRedirect method is invoked.
        /// </summary>
        public Func<RedirectContext<PassageAuthenticationOptions>, Task> OnRedirectToAuthorizationEndpoint { get; set; } = context =>
        {
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };

        /// <summary>
        /// Invoked whenever Cas successfully authenticates a user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task CreatingTicket(PassageAuthenticationCreatingTicketContext context) => OnCreatingTicket(context);

        /// <summary>
        /// Called when a Challenge causes a redirect to authorize endpoint in the Cas middleware
        /// </summary>
        /// <param name="context">Contains redirect URI and <see cref="AuthenticationProperties"/> of the challenge </param>
        public virtual Task RedirectToAuthorizationEndpoint(RedirectContext<PassageAuthenticationOptions> context) => OnRedirectToAuthorizationEndpoint(context);

        public virtual Task TicketAccepted(TicketAcceptedContext context) => OnTicketAccepted(context);

        public Func<TicketAcceptedContext, Task> OnTicketAccepted { get; set; } = _ => Task.CompletedTask;
    }
}
