using DotNetNote.Models.Notes;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.ViewComponents;

public class WhatsNew2ViewComponent : ViewComponent
{
    // 게시판 리포지토리 개체 
    private INoteRepository _repository;

    public WhatsNew2ViewComponent(INoteRepository repository) => _repository = repository;

    public IViewComponentResult Invoke()
    {
        return View(_repository.GetRecentPosts()); // 기본값: Default
    }
}
