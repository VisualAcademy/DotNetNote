﻿using Microsoft.AspNetCore.Mvc;

namespace DotNetSale.Controllers;

[Area("DotNetSale")]
public class CompanyController : Controller
{
    public IActionResult Index() => View();
}
