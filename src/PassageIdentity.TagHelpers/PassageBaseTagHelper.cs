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

            var script = $"<script src=\"https://psg.so/web.js\" defer></script>";
            output.PreElement.SetHtmlContent(script);

            if (!string.IsNullOrEmpty(AppId)) output.Attributes.Add(new TagHelperAttribute("app-id", AppId));

            return base.ProcessAsync(context, output);
        }
    }
}
