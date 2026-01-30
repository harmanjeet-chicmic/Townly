namespace RealEstateInvesting.Infrastructure.VectorSearch
{
    public class VectorSearchConfig
    {
        public string OpenAIApiKey { get; set; } = default!;
        public string EmbeddingModel { get; set; } = "text-embedding-3-small";

        public string QdrantUrl { get; set; } = default!;
        public string QdrantApiKey { get; set; } = default!;
        public string CollectionName { get; set; } = "properties";
    }
}
