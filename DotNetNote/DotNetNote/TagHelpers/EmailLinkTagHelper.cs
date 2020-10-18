using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DotNetNote.TagHelpers
{
    // <el>Help<el>
    //[HtmlTargetElement("email-link")]
    [HtmlTargetElement("el")]
    public class EmailLinkTagHelper : TagHelper
    {
        const string domain = "dotnetkorea.com";
        public override async Task ProcessAsync(
            TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            string originText = 
                (await output.GetChildContentAsync()).GetContent();

            string emailString = $"{originText}@{domain}";

            output.Attributes.Add("href", $"mailto:{emailString}");

            output.Content.SetContent(emailString);
        }
    }
}
