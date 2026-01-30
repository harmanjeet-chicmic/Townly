using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RealEstateInvesting.Infrastructure.VectorSearch
{
    public static class VectorSearchDependencyInjection
    {
        public static IServiceCollection AddVectorSearch(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient();

            services.AddSingleton(new VectorSearchConfig
            {
                OpenAIApiKey = configuration["OpenAI:ApiKey"]!,
                QdrantUrl = configuration["Qdrant:Url"]!,
                QdrantApiKey = configuration["Qdrant:ApiKey"]!,
                CollectionName = "properties"
            });

            services.AddScoped<OpenAIEmbeddingService>();
            services.AddScoped<QdrantVectorStore>();
            services.AddScoped<PropertyVectorIndexer>();

            return services;
        }
    }
}
