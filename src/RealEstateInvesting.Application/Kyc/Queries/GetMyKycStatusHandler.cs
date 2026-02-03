using RealEstateInvesting.Application.Kyc.Queries;

namespace RealEstateInvesting.Application.Kyc.Handlers;

public sealed class GetMyKycStatusHandler
{
    private readonly IGetMyKycStatusReadService _readService;

    public GetMyKycStatusHandler(IGetMyKycStatusReadService readService)
    {
        _readService = readService;
    }

    public Task<GetMyKycStatusResult> HandleAsync(
        GetMyKycStatusQuery query,
        CancellationToken cancellationToken)
        => _readService.GetAsync(query.UserId, cancellationToken);
}
