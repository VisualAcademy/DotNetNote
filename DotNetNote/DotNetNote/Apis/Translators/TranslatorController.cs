using System.Net.Http;
using System.Text.Json;

namespace DotNetNote.Apis.Translators;

[ApiController]
[Route("api/[controller]")]
public class TranslatorController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string SubscriptionKey = "YOUR_AZURE_SUBSCRIPTION_KEY";
    private const string Endpoint = "https://api.cognitive.microsofttranslator.com/";
    private const string Region = "koreacentral"; // 예: "koreacentral"
    private const string Route = "/translate?api-version=3.0&to=es";

    public TranslatorController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> Translate([FromBody] TranslationRequest request)
    {
        try
        {
            var translatedText = await TranslateTextAsync(request.Text);
            return Ok(new { TranslatedText = translatedText });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, $"번역 실패: {ex.Message}");
        }
    }

    private async Task<string> TranslateTextAsync(string input)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", Region);

        var requestBody = new object[] { new { Text = input } };
        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(Endpoint + Route, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"번역 요청 실패: {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonResponse);

        var translatedText = doc.RootElement[0]
            .GetProperty("translations")[0]
            .GetProperty("text")
            .GetString();

        return translatedText ?? string.Empty;
    }
}

public class TranslationRequest
{
    public string Text { get; set; }
}
