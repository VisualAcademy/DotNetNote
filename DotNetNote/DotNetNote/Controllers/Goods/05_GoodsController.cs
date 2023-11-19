using Microsoft.AspNetCore.Mvc;
using Dul.Web;

namespace DotNetNote.Controllers;

/// <summary>
/// [5] 컨트롤러 클래스: ASP.NET Core - MVC 파트 
/// </summary>
public class GoodsController : Controller
{
    private IGoodsRepository _repository;

    public GoodsController(IGoodsRepository repository) => _repository = repository;

    /// <summary>
    /// 리스트
    /// </summary>
    public IActionResult Index(int page = 1)
    {
        int pageSize = 10; 
        var goodsSet = _repository.GetAllGoodsWithPaging(page, pageSize);

        ViewBag.PageNumber = page;

        // 페이저 컨트롤 적용
        ViewBag.PageModel = new PagerBase
        {
            Url = "Goods/Index",
            RecordCount = goodsSet.GoodsCount,
            PageSize = pageSize,
            PageNumber = page,

            SearchMode = false,
            SearchField = "",
            SearchQuery = ""
        };

        return View(goodsSet);
    }
    //public IActionResult Index(int pageNumber = 1)
    //{
    //    var goodsSet = _repository.GetAllGoodsWithPaging(pageNumber, 10);

    //    ViewBag.PageNumber = pageNumber; 

    //    return View(goodsSet);
    //}

    #region 입력
    /// <summary>
    /// 입력 폼
    /// </summary>
    [HttpGet]
    public IActionResult Create() => View();

    /// <summary>
    /// 입력 처리 
    /// </summary>
    [HttpPost]
    public IActionResult Create(string goodsName, string goodsDescription)
    {
        GoodsBase model = new GoodsBase()
        {
            GoodsName = goodsName,
            GoodsDescription = goodsDescription
        };

        _repository.AddGoods(model);

        //return Redirect("/Goods"); // 리스트 페이지로 이동
        return RedirectToAction(nameof(Index));
    } 
    #endregion

    /// <summary>
    /// 상세 보기 
    /// </summary>
    public IActionResult Details(int id)
    {
        var goods = _repository.GetGoodsById(id);
        return View(goods); 
    }

    /// <summary>
    /// 수정 처리
    /// </summary>
    [HttpPost]
    public IActionResult Edit(int goodsId, string goodsName, string goodsDescription)
    {
        var goods = new GoodsBase {
            GoodsId = goodsId, GoodsName = goodsName, GoodsDescription = goodsDescription
        };

        _repository.UpdateGoods(goods);

        return RedirectToAction(nameof(Details), new { Id = goodsId }); 
    }

    /// <summary>
    /// 삭제 처리 
    /// </summary>
    [HttpGet]
    public IActionResult Delete(int id)
    {
        _repository.RemoveGoods(id);
        return RedirectToAction(nameof(Index)); 
    }
}
