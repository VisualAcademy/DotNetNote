using DotNetNote.Models.Notes;

namespace DotNetNote.ViewComponents
{
    public class RecentlyCommentList2ViewComponent : ViewComponent
    {
        // 댓글 리포지토리 개체
        private INoteCommentRepository _repository;

        public RecentlyCommentList2ViewComponent(
            INoteCommentRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            // 최근 댓글 리스트 전달
            return View(_repository.GetRecentCommentsNoCache());
        }
    }
}
