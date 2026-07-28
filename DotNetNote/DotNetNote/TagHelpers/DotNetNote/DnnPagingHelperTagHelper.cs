using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DotNetNote.TagHelpers;

/// <summary>
/// 페이징 헬퍼(dnn-paging-helper)
/// </summary>
public class DnnPagingHelperTagHelper : TagHelper
{
    /// <summary>
    /// 기본 리스트면 false, 검색 결과에 대한 페이징 리스트면 true
    /// </summary>
    public bool SearchMode { get; set; }

    /// <summary>
    /// 검색할 필드: Name, Title, Content
    /// </summary>
    public string SearchField { get; set; } = string.Empty;

    /// <summary>
    /// 검색할 내용
    /// </summary>
    public string SearchQuery { get; set; } = string.Empty;

    /// <summary>
    /// 현재 보여줄 페이지 인덱스: 0, 1, 2
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 총 페이지 개수
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// 한 페이지에 보여줄 아티클 개수
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 페이징 헬퍼가 실행될 URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    private int _recordCount;

    /// <summary>
    /// 총 레코드 수
    /// </summary>
    public int RecordCount
    {
        get => _recordCount;
        set
        {
            _recordCount = value;

            PageCount = PageSize > 0
                ? ((_recordCount - 1) / PageSize) + 1
                : 0;
        }
    }

    /// <summary>
    /// 페이저에 몇 개씩 페이지 버튼을 표시할지
    /// </summary>
    public int PagerButtonCount { get; set; } = 5;

    public override void Process(
        TagHelperContext context,
        TagHelperOutput output)
    {
        output.TagName = "ul";
        output.Attributes.SetAttribute(
            "class",
            "pagination pagination-sm mx-auto");

        if (PageIndex <= 0)
        {
            PageIndex = 1;
        }

        var page = 0;
        var pageHtml = string.Empty;

        if (PageIndex > 10)
        {
            var previousPage = ((PageIndex - 1) / 10) * 10;

            if (!SearchMode)
            {
                pageHtml +=
                    $"<li><a href=\"{Url}?Page={previousPage}\">◀</a></li>";
            }
            else
            {
                pageHtml +=
                    $"<li><a href=\"{Url}?Page={previousPage}" +
                    $"&SearchField={SearchField}" +
                    $"&SearchQuery={SearchQuery}\">◀</a></li>";
            }
        }
        else
        {
            pageHtml += "<li class=\"disabled\"><a>◁</a></li>";
        }

        var firstPage = ((PageIndex - 1) / 10) * 10 + 1;
        var lastPage = (((PageIndex - 1) / 10) + 1) * 10;

        for (page = firstPage; page <= lastPage; page++)
        {
            if (page > PageCount)
            {
                break;
            }

            if (page == PageIndex)
            {
                pageHtml +=
                    $"<li class=\"active\"><a href=\"#\">{page}</a></li>";
            }
            else if (!SearchMode)
            {
                pageHtml +=
                    $"<li><a href=\"{Url}?Page={page}\">{page}</a></li>";
            }
            else
            {
                pageHtml +=
                    $"<li><a href=\"{Url}?Page={page}" +
                    $"&SearchField={SearchField}" +
                    $"&SearchQuery={SearchQuery}\">{page}</a></li>";
            }
        }

        if (page < PageCount)
        {
            var nextPage = ((PageIndex - 1) / 10) * 10 + 11;

            if (!SearchMode)
            {
                pageHtml +=
                    $"<li><a href=\"{Url}?Page={nextPage}\">▶</a></li>";
            }
            else
            {
                pageHtml +=
                    $"<li><a href=\"{Url}?Page={nextPage}" +
                    $"&SearchField={SearchField}" +
                    $"&SearchQuery={SearchQuery}\">▶</a></li>";
            }
        }
        else
        {
            pageHtml += "<li class=\"disabled\"><a>▷</a></li>";
        }

        output.Content.SetHtmlContent(pageHtml);
    }
}