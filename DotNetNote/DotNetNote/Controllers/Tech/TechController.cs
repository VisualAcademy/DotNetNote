using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DotNetNote.Controllers;

public class TechController : Controller
{
    public IActionResult Index() => View();

    public async Task<IActionResult> GetTechAllWithHttpClient()
    {
        List<Tech> teches = await GetAll();
        return View(teches);
    }

    /// <summary>
    /// [1] GET: List of Tech 형태의 데이터를 JSON으로 받아오는 공식 코드
    /// </summary>
    private static async Task<List<Tech>> GetAll()
    {
        var baseUri = "http://localhost:16929/";
        List<Tech> teches = new();

        using HttpClient httpClient = new();
        httpClient.BaseAddress = new Uri(baseUri);
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        // 데이터 가져오기
        HttpResponseMessage response = await httpClient.GetAsync("api/TechesApi");
        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            teches = JsonConvert.DeserializeObject<List<Tech>>(json) ?? new List<Tech>();
        }

        return teches;
    }
}