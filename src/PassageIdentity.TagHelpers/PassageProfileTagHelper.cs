using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PassageIdentity.TagHelpers
{
    [HtmlTargetElement("passage-profile", TagStructure = TagStructure.WithoutEndTag)]
    public class PassageProfileTagHelper : PassageBaseTagHelper
    {
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.TagName = "passage-profile";

            return base.ProcessAsync(context, output);
        }
    }
}
