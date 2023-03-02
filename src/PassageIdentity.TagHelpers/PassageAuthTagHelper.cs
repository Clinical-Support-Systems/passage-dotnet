using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PassageIdentity.TagHelpers
{
    [HtmlTargetElement("passage-auth", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class PassageAuthTagHelper : TagHelper
    {
        /// <summary>
        /// The application id
        /// </summary>
        [HtmlAttributeName("app-id")]
        public string? AppId { get; set; }

        public bool Test { get; set; } = false;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            // Add a script tag with the web.js source
            var script = $"<script src=\"https://psg.so/web.js\"></script>";

            // Get the existing content of the head element
            var headContent = output
                .GetChildContentAsync()
                .Result
                .GetContent();

            // Append the script tag to the head content
            var newHeadContent = headContent + script;

            // Update the output to include the modified head content
            output.Content.SetHtmlContent(newHeadContent);

            output.TagName = "passage-auth";
            if (!string.IsNullOrEmpty(AppId)) output.Attributes.Add(new TagHelperAttribute("app-id", AppId));
        }
    }
}
