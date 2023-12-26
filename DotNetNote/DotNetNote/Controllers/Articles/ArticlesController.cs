using DotNetNote.Controllers.Articles;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DotNetNote.Controllers;

public class ArticlesController(IBlogService service) : Controller
{
    // GET: Articles
    public IActionResult Index()
    {
        var posts = service.GetPosts().ToList();

        return View(posts);
    }
}
