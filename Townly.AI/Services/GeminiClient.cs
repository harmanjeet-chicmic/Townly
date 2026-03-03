using System.Text.Json;
using Townly.AI.Services.Interfaces;

namespace Townly.AI.Services;

public class GeminiClient : ILlmClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeminiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GenerateAsync(string prompt)
{
    var apiKey = _configuration["Gemini:ApiKey"];

    var requestBody = new
    {
        contents = new[]
        {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        }
    };

    var response = await _httpClient.PostAsJsonAsync(
        $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}",
        requestBody);

    var responseContent = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        throw new Exception($"Gemini API Error: {responseContent}");

    var json = JsonDocument.Parse(responseContent);

    var text = json.RootElement
        .GetProperty("candidates")[0]
        .GetProperty("content")
        .GetProperty("parts")[0]
        .GetProperty("text")
        .GetString();

    return text ?? "";
}
}