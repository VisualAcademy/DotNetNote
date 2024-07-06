using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DotNetNote.TagHelpers;

[HtmlTargetElement("markdown-viewer")]
public class MarkdownViewerTagHelper : TagHelper
{
    public async override Task ProcessAsync(
        TagHelperContext context, TagHelperOutput output)
    {
        // 태그 헬퍼 안의 텍스트 가져오기
        var content = await output.GetChildContentAsync();
        var result =
            CommonMark.CommonMarkConverter.Convert(
                content.GetContent()); // HTML 결과
        output.Content.SetHtmlContent(result); // HTML 출력
        output.TagName = null; // 따로 특정 태그로 묶이지 않음
    }
}
