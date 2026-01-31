namespace RealEstateInvesting.Application.VectorSearch;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}
