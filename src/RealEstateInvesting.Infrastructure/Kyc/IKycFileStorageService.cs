using Microsoft.AspNetCore.Http;

namespace RealEstateInvesting.Infrastructure.Kyc;

public interface IKycFileStorageService
{
    Task<(string documentUrl, string selfieUrl)> SaveKycFilesAsync(
        Guid userId,
        IFormFile documentFile,
        IFormFile selfieFile,
        CancellationToken cancellationToken = default);
}
