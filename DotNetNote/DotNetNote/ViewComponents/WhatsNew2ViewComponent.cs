using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.ViewComponents
{
    public class WhatsNew2ViewComponent : ViewComponent
    {
        // 게시판 리파지터리 개체 
        private INoteRepository _repository;

        public WhatsNew2ViewComponent(INoteRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            return View(_repository.GetRecentPosts()); // 기본값: Default
        }
    }
}
