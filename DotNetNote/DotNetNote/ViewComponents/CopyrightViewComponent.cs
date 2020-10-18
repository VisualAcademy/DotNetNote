using Microsoft.AspNetCore.Mvc;
using System;

namespace DotNetNote.ViewComponents
{
    /// <summary>
    /// Copyright 뷰 컴포넌트
    /// </summary>
    public class CopyrightViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // 초 단위로 짝수일 때와 홀수일 때 서로 다른 뷰 출력
            string viewName = "Default";
            if (DateTime.Now.Second % 2 == 0)
            {
                viewName = "Details";
            }

            return View(viewName);
        }
    }
}
