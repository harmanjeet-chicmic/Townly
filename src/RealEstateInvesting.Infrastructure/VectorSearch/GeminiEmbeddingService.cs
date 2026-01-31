using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RealEstateInvesting.Application.VectorSearch;
using Microsoft.Extensions.Configuration;

namespace RealEstateInvesting.Infrastructure.VectorSearch;

public class GeminiEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiEmbeddingService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"]
            ?? throw new InvalidOperationException("Gemini:ApiKey is not configured");
        Console.WriteLine("============================API KEY : "+ _apiKey);

        _httpClient.BaseAddress =
            new Uri("https://generativelanguage.googleapis.com/v1beta/");
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var payload = new
        {
            model = "models/embedding-001",
            content = new
            {
                parts = new[]
                {
                    new { text }
                }
            }
        };

        var response = await _httpClient.PostAsync(
            $"models/gemini-embedding-001:embedContent?key={_apiKey}",
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await response.Content.ReadAsStringAsync());

        using var doc =
            JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        var values = doc.RootElement
            .GetProperty("embedding")
            .GetProperty("values");

        var vector = new float[values.GetArrayLength()];
        for (int i = 0; i < vector.Length; i++)
            vector[i] = values[i].GetSingle();

        return vector;
    }
}
