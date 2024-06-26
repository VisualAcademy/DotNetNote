﻿namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class WebApiNoteController : Controller
{
    [HttpGet("{id}")]
    public string Get(int id) => $"넘어온 값: {id}";

    [HttpGet]
    public double GetByNumber(int number) => number;
}

public class WebApiNoteTestController : Controller
{
    public IActionResult GetByNumber() => View();
}
