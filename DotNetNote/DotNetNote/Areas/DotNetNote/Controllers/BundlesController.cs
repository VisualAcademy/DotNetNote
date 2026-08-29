using Azunt.BundleManagement;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Areas.DotNetNote.Controllers;

[Area("DotNetNote")]
[Route("DotNetNote/Bundles")]
public sealed class BundlesController : Controller
{
    private readonly IBundleRepository _repository;

    public BundlesController(IBundleRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        string searchQuery = "",
        string status = "",
        bool activeOnly = false,
        string sortOrder = "",
        int page = 1)
    {
        var pageIndex = Math.Max(0, page - 1);

        var result = await _repository.GetPagedAsync(
            new BundleFilterOptions
            {
                PageIndex = pageIndex,
                PageSize = 20,
                SearchQuery = searchQuery ?? string.Empty,
                Status = string.IsNullOrWhiteSpace(status) ? null : status,
                ActiveOnly = activeOnly,
                SortOrder = sortOrder ?? string.Empty
            });

        ViewBag.SearchQuery = searchQuery ?? string.Empty;
        ViewBag.Status = status ?? string.Empty;
        ViewBag.ActiveOnly = activeOnly;
        ViewBag.SortOrder = sortOrder ?? string.Empty;
        ViewBag.Page = pageIndex + 1;
        ViewBag.TotalCount = result.TotalCount;
        ViewBag.PageSize = 20;

        return View(result.Items.ToList());
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View(new Bundle
        {
            Status = "Active",
            IsActive = true,
            CreatedBy = "DotNetNote Test"
        });
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Bundle model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Id = 0;
        model.CreatedAt = DateTimeOffset.UtcNow;
        model.ModifiedAt = null;

        await _repository.AddAsync(model);

        TempData["StatusMessage"] = $"Bundle #{model.Id} created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _repository.GetByIdAsync(id);

        if (model is null)
        {
            return NotFound();
        }

        model.ModifiedBy ??= "DotNetNote Test";
        return View(model);
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Bundle model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = await _repository.GetByIdAsync(id);

        if (existing is null)
        {
            return NotFound();
        }

        // Preserve the original creation audit values.
        model.CreatedAt = existing.CreatedAt;
        model.CreatedBy = existing.CreatedBy;
        model.ModifiedAt = DateTimeOffset.UtcNow;

        var updated = await _repository.UpdateAsync(model);

        if (!updated)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = $"Bundle #{model.Id} updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _repository.GetByIdAsync(id);

        return model is null
            ? NotFound()
            : View(model);
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = $"Bundle #{id} deleted.";
        return RedirectToAction(nameof(Index));
    }
}
