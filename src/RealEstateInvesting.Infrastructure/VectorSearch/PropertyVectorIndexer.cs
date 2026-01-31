using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.VectorSearch;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.VectorSearch;

public class PropertyVectorIndexer
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorStore;

    public PropertyVectorIndexer(
        IEmbeddingService embeddingService,
        IVectorStore vectorStore)
    {
        _embeddingService = embeddingService;
        _vectorStore = vectorStore;
    }

    public async Task IndexAsync(Property property)
    {
        var embeddingText = $"""
        Property name: {property.Name}
        Property type: {property.PropertyType}
        Location: {property.Location}

        {property.Description}

        Approved valuation: {property.ApprovedValuation}
        Total units: {property.TotalUnits}
        Annual yield: {property.AnnualYieldPercent}
        """;

        var vector = await _embeddingService.GenerateEmbeddingAsync(embeddingText);

        await _vectorStore.UpsertAsync(
            property.Id,
            vector,
            new Dictionary<string, object>
            {
                ["propertyId"] = property.Id.ToString(),
                ["location"] = property.Location,
                ["type"] = property.PropertyType
            });
    }
}
