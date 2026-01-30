using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace RealEstateInvesting.Infrastructure.Kyc;

public class KycFileStorageService : IKycFileStorageService
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
    private readonly IWebHostEnvironment _environment;

    public KycFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<(string documentUrl, string selfieUrl)> SaveKycFilesAsync(
        Guid userId,
        IFormFile documentFile,
        IFormFile selfieFile,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(documentFile);
        ValidateFile(selfieFile);

        var userFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "kyc",
            userId.ToString()
        );

        Directory.CreateDirectory(userFolder);

        var documentUrl = await SaveFileAsync(documentFile, userFolder, "document", cancellationToken);
        var selfieUrl = await SaveFileAsync(selfieFile, userFolder, "selfie", cancellationToken);

        return (documentUrl, selfieUrl);
    }

    private static void ValidateFile(IFormFile file)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("Uploaded file is empty.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Only JPG and PNG files are allowed.");
    }

    private static async Task<string> SaveFileAsync(
        IFormFile file,
        string folderPath,
        string prefix,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{prefix}_{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        var userFolder = Path.GetFileName(Path.GetDirectoryName(fullPath)!);
        return $"/uploads/kyc/{userFolder}/{fileName}";
    }
}
