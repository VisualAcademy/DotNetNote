using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class ListOfCategoryController : Controller
{
    private readonly ICategoryRepository _repository;

    public ListOfCategoryController(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        // var categoryRepository = new CategoryRepositoryInMemory();
        // var categories = categoryRepository.GetCategories();

        var categories = _repository.GetCategories();

        return View(categories);
    }
}
