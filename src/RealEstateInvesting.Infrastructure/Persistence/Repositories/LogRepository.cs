using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class LogRepository : ILogRepository
{
    private readonly AppDbContext _context;


    public LogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CountAsync()
    {
        return await _context.Logs.CountAsync();
    }

    public async Task AddAsync(Log log)
    {
        await _context.Logs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<Log?> GetOldestAsync()
    {
        return await _context.Logs
            .OrderBy(l => l.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(Log log)
    {
        _context.Logs.Remove(log);
        await _context.SaveChangesAsync();
    }


}
