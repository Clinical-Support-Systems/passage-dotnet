using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

        /// <summary>
        /// The <see cref="ViewContext"/> for this control, gets injected.
        /// </summary>
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        public bool Test { get; set; }
        private const string PassageScriptKey = "Passage";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            await base.ProcessAsync(context, output).ConfigureAwait(false);

            var isInitialized = ViewContext.HttpContext.Items.ContainsKey(PassageScriptKey);
            ViewContext.HttpContext.Items[PassageScriptKey] = true;

            //if (!isInitialized)
            //{
            // Add a script tag with the web.js source
            var script = $"<script src=\"https://psg.so/web.js\"></script>";

            //// Get the existing content of the head element
            //var headContent = output
            //    .GetChildContentAsync()
            //    .Result
            //    .GetContent();

            //// Append the script tag to the head content
            //var newHeadContent = headContent + script;

            //// Update the output to include the modified head content
            //output.Content.SetHtmlContent(newHeadContent);

            output.PreContent.AppendHtml(script);
            // }

            output.TagName = "passage-auth";
            if (!string.IsNullOrEmpty(AppId)) output.Attributes.Add(new TagHelperAttribute("app-id", AppId));
        }
    }
}
