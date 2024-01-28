namespace DotNetNote.Controllers;

public class VariableTestController(IVariableRepository repository) : Controller
{
    // GET: /<controller>/
    public IActionResult Index()
    {
        var variables = repository.GetAll();
        ViewBag.Variables = variables;
        return View();
    }
}
