using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
public class OrganizationRepository : IOrganizationRepository
{
    private readonly AppDbContext _context;

    public OrganizationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Organization?> GetByIdAsync(Guid id)
    {
        return await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }
    public async Task<List<Organization>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Organizations
            .Where(o => !o.IsDeleted)
            .ToListAsync(ct);
    }
}