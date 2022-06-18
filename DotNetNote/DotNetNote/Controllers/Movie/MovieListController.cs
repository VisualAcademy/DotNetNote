using Microsoft.AspNetCore.Mvc;
using System;

namespace DotNetNote.Controllers;

public class MovieListController : Controller
{
    /// <summary>
    /// 컬렉션 형태의 데이터를 뷰 페이지에 표 형태로 출력하기
    /// </summary>
    public IActionResult Index()
    {
        List<MovieViewModel> vms = new List<MovieViewModel>() {
            new MovieViewModel { Id = 1, Title = "명량",
                CreationDate = new DateTime(2014, 1, 1) },
            new MovieViewModel { Id = 2, Title = "실미도",
                CreationDate = new DateTime(2003, 1, 1) },
            new MovieViewModel { Id = 3, Title = "베테랑",
                CreationDate = new DateTime(2015, 1, 1) }
        };

        return View(vms);
    }
}
