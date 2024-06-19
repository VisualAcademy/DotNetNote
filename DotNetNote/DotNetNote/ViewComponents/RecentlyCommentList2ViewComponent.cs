using DotNetNote.Models.Notes;

namespace DotNetNote.ViewComponents
{
    public class RecentlyCommentList2ViewComponent(
        INoteCommentRepository repository) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // 최근 댓글 리스트 전달
            return View(repository.GetRecentCommentsNoCache());
        }
    }
}
