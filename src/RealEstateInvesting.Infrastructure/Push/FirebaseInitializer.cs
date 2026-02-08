using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace RealEstateInvesting.Infrastructure.Push;

public static class FirebaseInitializer
{
    public static void Initialize(string? serviceAccountJsonPath)
    {
        if (FirebaseApp.DefaultInstance != null)
            return;

        if (string.IsNullOrWhiteSpace(serviceAccountJsonPath))
            throw new InvalidOperationException(
                "Firebase ServiceAccountPath is not configured. " +
                "Ensure it exists in appsettings.json or appsettings.Development.json");

        if (!File.Exists(serviceAccountJsonPath))
            throw new FileNotFoundException(
                $"Firebase service account file not found at path: {serviceAccountJsonPath}");

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(serviceAccountJsonPath)
        });
    }

}
