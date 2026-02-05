using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface ITokenRequestRepository
{
    Task AddAsync(TokenRequest request);
    Task<TokenRequest?> GetByIdAsync(Guid id);
    Task<List<TokenRequest>> GetPendingAsync();
    Task<List<TokenRequest>> GetByUserAsync(Guid userId);
    Task SaveChangesAsync();
    
}   
