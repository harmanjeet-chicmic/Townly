using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class OnChainKycActionRepository : IOnChainKycActionRepository
{
    private readonly AppDbContext _context;

    public OnChainKycActionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OnChainKycAction action, CancellationToken cancellationToken = default)
    {
        await _context.OnChainKycActions.AddAsync(action, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordIdentityUpdateAsync(string walletAddress, string identityContractAddress, string transactionHash, Guid? performedByAdminId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveUserIdAsync(walletAddress, cancellationToken);
        var action = OnChainKycAction.ForIdentityUpdate(walletAddress, userId, identityContractAddress, transactionHash, performedByAdminId);
        await _context.OnChainKycActions.AddAsync(action, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordCountryUpdateAsync(string walletAddress, ushort countryCode, string transactionHash, Guid? performedByAdminId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveUserIdAsync(walletAddress, cancellationToken);
        var action = OnChainKycAction.ForCountryUpdate(walletAddress, userId, countryCode, transactionHash, performedByAdminId);
        await _context.OnChainKycActions.AddAsync(action, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordRegisterIdentityAsync(string walletAddress, string identityContractAddress, ushort countryCode, string transactionHash, Guid? performedByAdminId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveUserIdAsync(walletAddress, cancellationToken);
        var action = OnChainKycAction.ForRegisterIdentity(walletAddress, userId, identityContractAddress, countryCode, transactionHash, performedByAdminId);
        await _context.OnChainKycActions.AddAsync(action, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<Guid?> ResolveUserIdAsync(string walletAddress, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(walletAddress)) return null;
        var normalized = walletAddress.Trim().ToLowerInvariant();
        if (!normalized.StartsWith("0x")) normalized = "0x" + normalized;
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.WalletAddress == normalized)
            .Select(u => new { u.Id })
            .FirstOrDefaultAsync(cancellationToken);
        return user?.Id;
    }
}
