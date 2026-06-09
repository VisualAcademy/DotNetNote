using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace DotNetNote.Controllers;

public class HttpClientDemoController : Controller
{
    private readonly HttpClient _httpClient;

    public HttpClientDemoController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HttpClientDemoResult
        {
            Url = "https://jsonplaceholder.typicode.com/todos/1"
        };

        try
        {
            string responseText = await _httpClient.GetStringAsync(model.Url);

            model.ResponseText = responseText;
            model.IsSuccess = true;
        }
        catch (Exception ex)
        {
            model.IsSuccess = false;
            model.ErrorMessage = ex.Message;
        }

        return View(model);
    }
}