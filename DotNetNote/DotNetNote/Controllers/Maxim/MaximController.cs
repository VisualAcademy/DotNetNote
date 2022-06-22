using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class MaximController : Controller
    {
        private MaximServiceRepository _repo;

        public MaximController(MaximServiceRepository maximService)
        {
            _repo = maximService;
        }

        // GET: Maxim
        public IActionResult Index()
        {
            return View(_repo.GetMaxims());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Maxim model)
        {
            _repo.AddMaxim(model);

            return RedirectToAction("Index", "Maxim");
        }
    }
}
