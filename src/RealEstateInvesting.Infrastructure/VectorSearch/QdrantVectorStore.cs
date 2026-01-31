using System.Text;
using System.Text.Json;
using RealEstateInvesting.Application.VectorSearch;
using System.Net.Http.Json;

namespace RealEstateInvesting.Infrastructure.VectorSearch;

public class QdrantVectorStore : IVectorStore
{
    private readonly HttpClient _httpClient;
    private readonly VectorSearchConfig _config;

    public QdrantVectorStore(
        HttpClient httpClient,
        VectorSearchConfig config)
    {
        _httpClient = httpClient;
        _config = config;

        _httpClient.BaseAddress = new Uri(_config.QdrantUrl);
        _httpClient.DefaultRequestHeaders.Add(
            "api-key", _config.QdrantApiKey);
    }

    public async Task<List<Guid>> SearchSimilarAsync(
        Guid sourcePropertyId,
        float[] vector,
        int limit = 6)
    {
        var payload = new
        {
            vector,
            limit = limit + 1
        };

        var response = await _httpClient.PostAsync(
            $"/collections/{_config.CollectionName}/points/search",
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await response.Content.ReadAsStringAsync());

        using var doc = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        return doc.RootElement
            .GetProperty("result")
            .EnumerateArray()
            .Select(x => x.GetProperty("id").GetGuid())
            .Where(id => id != sourcePropertyId)
            .ToList();
    }
    public async Task UpsertAsync(
    Guid id,
    float[] vector,
    Dictionary<string, object>? payload = null)
    {
        var point = new
        {
            id = id,
            vector = vector,
            payload = payload
        };

        var request = new
        {
            points = new[] { point }
        };

        var response = await _httpClient.PutAsJsonAsync(
            $"/collections/{_config.CollectionName}/points",
            request);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await response.Content.ReadAsStringAsync());
    }

}
