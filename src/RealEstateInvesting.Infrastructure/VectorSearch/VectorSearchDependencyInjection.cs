// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using RealEstateInvesting.Application.VectorSearch;

// namespace RealEstateInvesting.Infrastructure.VectorSearch;

// public static class VectorSearchDependencyInjection
// {
//     public static IServiceCollection AddVectorSearch(
//         this IServiceCollection services,
//         IConfiguration config)
//     {
//         services.AddHttpClient();

//         var qdrantUrl = config["Qdrant:Url"];
//         if (string.IsNullOrWhiteSpace(qdrantUrl))
//             throw new InvalidOperationException("Qdrant:Url is not configured");

//         services.AddSingleton(new VectorSearchConfig
//         {
//             OpenAIApiKey = config["OpenAI:ApiKey"]!,
//             QdrantUrl = qdrantUrl,
//             QdrantApiKey = config["Qdrant:ApiKey"]!
//         });

//         services.AddScoped<IEmbeddingService, OpenAIEmbeddingService>();
//         services.AddScoped<IVectorStore, QdrantVectorStore>();

//         return services;
//     }
// }
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateInvesting.Application.VectorSearch;

namespace RealEstateInvesting.Infrastructure.VectorSearch;

public static class VectorSearchDependencyInjection
{
    public static IServiceCollection AddVectorSearch(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // HttpClient for external APIs (Gemini + Qdrant)
        services.AddHttpClient();

        // ---------------------------
        // Qdrant Configuration
        // ---------------------------
        var qdrantUrl = configuration["Qdrant:Url"];
        var qdrantApiKey = configuration["Qdrant:ApiKey"];

        if (string.IsNullOrWhiteSpace(qdrantUrl))
            throw new InvalidOperationException("Qdrant:Url is not configured");

        if (string.IsNullOrWhiteSpace(qdrantApiKey))
            throw new InvalidOperationException("Qdrant:ApiKey is not configured");

        services.AddSingleton(new VectorSearchConfig
        {
            QdrantUrl = qdrantUrl,
            QdrantApiKey = qdrantApiKey,
            CollectionName = "properties"
        });

        // ---------------------------
        // Embedding Provider (Gemini)
        // ---------------------------
        var geminiApiKey = configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(geminiApiKey))
            throw new InvalidOperationException("Gemini:ApiKey is not configured");

        services.AddScoped<IEmbeddingService, GeminiEmbeddingService>();

        // ---------------------------
        // Vector Store
        // ---------------------------
        services.AddScoped<IVectorStore, QdrantVectorStore>();
        services.AddScoped<PropertyVectorIndexer>();

        return services;
    }
}
