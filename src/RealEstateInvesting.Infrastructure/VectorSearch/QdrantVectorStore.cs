using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RealEstateInvesting.Infrastructure.VectorSearch
{
    public class QdrantVectorStore
    {
        private readonly HttpClient _httpClient;
        private readonly VectorSearchConfig _config;

        public QdrantVectorStore(HttpClient httpClient, VectorSearchConfig config)
        {
            _httpClient = httpClient;
            _config = config;

            _httpClient.BaseAddress = new Uri(_config.QdrantUrl);
            _httpClient.DefaultRequestHeaders.Add("api-key", _config.QdrantApiKey);
        }

        public async Task UpsertAsync(Guid propertyId, float[] vector)
        {
            var payload = new
            {
                points = new[]
                {
                    new
                    {
                        id = propertyId,
                        vector = vector
                    }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync(
                $"/collections/{_config.CollectionName}/points",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Qdrant upsert failed: {error}");
            }
        }

        public async Task<List<Guid>> SearchSimilarAsync(Guid propertyId, float[] vector, int limit = 6)
        {
            var payload = new
            {
                vector = vector,
                limit = limit + 1
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"/collections/{_config.CollectionName}/points/search",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Qdrant search failed: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var result = new List<Guid>();

            foreach (var item in doc.RootElement.GetProperty("result").EnumerateArray())
            {
                var id = item.GetProperty("id").GetGuid();
                if (id != propertyId)
                    result.Add(id);
            }

            return result;
        }
    }
}
