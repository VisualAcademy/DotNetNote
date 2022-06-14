using DotNetNote.Models.Categories;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers.DotNetSale
{
    [Route("api/[controller]")]
    public class CategoryServicesController : Controller
    {
        private readonly ICategoryRepository _repository;

        public CategoryServicesController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Category> Get()
        {
            //return (new CategoryRepositoryInMemory()).GetCategories();
            //return (new CategoryRepositorySqlServer()).GetCategories();
            return _repository.GetCategories();
        }
    }
}
