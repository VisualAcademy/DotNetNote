using DotNetNote.Models.Notes;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.ViewComponents
{
    public class MainSummaryBlogPostListViewComponent : ViewComponent
    {
        private INoteRepository _repository;

        public MainSummaryBlogPostListViewComponent(INoteRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            return View(_repository.GetNoteSummaryByCategoryBlog("Blog"));
        }
    }
}
