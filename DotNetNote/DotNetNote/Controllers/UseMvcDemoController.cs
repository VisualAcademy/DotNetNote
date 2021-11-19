using Microsoft.AspNetCore.Mvc;
using System;

namespace DotNetNote.Controllers;

public class UseMvcDemoController : Controller
{
    public string StringResultDemo()
    {
        return "/UseMvcDemp/StringResultDemo 경로로 실행됨";
    }

    public DateTime DateTimeDemo() => DateTime.Now;

    public IActionResult HtmlOnly() => View();

    public IActionResult UsingModelDemo()
    {
        DemoClass dc = new DemoClass() { Id = 1, Name = "박용준" };

        return View(dc);
    }
}
