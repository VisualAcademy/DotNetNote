using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DotNetNote.Settings;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetNote.ViewModels.Translators;

namespace DotNetNote.Controllers.Translators
{
    public class TranslationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AzureTranslatorSettings _translatorSettings;

        public TranslationController(IHttpClientFactory httpClientFactory, IOptions<AzureTranslatorSettings> translatorSettings)
        {
            _httpClientFactory = httpClientFactory;
            _translatorSettings = translatorSettings.Value;
        }

        [HttpGet]
        public IActionResult Translate()
        {
            return View(new TranslationRequestModel());
        }

        [HttpPost]
        public async Task<IActionResult> Translate(TranslationRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
            {
                ModelState.AddModelError(nameof(model.Text), "텍스트를 입력하세요.");
                return View(model);
            }

            try
            {
                var translatedText = await TranslateTextAsync(model.Text);
                ViewData["TranslatedText"] = translatedText;
                return View("TranslateResult");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"번역 실패: {ex.Message}");
                return View(model);
            }
        }

        private async Task<string> TranslateTextAsync(string input)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _translatorSettings.SubscriptionKey);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", _translatorSettings.Region);

            var requestBody = new object[] { new { Text = input } };
            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync($"{_translatorSettings.Endpoint}/translate?api-version=3.0&to=es", content);

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
}