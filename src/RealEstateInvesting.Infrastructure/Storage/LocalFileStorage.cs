using Microsoft.AspNetCore.Http;

namespace RealEstateInvesting.Infrastructure.Storage;

public static class LocalFileStorage
{
    public static async Task<string> SaveAsync(
        IFormFile file,
        string folderPath,
        string publicBasePath)
    {
        Directory.CreateDirectory(folderPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"{publicBasePath}/{fileName}";
    }
}


// using RealEstateInvesting.Application.Common.Interfaces;

// namespace RealEstateInvesting.Infrastructure.Storage;

// public class LocalFileStorage : IFileStorage
// {
//     private readonly string _rootPath;
//     private readonly string _publicBasePath;

//     public LocalFileStorage(string rootPath, string publicBasePath)
//     {
//         _rootPath = rootPath;
//         _publicBasePath = publicBasePath;
//     }

//     public async Task<string> SaveAsync(
//         Stream fileStream,
//         string contentType,
//         string fileName,
//         string folder,
//         CancellationToken cancellationToken = default)
//     {
//         var folderPath = Path.Combine(_rootPath, folder);
//         Directory.CreateDirectory(folderPath);

//         var finalName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
//         var fullPath = Path.Combine(folderPath, finalName);

//         using var output = new FileStream(fullPath, FileMode.Create);
//         await fileStream.CopyToAsync(output, cancellationToken);

//         return $"{_publicBasePath}/{folder}/{finalName}";
//     }
// }
