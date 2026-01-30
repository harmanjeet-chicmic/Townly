using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class KycRecordRepository : IKycRecordRepository
{
    private readonly AppDbContext _context;

    public KycRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPendingKycAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.KycRecords
            .AnyAsync(k =>
                k.UserId == userId &&
                k.Status == KycStatus.Pending,
                cancellationToken);
    }

    public async Task AddAsync(KycRecord kycRecord, CancellationToken cancellationToken = default)
    {
        await _context.KycRecords.AddAsync(kycRecord, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
