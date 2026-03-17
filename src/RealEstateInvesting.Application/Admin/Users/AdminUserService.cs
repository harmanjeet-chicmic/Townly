using RealEstateInvesting.Application.Admin.Users.DTOs;
using RealEstateInvesting.Application.Admin.Users.Interfaces;
using RealEstateInvesting.Application.Common.DTOs;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Admin.Users;

public class AdminUserService : IAdminUserService
{
    private readonly IAdminUserRepository _userRepo;
    private readonly IKycRecordRepository _kycRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IAnalyticsSnapshotRepository _analyticsSnapshotRepository;

    public AdminUserService(
        IAdminUserRepository userRepo,
        IKycRecordRepository kycRepository,
        IPropertyRepository propertyRepository,
        IInvestmentRepository investmentRepository,
        IAnalyticsSnapshotRepository analyticsSnapshotRepository)
    {
        _userRepo = userRepo;
        _kycRepository = kycRepository;
        _propertyRepository = propertyRepository;
        _investmentRepository = investmentRepository;
        _analyticsSnapshotRepository = analyticsSnapshotRepository;
    }


    public async Task<PaginatedResponse<AdminUserPortfolioDto>> GetAllAsync(AdminUserQuery query)
    {
        var (users, totalCount) = await _userRepo.GetAllAsync(query);

        var result = new List<AdminUserPortfolioDto>();

        foreach (var user in users)
        {
            // 🔥 KYC
            var kyc = await _kycRepository.GetByUserIdAsync(user.Id);

            // 🔥 Properties created
            var properties = await _propertyRepository.GetByOwnerIdAsync(user.Id);
            var propertiesCount = properties.Count();

            // 🔥 Investments
            var investments = await _investmentRepository.GetByUserIdAsync(user.Id);
            var totalInvestment = investments.Sum(i => i.TotalAmount);

            // 🔥 Portfolio snapshot
            var snapshot = await _analyticsSnapshotRepository
                .GetLastUserSnapshotBeforeAsync(user.Id, DateTime.UtcNow);

            var portfolioValue = snapshot?.TotalInvested ?? 0;

            result.Add(new AdminUserPortfolioDto
            {
                Id = user.Id,
                Name = kyc?.FullName, // now works ✅
                WalletAddress = user.WalletAddress,
                Properties = propertiesCount,
                TotalInvestment = totalInvestment,
                PortfolioValue = portfolioValue,
                KycStatus = MapKycStatus(user.KycStatus)
            });
        }

        return new PaginatedResponse<AdminUserPortfolioDto>
        {
            Items = result,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    private string MapKycStatus(KycStatus status)
    {
        return status switch
        {
            KycStatus.NotStarted => "not_started",
            KycStatus.Pending => "pending",
            KycStatus.Approved => "verified",
            KycStatus.Rejected => "rejected",
            _ => "unknown"
        };
    }
}