using Org.BouncyCastle.Math.EC.Rfc7748;
using RealEstateInvesting.Application.Common.Exceptions;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Transactions.Dtos;
using RealEstateInvesting.Domain.Enums;
namespace RealEstateInvesting.Application.Transactions;

public class TransactionQueryService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserRepository _userRepository;
    public TransactionQueryService(ITransactionRepository transactionRepository, IPropertyRepository propertyRepository,
    IUserRepository userRepository)
    {
        _transactionRepository = transactionRepository;
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
    }
    public async Task<object> GetMyTransactionsAsync(
    Guid userId,
    int page,
    int pageSize,
    TransactionType? type)
    {
        // 1️⃣ Get paginated transactions
        var (transactions, totalCount) =
            await _transactionRepository
                .GetByUserIdPagedAsync(userId, page, pageSize, type);

        if (!transactions.Any())
        {
            return new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = 0,
                HasMore = false,
                Items = new List<MyTransactionDto>()
            };
        }

        // 2️⃣ Load investor (current user)
        var investor = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var investorWallet = investor.WalletAddress;

        // 3️⃣ Collect property IDs
        var propertyIds = transactions
            .Where(t => t.PropertyId.HasValue)
            .Select(t => t.PropertyId!.Value)
            .Distinct()
            .ToList();

        // 4️⃣ Load properties
        var properties =
            await _propertyRepository.GetByIdsAsync(propertyIds);

        var propertyDict = properties.ToDictionary(p => p.Id);

        // 5️⃣ Collect owner IDs
        var ownerIds = properties
            .Select(p => p.OwnerUserId)
            .Distinct()
            .ToList();

        // 6️⃣ Load property owners
        var owners =
            await _userRepository.GetByIdsAsync(ownerIds);

        var ownerWalletMap =
            owners.ToDictionary(o => o.Id, o => o.WalletAddress);

        // 7️⃣ Map transactions
        var items = transactions.Select(t =>
    {
        string? propertyName = null;
        string? fromWallet = null;
        string? toWallet = null;

        if (t.PropertyId.HasValue &&
            propertyDict.TryGetValue(t.PropertyId.Value, out var property))
        {
            propertyName = property.Name;

            var ownerWallet = ownerWalletMap[property.OwnerUserId];
            var currentUserWallet = investorWallet;

            if (t.UserId == property.OwnerUserId)
            {
                // 🔵 Current user is OWNER

                if (t.Type == TransactionType.RentalIncome)
                {
                    // Owner received rental income
                    fromWallet = "System";
                    toWallet = ownerWallet;
                }
                else if (t.Type == TransactionType.Investment)
                {
                    // Owner received investment
                    fromWallet = "Investor";
                    toWallet = ownerWallet;
                }
            }
            else
            {
                // 🟢 Current user is INVESTOR

                if (t.Type == TransactionType.Investment)
                {
                    // Investor paid investment
                    fromWallet = currentUserWallet;
                    toWallet = ownerWallet;
                }
                else if (t.Type == TransactionType.RentalIncome)
                {
                    // Investor received rental income
                    fromWallet = ownerWallet;
                    toWallet = currentUserWallet;
                }
            }
        }

        return new MyTransactionDto
        {
            TransactionId = t.Id,
            PropertyId = t.PropertyId,
            PropertyName = propertyName,
            Type = t.Type,
            AmountUsd = t.AmountUsd,
            AmountEth = t.EthAmountAtExecution,
            EthUsdRateAtExecution = t.EthUsdRateAtExecution,
            Status = t.IsSuccessful ? "Completed" : "Pending",
            FromWalletAddress = fromWallet,
            ToWalletAddress = toWallet,
            CreatedAt = t.CreatedAt
        };
    });

        return new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasMore = page * pageSize < totalCount,
            Items = items
        };
    }
    public async Task<TransactionDetailsDto?>
GetMyTransactionDetailsAsync(Guid userId, Guid transactionId)
    {
        var transaction = await
            _transactionRepository
                .GetByIdForUserAsync(transactionId, userId);

        if (transaction == null)
            return null;

        string? propertyName = null;


        if (transaction.PropertyId.HasValue)
        {
            var property = await
                _propertyRepository
                    .GetByIdAsync(transaction.PropertyId.Value);

            propertyName = property?.Name;
        }

        return new TransactionDetailsDto
        {
            TransactionId = transaction.Id,
            PropertyId = transaction.PropertyId,
            PropertyName = propertyName,

            Type = transaction.Type,
            AmountUsd = transaction.AmountUsd,
            Currency = transaction.Currency,

            EthAmountAtExecution = transaction.EthAmountAtExecution,

            AmountEth = transaction.EthAmountAtExecution,
            EthUsdRateAtExecution = transaction.EthUsdRateAtExecution,

            Status = transaction.IsSuccessful ? "Completed" : "Pending",

            CreatedAt = transaction.CreatedAt
        };
    }


}
