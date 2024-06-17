using DotNetNote.Models.Notes;

namespace DotNetNote.ViewComponents;

public class MainSummaryBlogPostListViewComponent(INoteRepository repository) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(repository.GetNoteSummaryByCategoryBlog("Blog"));
    }
}
