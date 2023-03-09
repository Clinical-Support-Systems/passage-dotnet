using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PassageIdentity.TagHelpers
{
    [HtmlTargetElement("passage-login")]
    public class PassageLoginTagHelper : PassageBaseTagHelper
    {
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.TagName = "passage-login";

            return base.ProcessAsync(context, output);
        }
    }
}
