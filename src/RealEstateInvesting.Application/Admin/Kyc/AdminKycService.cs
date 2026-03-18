using RealEstateInvesting.Application.Admin.Kyc.DTOs;
using RealEstateInvesting.Application.Admin.Kyc.Interfaces;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Common.DTOs;
using RealEstateInvesting.Application.Common.Exceptions;

namespace RealEstateInvesting.Application.Admin.Kyc;

public class AdminKycService : IAdminKycService
{
    private readonly IAdminKycRepository _kycRepo;
    private readonly IUserRepository _userRepo;
    private readonly INotificationService _notificationService;

    public AdminKycService(
        IAdminKycRepository kycRepo,
        IUserRepository userRepo,
        INotificationService notificationService)
    {
        _kycRepo = kycRepo;
        _userRepo = userRepo;
        _notificationService = notificationService;
    }

    public async Task<PaginatedResponse<AdminKycListDto>> GetFilteredAsync(AdminKycQuery query)
    {
        var records = await _kycRepo.GetFilteredAsync(query);
        var totalCount = await _kycRepo.GetFilteredCountAsync(query);

        var items = records.Select(x => new AdminKycListDto
        {
            KycId = x.Id,
            UserId = x.UserId,
            FullName = x.FullName,
            Status = x.Status,
            RejectionReason = x.RejectionReason,
            CreatedAt = x.CreatedAt
        }).ToList();

        return new PaginatedResponse<AdminKycListDto>
        {
            Items = items,
            Page = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    public async Task ApproveAsync(Guid kycId, Guid adminUserId)
    {
        var kyc = await _kycRepo.GetByIdAsync(kycId)
            ?? throw new NotFoundException("KYC record not found");

        var user = await _userRepo.GetByIdAsync(kyc.UserId)
            ?? throw new NotFoundException("User not found");

        kyc.Approve(adminUserId);
        user.UpdateKycStatus(KycStatus.Approved);

        await _userRepo.UpdateAsync(user);
        await _kycRepo.SaveChangesAsync();

        // 🔔 Notification
        await _notificationService.CreateAsync(
            user.Id,
            NotificationType.KycApproved,
            "KYC Approved",
            "Your KYC has been approved successfully."
        );
    }

    public async Task RejectAsync(Guid kycId, Guid adminUserId, RejectKycRequest request)
    {
        var kyc = await _kycRepo.GetByIdAsync(kycId)
            ?? throw new NotFoundException("KYC record not found");

        var user = await _userRepo.GetByIdAsync(kyc.UserId)
            ?? throw new NotFoundException("User not found");

        kyc.Reject(adminUserId, request.Reason ?? "No reason provided");
        user.UpdateKycStatus(KycStatus.Rejected);

        await _userRepo.UpdateAsync(user);
        await _kycRepo.SaveChangesAsync();

        // 🔔 Notification
        await _notificationService.CreateAsync(
            user.Id,
            NotificationType.KycRejected,
            "KYC Rejected",
            $"Your KYC has been rejected. Reason: {request.Reason}"
        );
    }
}
