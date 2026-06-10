using System.Net.Http.Json;
using System.Text.Json;

namespace LinguaLib.Services;

public class TranslationService
{
    private readonly HttpClient _http = new();

    public async Task<string> TranslateAsync(string text, string from = "en", string to = "ru")
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        try
        {
            var url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair={from}|{to}";
            var json = await _http.GetFromJsonAsync<JsonElement>(url);
            return json.GetProperty("responseData").GetProperty("translatedText").GetString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
