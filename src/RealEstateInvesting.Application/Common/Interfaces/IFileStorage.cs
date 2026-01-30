namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(
        Stream fileStream,
        string contentType,
        string fileName,
        string folder,
        CancellationToken cancellationToken = default);
}
