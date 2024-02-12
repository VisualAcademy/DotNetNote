using DotNetNote.Models.Notes;

namespace DotNetNote.ViewComponents
{
    /// <summary>
    /// 최근 댓글 리스트
    /// </summary>
    public class RecentlyCommentListViewComponent : ViewComponent
    {
        // 댓글 리포지토리 개체
        private INoteCommentRepository _repository;

        public RecentlyCommentListViewComponent(
            INoteCommentRepository repository)
        {
            _repository = repository;
        }

        // 최근 댓글 리스트 전달
        public IViewComponentResult Invoke() => View(_repository.GetRecentComments());
    }
}
