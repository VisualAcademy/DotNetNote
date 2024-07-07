using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DotNetNote.TagHelpers;

public class UnixTimeConverterTagHelper : TagHelper
{
    //public override void Process(
    //    TagHelperContext context, TagHelperOutput output)
    //{
    //    output.Content.SetContent("UNIX 시간 변경기");
    //}

    public override async Task ProcessAsync(
        TagHelperContext context, TagHelperOutput output)
    {
        var childContent = (await output.GetChildContentAsync()).GetContent();

        var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var currentTime = unixTime.AddSeconds(Convert.ToDouble(childContent));

        output.Content.SetContent(currentTime.ToString(Formatter));
    }

    public string Formatter { get; set; } = "yyyy-MM-dd hh:mm:ss";
}
