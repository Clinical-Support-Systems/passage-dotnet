using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PassageIdentity.TagHelpers
{
    public class PassageLoginTagHelper : TagHelper
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

            output.TagName = "passage-login";

            return Task.CompletedTask;
        }
    }
}
