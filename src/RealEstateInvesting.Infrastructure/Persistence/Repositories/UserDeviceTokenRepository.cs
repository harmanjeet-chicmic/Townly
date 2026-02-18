using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class UserDeviceTokenRepository : IUserDeviceTokenRepository
{
    private readonly AppDbContext _context;

    public UserDeviceTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDeviceToken?> GetByTokenAsync(string deviceToken)
    {
        return await _context.Set<UserDeviceToken>()
            .FirstOrDefaultAsync(x => x.DeviceToken == deviceToken);
    }

    public async Task<List<UserDeviceToken>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.Set<UserDeviceToken>()
            .Where(x => x.UserId == userId && x.IsActive)
            .ToListAsync();
    }
    

    public async Task AddAsync(UserDeviceToken token)
    {
        await _context.Set<UserDeviceToken>().AddAsync(token);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
