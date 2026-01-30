using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RealEstateInvesting.Infrastructure.VectorSearch
{
    public class OpenAIEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly VectorSearchConfig _config;

        public OpenAIEmbeddingService(HttpClient httpClient, VectorSearchConfig config)
        {
            _httpClient = httpClient;
            _config = config;

            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config.OpenAIApiKey);
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var requestBody = new
            {
                model = _config.EmbeddingModel,
                input = text
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("embeddings", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"OpenAI embedding failed: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);

            var embeddingArray = document
                .RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding");

            var vector = new float[embeddingArray.GetArrayLength()];
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] = embeddingArray[i].GetSingle();
            }

            return vector;
        }
    }
}
