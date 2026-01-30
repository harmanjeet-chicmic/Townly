namespace RealEstateInvesting.Application.Common.Interfaces;
public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
