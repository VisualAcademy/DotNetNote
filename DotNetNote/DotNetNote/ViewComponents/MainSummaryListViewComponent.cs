using Microsoft.AspNetCore.Mvc;
using System;

namespace DotNetNote.ViewComponents
{
    public class MainSummaryListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // 데이터 리포지토리 클래스에서 데이터 가져오기 영역

            return View(); 
        }
    }
}
