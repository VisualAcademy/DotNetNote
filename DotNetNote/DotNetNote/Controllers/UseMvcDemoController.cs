﻿namespace DotNetNote.Controllers;

public class UseMvcDemoController : Controller
{
    public string StringResultDemo() => "/UseMvcDemp/StringResultDemo 경로로 실행됨";

    public DateTime DateTimeDemo() => DateTime.Now;

    public IActionResult HtmlOnly() => View();

    public IActionResult UsingModelDemo()
    {
        DemoClass dc = new() { Id = 1, Name = "박용준" };

        return View(dc);
    }
}
