using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotNetNote.Controllers
{
    public class VariableTestController : Controller
    {
        private readonly IVariableRepository _repository;

        public VariableTestController(IVariableRepository repository)
        {
            _repository = repository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var variables = _repository.GetAll();
            ViewBag.Variables = variables; 
            return View();
        }
    }
}
