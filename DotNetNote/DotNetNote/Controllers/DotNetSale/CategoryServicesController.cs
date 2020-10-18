using System.Collections.Generic;
using DotNetNote.Models;
using DotNetSaleCom.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetSaleCom.Controllers
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
