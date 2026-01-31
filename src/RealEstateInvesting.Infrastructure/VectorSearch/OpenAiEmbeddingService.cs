using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RealEstateInvesting.Application.VectorSearch;

namespace RealEstateInvesting.Infrastructure.VectorSearch;

public class OpenAIEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly VectorSearchConfig _config;

    public OpenAIEmbeddingService(
        HttpClient httpClient,
        VectorSearchConfig config)
    {
        _httpClient = httpClient;
        _config = config;

        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config.OpenAIApiKey);
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var payload = new
        {
            model = _config.EmbeddingModel,
            input = text
        };

        var response = await _httpClient.PostAsync(
            "embeddings",
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await response.Content.ReadAsStringAsync());

        using var doc = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var embedding = doc.RootElement
            .GetProperty("data")[0]
            .GetProperty("embedding");

        var vector = new float[embedding.GetArrayLength()];
        for (int i = 0; i < vector.Length; i++)
            vector[i] = embedding[i].GetSingle();

        return vector;
    }
}
