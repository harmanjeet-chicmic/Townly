using FirebaseAdmin.Messaging;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Notifications.Interfaces;

namespace RealEstateInvesting.Infrastructure.Push;

public class PushNotificationService : IPushNotificationService
{
    private readonly IUserDeviceTokenRepository _deviceTokenRepo;

    public PushNotificationService(IUserDeviceTokenRepository deviceTokenRepo)
    {
        _deviceTokenRepo = deviceTokenRepo;
    }
    public async Task SendToUserAsync(
     Guid userId,
     string title,
     string body,
     string type,
     Guid? referenceId = null)
    {
        var tokens = await _deviceTokenRepo.GetActiveByUserIdAsync(userId);
        

        Console.WriteLine($"[PUSH] UserId: {userId}, Tokens found: {tokens.Count}");

        if (tokens.Count == 0)
        {
            Console.WriteLine("[PUSH] No active device tokens");
            return;
        }

        foreach (var t in tokens)
        {
            Console.WriteLine($"[PUSH] Sending to token: {t.DeviceToken}");
        }

        foreach (var t in tokens)
        {
            var msg = new Message
            {
                Token = t.DeviceToken,

                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },

                Data = new Dictionary<string, string>
                {
                    ["type"] = type,
                    ["referenceId"] = referenceId?.ToString() ?? ""
                },

                Android = new AndroidConfig
                {
                    Priority = Priority.High
                },

                Apns = new ApnsConfig
                {
                    Headers = new Dictionary<string, string>
                    {
                        ["apns-priority"] = "10"
                    },
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                }
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
                Console.WriteLine($"[PUSH] UserId: {userId}, Tokens found: {tokens.Count}");

                Console.WriteLine($"[PUSH] ✅ Firebase accepted message. MessageId: {response}");
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine("[PUSH] ❌ Firebase send failed");
                Console.WriteLine($"Messaging Error Code: {ex.MessagingErrorCode}");

                if (ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                    ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
                {
                    Console.WriteLine("[PUSH] Deactivating invalid token");

                    t.Deactivate();
                    await _deviceTokenRepo.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[PUSH] Unexpected error");
                Console.WriteLine(ex.Message);
            }

        }
    }

}
