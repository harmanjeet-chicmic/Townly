namespace RealEstateInvesting.Application.Kyc.Queries;

public interface IGetMyKycStatusReadService
{
    Task<GetMyKycStatusResult> GetAsync(
        Guid userId,
        CancellationToken cancellationToken);
}
