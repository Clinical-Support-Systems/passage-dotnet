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

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.TagName = "passage-auth";

            return base.ProcessAsync(context, output);
        }
    }
}
