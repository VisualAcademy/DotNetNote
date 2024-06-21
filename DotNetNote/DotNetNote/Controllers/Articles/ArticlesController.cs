using DotNetNote.Controllers.Articles;

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
