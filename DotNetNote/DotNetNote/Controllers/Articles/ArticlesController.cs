using MemoEngineCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DotNetNote.Controllers;

public class ArticlesController : Controller
{
    private readonly IBlogService _service;

    public ArticlesController(IBlogService service)
    {
        this._service = service;
    }

    // GET: Articles
    public IActionResult Index()
    {
        var posts = _service.GetPosts().ToList();

        return View(posts);
    }
}
