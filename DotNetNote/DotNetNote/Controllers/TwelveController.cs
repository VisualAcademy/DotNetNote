using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class TwelveController : Controller
{
    private readonly ITwelveRepository _repository;

    public TwelveController(ITwelveRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult SeedTest()
    {
        var parentId = 1;
        ViewBag.ParentId = parentId;
        var twelves = _repository.Seed(parentId);
        return View(twelves);
    }

    public IActionResult AddProfit()
    {
        for (int i = 1; i <= 12; i++)
        {
            _repository.SaveOrUpdateProfit(1, i, i * 10);
        }

        return View();
    }

    public IActionResult GetTwelves()
    {
        ViewBag.ParentId = 1;
        var twelves = _repository.GetTwelves(1);
        return View(twelves);
    }
}
