using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class DataController : Controller
    {
        public IActionResult Index()
        {
            // 모든 데이터를 읽어서 View 페이지에 전달
            DataService demoService = new DataService();
            var data = demoService.GetAll();
            return View(data);
        }
    }
}
