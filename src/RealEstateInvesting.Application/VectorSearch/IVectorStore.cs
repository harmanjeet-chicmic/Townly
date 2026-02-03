namespace RealEstateInvesting.Application.VectorSearch;

public interface IVectorStore
{
    Task<List<Guid>> SearchSimilarAsync(
        Guid sourcePropertyId,
        float[] vector,
        int limit = 6);
        Task UpsertAsync(
        Guid id,
        float[] vector,
        Dictionary<string, object>? payload = null);

}
