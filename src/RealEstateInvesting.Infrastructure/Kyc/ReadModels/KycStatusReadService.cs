using Microsoft.EntityFrameworkCore;
using Nethereum.Siwe.Model;
using RealEstateInvesting.Application.Kyc.Queries;
using RealEstateInvesting.Infrastructure.Persistence;
using RealEstateInvesting.Domain.Enums;
namespace RealEstateInvesting.Infrastructure.Kyc.ReadModels;

public sealed class KycStatusReadService : IGetMyKycStatusReadService
{
    private readonly AppDbContext _context;

    public KycStatusReadService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetMyKycStatusResult> GetAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var kyc = await _context.KycRecords
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new
            {
                x.Status,
                x.CreatedAt,
                x.RejectionReason
            })
            .FirstOrDefaultAsync(cancellationToken);
        var kycvalue = _context.KycRecords.Where(x => x.UserId == userId).Select(x => x.Status);

        if (kyc is null)
        {
            return new GetMyKycStatusResult
            {
                Status = KycStatus.NotStarted
            };
        }



        return new GetMyKycStatusResult
        {
            Status = kyc.Status, // enum
            SubmittedAt = kyc.CreatedAt,
            RejectionReason = kyc.RejectionReason
        };
    }
}

// public enum KycStatus
// {
//     NotStarted = 0,
//     Pending = 1,
//     Approved = 2,
//     Rejected = 3
// }
