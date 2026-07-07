using DotNetNote.Models.Notes;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DotNetNote.TagHelpers;

[HtmlTargetElement("dnn-main-summary")]
public class DotNetNoteMainSummaryTagHelper : TagHelper
{
    private readonly INoteRepository _repository;

    /// <summary>
    /// 게시판 카테고리: Notice, Free, Data, Qna, ...
    /// </summary>
    public string Category { get; set; } = string.Empty;

    public DotNetNoteMainSummaryTagHelper(INoteRepository repository) => _repository = repository;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";

        var html = new System.Text.StringBuilder();

        var list = _repository.GetNoteSummaryByCategoryCache(Category);

        if (list is { Count: > 0 })
        {
            foreach (var note in list)
            {
                var title = Dul.StringLibrary
                    .CutStringUnicode(note.Title, 33)
                    .Replace("<", "&lt;");

                html.Append("<div class='post_item'>");
                html.Append("<div class='post_item_text'>");
                html.Append($"<span class='post_date'>{note.PostDate:yyyy-MM-dd}</span>");
                html.Append("<span class='post_title'>");
                html.Append($"<a href='/DotNetNote/Details/{note.Id}'>{title}</a>");
                html.Append("</span>");
                html.Append("</div>");
                html.Append("</div>");
            }
        }
        else
        {
            html.Append("""
                <div class='text-center'>
                    항목이 없습니다.
                </div>
                """);
        }

        output.Content.AppendHtml(html.ToString());
    }
}