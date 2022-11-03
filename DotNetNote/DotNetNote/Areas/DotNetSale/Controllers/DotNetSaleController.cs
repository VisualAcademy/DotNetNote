﻿using Microsoft.AspNetCore.Mvc;

namespace DotNetSale.Controllers;

[Area("DotNetSale")]
public class DotNetSaleController : Controller
{
    public IActionResult Index() => View();

    public IActionResult CategoryAdd() => View();

    public IActionResult CategoryList() => View();

    [HttpGet]
    public IActionResult ProductAdd() => View();

    [HttpGet]
    public IActionResult ProductsList(int categoryId = 0)
    {
        ViewBag.CategoryId = categoryId; 

        return View();
    }

    public IActionResult ProductPages() => View();

    [HttpGet]
    public IActionResult ReviewList(int productId = 0)
    {
        ViewData["Product"] = productId; 
        return View(); 
    }

    [HttpGet]
    public IActionResult AddToCart(
        int productId = 0, int quantity = 0)
    {
        ViewBag.ProductId = productId;
        ViewData["Quantity"] = quantity; 

        return View(); 
    }

    [HttpGet]
    public IActionResult ShoppingCart() => View();

    [HttpPost]
    public IActionResult ShoppingCart(
        int productId = 0, int quantity = 0)
    {
        ViewBag.ProductId = productId;
        ViewData["Quantity"] = quantity;

        return View();
    }

    [HttpGet]
    public IActionResult SearchForm() => View();

    [HttpPost]
    public IActionResult SearchResults(string modelName)
    {
        ViewBag.ModelName = modelName; 

        return View(); 
    }
}
