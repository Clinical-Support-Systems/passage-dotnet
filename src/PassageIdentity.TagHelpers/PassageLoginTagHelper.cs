using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PassageIdentity.TagHelpers
{
    [HtmlTargetElement("passage-login", TagStructure = TagStructure.WithoutEndTag)]
    public class PassageLoginTagHelper : TagHelper
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
