using Microsoft.AspNetCore.Http;

namespace ScaffoldingDemo.Controllers
{
    public class DefaultMvcController : Controller
    {
        // GET: DefaultMvc
        public ActionResult Index()
        {
            return View();
        }

        // GET: DefaultMvc/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DefaultMvc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DefaultMvc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DefaultMvc/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DefaultMvc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DefaultMvc/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DefaultMvc/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}