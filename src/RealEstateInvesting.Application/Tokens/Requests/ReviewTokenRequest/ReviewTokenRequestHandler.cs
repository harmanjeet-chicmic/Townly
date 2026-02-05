using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Tokens.Requests;

public class ReviewTokenRequestHandler
{
    private readonly ITokenRequestRepository _requestRepo;
    private readonly IUserTokenBalanceRepository _balanceRepo;
    private readonly ITokenTransactionRepository _transactionRepo;

    public ReviewTokenRequestHandler(
        ITokenRequestRepository requestRepo,
        IUserTokenBalanceRepository balanceRepo,
        ITokenTransactionRepository transactionRepo)
    {
        _requestRepo = requestRepo;
        _balanceRepo = balanceRepo;
        _transactionRepo = transactionRepo;
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
            request.Reject(command.AdminId, command.RejectionReason!);
        }

        await _requestRepo.SaveChangesAsync();
        await _balanceRepo.SaveChangesAsync();
        await _transactionRepo.SaveChangesAsync();
    }
}
