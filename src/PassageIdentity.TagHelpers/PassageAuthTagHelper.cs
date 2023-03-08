using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PassageIdentity.TagHelpers
{

    [HtmlTargetElement("passage-auth", TagStructure = TagStructure.WithoutEndTag)]
    public class PassageAuthTagHelper : PassageBaseTagHelper
    {
        /// <summary>
        /// The <see cref="ViewContext"/> for this control, gets injected.
        /// </summary>
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

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
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.TagName = "passage-auth";

            if (!string.IsNullOrEmpty(BeforeAuth))
            {
                var script = $"<script>document.querySelector('passage-auth').beforeAuth = function(arg){{ {BeforeAuth}(arg);}};</script>";
                output.PostElement.AppendHtml(script);
            }

            if (!string.IsNullOrEmpty(OnSuccess))
            {
                var script = $"<script>document.querySelector('passage-auth').onSuccess = function(arg){{ {OnSuccess}(arg); }};</script>";
                output.PostElement.AppendHtml(script);
            }

            return base.ProcessAsync(context, output);
        }
    }
}
