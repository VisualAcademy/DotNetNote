using DotNetNote.Models.Categories;

namespace DotNetNote.Controllers.DotNetSale;

[Route("api/[controller]")]
public class CategoryServicesController(ICategoryRepository repository) : Controller
{
    [HttpGet]
    public IEnumerable<Category> Get()
    {
        //return (new CategoryRepositoryInMemory()).GetCategories();
        //return (new CategoryRepositorySqlServer()).GetCategories();
        return repository.GetCategories();
    }
}
