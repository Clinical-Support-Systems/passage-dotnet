using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PassageIdentity.TagHelpers
{
    public class PassageBaseTagHelper : TagHelper
    {
        /// <summary>
        /// The application id
        /// </summary>
        [HtmlAttributeName("app-id")]
        public string? AppId { get; set; }

        /// <summary>
        /// The ISO-3166-2 country code representing the language to use, when possible
        /// </summary>
        [HtmlAttributeName("default-country-code")]
        public string? DefaultCountryCode { get; set; }

        /// <summary>
        /// Javascript function name. The user's identifier is provided as a parameter.
        /// After doing additional work to validate any other requirements, the callback
        /// should return true to continue with the registration/login, or return
        /// false to cancel the registration/login.
        /// </summary>
        [HtmlAttributeName("on-before-auth")]
        public string? BeforeAuth { get; set; }

        [HtmlAttributeName("on-success")]
        public string? OnSuccess { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var script = "<script src=\"https://psg.so/web.js\" defer></script>";
            output.PreElement.SetHtmlContent(script);

            if (!string.IsNullOrEmpty(AppId)) output.Attributes.Add(new TagHelperAttribute("app-id", AppId));
            if (!string.IsNullOrEmpty(DefaultCountryCode)) output.Attributes.Add(new TagHelperAttribute("default-country-code", DefaultCountryCode));

            if (!string.IsNullOrEmpty(BeforeAuth))
            {
                var beforeAuthScript = $"<script>document.querySelector('passage-auth').beforeAuth = function(arg){{ {BeforeAuth}(arg);}};</script>";
                output.PostElement.AppendHtml(beforeAuthScript);
            }

            if (!string.IsNullOrEmpty(OnSuccess))
            {
                var onSuccessScript = $"<script>document.querySelector('passage-auth').onSuccess = function(arg){{ {OnSuccess}(arg); }};</script>";
                output.PostElement.AppendHtml(onSuccessScript);
            }

            return base.ProcessAsync(context, output);
        }
    }
}
