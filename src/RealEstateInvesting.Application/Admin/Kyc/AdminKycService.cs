using RealEstateInvesting.Application.Admin.Kyc.DTOs;
using RealEstateInvesting.Application.Admin.Kyc.Interfaces;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Admin.Kyc;

public class AdminKycService : IAdminKycService
{
    private readonly IAdminKycRepository _kycRepo;
    private readonly IUserRepository _userRepo;

    public AdminKycService(
        IAdminKycRepository kycRepo,
        IUserRepository userRepo)
    {
        _kycRepo = kycRepo;
        _userRepo = userRepo;
    }

    public async Task<List<AdminKycListDto>> GetPendingAsync()
    {
        var records = await _kycRepo.GetPendingAsync();

        return records.Select(x => new AdminKycListDto
        {
            KycId = x.Id,
            UserId = x.UserId,
            FullName = x.FullName,
            Status = x.Status,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task ApproveAsync(Guid kycId, Guid adminUserId)
    {
        var kyc = await _kycRepo.GetByIdAsync(kycId)
            ?? throw new InvalidOperationException("KYC record not found");

        kyc.Approve(adminUserId);

        var user = await _userRepo.GetByIdAsync(kyc.UserId)
            ?? throw new InvalidOperationException("User not found");

        user.UpdateKycStatus(KycStatus.Approved);

        // persist user using existing repo
        await _userRepo.UpdateAsync(user);

        // persist KYC using admin repo
        await _kycRepo.SaveChangesAsync();
    }

    public async Task RejectAsync(Guid kycId, Guid adminUserId, string reason)
    {
        var kyc = await _kycRepo.GetByIdAsync(kycId)
            ?? throw new InvalidOperationException("KYC record not found");

        kyc.Reject(adminUserId, reason);

        var user = await _userRepo.GetByIdAsync(kyc.UserId)
            ?? throw new InvalidOperationException("User not found");

        user.UpdateKycStatus(KycStatus.Rejected);

        await _userRepo.UpdateAsync(user);
        await _kycRepo.SaveChangesAsync();
    }
}
