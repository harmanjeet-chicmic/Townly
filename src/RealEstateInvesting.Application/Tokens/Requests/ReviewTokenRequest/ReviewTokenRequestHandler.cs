using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Domain.Enums;
namespace RealEstateInvesting.Application.Tokens.Requests;

public class ReviewTokenRequestHandler
{
    private readonly ITokenRequestRepository _requestRepo;
    private readonly IUserTokenBalanceRepository _balanceRepo;
    private readonly ITokenTransactionRepository _transactionRepo;
    private readonly INotificationService _notificationService;
    public ReviewTokenRequestHandler(
        ITokenRequestRepository requestRepo,
        IUserTokenBalanceRepository balanceRepo,
        ITokenTransactionRepository transactionRepo,
        INotificationService notificationService)
    {
        _requestRepo = requestRepo;
        _balanceRepo = balanceRepo;
        _transactionRepo = transactionRepo;
        _notificationService = notificationService;
    }

    public async Task Handle(ReviewTokenRequestCommand command)
    {
        var request = await _requestRepo.GetByIdAsync(command.RequestId)
            ?? throw new InvalidOperationException("Token request not found.");

        if (command.Approve)
        {
            request.Approve(command.AdminId);

            var balance = await _balanceRepo.GetByUserIdAsync(request.UserId);

            if (balance == null)
            {
                balance = UserTokenBalance.Create(request.UserId);
                await _balanceRepo.AddAsync(balance);
            }
            await _notificationService.CreateAsync(
request.UserId,
NotificationType.TokenRequestApproved,
"Tokens Approved",
$"Your token request for {request.RequestedAmount} ETH has been approved.",
request.Id
);

            balance.Grant(request.RequestedAmount);

            var tx = TokenTransaction.Create(
                request.UserId,
                request.RequestedAmount,
                "Grant",
                $"TokenRequest:{request.Id}");

            await _transactionRepo.AddAsync(tx);

        }
        else
        {   
            await _notificationService.CreateAsync(
        request.UserId,
        NotificationType.TokenRequestRejected,
        "Tokens Rejected",
        $"Your token request was rejected. Reason: {command.RejectionReason}",
    request.Id
);

            request.Reject(command.AdminId, command.RejectionReason!);
        }

        await _requestRepo.SaveChangesAsync();
        await _balanceRepo.SaveChangesAsync();
        await _transactionRepo.SaveChangesAsync();
    }
}
