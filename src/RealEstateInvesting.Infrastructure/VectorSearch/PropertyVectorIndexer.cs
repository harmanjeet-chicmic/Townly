
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.VectorSearch
{
    public class PropertyVectorIndexer
    {
        private readonly OpenAIEmbeddingService _embeddingService;
        private readonly QdrantVectorStore _vectorStore;

        public PropertyVectorIndexer(
            OpenAIEmbeddingService embeddingService,
            QdrantVectorStore vectorStore)
        {
            _embeddingService = embeddingService;
            _vectorStore = vectorStore;
        }

        public async Task IndexAsync(Property property)
        {
            // Build semantic text
            var embeddingText = BuildEmbeddingText(property);

            // Generate vector
            var vector = await _embeddingService.GenerateEmbeddingAsync(embeddingText);

            // Store in Qdrant
            await _vectorStore.UpsertAsync(property.Id, vector);
        }

        private static string BuildEmbeddingText(Property property)
        {
            return $"""
            Property name: {property.Name}
            Property type: {property.PropertyType}
            Location: {property.Location}

            Description:
            {property.Description}

            Approved valuation: {property.ApprovedValuation} USD
            Total units: {property.TotalUnits}
            Expected annual yield: {property.AnnualYieldPercent} percent
            Status: {property.Status}
            """;
        }
    }
}
