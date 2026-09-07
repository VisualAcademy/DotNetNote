using DotNetNote.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class AssetRecordsController : Controller
{
    private readonly IAssetRecordRepository _repository;

    public AssetRecordsController(IAssetRecordRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View(_repository.GetAll());
    }

    public IActionResult Details(int id)
    {
        return View(_repository.GetById(id));
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Edit(int id)
    {
        return View(_repository.GetById(id));
    }

    public IActionResult Delete(int id)
    {
        return View(_repository.GetById(id));
    }
}