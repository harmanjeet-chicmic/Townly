using Amazon.S3;
using Amazon.S3.Model;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Infrastructure.Storage;

public class S3FileStorage : IFileStorage
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucketName;
    private readonly string _basePrefix;

    public S3FileStorage(
        IAmazonS3 s3,
        string bucketName,
        string basePrefix)
    {
        _s3 = s3;
        _bucketName = bucketName;
        _basePrefix = basePrefix.Trim('/');
    }


    public async Task<string> SaveAsync(
        Stream fileStream,
        string contentType,
        string fileName,
        string folder,
        CancellationToken cancellationToken = default)
    {
        var key = $"{_basePrefix}/{Guid.NewGuid()}{Path.GetExtension(fileName)}";

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            
        };

        await _s3.PutObjectAsync(request, cancellationToken);

        return $"https://{_bucketName}.s3.amazonaws.com/{key}";
    }
}
