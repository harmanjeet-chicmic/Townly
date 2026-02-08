namespace RealEstateInvesting.API.Contracts;

public class RegisterDeviceTokenRequest
{
    public string DeviceToken { get; set; } = default!;
    public string Platform { get; set; } = default!; // Android / iOS
}
