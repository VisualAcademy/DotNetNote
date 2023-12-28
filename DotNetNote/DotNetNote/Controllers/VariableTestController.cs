using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
