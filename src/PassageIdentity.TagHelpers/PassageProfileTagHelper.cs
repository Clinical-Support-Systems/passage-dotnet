using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PassageIdentity.TagHelpers
{
    public class PassageProfileTagHelper : TagHelper
    {
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

            output.TagName = "passage-profile";

            return Task.CompletedTask;
        }
    }
}
